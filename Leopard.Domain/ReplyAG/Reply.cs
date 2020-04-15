using Dawn;
using NpgsqlTypes;
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

		public NpgsqlTsVector TextTsv { get; private set; }

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
			Guard.Argument(() => text).NotNull().MinLength(1);
			Text = text;
			UpdatedAtNow();
		}
	}
}
