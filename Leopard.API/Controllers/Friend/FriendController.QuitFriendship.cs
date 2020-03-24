using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("quit")]
		[AreFriends(true)]
		public async Task<IActionResult> QuitFriendship([FromBody]QuitFriendshipModel model)
		{
			Z.Dealer.InvalidateFriendship();
			await Z.FriendshipDealerRepository.PutAsync(Z.Dealer);
			return Ok();
		}
	}

	public class QuitFriendshipModel : IFriendshipModel
	{
		public string TargetId { get; set; }
	}
}
