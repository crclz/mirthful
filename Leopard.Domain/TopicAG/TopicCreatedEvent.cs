using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicAG
{
	public class TopicCreatedEvent : BaseNotification
	{
		public TopicCreatedEvent(ObjectId topicId, bool isGroup, ObjectId creatorId)
		{
			TopicId = topicId;
			IsGroup = isGroup;
			CreatorId = creatorId;
		}

		public ObjectId TopicId { get; private set; }
		public bool IsGroup { get; private set; }
		public ObjectId CreatorId { get; private set; }
	}
}
