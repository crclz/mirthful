using Leopard.API.Controllers.Chat;
using Leopard.API.Controllers.Query.DTO;
using Leopard.API.Filters;
using Leopard.Domain.Model.UserAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Query
{
	[Route("query/chat")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	[ServiceFilter(typeof(IsSessionMemberFilter))]
	public class QueryChatController : ControllerBase
	{
		private readonly LeopardDatabase db;
		private readonly MiddleStore store;

		public QueryChatController(LeopardDatabase db, MiddleStore store, SessionStore sessionStore)
		{
			this.db = db;
			this.store = store;
			SessionStore = sessionStore;
		}

		public SessionStore SessionStore { get; }

		[HttpPost("messages")]
		[Produces(typeof(QChatMessage[]))]
		public async Task<IActionResult> GetChat([FromBody]GetChatModel model)
		{
			var limit = Math.Min(model.Limit, 25);

			var q = db.ChatMessages.AsQueryable()
				.Where(p => p.SessionId == SessionStore.SessionId)
				.Page(model.Pt, model.Pid, model.Earlier, model.Limit)
				.OrderBy(p => p.CreatedAt);

			var messages = await q.ToListAsync();

			var data = messages.Select(p => QChatMessage.NormalView(p)).ToList();

			return new JsonResult(data);
		}

		[HttpPost("messages/displaying")]
		[Produces(typeof(QChatDisplay[]))]
		public async Task<IActionResult> GetChatWithDisplayData([FromBody]GetChatModel model)
		{
			var limit = Math.Min(model.Limit, 25);

			var q = db.ChatMessages.AsQueryable()
				.Where(p => p.SessionId == SessionStore.SessionId)
				.Page(model.Pt, model.Pid, model.Earlier, model.Limit)
				.OrderBy(p => p.CreatedAt);

			var messages = await q.ToListAsync();

			var data = new List<QChatDisplay>();

			foreach (var message in messages)
			{
				var user = await db.Users.AsQueryable().Where(p => p.Id == message.SenderId).FirstOrDefaultAsync();

				var item = QChatDisplay.NormalView(message, user);
				data.Add(item);
			}

			return new JsonResult(data);
		}

		public class GetChatModel : ISessionModel
		{
			public string SessionId { get; set; }
			public long Pt { get; set; }
			public string Pid { get; set; }
			public bool Earlier { get; set; }
			public int Limit { get; set; }
		}
	}
}