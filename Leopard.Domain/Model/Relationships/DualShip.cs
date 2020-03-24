using MongoDB.Bson;
using System;

namespace Leopard.Domain.Model.Relationships
{
	public class DualShip : ValueObject
	{
		public ObjectId SenderId { get; private set; }

		public ObjectId ReceiverId { get; private set; }

		protected DualShip()
		{
			// Required by EF
		}

		public DualShip(ObjectId senderId, ObjectId receiverId)
		{
			if (senderId == receiverId)
			{
				throw new ArgumentException($"Argument senderId ({senderId}) and receiverId ({receiverId}) should no be equal.");
			}
			SenderId = senderId;
			ReceiverId = receiverId;
		}
	}
}
