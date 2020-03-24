using Dawn;
using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public class UserRequirementTypeAttribute : ActionFilterAttribute
	{
		public UserRequirementTypeAttribute(RelationshipRequirementType type = default)
		{
			Guard.Argument(() => type).Defined();
			Type = type;
		}

		public RelationshipRequirementType Type { get; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var z = context.HttpContext.RequestServices.GetRequiredService<FriendPipelineContext>();

			if (z.TargetUser.RelationshipRequirement.Type != Type)
			{
				context.Result = new ApiError(MyErrorCode.RequirementTypeMismatch, "Incorrect requirement type").Wrap();
				return;
			}
			await next();
		}
	}
}
