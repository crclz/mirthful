using Leopard.Domain.Model.ChatMessageAggregate;
using Leopard.Domain.Model.UserAggregate;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;

namespace Leopard.API.Controllers.Query.DTO
{
	public class QChatMessage
	{
		public string Id { get; private set; }
		public string SessionId { get; private set; }
		public string SenderId { get; private set; }
		public ChatMessageType Type { get; private set; }
		public string Text { get; private set; }
		public string InvitationGroupId { get; private set; }
		public string BusinessCardUserId { get; private set; }
		public IEnumerable<string> Blobs { get; private set; }
		public long CreatedAt { get; private set; }
		public QBirthdayInfo BirthdayInfo { get; private set; }

		public static QChatMessage NormalView(ChatMessage p)
		{
			if (p == null)
				return null;

			return new QChatMessage
			{
				Id = p.Id.ToString(),
				SessionId = p.SessionId.ToString(),
				SenderId = p.SenderId.ToString(),
				Type = p.Type,
				Text = p.Text,
				InvitationGroupId = p.InvitationGroupId.ToString(),
				BusinessCardUserId = p.BusinessCardUserId.ToString(),
				Blobs = p.Blobs,
				CreatedAt = p.CreatedAt,
				BirthdayInfo = QBirthdayInfo.NormalView(p.BirthdayInfo)
			};
		}
	}

	public class QChatDisplay
	{
		public QChatMessage ChatMessage { get; set; }
		public QUser User { get; set; }

		public static QChatDisplay NormalView(ChatMessage msg, User user)
		{
			var a = new QChatDisplay();
			a.ChatMessage = QChatMessage.NormalView(msg);
			a.User = QUser.NormalView(user);
			return a;
		}
	}

	public class QBirthdayInfo
	{
		public string UserId { get; set; }
		public DateTimeOffset Birthday { get; set; }

		public static QBirthdayInfo NormalView(BirthdayInfo p)
		{
			if (p == null)
				return null;

			return new QBirthdayInfo
			{
				UserId = p.UserId.ToString(),
				Birthday = p.Birthday
			};
		}
	}
}
