using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("request/send-validation")]
		[AreFriends(false)]
		[HasUnhandledRequest(Who.Target, false)]
		[SelfNotBlocked]
		[UserRequirementType(RelationshipRequirementType.ValidationMessageRequired)]
		public async Task<IActionResult> SendValidation([FromBody]SendValidationModel model)
		{
			var dealer = Z.Dealer;

			dealer.SendRequest(Z.UserId, model.Message);

			await Z.FriendshipDealerRepository.PutAsync(dealer);
			return Ok();
		}

		public class SendValidationModel : IFriendshipModel
		{
			public string TargetId { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Message { get; set; }
		}
	}
}
