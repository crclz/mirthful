using MediatR;
using MongoDB.Bson;

namespace Leopard.Domain
{
	public abstract class BaseNotification : INotification
	{
		public ObjectId Id { get; private set; }
		public bool AllAck { get; private set; }

		public BaseNotification()
		{
			Id = ObjectId.GenerateNewId();
			AllAck = false;
		}

		public void AllAcknowledged()
		{
			AllAck = true;
		}
	}
}
