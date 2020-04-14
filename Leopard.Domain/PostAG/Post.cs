using Dawn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.PostAG
{
	public class Post : RootEntity
	{
		public Guid SenderId { get; private set; }
		public Guid TopicId { get; private set; }
		public string Title { get; private set; }
		public string Text { get; private set; }
		public bool IsPinned { get; private set; }
		public bool IsEssence { get; private set; }

		private Post()
		{

		}

		public Post(Guid senderId, Guid topicId, string text, string title = null)
		{
			SenderId = senderId;
			TopicId = topicId;
			Title = title;
			Text = text;
		}

		public void SetPinned(bool isPinned)
		{
			IsPinned = isPinned;
			UpdatedAtNow();
		}

		public void SetText(string text)
		{
			Guard.Argument(() => text).NotNull().MinLength(15);
			Text = text;
		}

		public void SetEssence(bool isEssence)
		{
			IsEssence = isEssence;
			UpdatedAtNow();
		}
	}
}
