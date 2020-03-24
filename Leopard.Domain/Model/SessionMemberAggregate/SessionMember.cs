using Dawn;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leopard.Domain.Model.SessionMemberAggregate
{
	public class SessionMember : RootEntity
	{
		public ObjectId SessionId { get; private set; }
		public ObjectId UserId { get; private set; }

		public SessionNotificationPolicy NotificationPolicy { get; private set; }

		private List<NotificationRule> _notificationRules { get; set; } = new List<NotificationRule>();

		public IEnumerable<NotificationRule> NotificationRules => _notificationRules?.AsReadOnly();

		private SessionMember()
		{
			// Required by EF
		}

		public SessionMember(ObjectId sessionId, ObjectId userId)
		{
			SessionId = sessionId;
			UserId = userId;
			NotificationPolicy = SessionNotificationPolicy.Normal;
		}

		public void SetNotificationPolicy(SessionNotificationPolicy notificationPolicy)
		{
			Guard.Argument(() => notificationPolicy).Defined();
			NotificationPolicy = notificationPolicy;
			UpdatedAtNow();
		}

		public bool HasNotificationRuleWithUserId(ObjectId userId)
		{
			return _notificationRules.Any(p => p.UserId == userId);
		}

		public void AddNotificationRule(ObjectId userId, NotificationLevel level)
		{
			Guard.Argument(() => level).Defined().NotEqual(NotificationLevel.Normal);
			if (_notificationRules.Count < 50) // TODO: Be Explicit
			{
				if (HasNotificationRuleWithUserId(userId))
				{
					throw new ArgumentException("Already have a rule with the same notification id");
				}
				else
				{
					_notificationRules.Add(new NotificationRule(userId, level));
				}
			}
			else
			{
				throw new InvalidOperationException("Too many notification rules");
			}
			UpdatedAtNow();
		}

		public void RemoveNotificationRuleByUserId(ObjectId userId)
		{
			_notificationRules.RemoveAll(p => p.UserId == userId);
			UpdatedAtNow();
		}
	}
}
