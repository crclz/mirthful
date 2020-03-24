using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public class AreFriendsAttribute : ActionFilterAttribute
	{
		public AreFriendsAttribute(bool areFriends = false)
		{
			AreFriends = areFriends;
		}

		public bool AreFriends { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var z = context.HttpContext.RequestServices.GetRequiredService<FriendPipelineContext>();

			if (z.Dealer.AreFriends() != AreFriends)
			{
				if (z.Dealer.AreFriends())
					context.Result = new ApiError(MyErrorCode.AlreadFriends, "Already friends").Wrap();
				else
					context.Result = new ApiError(MyErrorCode.NotFriends, "Not friends").Wrap();

				return;
			}
			await next();
		}
	}
}
