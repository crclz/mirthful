using Leopard.API.Controllers.Friend;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.API.ValidationAttributes;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Domain.Model.Relationships;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipAPI
{
	[Route("api/groupship")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	[Groupship]
	public class GroupshipController : ControllerBase
	{
		public GroupshipPipelineContext Z { get; }

		public GroupshipController(GroupshipPipelineContext z)
		{
			Z = z;
		}

		[HttpPost("join-directly")]
		[InGroup(false)]
		[GroupRequirementType(RelationshipRequirementType.Anyone)]
		public async Task<IActionResult> JoinDirectly([FromBody]JoinGroupDiretlyModel model)
		{
			Z.Dealer.JoinGroupAndDeleteUnhandledRequest();

			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);

			return Ok();
		}
		public class JoinGroupDiretlyModel : IGroupModel
		{
			public string GroupId { get; set; }
		}


		[HttpPost("join-with-answer")]
		[InGroup(false)]
		[GroupRequirementType(RelationshipRequirementType.CorrectAnswerRequired)]
		public async Task<IActionResult> JoinWithAnswer([FromBody]JoinWithAnswerModel model)
		{
			if (!Z.Group.IsAnswersCorrect(model.Answers))
				return new ApiError(MyErrorCode.WrongAnswer, "Wrong answer").Wrap();

			Z.Dealer.JoinGroupAndDeleteUnhandledRequest();

			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);

			return Ok();
		}
		public class JoinWithAnswerModel : IGroupModel
		{
			public string GroupId { get; set; }

			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public string[] Answers { get; set; }
		}


		[HttpPost("request/send-validation")]
		[InGroup(false)]
		[HasUnhandledGroupRequest(false)]
		[NotBlocked]
		[GroupRequirementType(RelationshipRequirementType.ValidationMessageRequired)]
		public async Task<IActionResult> SendValidation([FromBody] SendValidationModel2 model)
		{
			var group = Z.Group;
			var dealer = Z.Dealer;


			dealer.SendRequest(model.Message);

			await Z.GroupshipDealerRepository.PutAsync(dealer);

			return Ok();
		}
		public class SendValidationModel2 : IGroupModel
		{
			public string GroupId { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Message { get; set; }
		}


		[HttpPost("request/send-investigation")]
		[InGroup(false)]
		[HasUnhandledGroupRequest(false)]
		[NotBlocked]
		[GroupRequirementType(RelationshipRequirementType.InvestigationRequired)]
		public async Task<IActionResult> SendInvestigation([FromBody]InvestigationAnswerListModel2 model)
		{
			var group = Z.Group;
			var dealer = Z.Dealer;

			if (group.RelationshipRequirement.Type != RelationshipRequirementType.InvestigationRequired)
				return new ApiError(MyErrorCode.RequirementTypeMismatch, "RequirementTypeMismatch").Wrap();

			if (model.Answers.Length != group.RelationshipRequirement.Investigations.Count())
				return new ApiError(MyErrorCode.CountMismatch, "CountMismatch").Wrap();

			var ias = new List<InvestigationAndAnswer>();

			var e1 = model.Answers.AsEnumerable().GetEnumerator();
			var e2 = group.RelationshipRequirement.Investigations.GetEnumerator();

			while (e1.MoveNext() && e2.MoveNext())
			{
				var k = new InvestigationAndAnswer(e2.Current, e1.Current.Answer);
				ias.Add(k);
			}

			dealer.SendRequest(ias);

			await Z.GroupshipDealerRepository.PutAsync(dealer);

			return Ok();
		}
		public class InvestigationAnswerListModel2 : IGroupModel
		{
			public string GroupId { get; set; }

			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public InvestigationAnswerModel[] Answers { get; set; }
		}


		[HttpPost("request/abandon")]
		[InGroup(false)]
		[HasUnhandledGroupRequest(true)]
		public async Task<IActionResult> AbandonRequest([FromBody]AbandonRequestModel2 model)
		{
			Z.Dealer.AbandonUnhandledRequest();

			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);

			return Ok();
		}
		public class AbandonRequestModel2 : IGroupModel
		{
			public string GroupId { get; set; }
		}


		[HttpPut("group-displayname")]
		[InGroup(true)]
		public async Task<IActionResult> SetGroupDisplayName([FromBody]SetGroupDisplayNameModel model)
		{
			Z.Dealer.SetGroupDisplayName(model.Name);
			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);
			return Ok();
		}

		[HttpPut("self-displayname")]
		[InGroup(true)]
		public async Task<IActionResult> SetSelfDisplayName([FromBody]SetGroupDisplayNameModel model)
		{
			Z.Dealer.SetUserDisplayname(model.Name);
			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);
			return Ok();
		}
		public class SetGroupDisplayNameModel : IGroupModel
		{
			public string GroupId { get; set; }

			[Required]
			[StringLength(16, MinimumLength = 1)]
			public string Name { get; set; }
		}


		[HttpPost("quit-admin")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Admin)]
		public async Task<IActionResult> QuitAdmin([FromBody]SimpleGroupshipModel model)
		{
			Z.Dealer.SetRole(GroupRole.Normal);

			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);

			return Ok();
		}
		public class SimpleGroupshipModel : IGroupModel
		{
			public string GroupId { get; set; }
		}


		[HttpPost("quit-group")]
		[InGroup(true)]
		public async Task<IActionResult> QuitGroup(SimpleGroupshipModel model)
		{
			// Founder cannot quit group
			if (Z.Dealer.GroupShip.Role == GroupRole.Founder)
				return new ApiError(MyErrorCode.FounderCannotQuitGroup, "Founder cannot quit the group").Wrap();

			Z.Dealer.QuitGroup();

			await Z.GroupshipDealerRepository.PutAsync(Z.Dealer);

			return Ok();
		}


		// -------- Group Management

		[HttpPut("profile")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Admin)]
		public async Task<IActionResult> PutProfile(PutGroupProfileModel model)
		{
			Z.Group.SetName(model.Name);
			Z.Group.SetDescription(model.Description);

			await Z.GroupRepository.PutAsync(Z.Group);

			return Ok();
		}
		public class PutGroupProfileModel : IGroupModel
		{
			public string GroupId { get; set; }

			[Required]
			[MinLength(1)]
			[MaxLength(16)]
			public string Name { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Description { get; set; }
		}



		[HttpPut("avatar")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Admin)]
		public async Task<IActionResult> PutAvatar(PutGroupAvatarModel model, [FromServices]IBlobBucket bucket)
		{
			if (model.AvatarFile.Length > 1 * 1024 * 1024)// 1024KB
				return new ApiError(MyErrorCode.FileTooLarge, "File size must <= 1MB").Wrap();

			using (var stream = model.AvatarFile.OpenReadStream())
			{
				var avatarUrl = await bucket.PutBlobAsync(stream, Path.GetRandomFileName());
				Z.Group.SetAvatar(avatarUrl);
			}

			await Z.GroupRepository.PutAsync(Z.Group);

			return Ok(new { avatarUrl = Z.Group.Avatar });
		}
		public class PutGroupAvatarModel : IGroupModel
		{
			public string GroupId { get; set; }

			[Required]
			public IFormFile AvatarFile { get; set; }
		}



		[HttpPost("set-requirement/anyone")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Founder)]
		public async Task<IActionResult> SetRequirementToAnyone([FromBody]SimpleGroupshipModel model)
		{
			Z.Group.SetRequirement(RelationshipRequirement.AnyoneRequirement);

			await Z.GroupRepository.PutAsync(Z.Group);

			return Ok();
		}

		[HttpPost("set-requirement/validation")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Founder)]
		public async Task<IActionResult> SetRequirementToValidation([FromBody]SimpleGroupshipModel model)
		{
			Z.Group.SetRequirement(RelationshipRequirement.ValidationRequiredRequirement);

			await Z.GroupRepository.PutAsync(Z.Group);

			return Ok();
		}


		[HttpPost("set-requirement/investigation")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Founder)]
		public async Task<IActionResult> SetRequirementToInvestigation([FromBody]GInvestigationListModel model)
		{
			Z.Group.SetRequirement(new RelationshipRequirement(
				model.Investigations.Select(p => new Investigation(p.Content)).ToList()));

			await Z.GroupRepository.PutAsync(Z.Group);

			return Ok();
		}
		public class GInvestigationListModel : IGroupModel
		{
			public string GroupId { get; set; }

			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public GInvestigationModel[] Investigations { get; set; }

			public class GInvestigationModel
			{
				[Required]
				[MinLength(1)]
				[MaxLength(32)]
				public string Content { get; set; }
			}
		}


		[HttpPost("set-requirement/answer")]
		[InGroup(true)]
		[AtLeastRole(GroupRole.Founder)]
		public async Task<IActionResult> SetRequirementToAnswer([FromBody]GQuestionAndAnswerListModel model)
		{
			Z.Group.SetRequirement(new RelationshipRequirement(
				model.QuestionAndAnswers.Select(p => new QuestionAndAnswer(p.Question, p.Answer)).ToList()));

			await Z.GroupRepository.PutAsync(Z.Group);

			return Ok();
		}
		public class GQuestionAndAnswerListModel : IGroupModel
		{
			public string GroupId { get; set; }


			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public GQuestionAndAnswerModel[] QuestionAndAnswers { get; set; }

			public class GQuestionAndAnswerModel
			{
				[Required]
				[MinLength(1)]
				[MaxLength(32)]
				[NotWhitespace]
				public string Question { get; set; }

				[Required]
				[MinLength(1)]
				[MaxLength(32)]
				[NotWhitespace]
				public string Answer { get; set; }
			}
		}
	}
}