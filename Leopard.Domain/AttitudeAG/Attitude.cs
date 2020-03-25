using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.AttitudeAG
{
	public class Attitude : RootEntity
	{
		public ObjectId SenderId { get; private set; }
		public ObjectId CommentId { get; private set; }
		public bool Agree { get; private set; }

		private Attitude()
		{

		}

		public Attitude(ObjectId senderId, ObjectId commentId, bool agree)
		{
			SenderId = senderId;
			CommentId = commentId;
			Agree = agree;
		}

		public void SetAgree(bool agree)
		{
			Agree = agree;
			UpdatedAtNow();
		}
	}
}
