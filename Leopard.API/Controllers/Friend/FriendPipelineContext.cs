using Leopard.API.Filters;
using Leopard.Domain.Model.FriendshipDealerAggregates;
using Leopard.Domain.Model.UserAggregate;
using Leopard.Infrastructure;
using MongoDB.Bson;

namespace Leopard.API.Controllers.Friend
{
	public class FriendPipelineContext : IPipelineContext
	{
		public MiddleStore Store { get; }
		public Repository<User> UserRepository { get; }
		public Repository<FriendshipDealer> FriendshipDealerRepository { get; }
		public ObjectId UserId => (ObjectId)Store.UserId;

		public FriendPipelineContext(MiddleStore store,
			Repository<User> userRepository, Repository<FriendshipDealer> friendshipDealerRepository)
		{
			Store = store;
			UserRepository = userRepository;
			FriendshipDealerRepository = friendshipDealerRepository;
		}

		public User TargetUser { get; set; }
		public FriendshipDealer Dealer { get; set; }
	}
}
