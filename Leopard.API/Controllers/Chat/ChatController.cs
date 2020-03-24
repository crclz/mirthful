using Leopard.API.Filters;
using Leopard.Domain.Model.ChatMessageAggregate;
using Leopard.Domain.Model.SessionMemberAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Leopard.API.Controllers.Chat
{
	[Route("api/chat")]
	[ApiController]
	public partial class ChatController : ControllerBase
	{
		public ChatController(Repository<ChatMessage> chatMessageRepository,
			Repository<SessionMember> sessionMemberRepository,
			SessionStore sessionStore, MiddleStore store)
		{
			ChatMessageRepository = chatMessageRepository;
			SessionMemberRepository = sessionMemberRepository;
			SessionStore = sessionStore;
			Store = store;
		}

		public Repository<ChatMessage> ChatMessageRepository { get; }
		public Repository<SessionMember> SessionMemberRepository { get; }
		public SessionStore SessionStore { get; }
		public MiddleStore Store { get; }
	}

	public interface ISessionModel
	{
		public string SessionId { get; }
	}
}