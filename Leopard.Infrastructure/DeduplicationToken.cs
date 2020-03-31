using System;
using Dawn;

namespace Leopard.Infrastructure
{
	public class DeduplicationToken
	{
		public Guid Id { get; private set; }
		public Guid EventId { get; private set; }
		public string HandlerType { get; private set; }
		public string Additional { get; private set; }

		public DeduplicationToken(Guid eventId, string handlerType, string additional)
		{
			Guard.Argument(() => handlerType).NotNull();
			Id = Guid.NewGuid();
			EventId = eventId;
			HandlerType = handlerType;
			Additional = additional;
		}
	}
}
