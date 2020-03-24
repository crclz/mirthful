using Dawn;
using Leopard.Domain.Model.Relationships;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Leopard.Domain.Model.GroupshipDealerAggregate
{
	public class GroupshipDealer : RootEntity
	{
		public ObjectId UserId { get; private set; }

		public ObjectId GroupId { get; private set; }

		public GroupShip GroupShip { get; private set; }

		public GroupshipRequest Request { get; private set; }

		protected GroupshipDealer()
		{
			// Required by EF
		}

		public GroupshipDealer(ObjectId userId, ObjectId groupId)
		{
			UserId = userId;
			GroupId = groupId;
		}

		public bool IsUserInGroup()
		{
			// Not null && IsValid==true
			return GroupShip?.IsValid == true;
		}

		public bool IsBlocked()
		{
			return Request?.Status == RelationshipRequestStatus.RefusedAndBlocked;
		}

		public bool HasUnhandledRequest()
		{
			return Request?.Status == RelationshipRequestStatus.Unhandled;
		}

		public void AbandonUnhandledRequest()
		{
			if (Request?.Status == RelationshipRequestStatus.Unhandled)
				Request.Abandon();
			else
				throw new InvalidOperationException();
			UpdatedAtNow();
		}

		private RelationshipRequest GetUnhandledRequest()
		{
			return Request?.Status == RelationshipRequestStatus.Unhandled ? Request : null;
		}

		public bool CanSendRequest()
		{
			return !IsUserInGroup() && !HasUnhandledRequest() && !IsBlocked();
		}

		protected void SendRequest(GroupshipRequest request)
		{
			Guard.Argument(() => request).NotNull();
			if (!CanSendRequest())
				throw new InvalidOperationException();

			Request = request;
			UpdatedAtNow();
		}

		public void SendRequest(string validationMessage)
		{
			Guard.Argument(() => validationMessage).NotNull().MaxLength(32);
			var request = new GroupshipRequest(GroupId, UserId, validationMessage);
			SendRequest(request);

			UpdatedAtNow();
		}

		public void SendRequest(IEnumerable<InvestigationAndAnswer> investigationAndAnswers)
		{
			Guard.Argument(() => investigationAndAnswers).SatisfyDomainRule();
			var request = new GroupshipRequest(GroupId, UserId, investigationAndAnswers);
			SendRequest(request);

			UpdatedAtNow();
		}

		public void AbandonUnhandledGroupship()
		{
			if (!HasUnhandledRequest())
				throw new InvalidOperationException();
			Request.Abandon();
			UpdatedAtNow();
		}

		public void RefuseRequest(string message, bool block)
		{
			if (!HasUnhandledRequest())
				throw new InvalidOperationException();

			Request.Refuse(message, block);

			UpdatedAtNow();
		}

		public void AcceptRequest()
		{
			if (!HasUnhandledRequest())
				throw new InvalidOperationException();

			Request.Accept();

			JoinGroup(GroupRole.Normal);

			UpdatedAtNow();
		}

		public void JoinGroupAndDeleteUnhandledRequest()
		{
			Request = HasUnhandledRequest() ? null : Request;
			JoinGroup(GroupRole.Normal);
			UpdatedAtNow();
		}

		private void JoinGroup(GroupRole role)
		{
			Guard.Argument(() => role).Defined();
			if (IsUserInGroup())
			{
				throw new InvalidOperationException("Already in group");
			}
			else
			{
				GroupShip = new GroupShip(GroupId, UserId, role, remindBirthday: true);
			}

			PushDomainEvent(new UserJoinGroupEvent(UserId, GroupId));
			UpdatedAtNow();
		}

		public void QuitGroup()
		{
			if (IsUserInGroup())
			{
				GroupShip.Invalidate();
				UpdatedAtNow();

				PushDomainEvent(new UserQuitGroupEvent(UserId, GroupId));
			}
			else
			{
				throw new InvalidOperationException("User is not in group");
			}
			UpdatedAtNow();
		}

		public void SetUserDisplayname(string name)
		{
			if (!IsUserInGroup())
				throw new InvalidOperationException();
			GroupShip.SetUserDisplayName(name);
			UpdatedAtNow();
		}

		public void SetGroupDisplayName(string name)
		{
			if (!IsUserInGroup())
				throw new InvalidOperationException();
			GroupShip.SetGroupDisplayName(name);
			UpdatedAtNow();
		}

		public void SetRemindBirthday(bool remindBirthday)
		{
			if (!IsUserInGroup())
				throw new InvalidOperationException();
			GroupShip.SetRemindBirthday(remindBirthday);
			UpdatedAtNow();
		}

		public void SetRole(GroupRole role)
		{
			if (!IsUserInGroup())
				throw new InvalidOperationException();
			GroupShip.SetRole(role);
			UpdatedAtNow();
		}
	}
}
