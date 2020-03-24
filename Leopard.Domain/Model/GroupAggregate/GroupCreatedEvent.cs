using MongoDB.Bson;

namespace Leopard.Domain.Model.GroupAggregate
{
	public class GroupCreatedEvent : BaseNotification
	{
		public GroupCreatedEvent(ObjectId groupId, ObjectId desiredSessionId, ObjectId creatorId)
		{
			DesiredSessionId = desiredSessionId;
			CreatorId = creatorId;
			GroupId = groupId;
		}

		public ObjectId DesiredSessionId { get; private set; }
		public ObjectId CreatorId { get; private set; }
		public ObjectId GroupId { get; private set; }
	}
}
