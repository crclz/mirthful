using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.DiscussionAG
{
	public class Discussion : RootEntity
	{
		public Guid TopicId { get; private set; }
		public Guid SenderId { get; private set; }
		public string Text { get; private set; }
		public string Image { get; private set; }

		public NpgsqlTsVector TextTsv { get; private set; }

		private Discussion()
		{
			// Required by EF
		}

		public Discussion(Guid topicId, Guid senderId, string text, string image)
		{
			TopicId = topicId;
			SenderId = senderId;
			Text = text;
			Image = image;
		}
	}
}
