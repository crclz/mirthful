using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class HasUnhandledGroupRequestAttribute : ActionFilterAttribute
	{
		public HasUnhandledGroupRequestAttribute(bool hasUnhandled = default)
		{
			HasUnhandled = hasUnhandled;
		}

		public bool HasUnhandled { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipPipelineContext>();

			if (HasUnhandled != Z.Dealer.HasUnhandledRequest())
			{
				context.Result = new ApiError(MyErrorCode.HasUnhandledRequestMismatch, "HasUnhandledRequestMismatch").Wrap();
				return;
			}

			await next();
		}
	}
}
