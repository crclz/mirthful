using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class NotBlockedAttribute : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipPipelineContext>();

			if (Z.Dealer.IsBlocked())
			{
				context.Result = new ApiError(MyErrorCode.InGroupMismatch, "InGroupMismatch").Wrap();
				return;
			}

			await next();
		}
	}
}
