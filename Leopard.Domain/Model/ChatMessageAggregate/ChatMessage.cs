using Dawn;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leopard.Domain.Model.ChatMessageAggregate
{
	public class ChatMessage : RootEntity
	{
		public ObjectId SessionId { get; private set; }

		[BsonIgnoreIfNull]
		public ObjectId? SenderId { get; private set; }
		public ChatMessageType Type { get; private set; }

		[BsonIgnoreIfNull]
		public string Text { get; private set; }

		[BsonIgnoreIfNull]
		public ObjectId? InvitationGroupId { get; private set; }

		[BsonIgnoreIfNull]
		public ObjectId? BusinessCardUserId { get; private set; }

		[BsonIgnoreIfNull]
		private List<string> _blobs { get; set; }

		//[BsonIgnore]
		public IEnumerable<string> Blobs => _blobs?.AsReadOnly();


		[BsonIgnoreIfNull]
		public BirthdayInfo BirthdayInfo { get; private set; }

		protected ChatMessage()
		{
			// Required by EF
		}

		private ChatMessage(ObjectId sessionId, ObjectId? senderId)
		{
			SessionId = sessionId;
			SenderId = senderId;
		}

		public static ChatMessage CreateTextMessage(ObjectId sessionId, ObjectId senderId, string text)
		{
			Guard.Argument(() => text).SatisfyChatText();
			var x = new ChatMessage(sessionId, senderId);
			x.Text = text;
			x.Type = ChatMessageType.Text;
			return x;
		}

		public static ChatMessage CreateTextAndImageMessage(ObjectId sessionId, ObjectId senderId, string text, IEnumerable<string> images)
		{
			Guard.Argument(() => text).SatisfyChatText();
			Guard.Argument(() => images).DoesNotContainNull();

			var x = new ChatMessage(sessionId, senderId);
			x.Text = text;
			x._blobs = images.ToList();
			x.Type = ChatMessageType.TextAndImages;
			return x;
		}

		public static ChatMessage CreateFileMessage(ObjectId sessionId, ObjectId senderId, string blob)
		{
			Guard.Argument(() => blob).NotNull();

			var x = new ChatMessage(sessionId, senderId);
			x._blobs = new List<string> { blob };
			x.Type = ChatMessageType.File;
			return x;
		}

		public static ChatMessage CreateVoiceMessage(ObjectId sessionId, ObjectId senderId, string blob)
		{
			Guard.Argument(() => blob).NotNull();

			var x = new ChatMessage(sessionId, senderId);
			x._blobs = new List<string> { blob };
			x.Type = ChatMessageType.Voice;
			return x;
		}

		public static ChatMessage CreateBusinessCardMessage(ObjectId sessionId, ObjectId senderId, ObjectId cardUserId)
		{
			var x = new ChatMessage(sessionId, senderId);
			x.Type = ChatMessageType.BusinessCard;
			x.BusinessCardUserId = cardUserId;
			return x;
		}

		public static ChatMessage CreateInvitaionMessage(ObjectId sessionId, ObjectId senderId, ObjectId groupId)
		{
			var x = new ChatMessage(sessionId, senderId);
			x.Type = ChatMessageType.BusinessCard;
			x.InvitationGroupId = groupId;
			return x;
		}

		public static ChatMessage CreateBirthdayMessage(ObjectId sessionId, ObjectId birthdayUserId, DateTimeOffset birthday)
		{
			var info = new BirthdayInfo(birthdayUserId, birthday);
			var x = new ChatMessage(sessionId, null);
			x.Type = ChatMessageType.Birthday;
			x.BirthdayInfo = info;
			return x;
		}
	}

	public enum ChatMessageType
	{
		Text = 0,
		TextAndImages = 1,
		File = 2,
		Voice = 3,
		BusinessCard = 4,
		Invitation = 5,
		Birthday = 6,
	}
}
