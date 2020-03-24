using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leopard.Domain;
using Leopard.Domain.Model.GroupAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Leopard.API.Controllers.Query
{
	[Route("api/query/group")]
	[ApiController]
	public class QueryGroupController : ControllerBase
	{
		private readonly LeopardDatabase db;

		public QueryGroupController(LeopardDatabase db)
		{
			this.db = db;
		}


		[HttpGet("one")]
		public async Task<IActionResult> GetOneGroup(string id)
		{
			var groupId = Useful.ParseId(id);
			if (groupId == null)
				return new JsonResult(null);

			var group = await db.GetCollection<Group>().AsQueryable()
				.Where(p => p.Id == groupId)
				.FirstOrDefaultAsync();

			if (group == null)
				return new JsonResult(null);

			dynamic questions = null;

			if (group.RelationshipRequirement.Type == Domain.Model.Relationships.RelationshipRequirementType.CorrectAnswerRequired)
			{
				questions = group.RelationshipRequirement.QuestionAndAnswers.Select(p => p.Question).ToList();
			}

			var data = new
			{
				group.Id,
				group.Name,
				group.Avatar,
				group.CreatedAt,
				group.Creator,
				group.Description,
				group.SessionId,
				group.UpdatedAt,
				RelationshipRequirement = new
				{
					group.RelationshipRequirement.Type,
					group.RelationshipRequirement.Investigations,
					questions
				}
			};

			return new JsonResult(data);
		}
	}
}