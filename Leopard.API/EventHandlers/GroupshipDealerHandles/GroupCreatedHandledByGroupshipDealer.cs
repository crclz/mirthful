using Leopard.Domain.Model.GroupAggregate;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.GroupshipDealerHandles
{
	public class GroupCreatedHandledByGroupshipDealer : INotificationHandler<GroupCreatedEvent>
	{
		public GroupCreatedHandledByGroupshipDealer(Repository<GroupshipDealer> dealerRepository)
		{
			DealerRepository = dealerRepository;
		}

		public Repository<GroupshipDealer> DealerRepository { get; }

		public async Task Handle(GroupCreatedEvent e, CancellationToken cancellationToken)
		{
			var dealer = new GroupshipDealer(e.CreatorId, e.GroupId);
			dealer.JoinGroupAndDeleteUnhandledRequest();
			dealer.SetRole(GroupRole.Founder);

			await DealerRepository.PutAsync(dealer, new DeduplicationToken(e.Id, "GroupCreatedHandledByGroupshipDealer", null));
		}
	}
}
