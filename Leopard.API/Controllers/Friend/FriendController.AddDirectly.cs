using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("add-directly")]
		[AreFriends(false)]
		[UserRequirementType(RelationshipRequirementType.Anyone)]
		public async Task<IActionResult> AddDirectly([FromBody]AddDirectlyModel model)
		{
			Z.Dealer.MakeFriendsAndDeleteUnhandledRequest();

			await Z.FriendshipDealerRepository.PutAsync(Z.Dealer);

			return Ok();
		}

		public class AddDirectlyModel : IFriendshipModel
		{
			public string TargetId { get; set; }
		}
	}
}
