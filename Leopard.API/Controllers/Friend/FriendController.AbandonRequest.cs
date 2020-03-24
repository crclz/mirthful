using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("request/abandon")]
		[HasUnhandledRequest(Who.Target, true)]
		public async Task<IActionResult> AbandonRequest([FromBody]AbandonRequestModel model)
		{
			// TODO: pipeline context could be also be used in controller

			var dealer = Z.Dealer;
			var targetId = Z.TargetUser.Id;

			if (dealer.HasUnhandledRequest(targetId))
			{
				dealer.AbandonUnhandledRequest(Z.UserId);
			}
			else
			{
				return new ApiError(MyErrorCode.NoUnhandledReuqest, "Target user has no unhanled request").Wrap();
			}

			await Z.FriendshipDealerRepository.PutAsync(dealer);

			return Ok();
		}

		public class AbandonRequestModel : IFriendshipModel
		{
			public string TargetId { get; set; }
		}
	}
}
