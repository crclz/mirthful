﻿using Dawn;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.CommentAG
{
	public class Comment : RootEntity
	{
		public ObjectId SenderId { get; private set; }
		public ObjectId WorkId { get; private set; }
		public string Title { get; private set; }
		public string Text { get; private set; }
		public int Rating { get; private set; }

		private Comment()
		{
		}

		public Comment(ObjectId senderId, ObjectId workId, string title, string text, int rating)
		{
			SenderId = senderId;
			WorkId = workId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Text = text ?? throw new ArgumentNullException(nameof(text));
			SetRating(rating);
		}

		private void SetRating(int rating)
		{
			Guard.Argument(() => rating).InRange(1, 5);
			Rating = rating;
		}
	}
}
