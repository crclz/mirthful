using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPut("nickname")]
		[AreFriends(true)]
		public async Task<IActionResult> PutFriendship([FromBody]PutNicknameModel model)
		{
			Z.Dealer.SetNickname(Z.UserId, model.Nickname);
			Z.Dealer.SetRemindBirthday(Z.UserId, (bool)model.RemindBirthday);
			await Z.FriendshipDealerRepository.PutAsync(Z.Dealer);
			return Ok();
		}
	}

	public class PutNicknameModel : IFriendshipModel
	{
		public string TargetId { get; set; }

		[MaxLength(16)]
		public string Nickname { get; set; }

		[Required]
		public bool? RemindBirthday { get; set; }
	}
}
