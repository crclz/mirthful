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
		/// <summary>
		/// 普通成员
		/// </summary>
		Normal = 0,

		/// <summary>
		/// 管理员
		/// </summary>
		Admin = 1,

		/// <summary>
		/// 超级管理员。超级管理员默认是小组的创建者。
		/// 只有超级管理员能够处理请求。
		/// </summary>
		Super = 2,
	}
}
