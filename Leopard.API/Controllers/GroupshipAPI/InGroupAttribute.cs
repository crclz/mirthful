using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class InGroupAttribute : ActionFilterAttribute
	{
		public InGroupAttribute(bool inGroup = default)
		{
			InGroup = inGroup;
		}

		public bool InGroup { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipPipelineContext>();

			if (InGroup != Z.Dealer.IsUserInGroup())
			{
				context.Result = new ApiError(MyErrorCode.InGroupMismatch, "In group status mismatch").Wrap();
				return;
			}

			await next();
		}
	}
}
