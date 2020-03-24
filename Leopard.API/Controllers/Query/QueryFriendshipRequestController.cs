using Leopard.API.Controllers.Query.DTO;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.Model.FriendshipDealerAggregates;
using Leopard.Domain.Model.Relationships;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Query
{
	[Route("query/friendshipRequests")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	public class QueryFriendshipRequestController : ControllerBase
	{
		private readonly LeopardDatabase db;
		private readonly MiddleStore store;

		public QueryFriendshipRequestController(LeopardDatabase db, MiddleStore store)
		{
			this.db = db;
			this.store = store;
		}

		// Lession: do not use IQueryable. Use IMongoQueryable instead

		[HttpGet("my")]
		[Produces(typeof(QFriendRequest[]))]
		public async Task<IActionResult> GetLatestFriendshipRequests(
			bool send, string status, long pt, string pid, int limit, bool earlier)
		{
			var recordId = Useful.ParseId(pid);
			limit = Math.Min(limit, 10);
			if (pt == -1)
				pt = long.MaxValue;

			if (!new string[] { "unhandled", "other" }.Contains(status))
				return new ApiError(MyErrorCode.ModelInvalid, "Unknown parameter value 'status'.").Wrap();


			var q1 = db.FriendshipDealers.AsQueryable().Select(p => p.AToBRequest);
			var q2 = db.FriendshipDealers.AsQueryable().Select(p => p.BToARequest);

			q1 = filter(q1);
			q2 = filter(q2);

			var list = await q1.ToListAsync();
			//q2 = q2.Where(p => p != null);
			list.AddRange(await q2.ToListAsync());

			list = list.OrderByDescending(p => p.UpdatedAt).ThenByDescending(p => p.Id).ToList();

			var data = list.Select(p => QFriendRequest.NormalView(p)).ToList();

			return new JsonResult(data);



			IMongoQueryable<FriendshipRequest> filter(IMongoQueryable<FriendshipRequest> query)
			{
				if (send)
					query = query.Where(p => p.Dual.SenderId == store.UserId);
				else
					query = query.Where(p => p.Dual.ReceiverId == store.UserId);

				switch (status)
				{
					case "unhandled":
						query = query.Where(p => p.Status == RelationshipRequestStatus.Unhandled);
						break;
					case "other":
						query = query.Where(p => p.Status != RelationshipRequestStatus.Unhandled);
						break;
					default:
						throw new ArgumentException("status");
				}

				query = query.Page(pt, pid, earlier, limit);// TODO

				return query;
			};
		}
	}
}