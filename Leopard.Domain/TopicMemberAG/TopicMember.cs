using Dawn;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicMemberAG
{
	public class TopicMember : RootEntity
	{
		public ObjectId TopicId { get; private set; }
		public ObjectId UserId { get; private set; }
		public MemberRole Role { get; private set; }

		public TopicMember(ObjectId topicId, ObjectId userId, MemberRole role, bool suppressJoinTopicEvent = false)
		{
			Guard.Argument(() => role).Defined();

			TopicId = topicId;
			UserId = userId;
			Role = role;

			if (!suppressJoinTopicEvent)
			{
				PushDomainEvent(new JoinTopicEvent(Id, userId));
			}
		}

		public void SetRole(MemberRole role)
		{
			Guard.Argument(() => role).Defined();
			Role = role;
		}
	}

	public enum MemberRole
	{
		Normal = 0,
		Admin = 1,
		Super = 2,
	}
}
