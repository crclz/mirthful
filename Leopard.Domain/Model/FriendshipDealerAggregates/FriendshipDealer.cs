using Dawn;
using Leopard.Domain.Model.Relationships;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Leopard.Domain.Model.FriendshipDealerAggregates
{
	public class FriendshipDealer : RootEntity
	{
		public ObjectId AUserId { get; private set; }

		public ObjectId BUserId { get; private set; }

		public Friendship AToBFriendship { get; private set; }

		public Friendship BToAFriendship { get; private set; }

		public FriendshipRequest AToBRequest { get; private set; }

		public FriendshipRequest BToARequest { get; private set; }

		protected FriendshipDealer()
		{
			// Required by EF
		}

		public FriendshipDealer(ObjectId id1, ObjectId id2)
		{
			if (id1 < id2)
			{
				AUserId = id1;
				BUserId = id2;
			}
			else
			{
				throw new ArgumentException("Arguments should satisfy id1<id2");
			}
		}

		public bool IsA(ObjectId userId)
		{
			if (userId == AUserId)
			{
				return true;
			}
			else if (userId == BUserId)
			{
				return false;
			}
			else
			{
				throw new ArgumentException("Wrong userId", nameof(userId));
			}
		}

		public ObjectId GetAnotherId(ObjectId userId)
		{
			if (IsA(userId))
				return BUserId;
			else
				return AUserId;
		}

		public bool AreFriends()
		{
			if (AToBFriendship != null && AToBFriendship.IsValid)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsBlocked(ObjectId userId)
		{
			if (IsA(userId))
			{
				if (AToBRequest == null)
					return false;
				else
					return AToBRequest.Status == RelationshipRequestStatus.RefusedAndBlocked;
			}
			else
			{
				if (BToARequest == null)
					return false;
				else
					return BToARequest.Status == RelationshipRequestStatus.RefusedAndBlocked;
			}
		}

		public bool HasUnhandledRequest(ObjectId userId)
		{
			return GetUnhandledRequest(userId) != null;
		}

		private RelationshipRequest GetUnhandledRequest(ObjectId userId)
		{
			if (IsA(userId))
			{
				if (BToARequest == null || BToARequest.Status != RelationshipRequestStatus.Unhandled)
					return null;
				return BToARequest;
			}
			else
			{
				if (AToBRequest == null || AToBRequest.Status != RelationshipRequestStatus.Unhandled)
					return null;
				return AToBRequest;
			}
		}

		public bool CanSendRequest(ObjectId userId)
		{
			var id2 = GetAnotherId(userId);
			return !AreFriends() && GetUnhandledRequest(userId) == null && GetUnhandledRequest(id2) == null && !IsBlocked(userId);
		}

		private void SendRequest(ObjectId userId, FriendshipRequest request)
		{
			if (!CanSendRequest(userId))
				throw new InvalidOperationException("Cannot send request");

			if (IsA(userId))
			{
				AToBRequest = request;
			}
			else
			{
				BToARequest = request;
			}
			UpdatedAtNow();
		}

		public void SendRequest(ObjectId userId, string validationMessage)
		{
			Guard.Argument(() => validationMessage).SatisfyValidationMessage();

			if (!CanSendRequest(userId))
				throw new InvalidOperationException("Cannot send request");

			var request = new FriendshipRequest(new DualShip(userId, GetAnotherId(userId)), validationMessage);

			SendRequest(userId, request);

			UpdatedAtNow();
		}

		public void SendRequest(ObjectId userId, IEnumerable<InvestigationAndAnswer> investigationAndAnswers)
		{
			Guard.Argument(() => investigationAndAnswers).SatisfyDomainRule();

			var request = new FriendshipRequest(new DualShip(userId, GetAnotherId(userId)), investigationAndAnswers);

			SendRequest(userId, request);

			UpdatedAtNow();
		}

		public void AbandonUnhandledRequest(ObjectId userId)
		{
			var request = GetUnhandledRequest(GetAnotherId(userId));
			if (request != null)
			{
				request.Abandon();
			}
			else
			{
				throw new InvalidOperationException("No unhandled request");
			}
			UpdatedAtNow();
		}

		public void RefuseRequest(ObjectId userId, string message, bool block = false)
		{
			var r = GetUnhandledRequest(userId);

			if (r == null)
				throw new InvalidOperationException("No unhandled request");

			r.Refuse(message, block);

			UpdatedAtNow();
		}

		public void AcceptRequest(ObjectId userId)
		{
			if (!HasUnhandledRequest(userId))
				throw new InvalidOperationException();

			var r = GetUnhandledRequest(userId);

			r.Accept();

			MakeFriends();

			UpdatedAtNow();
		}

		public bool WasFriends()
		{
			if (AreFriends())
			{
				return false;
			}
			else if (AToBFriendship?.IsValid == false && BToAFriendship?.IsValid == false)
			{
				return true;
			}
			else if (AToBFriendship == null && BToAFriendship == null)
			{
				return false;
			}
			else
			{
				var e = new DataCorruptionException();
				e.Data["this"] = this;
				throw e;
			}
		}

		/// <summary>
		/// Make friends an get this object to a clean state
		/// </summary>
		public void MakeFriendsAndDeleteUnhandledRequest()
		{
			if (AreFriends())
			{
				throw new InvalidOperationException("Already friends");
			}

			// Get to a clean state
			if (AToBRequest?.Status == RelationshipRequestStatus.Unhandled)
				AToBRequest = null;
			if (BToARequest?.Status == RelationshipRequestStatus.Unhandled)
				BToARequest = null;

			MakeFriends();

			UpdatedAtNow();
		}

		private void MakeFriends()
		{
			if (AreFriends())
			{
				throw new InvalidOperationException("Already friends");
			}

			if (WasFriends())
			{
				var sessionId = AToBFriendship.SessionId;
				AToBFriendship = new Friendship(new DualShip(AUserId, BUserId), sessionId, remindBirthday: true);
				BToAFriendship = new Friendship(new DualShip(BUserId, AUserId), sessionId, remindBirthday: true);
			}
			else
			{
				// New friends
				var desiredSessionId = ObjectId.GenerateNewId();
				AToBFriendship = new Friendship(new DualShip(AUserId, BUserId), desiredSessionId, remindBirthday: true);
				BToAFriendship = new Friendship(new DualShip(BUserId, AUserId), desiredSessionId, remindBirthday: true);

				PushDomainEvent(new FriendshipEstablishedEvent(AUserId, BUserId, desiredSessionId));
			}
			UpdatedAtNow();
		}

		public void InvalidateFriendship()
		{
			if (AreFriends())
			{
				AToBFriendship.Invalidate();
				BToAFriendship.Invalidate();
			}
			else
			{
				throw new InvalidOperationException("Are not friends");
			}
			UpdatedAtNow();
		}

		public void SetNickname(ObjectId senderId, string nickname)
		{
			Guard.Argument(() => nickname).MaxLength(16);
			if (!AreFriends())
				throw new InvalidOperationException();
			if (IsA(senderId))
				AToBFriendship.SetNickname(nickname);
			else
				BToAFriendship.SetNickname(nickname);
			UpdatedAtNow();
		}

		public void SetRemindBirthday(ObjectId senderId, bool remindBirthday)
		{
			if (!AreFriends())
				throw new InvalidOperationException();
			if (IsA(senderId))
				AToBFriendship.SetRemindBirthday(remindBirthday);
			else
				BToAFriendship.SetRemindBirthday(remindBirthday);
			UpdatedAtNow();
		}
	}
}
