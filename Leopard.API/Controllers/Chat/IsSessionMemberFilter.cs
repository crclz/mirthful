using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.Model.SessionMemberAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Chat
{
	public class IsSessionMemberFilter : IAsyncActionFilter
	{
		public IsSessionMemberFilter(Repository<SessionMember> sessionMemberRepository,
			SessionStore sessionStore, MiddleStore store)
		{
			SessionMemberRepository = sessionMemberRepository;
			SessionStore = sessionStore;
			Store = store;
		}

		public Repository<SessionMember> SessionMemberRepository { get; }
		public SessionStore SessionStore { get; }
		public MiddleStore Store { get; }

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var model = (ISessionModel)context.ActionArguments.First().Value;
			var sessionId = Useful.ParseId(model.SessionId);
			if (sessionId == null)
			{
				context.Result = new ApiError(MyErrorCode.ModelInvalid, "SessionId parse error").Wrap();
				return;
			}

			SessionStore.SessionId = (ObjectId)sessionId;

			var member = await SessionMemberRepository
				.FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == Store.UserId);

			if (member == null)
			{
				context.Result = new ApiError(MyErrorCode.NotAMember, "You are not a member of the chat session").Wrap();
				return;
			}

			SessionStore.Member = member;

			await next();
		}
	}
}