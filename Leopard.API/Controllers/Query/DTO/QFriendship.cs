using Leopard.Domain.Model.FriendshipDealerAggregates;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Query.DTO
{
	public class QFriendship
	{
		public string Id { get; private set; }
		public long CreatedAt { get; private set; }
		public string SenderId { get; private set; }
		public string ReceiverId { get; private set; }
		public bool IsValid { get; private set; }
		public string Nickname { get; private set; }
		public bool RemindBirthday { get; private set; }
		public string SessionId { get; private set; }
		public long UpdatedAt { get; private set; }

		public static QFriendship NormalView(Friendship f)
		{
			if (f == null)
				return null;

			return new QFriendship
			{
				Id = f.Id.ToString(),
				CreatedAt = f.CreatedAt,
				SenderId = f.Dual.SenderId.ToString(),
				ReceiverId = f.Dual.ReceiverId.ToString(),
				IsValid = f.IsValid,
				Nickname = f.Nickname,
				RemindBirthday = f.RemindBirthday,
				SessionId = f.SessionId.ToString(),
				UpdatedAt = f.UpdatedAt
			};
		}
	}
}
