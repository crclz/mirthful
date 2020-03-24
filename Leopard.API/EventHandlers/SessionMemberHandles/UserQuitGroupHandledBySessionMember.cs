using Leopard.Domain.Model.GroupAggregate;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Domain.Model.SessionMemberAggregate;
using Leopard.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.SessionMemberHandles
{
	public class UserQuitGroupHandledBySessionMember : INotificationHandler<UserQuitGroupEvent>
	{
		public UserQuitGroupHandledBySessionMember(
			Repository<SessionMember> sessionMemberRepository, Repository<Group> groupRepository)
		{
			SessionMemberRepository = sessionMemberRepository;
			GroupRepository = groupRepository;
		}

		public Repository<SessionMember> SessionMemberRepository { get; }
		public Repository<Group> GroupRepository { get; }

		public async Task Handle(UserQuitGroupEvent e, CancellationToken cancellationToken)
		{
			var group = await GroupRepository.FirstOrDefaultAsync(p => p.Id == e.GroupId);

			var member = await SessionMemberRepository.FirstOrDefaultAsync(p => p.Id == group.SessionId);
			member.MarkForDeletion();

			await SessionMemberRepository.PutAsync(member,
				new DeduplicationToken(e.Id, "UserQuitGroupHandledBySessionMember", null));
		}
	}
}
