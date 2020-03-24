using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public class SelfNotBlockedAttribute : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var z = context.HttpContext.RequestServices.GetRequiredService<FriendPipelineContext>();

			if (z.Dealer.IsBlocked(z.UserId))
			{
				context.Result = new ApiError(MyErrorCode.Blocked, "You are blocked").Wrap();
				return;
			}
			await next();
		}
	}
}
