using Leopard.API.Filters;
using Leopard.Domain.Model.FriendshipDealerAggregates;
using Microsoft.AspNetCore.Mvc;

namespace Leopard.API.Controllers.Friend
{
	[Route("api/friend")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	[Friendship]
	public partial class FriendController : ControllerBase
	{
		public FriendController(FriendPipelineContext z)
		{
			Z = z;
		}

		public FriendPipelineContext Z { get; }
	}
}