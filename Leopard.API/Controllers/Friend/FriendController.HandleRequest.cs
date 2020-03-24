using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("request/handle")]
		[AreFriends(false)]
		[HasUnhandledRequest(Who.Self, hasUnhandledRequest: true)]
		// TODO: Concurrency conflict (but seems not needed)
		public async Task<IActionResult> HandleRequest([FromBody]HandleRequestModel model)
		{
			if (model.Accept && model.Block)
				return new ApiError(MyErrorCode.ModelInvalid, "Invalid: accept==true and block==true").Wrap();

			var dealer = Z.Dealer;
			var meId = Z.UserId;

			if (model.Accept)
			{
				dealer.AcceptRequest(meId);
			}
			else
			{
				dealer.RefuseRequest(meId, model.Message, model.Block);
			}

			await Z.FriendshipDealerRepository.PutAsync(dealer);

			return Ok();
		}

		public class HandleRequestModel : IFriendshipModel
		{
			public string TargetId { get; set; }

			public bool Accept { get; set; }

			public bool Block { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Message { get; set; }
		}
	}
}
