using Leopard.Domain.Model;
using Leopard.Domain.Model.GroupAggregate;
using Leopard.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.SessionHandles
{
	public class GroupCreatedHandledBySession : INotificationHandler<GroupCreatedEvent>
	{
		public GroupCreatedHandledBySession(Repository<Session> sessionRepository)
		{
			SessionRepository = sessionRepository;
		}

		public Repository<Session> SessionRepository { get; }

		public async Task Handle(GroupCreatedEvent e, CancellationToken cancellationToken)
		{
			var session = new Session(e.DesiredSessionId);

			await SessionRepository.PutAsync(session, new DeduplicationToken(e.Id, "GroupCreatedHandledBySession", null));
		}
	}
}
