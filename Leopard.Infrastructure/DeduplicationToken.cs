using Dawn;
using MongoDB.Bson;

namespace Leopard.Infrastructure
{
	public class DeduplicationToken
	{
		public DeduplicationToken(ObjectId eventId, string handlerType, string additional)
		{
			Guard.Argument(() => handlerType).NotNull();
			Id = ObjectId.GenerateNewId();
			EventId = eventId;
			HandlerType = handlerType;
			Additional = additional;
		}

		public ObjectId Id { get; private set; }
		public ObjectId EventId { get; private set; }
		public string HandlerType { get; private set; }
		public string Additional { get; private set; }
	}
}
