using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.AttitudeAG
{
	public class Attitude : RootEntity
	{
		public ObjectId SenderId { get; private set; }
		public ObjectId ReceiverId { get; private set; }
		public bool Agree { get; private set; }

		private Attitude()
		{

		}

		public Attitude(ObjectId senderId, ObjectId receiverId, bool agree)
		{
			SenderId = senderId;
			ReceiverId = receiverId;
			Agree = agree;
		}
	}
}
