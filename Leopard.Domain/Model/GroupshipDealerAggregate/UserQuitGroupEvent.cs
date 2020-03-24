using MongoDB.Bson;

namespace Leopard.Domain.Model.GroupshipDealerAggregate
{
	public class UserQuitGroupEvent : BaseNotification
	{
		public UserQuitGroupEvent(ObjectId userId, ObjectId groupId)
		{
			UserId = userId;
			GroupId = groupId;
		}

		public ObjectId UserId { get; private set; }
		public ObjectId GroupId { get; private set; }
	}
}
