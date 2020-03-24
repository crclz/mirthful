using Leopard.API.Filters;
using Leopard.Domain.Model.GroupAggregate;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Infrastructure;
using MongoDB.Bson;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class GroupshipPipelineContext : IPipelineContext
	{
		public Repository<Group> GroupRepository { get; }
		public Repository<GroupshipDealer> GroupshipDealerRepository { get; }
		public MiddleStore Store { get; set; }

		public GroupshipPipelineContext(MiddleStore store,
			Repository<Group> groupRepository, Repository<GroupshipDealer> groupshipDealerRepository)
		{
			Store = store;
			GroupRepository = groupRepository;
			GroupshipDealerRepository = groupshipDealerRepository;
		}

		public ObjectId UserId => (ObjectId)Store.UserId;
		public Group Group { get; set; }
		public GroupshipDealer Dealer { get; set; }
	}
}
