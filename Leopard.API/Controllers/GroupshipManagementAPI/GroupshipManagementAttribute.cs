using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipManagementAPI
{
	public class GroupshipManagementAttribute : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipManagementPipelineContext>();

			var model = (IGroupshipManagementModel)context.ActionArguments.First().Value;
			var itId = Useful.ParseId(model.ItId);
			if (itId == null)
			{
				context.Result = new ApiError(MyErrorCode.ModelInvalid, "ItId parse error").Wrap();
				return;
			}
			Z.ItId = (ObjectId)itId;

			var groupId = Useful.ParseId(model.GroupId);
			if (groupId == null)
			{
				context.Result = new ApiError(MyErrorCode.ModelInvalid, "GroupId parse error").Wrap();
				return;
			}

			Z.Group = await Z.GroupRepository.FirstOrDefaultAsync(p => p.Id == groupId);
			if (Z.Group == null)
			{
				context.Result = new ApiError(MyErrorCode.IdNotFound, "Group not found").Wrap();
				return;
			}

			// Get dealer of I & append
			Z.MyDealer = await Z.GroupshipDealerRepository.FirstOrDefaultAsync(p => p.GroupId == groupId && p.UserId == Z.MeId);

			// Check I am at least admin
			if (Z.MyDealer == null || !Z.MyDealer.IsUserInGroup() || (int)Z.MyDealer.GroupShip.Role < (int)GroupRole.Admin)
			{
				context.Result = new ApiError(MyErrorCode.PermissionDenied, "PermissionDenied").Wrap();
				return;
			}

			// get or create its dealer & append
			Z.ItsDealer = await Z.GroupshipDealerRepository.FirstOrDefaultAsync(p => p.GroupId == groupId && p.UserId == Z.ItId);
			Z.ItsDealer ??= new GroupshipDealer(Z.ItId, (ObjectId)groupId);


			await next();
		}
	}
}
