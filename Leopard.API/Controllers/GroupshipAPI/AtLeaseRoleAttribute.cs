using Dawn;
using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class AtLeastRoleAttribute : ActionFilterAttribute
	{
		public AtLeastRoleAttribute(GroupRole role = default)
		{
			Guard.Argument(() => role).Defined().NotEqual(GroupRole.Normal);
			Role = role;
		}

		public GroupRole Role { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipPipelineContext>();

			bool ok = false;

			if (Z.Dealer.GroupShip.Role == GroupRole.Founder)
				ok = true;

			if (Z.Dealer.GroupShip.Role == Role)
				ok = true;

			if (!ok)
			{
				context.Result = new ApiError(MyErrorCode.PermissionDenied, "Permission denied").Wrap();
				return;
			}

			await next();
		}
	}
}
