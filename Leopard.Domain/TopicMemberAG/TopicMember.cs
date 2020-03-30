using Dawn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicMemberAG
{
	public class TopicMember : RootEntity
	{
		public Guid TopicId { get; private set; }
		public Guid UserId { get; private set; }
		public MemberRole Role { get; private set; }

		private TopicMember()
		{

		}

		public TopicMember(Guid topicId, Guid userId, MemberRole role, bool suppressJoinTopicEvent = false)
		{
			Guard.Argument(() => role).Defined();

			TopicId = topicId;
			UserId = userId;
			Role = role;

			if (!suppressJoinTopicEvent)
			{
				PushDomainEvent(new JoinTopicEvent(topicId, userId));
			}
		}

		public void SetRole(MemberRole role)
		{
			Guard.Argument(() => role).Defined();
			Role = role;
			UpdatedAtNow();
		}
	}

	public enum MemberRole
	{
		Normal = 0,
		Admin = 1,
		Super = 2,
	}
}
