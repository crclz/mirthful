using Leopard.Domain.Model.FriendshipDealerAggregates;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Query.DTO
{
	public class QFriendshipContext
	{
		public QFriendship Friendship { get; set; }
		public QFriendRequest RequestSent { get; set; }
		public QFriendRequest RequestReceived { get; set; }

		public static QFriendshipContext MyView(FriendshipDealer dealer, ObjectId me)
		{
			if (dealer == null)
				return null;

			if (dealer.IsA(me))
			{
				return new QFriendshipContext
				{
					Friendship = QFriendship.NormalView(dealer.AToBFriendship),
					RequestSent = QFriendRequest.NormalView(dealer.AToBRequest),
					RequestReceived = QFriendRequest.NormalView(dealer.BToARequest)
				};
			}
			else
			{
				return new QFriendshipContext
				{
					Friendship = QFriendship.NormalView(dealer.BToAFriendship),
					RequestSent = QFriendRequest.NormalView(dealer.BToARequest),
					RequestReceived = QFriendRequest.NormalView(dealer.AToBRequest)
				};
			}
		}
	}
}
