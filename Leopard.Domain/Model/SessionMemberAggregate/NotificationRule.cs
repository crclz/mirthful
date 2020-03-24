using Dawn;
using MongoDB.Bson;

namespace Leopard.Domain.Model.SessionMemberAggregate
{
	public class NotificationRule : ValueObject
	{
		public ObjectId UserId { get; private set; }

		public NotificationLevel Level { get; private set; }

		protected NotificationRule()
		{
			// Required by EF
		}

		public NotificationRule(ObjectId userId, NotificationLevel level)
		{
			Guard.Argument(() => level).Defined();
			UserId = userId;
			Level = level;
		}
	}
}
