using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.Model.FriendshipDealerAggregates;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public class FriendshipAttribute : ActionFilterAttribute
	{
		public FriendshipAttribute()
		{

		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var z = context.HttpContext.RequestServices.GetRequiredService<FriendPipelineContext>();

			var model = (IFriendshipModel)context.ActionArguments.First().Value;
			var targetId = Useful.ParseId(model.TargetId);

			if (targetId == null)
			{
				context.Result = new ApiError(MyErrorCode.ModelInvalid, "TargetId parse error").Wrap();
				return;
			}

			if (targetId == z.UserId)
			{
				context.Result = new ApiError(MyErrorCode.IsSelf, "Target user should not be yourself").Wrap();
				return;
			}

			var targetUser = await z.UserRepository.FirstOrDefaultAsync(p => p.Id == targetId);
			if (targetUser == null)
			{
				context.Result = new ApiError(MyErrorCode.IdNotFound, "Target user not found").Wrap();
				return;
			}
			z.TargetUser = targetUser;

			var (aid, bid) = Useful.SmallerBigger(z.UserId, (ObjectId)targetId);
			var dealer = await z.FriendshipDealerRepository.FirstOrDefaultAsync(p => p.AUserId == aid && p.BUserId == bid);
			dealer = dealer ?? new FriendshipDealer(aid, bid);
			z.Dealer = dealer;
			await next();
		}
	}

	public interface IFriendshipModel
	{
		public string TargetId { get; set; }
	}
}
