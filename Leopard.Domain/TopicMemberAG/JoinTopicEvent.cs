using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicMemberAG
{
	public class JoinTopicEvent : BaseNotification
	{
		public Guid TopicId { get; private set; }
		public Guid UserId { get; private set; }

		public JoinTopicEvent(Guid topicId, Guid userId)
		{
			TopicId = topicId;
			UserId = userId;
		}
	}
}
