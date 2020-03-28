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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

		public GroupManageController(AuthStore authStore, Repository<Topic> topicRepository,
			Repository<TopicMember> memberRepository, Repository<AdminRequest> requestRepository)
		{
			AuthStore = authStore;
			TopicRepository = topicRepository;
			MemberRepository = memberRepository;
			RequestRepository = requestRepository;
		}


		[HttpPost("send-admin-request")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(AuthenticationService))]
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
	}
}