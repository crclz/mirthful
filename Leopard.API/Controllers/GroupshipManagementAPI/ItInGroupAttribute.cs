using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipManagementAPI
{
	public class ItInGroupAttribute : ActionFilterAttribute
	{
		public ItInGroupAttribute(bool inGroup = default)
		{
			InGroup = inGroup;
		}

		public bool InGroup { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipManagementPipelineContext>();

			if (Z.ItsDealer.IsUserInGroup() != InGroup)
			{
				context.Result = new ApiError(MyErrorCode.InGroupMismatch, "In group mismatch").Wrap();
				return;
			}

			await next();
		}
	}
}
