using Leopard.Domain.Model.GroupAggregate;
using Leopard.Domain.Model.SessionMemberAggregate;
using Leopard.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.SessionMemberHandles
{
	public class GroupCreatedHandledBySessionMember : INotificationHandler<GroupCreatedEvent>
	{
		public GroupCreatedHandledBySessionMember(Repository<SessionMember> sessionMemberReposiory)
		{
			SessionMemberReposiory = sessionMemberReposiory;
		}

		public Repository<SessionMember> SessionMemberReposiory { get; }

		public async Task Handle(GroupCreatedEvent e, CancellationToken cancellationToken)
		{
			// Add A as member
			var a = new SessionMember(e.DesiredSessionId, e.CreatorId);
			await SessionMemberReposiory.PutAsync(a,
				new DeduplicationToken(e.Id, "FriendshipEstablishedHandledBySessionMember", "A"));
		}
	}
}
