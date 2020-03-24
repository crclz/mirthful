using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	public class GroupshipAttribute : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var Z = context.HttpContext.RequestServices.GetRequiredService<GroupshipPipelineContext>();

			if (Z.UserId == null)
				throw new InvalidOperationException();
			var model = (IGroupModel)context.ActionArguments.First().Value;
			var groupId = Useful.ParseId(model.GroupId);
			if (groupId == null)
			{
				context.Result = new ApiError(MyErrorCode.ModelInvalid, "GroupId parse error").Wrap();
				return;
			}

			var group = await Z.GroupRepository.FirstOrDefaultAsync(p => p.Id == groupId);
			if (group == null)
			{
				context.Result = new ApiError(MyErrorCode.IdNotFound, "Group not found").Wrap();
				return;
			}
			Z.Group = group;

			var dealer = await Z.GroupshipDealerRepository.FirstOrDefaultAsync(p => p.GroupId == groupId && p.UserId == Z.UserId);
			dealer ??= new GroupshipDealer(Z.UserId, (ObjectId)groupId);
			Z.Dealer = dealer;
			await next();
		}
	}
}
