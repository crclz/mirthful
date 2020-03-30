using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicAG
{
	public class TopicCreatedEvent : BaseNotification
	{
		public TopicCreatedEvent(Guid topicId, bool isGroup, Guid creatorId)
		{
			TopicId = topicId;
			IsGroup = isGroup;
			CreatorId = creatorId;
		}

		public Guid TopicId { get; private set; }
		public bool IsGroup { get; private set; }
		public Guid CreatorId { get; private set; }
	}
}
