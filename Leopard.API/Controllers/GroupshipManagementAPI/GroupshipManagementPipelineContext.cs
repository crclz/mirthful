using Leopard.API.Filters;
using Leopard.Domain.Model.GroupAggregate;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Infrastructure;
using MongoDB.Bson;

namespace Leopard.API.Controllers.GroupshipManagementAPI
{
	public class GroupshipManagementPipelineContext : IPipelineContext
	{
		public MiddleStore Store { get; }
		public Repository<Group> GroupRepository { get; }
		public Repository<GroupshipDealer> GroupshipDealerRepository { get; }

		public GroupshipManagementPipelineContext(MiddleStore store, Repository<Group> groupRepository,
			Repository<GroupshipDealer> groupshipDealerRepository)
		{
			Store = store;
			GroupRepository = groupRepository;
			GroupshipDealerRepository = groupshipDealerRepository;
		}

		public ObjectId MeId => (ObjectId)Store.UserId;
		public ObjectId ItId { get; set; }
		public Group Group { get; set; }
		public GroupshipDealer MyDealer { get; set; }
		public GroupshipDealer ItsDealer { get; set; }
	}
}
