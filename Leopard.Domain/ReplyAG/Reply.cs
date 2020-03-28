using Dawn;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.ReplyAG
{
	public class Reply : RootEntity
	{
		public ObjectId SenderId { get; private set; }
		public ObjectId PostId { get; private set; }
		public string Text { get; private set; }

		private Reply()
		{

		}

		public Reply(ObjectId senderId, ObjectId postId, string text)
		{
			SenderId = senderId;
			PostId = postId;
			SetText(text);
		}

		public void SetText(string text)
		{
			Guard.Argument(() => text).NotNull().MinLength(25);
			Text = text;
			UpdatedAtNow();
		}
	}
}
