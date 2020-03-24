using Leopard.API.Controllers.Query.DTO;
using Leopard.API.Filters;
using Leopard.Domain;
using Leopard.Domain.Model.FriendshipDealerAggregates;
using Leopard.Domain.Model.UserAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Query
{
	[Route("query/friendships")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	public class QueryFriendshipController : ControllerBase
	{
		private readonly LeopardDatabase db;
		private readonly MiddleStore store;

		public QueryFriendshipController(LeopardDatabase db, MiddleStore store)
		{
			this.db = db;
			this.store = store;
		}

		public class QFriendshipWithQUser
		{
			QFriendship Friendship { get; set; }
			QUser User { get; set; }

			public static QFriendshipWithQUser MyView(Friendship friendship, User user)
			{
				var r = new QFriendshipWithQUser
				{
					Friendship = QFriendship.NormalView(friendship),
					User = QUser.NormalView(user)
				};

				return r;
			}
		}

		[HttpGet("my")]
		[Produces(typeof(QFriendshipWithQUser[]))]
		public async Task<IActionResult> MyFriendships(long pt, string pid, bool earlier, int limit)
		{
			pt = pt >= 0 ? pt : long.MaxValue;
			limit = Math.Min(limit, 50);
			var recordId = Useful.ParseId(pid);

			var query1 =
				from p in db.FriendshipDealers.AsQueryable().Where(p => p.AUserId == store.UserId)
				join q in db.Users.AsQueryable()
				on p.BUserId equals q.Id
				select new
				{
					Friendship = p.AToBFriendship,
					User = q
				};

			var query2 =
				from p in db.FriendshipDealers.AsQueryable().Where(p => p.BUserId == store.UserId)
				join q in db.Users.AsQueryable()
				on p.AUserId equals q.Id
				select new
				{
					Friendship = p.BToAFriendship,
					User = q
				};


			// TODO: Pagination & Sort

			// TODO: Learn Mongo View
			// TODO: Learn Mongo Materialized view

			var qdata = await query1.ToListAsync();
			var q2data = await query2.ToListAsync();
			qdata.AddRange(q2data);

			var data = qdata.Select(p => QFriendshipWithQUser.MyView(p.Friendship, p.User));

			return Ok(data);
		}

		[HttpGet("context")]
		[Produces(typeof(QFriendshipContext))]
		public async Task<IActionResult> GetFriendshipContext(string target)
		{
			var targetId = Useful.ParseId(target);
			if (targetId == null)
				return new JsonResult(null);

			var (aid, bid) = Useful.SmallerBigger((ObjectId)store.UserId, (ObjectId)targetId);

			var dealer = await db.FriendshipDealers.AsQueryable()
				.Where(p => p.AUserId == aid && p.BUserId == bid).FirstOrDefaultAsync();

			if (dealer == null)
				return new JsonResult(null);

			var meIsA = dealer.IsA((ObjectId)store.UserId);

			var data = QFriendshipContext.MyView(dealer, (ObjectId)store.UserId);

			return Ok(data);
		}
	}

}