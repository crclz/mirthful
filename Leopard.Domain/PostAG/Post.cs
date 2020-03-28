using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.PostAG
{
	public class Post : RootEntity
	{
		public ObjectId SenderId { get; private set; }
		public ObjectId TopicId { get; private set; }

		/// <summary>
		/// Only post of group need title
		/// </summary>
		public string Title { get; private set; }
		public string Text { get; private set; }
		public string Image { get; private set; }

		public bool IsPinned { get; private set; }
		public bool IsEssence { get; private set; }

		private Post()
		{

		}

		public Post(ObjectId senderId, ObjectId topicId, string text, string title = null, string imageUrl = null)
		{
			SenderId = senderId;
			TopicId = topicId;
			Title = title;
			Text = text;
			Image = imageUrl;
		}

		public void SetPinned(bool isPinned)
		{
			IsPinned = isPinned;
			UpdatedAtNow();
		}

		public void SetEssence(bool isEssence)
		{
			IsEssence = isEssence;
			UpdatedAtNow();
		}
	}
}
