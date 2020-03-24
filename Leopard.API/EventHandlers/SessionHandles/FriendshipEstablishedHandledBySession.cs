using Leopard.Domain.Model;
using Leopard.Domain.Model.FriendshipDealerAggregates;
using Leopard.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.SessionHandles
{
	public class FriendshipEstablishedHandledBySession : INotificationHandler<FriendshipEstablishedEvent>
	{
		public FriendshipEstablishedHandledBySession(Repository<Session> sessionRepository)
		{
			SessionRepository = sessionRepository;
		}

		public Repository<Session> SessionRepository { get; }

		public async Task Handle(FriendshipEstablishedEvent e, CancellationToken cancellationToken)
		{
			var session = new Session(e.DesiredSessionId);

			await SessionRepository.PutAsync(session, new DeduplicationToken(e.Id, "FriendshipEstablishedHandledBySession", null));
		}
	}
}
