using Dawn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.ReplyAG
{
	public class Reply : RootEntity
	{
		public Guid SenderId { get; private set; }
		public Guid PostId { get; private set; }
		public string Text { get; private set; }

		private Reply()
		{

		}

		public Reply(Guid senderId, Guid postId, string text)
		{
			SenderId = senderId;
			PostId = postId;
			SetText(text);
		}

		public void SetText(string text)
		{
			Guard.Argument(() => text).NotNull().MinLength(3);
			Text = text;
			UpdatedAtNow();
		}
	}
}
