using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("add-with-answer")]
		[AreFriends(false)]
		[UserRequirementType(RelationshipRequirementType.CorrectAnswerRequired)]
		public async Task<IActionResult> AddWithAnswer([FromBody]AddWithAnswerModel model)
		{
			var dealer = Z.Dealer;
			var targetUser = Z.TargetUser;
			var userId = Z.UserId;

			if (model.Answers.Length != targetUser.RelationshipRequirement.QuestionAndAnswers.Count())
				return new ApiError(MyErrorCode.CountMismatch, "Count of the answer not correct").Wrap();

			var e1 = model.Answers.AsEnumerable().GetEnumerator();
			var e2 = targetUser.RelationshipRequirement.QuestionAndAnswers.GetEnumerator();

			while (e1.MoveNext() && e2.MoveNext())
			{
				if (!e2.Current.IsCorrect(e1.Current))
					return new ApiError(MyErrorCode.WrongAnswer, "Wrong answer").Wrap();
			}

			dealer.MakeFriendsAndDeleteUnhandledRequest();

			await Z.FriendshipDealerRepository.PutAsync(dealer);

			return Ok();
		}

		public class AddWithAnswerModel : IFriendshipModel
		{
			public string TargetId { get; set; }

			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public string[] Answers { get; set; }
		}
	}
}
