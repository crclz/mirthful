using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicMemberAG
{
	public class JoinTopicEvent : BaseNotification
	{
		public ObjectId TopicId { get; private set; }
		public ObjectId UserId { get; private set; }

		public JoinTopicEvent(ObjectId topicId, ObjectId userId)
		{
			TopicId = topicId;
			UserId = userId;
		}
	}
}
