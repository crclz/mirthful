using Leopard.API.Filters;
using Leopard.Domain;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Query
{
	[Route("query/users")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	public class QueryUserController : ControllerBase
	{
		private readonly LeopardDatabase db;
		private readonly MiddleStore store;

		public QueryUserController(LeopardDatabase db, MiddleStore store)
		{
			this.db = db;
			this.store = store;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUser([FromRoute]string id)
		{
			var userId = Useful.ParseId(id);
			var user = await db.Users.Find(p => p.Id == userId).FirstOrDefaultAsync();

			if (user == null)
				return new JsonResult(null);

			dynamic questions = null, answers = null;

			if (user.RelationshipRequirement.Type == Domain.Model.Relationships.RelationshipRequirementType.CorrectAnswerRequired)
			{
				questions = user.RelationshipRequirement.QuestionAndAnswers.Select(p => p.Question).ToList();

				if (store.UserId == userId)
					answers = user.RelationshipRequirement.QuestionAndAnswers.Select(p => p.Answer).ToList();
			}

			var data = new
			{
				user.Id,
				user.Description,
				user.Nickname,
				user.Avatar,
				birthday = user.Birthday?.ToString("yyyy-MM-dd"),
				RelationshipRequirement = new
				{
					user.RelationshipRequirement.Type,
					user.RelationshipRequirement.Investigations,
					questions,
					answers
				}
			};

			return new JsonResult(data);
		}
	}
}