using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.ReportAG
{
	public class Report : RootEntity
	{
		public ObjectId SenderId { get; private set; }
		public ObjectId CommentId { get; private set; }
		public string Title { get; private set; }
		public string Text { get; private set; }
		public bool Handled { get; private set; }

		private Report()
		{

		}

		public Report(ObjectId senderId, ObjectId commentId, string title, string text)
		{
			SenderId = senderId;
			CommentId = commentId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Text = text ?? throw new ArgumentNullException(nameof(text));
			Handled = false;
		}
	}
}
