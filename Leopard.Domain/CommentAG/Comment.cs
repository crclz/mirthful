using Dawn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.CommentAG
{
	public class Comment : RootEntity
	{
		public Guid SenderId { get; private set; }
		public Guid WorkId { get; private set; }
		public string Title { get; private set; }
		public string Text { get; private set; }
		public int Rating { get; private set; }
		public int AgreeCount { get; private set; } = 0;
		public int DisagreeCount { get; private set; } = 0;

		private Comment()
		{
		}

		public Comment(Guid senderId, Guid workId, string title, string text, int rating)
		{
			SenderId = senderId;
			WorkId = workId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Text = text ?? throw new ArgumentNullException(nameof(text));
			SetRating(rating);
		}

		public void SetRating(int rating)
		{
			Guard.Argument(() => rating).InRange(1, 5);
			Rating = rating;
			UpdatedAtNow();
		}

		public void SetAgreeCount(int agreeCount)
		{
			Guard.Argument(() => agreeCount).Min(0);
			AgreeCount = agreeCount;
			UpdatedAtNow();
		}

		public void SetDisagreeCount(int disagreeCount)
		{
			Guard.Argument(() => disagreeCount).Min(0);
			DisagreeCount = disagreeCount;
			UpdatedAtNow();
		}
	}
}
