using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.GroupAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupCreationAPI
{
	[Route("api/group-creation")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	public class GroupCreationController : ControllerBase
	{
		public GroupCreationController(Repository<Group> groupRepository, MiddleStore store)
		{
			GroupRepository = groupRepository;
			Store = store;
		}

		public Repository<Group> GroupRepository { get; }
		public MiddleStore Store { get; }

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody]CreateGroupModel model)
		{
			var group = new Group(model.Name, model.Description, (ObjectId)Store.UserId, ObjectId.GenerateNewId());

			await GroupRepository.PutAsync(group);

			return Ok(new { id = group.Id });
		}
		public class CreateGroupModel
		{
			[Required]
			[MinLength(1)]
			[MaxLength(16)]
			public string Name { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Description { get; set; }
		}
	}
}