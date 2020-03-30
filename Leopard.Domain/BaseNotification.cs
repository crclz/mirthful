using MediatR;
using System;

namespace Leopard.Domain
{
	public abstract class BaseNotification : INotification
	{
		public Guid Id { get; private set; }
		public bool AllAck { get; private set; }

		public BaseNotification()
		{
			Id = Guid.NewGuid();
			AllAck = false;
		}

		public void AllAcknowledged()
		{
			AllAck = true;
		}
	}
}
