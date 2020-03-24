using Leopard.Domain.Model.FriendshipDealerAggregates;
using Leopard.Domain.Model.SessionMemberAggregate;
using Leopard.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.SessionMemberHandles
{
	public class FriendshipEstablishedHandledBySessionMember : INotificationHandler<FriendshipEstablishedEvent>
	{
		public FriendshipEstablishedHandledBySessionMember(Repository<SessionMember> sessionMemberReposiory)
		{
			SessionMemberReposiory = sessionMemberReposiory;
		}

		public Repository<SessionMember> SessionMemberReposiory { get; }

		public async Task Handle(FriendshipEstablishedEvent e, CancellationToken cancellationToken)
		{
			// Add A as member
			var a = new SessionMember(e.DesiredSessionId, e.AUserId);
			await SessionMemberReposiory.PutAsync(a,
				new DeduplicationToken(e.Id, "FriendshipEstablishedHandledBySessionMember", "A"));

			// Add B ad member
			var b = new SessionMember(e.DesiredSessionId, e.BUserId);
			await SessionMemberReposiory.PutAsync(b,
				new DeduplicationToken(e.Id, "FriendshipEstablishedHandledBySessionMember", "B"));
		}
	}
}
