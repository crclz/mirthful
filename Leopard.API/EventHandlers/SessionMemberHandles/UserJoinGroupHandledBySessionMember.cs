using Leopard.Domain.Model.GroupAggregate;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Domain.Model.SessionMemberAggregate;
using Leopard.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.SessionMemberHandles
{
	public class UserJoinGroupHandledBySessionMember : INotificationHandler<UserJoinGroupEvent>
	{
		public UserJoinGroupHandledBySessionMember(
			Repository<SessionMember> sessionMemberRepository, Repository<Group> groupRepository)
		{
			SessionMemberRepository = sessionMemberRepository;
			GroupRepository = groupRepository;
		}

		public Repository<SessionMember> SessionMemberRepository { get; }
		public Repository<Group> GroupRepository { get; }

		public async Task Handle(UserJoinGroupEvent e, CancellationToken cancellationToken)
		{
			var group = await GroupRepository.FirstOrDefaultAsync(p => p.Id == e.GroupId);

			var member = new SessionMember(group.SessionId, e.UserId);

			await SessionMemberRepository.PutAsync(member,
				new DeduplicationToken(e.Id, "UserJoinGroupHandledBySessionMember", null));
		}
	}
}
