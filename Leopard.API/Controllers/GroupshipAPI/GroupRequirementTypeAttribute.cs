using Dawn;
using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class GroupRequirementTypeAttribute : ActionFilterAttribute
	{
		public GroupRequirementTypeAttribute(RelationshipRequirementType type = default)
		{
			Guard.Argument(() => type).Defined();
			Type = type;
		}

		public RelationshipRequirementType Type { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipPipelineContext>();

			if (Z.Group.RelationshipRequirement.Type != Type)
			{
				context.Result = new ApiError(MyErrorCode.RequirementTypeMismatch, "RequirementTypeMismatch").Wrap();
				return;
			}

			await next();
		}
	}
}
