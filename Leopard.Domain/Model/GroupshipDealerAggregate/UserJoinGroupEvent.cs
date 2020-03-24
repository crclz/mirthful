using MongoDB.Bson;

namespace Leopard.Domain.Model.GroupshipDealerAggregate
{
	public class UserJoinGroupEvent : BaseNotification
	{
		public UserJoinGroupEvent(ObjectId userId, ObjectId groupId)
		{
			UserId = userId;
			GroupId = groupId;
		}

		public ObjectId UserId { get; private set; }
		public ObjectId GroupId { get; private set; }
	}
}
