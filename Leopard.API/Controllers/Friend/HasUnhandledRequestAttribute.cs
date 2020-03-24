using Dawn;
using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public class HasUnhandledRequestAttribute : ActionFilterAttribute
	{
		public HasUnhandledRequestAttribute(Who who = default, bool hasUnhandledRequest = default)
		{
			Guard.Argument(() => who).Defined();
			Who = who;
			HasUnhandledRequest = hasUnhandledRequest;
		}

		public Who Who { get; }
		public bool HasUnhandledRequest { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var z = context.HttpContext.RequestServices.GetRequiredService<FriendPipelineContext>();

			var id = Who == Who.Self ? z.UserId : z.TargetUser.Id;

			if (z.Dealer.HasUnhandledRequest(id) != HasUnhandledRequest)
			{
				context.Result = new ApiError(MyErrorCode.HasUnhandledRequestMismatch, "HasUnhandledRequestMismatch").Wrap();
				return;
			}

			await next();
		}
	}

	public enum Who
	{
		Self,
		Target
	}
}
