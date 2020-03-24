using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.API.ValidationAttributes;
using Leopard.Domain.Model.Relationships;
using Leopard.Domain.Model.UserAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Leopard.API.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly MiddleStore store;

		public Repository<User> UserRepository { get; }

		public UsersController(Repository<User> userRepository, MiddleStore store)
		{
			UserRepository = userRepository;
			this.store = store;
		}


		public class CreateUserResponse
		{
			public CreateUserResponse(string id)
			{
				Id = id ?? throw new ArgumentNullException(nameof(id));
			}

			public string Id { get; set; }
		}
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(typeof(CreateUserResponse))]
		public async Task<IActionResult> Create([FromBody]CreaterUserModel data)
		{
			var user = new User(data.Password, data.Nickname, data.Description);

			await UserRepository.PutAsync(user);

			return Ok(new CreateUserResponse(user.Id.ToString()));
		}
		public class CreaterUserModel
		{
			[Required]
			[StringLength(maximumLength: 32, MinimumLength = 8)]
			public string Password { get; set; }

			[Required(AllowEmptyStrings = true)]
			[StringLength(maximumLength: 16, MinimumLength = 1)]
			public string Nickname { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Description { get; set; }
		}


		[HttpPut("profile")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> PutProfile([FromBody]PutProfileModel data)
		{
			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == store.UserId);

			user.SetNickname(data.Nickname);
			user.SetDescription(data.Description);
			user.SetBirthday((DateTimeOffset)data.Birthday);

			await UserRepository.PutAsync(user);

			return Ok();
		}
		public class PutProfileModel
		{
			[MinLength(1)]
			[MaxLength(16)]
			public string Nickname { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Description { get; set; }

			[Required]
			public DateTimeOffset? Birthday { get; set; }
		}


		[HttpPut("avatar")]
		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> PutProfile([FromForm]PutAvatarModel model, [FromServices]IBlobBucket bucket)
		{
			if (model.AvatarFile.Length > 1 * 1024 * 1024)// 1024KB
				return new ApiError(MyErrorCode.FileTooLarge, "File size must <= 1MB").Wrap();

			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == store.UserId);

			using (var stream = model.AvatarFile.OpenReadStream())
			{
				var avatarUrl = await bucket.PutBlobAsync(stream, Path.GetRandomFileName());
				user.SetAvatar(avatarUrl);
			}

			await UserRepository.PutAsync(user);
			return Ok();
		}
		public class PutAvatarModel
		{
			[Required]
			public IFormFile AvatarFile { get; set; }
		}


		[HttpPost("friend-requirement/set-to-anyone")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> SetRequirementToAnyone()
		{
			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == store.UserId);

			user.SetRequirement(RelationshipRequirement.AnyoneRequirement);

			await UserRepository.PutAsync(user);

			return Ok();
		}


		[HttpPost("friend-requirement/set-to-validation-message-required")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> SetRequirementToValidationMessage()
		{
			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == store.UserId);

			user.SetRequirement(RelationshipRequirement.ValidationRequiredRequirement);

			await UserRepository.PutAsync(user);

			return Ok();
		}


		[HttpPost("friend-requirement/set-to-investigation-required")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> SetRequirementToInvestigation([FromBody]InvestigationListModel model)
		{
			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == store.UserId);

			user.SetRequirement(new RelationshipRequirement(
				model.Investigations.Select(p => new Investigation(p.Content)).ToList()));

			await UserRepository.PutAsync(user);

			return Ok();
		}
		public class InvestigationListModel
		{
			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public InvestigationModel[] Investigations { get; set; }

			public class InvestigationModel
			{
				[Required]
				[MinLength(1)]
				[MaxLength(32)]
				public string Content { get; set; }
			}
		}



		[HttpPost("friend-requirement/set-to-correct-answer-required")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> SetRequirementToCorrectAnswer([FromBody]QuestionAndAnswerListModel model)
		{
			// TODO: Should this function be provided as an attribute?
			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == store.UserId);

			user.SetRequirement(new RelationshipRequirement(
				model.QuestionAndAnswers.Select(p => new QuestionAndAnswer(p.Question, p.Answer)).ToList()));

			await UserRepository.PutAsync(user);

			return Ok();
		}
		public class QuestionAndAnswerListModel
		{
			[Required]
			[MinLength(1)]
			[MaxLength(3)]
			public QuestionAndAnswerModel[] QuestionAndAnswers { get; set; }

			public class QuestionAndAnswerModel
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