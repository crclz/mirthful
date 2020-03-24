using MongoDB.Bson;
using System;

namespace Leopard.Domain.Model.ChatMessageAggregate
{
	public class BirthdayInfo : ValueObject
	{
		private BirthdayInfo()
		{

		}

		public BirthdayInfo(ObjectId userId, DateTimeOffset birthday)
		{
			UserId = userId;
			Birthday = birthday;
		}

		public ObjectId UserId { get; private set; }
		public DateTimeOffset Birthday { get; private set; }
	}
}
