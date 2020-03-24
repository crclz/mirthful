using MongoDB.Bson;

namespace Leopard.Domain.Model.FriendshipDealerAggregates
{
	public class FriendshipEstablishedEvent : BaseNotification
	{
		public FriendshipEstablishedEvent(ObjectId aUserId, ObjectId bUserId, ObjectId desiredSessionId)
		{
			AUserId = aUserId;
			BUserId = bUserId;
			DesiredSessionId = desiredSessionId;
		}

		public ObjectId AUserId { get; private set; }
		public ObjectId BUserId { get; private set; }
		public ObjectId DesiredSessionId { get; private set; }
	}
}
