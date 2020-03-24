using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Friend
{
	public partial class FriendController
	{
		[HttpPost("request/send-investigation")]
		[AreFriends(false)]
		[HasUnhandledRequest(Who.Target, hasUnhandledRequest: false)]
		[SelfNotBlocked]
		[UserRequirementType(RelationshipRequirementType.InvestigationRequired)]
		public async Task<IActionResult> SendInvestigation([FromBody]InvestigationAnswerListModel model)
		{
			var targetUser = Z.TargetUser;
			var meId = Z.UserId;
			var dealer = Z.Dealer;

			if (model.Answers.Length != targetUser.RelationshipRequirement.Investigations.Count())
				return new ApiError(MyErrorCode.CountMismatch, "Count of the answer incorrect").Wrap();

			var ias = new List<InvestigationAndAnswer>();

			var e1 = model.Answers.AsEnumerable().GetEnumerator();
			var e2 = targetUser.RelationshipRequirement.Investigations.GetEnumerator();

			while (e1.MoveNext() && e2.MoveNext())
			{
				var k = new InvestigationAndAnswer(e2.Current, e1.Current.Answer);
				ias.Add(k);
			}

			dealer.SendRequest(meId, ias);

			await Z.FriendshipDealerRepository.PutAsync(dealer);

			return Ok();
		}
	}

	public class InvestigationAnswerModel
	{
		[Required]
		[MinLength(1)]
		[MaxLength(32)]
		public string Answer { get; set; }
	}

	public class InvestigationAnswerListModel : IFriendshipModel
	{
		public string TargetId { get; set; }

		[Required]
		[MinLength(1)]
		[MaxLength(3)]
		public InvestigationAnswerModel[] Answers { get; set; }
	}
}