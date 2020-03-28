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
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Leopard.API.Controllers
{
	[Route("api/group-management")]
	[ApiController]
	public class GroupManageController : ControllerBase
	{
		public AuthStore AuthStore { get; }
		public Repository<Topic> TopicRepository { get; }
		public Repository<TopicMember> MemberRepository { get; }
		public Repository<AdminRequest> RequestRepository { get; }
		public LeopardDatabase Db { get; }

		public GroupManageController(AuthStore authStore, Repository<Topic> topicRepository,
			Repository<TopicMember> memberRepository, Repository<AdminRequest> requestRepository,
			LeopardDatabase db)
		{
			AuthStore = authStore;
			TopicRepository = topicRepository;
			MemberRepository = memberRepository;
			RequestRepository = requestRepository;
			Db = db;
		}


		[HttpPost("send-admin-request")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> SendAdminRequest([FromBody]SendAdminRequestModel model)
		{
			var topicId = XUtils.ParseId(model.TopicId);

			// Topic is group topic
			var topic = await TopicRepository.FirstOrDefaultAsync(p => p.Id == topicId);
			if (topic == null) return new ApiError(MyErrorCode.IdNotFound, "Topic id not found").Wrap();
			if (topic.IsGroup == false)
				return new ApiError(MyErrorCode.TypeMismatch, "This topic is not group").Wrap();

			// should be normal member
			var member = await MemberRepository.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId);
			if (member?.Role != MemberRole.Normal)
				return new ApiError(MyErrorCode.RoleMismatch, "You are not normal row of the topic").Wrap();

			// send request
			var request = new AdminRequest(topicId.Value, AuthStore.UserId.Value, model.Text);
			await RequestRepository.PutAsync(request);
			return Ok(new IdResponse(request.Id));
		}
		public class SendAdminRequestModel
		{
			[Required]
			public string TopicId { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(64)]
			public string Text { get; set; }
		}


		[NotCommand]
		[HttpGet("get-request")]
		[Consumes(Application.Json)]
		[Produces(typeof(QAdminRequest))]
		public async Task<IActionResult> GetRequestById(string id)
		{
			var requestId = XUtils.ParseId(id);

			var request = await RequestRepository.FirstOrDefaultAsync(p => p.Id == requestId);

			var data = QAdminRequest.NormalView(request);
			Console.WriteLine($"AAAAA {request.Status}");

			return Ok(data);
		}
		public class QAdminRequest
		{
			public ObjectId Id { get; set; }
			public ObjectId TopicId { get; set; }
			public ObjectId SenderId { get; set; }
			public string Text { get; set; }
			public RequestStatus Status { get; set; }

			public static QAdminRequest NormalView(AdminRequest p)
			{
				return p == null ? null : new QAdminRequest
				{
					Id = p.Id,
					TopicId = p.TopicId,
					SenderId = p.SenderId,
					Text = p.Text,
					Status = p.Status
				};
			}
		}


		[HttpPost("handle-request")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> HandleRequest([FromBody]HandleRequestModel model)
		{
			// request should be unhandled
			var requestId = XUtils.ParseId(model.RequestId);
			var request = await RequestRepository.FirstOrDefaultAsync(p => p.Id == requestId);
			if (request == null)
				return new ApiError(MyErrorCode.IdNotFound, "Request id not found").Wrap();
			if (request.Status != RequestStatus.Unhandled)
				return new ApiError(MyErrorCode.TypeMismatch, "Request is not unhandled").Wrap();

			// should have super role
			var member = await MemberRepository
				.FirstOrDefaultAsync(p => p.UserId == AuthStore.UserId && p.TopicId == request.TopicId);
			if (member.Role != MemberRole.Super)
				return new ApiError(MyErrorCode.PermissionDenied, "You are not super administrator of the group").Wrap();

			// handle
			request.Handle(model.Accept);
			await RequestRepository.PutAsync(request);

			if (model.Accept)
			{
				var membership2 = await MemberRepository
					.FirstOrDefaultAsync(p => p.UserId == request.SenderId && p.TopicId == request.TopicId);

				membership2.SetRole(MemberRole.Admin);
				await MemberRepository.PutAsync(membership2);
			}

			return Ok();

		}
		public class HandleRequestModel
		{
			[Required]
			public string RequestId { get; set; }

			[Required]
			public bool Accept { get; set; }
		}


		[NotCommand]
		[HttpGet("unhandled-requests")]
		[Produces(typeof(QAdminRequest[]))]
		public async Task<IActionResult> GetUnhandledRequests(string topicId, int page, bool newest)
		{
			const int pageSize = 20;

			var tid = XUtils.ParseId(topicId);
			var query = Db.GetCollection<AdminRequest>().AsQueryable()
				.Where(p => p.TopicId == tid && p.Status == RequestStatus.Unhandled);

			if (newest)
				query = query.OrderByDescending(p => p.CreatedAt);
			else
				query = query.OrderBy(p => p.CreatedAt);

			query = query.Skip(page * pageSize).Take(pageSize);

			var requests = await query.ToListAsync();
			var data = requests.Select(p => QAdminRequest.NormalView(p)).ToList();

			return Ok(data);
		}
	}
}