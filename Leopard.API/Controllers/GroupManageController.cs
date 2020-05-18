using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.AdminRequestAG;
using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Domain.UserAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Leopard.API.Controllers
{
	[Route("api/group-management")]
	[ApiController]
	public class GroupManageController : ControllerBase
	{
		public AuthStore AuthStore { get; }
		public OneContext Context { get; }

		public GroupManageController(AuthStore authStore, OneContext context)
		{
			AuthStore = authStore;
			Context = context;
		}


		/// <summary>
		/// 发送请求，希望成为管理员。注意，只有小组有管理员。
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("send-admin-request")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> SendAdminRequest([FromBody]SendAdminRequestModel model)
		{
			var topicId = XUtils.ParseId(model.TopicId);

			// Topic is group topic
			var topic = await Context.Topics.FirstOrDefaultAsync(p => p.Id == topicId);
			if (topic == null) return new ApiError(MyErrorCode.IdNotFound, "Topic id not found").Wrap();
			if (topic.IsGroup == false)
				return new ApiError(MyErrorCode.TypeMismatch, "This topic is not group").Wrap();

			// should be normal member
			var member = await Context.TopicMembers.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId);
			if (member?.Role != MemberRole.Normal)
				return new ApiError(MyErrorCode.RoleMismatch, "You are not normal row of the topic").Wrap();

			// unhandled request exist?
			var previous = await Context.AdminRequests.FirstOrDefaultAsync(
				p => p.TopicId == topicId && p.SenderId == AuthStore.UserId && p.Status == RequestStatus.Unhandled);
			if (previous != null)
				return new ApiError(MyErrorCode.ExistUnhandledReuqest, "你已经发送过请求，但未被处理").Wrap();

			// send request
			var request = new AdminRequest(topicId.Value, AuthStore.UserId.Value, model.Text);

			await Context.AddAsync(request);
			await Context.GoAsync();
			return Ok(new IdResponse(request.Id));
		}
		public class SendAdminRequestModel
		{
			/// <summary>
			/// 小组id
			/// </summary>
			[Required]
			public string TopicId { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(64)]
			public string Text { get; set; }
		}


		/// <summary>
		/// 根据某一个管理员请求。
		/// </summary>
		/// <param name="id">请求id</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("get-request")]
		[Consumes(Application.Json)]
		[Produces(typeof(QAdminRequest))]
		public async Task<IActionResult> GetRequestById(string id)
		{
			var requestId = XUtils.ParseId(id);

			var query = from p in Context.AdminRequests
						where p.Id == requestId
						join q in Context.Users
						on p.SenderId equals q.Id
						select new { request = p, user = q };

			var data = await query.FirstOrDefaultAsync();

			var requestV = QAdminRequest.NormalView(data.request, QUser.NormalView(data.user));

			return Ok(requestV);
		}
		public class QAdminRequest
		{
			public Guid Id { get; set; }

			/// <summary>
			/// 小组id
			/// </summary>
			public Guid TopicId { get; set; }

			/// <summary>
			/// 发送者id
			/// </summary>
			public Guid SenderId { get; set; }

			/// <summary>
			/// 原因
			/// </summary>
			public string Text { get; set; }

			/// <summary>
			/// 请求状态
			/// </summary>
			public RequestStatus Status { get; set; }

			/// <summary>
			/// 请求发送者
			/// </summary>
			public QUser User { get; set; }

			public static QAdminRequest NormalView(AdminRequest p, QUser user)
			{
				return p == null ? null : new QAdminRequest
				{
					Id = p.Id,
					TopicId = p.TopicId,
					SenderId = p.SenderId,
					Text = p.Text,
					Status = p.Status,

					User = user
				};
			}
		}


		/// <summary>
		/// （超级管理员）处理请求
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("handle-request")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> HandleRequest([FromBody]HandleRequestModel model)
		{
			// request should be unhandled
			var requestId = XUtils.ParseId(model.RequestId);
			var request = await Context.AdminRequests.FirstOrDefaultAsync(p => p.Id == requestId);
			if (request == null)
				return new ApiError(MyErrorCode.IdNotFound, "Request id not found").Wrap();
			if (request.Status != RequestStatus.Unhandled)
				return new ApiError(MyErrorCode.TypeMismatch, "Request is not unhandled").Wrap();

			// should have super role
			var member = await Context.TopicMembers
				.FirstOrDefaultAsync(p => p.UserId == AuthStore.UserId && p.TopicId == request.TopicId);
			if (member.Role != MemberRole.Super)
				return new ApiError(MyErrorCode.PermissionDenied, "You are not super administrator of the group").Wrap();

			// handle
			request.Handle(model.Accept);


			if (model.Accept)
			{
				var membership2 = await Context.TopicMembers
					.FirstOrDefaultAsync(p => p.UserId == request.SenderId && p.TopicId == request.TopicId);

				membership2.SetRole(MemberRole.Admin);
			}

			await Context.GoAsync();

			return Ok();

		}
		public class HandleRequestModel
		{
			/// <summary>
			/// 请求id
			/// </summary>
			[Required]
			public string RequestId { get; set; }

			/// <summary>
			/// 是否接受
			/// </summary>
			[Required]
			public bool Accept { get; set; }
		}


		/// <summary>
		/// 获取某小组未处理的请求
		/// </summary>
		/// <param name="topicId">小组id</param>
		/// <param name="page">页码，从0开始。</param>
		/// <param name="newest">true：最新（时间倒序）；false：最旧（时间正序）</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("unhandled-requests")]
		[Produces(typeof(QAdminRequest[]))]
		public async Task<IActionResult> GetUnhandledRequests(string topicId, int page, bool newest)
		{
			const int pageSize = 20;

			var tid = XUtils.ParseId(topicId);

			var query = from p in Context.AdminRequests
						where p.TopicId == tid && p.Status == RequestStatus.Unhandled
						join q in Context.Users
						on p.SenderId equals q.Id
						select new { request = p, user = q };

			if (newest)
				query = query.OrderByDescending(p => p.request.CreatedAt);
			else
				query = query.OrderBy(p => p.request.CreatedAt);

			query = query.Skip(page * pageSize).Take(pageSize);

			var requests = await query.ToListAsync();
			var data = requests.Select(p => QAdminRequest.NormalView(p.request, QUser.NormalView(p.user))).ToList();

			return Ok(data);
		}
	}
}