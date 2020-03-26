using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.PostAG;
using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using static System.Net.Mime.MediaTypeNames;

namespace Leopard.API.Controllers
{
	[Route("api/topic")]
	[ApiController]
	public class TopicController : ControllerBase
	{
		public Repository<Topic> TopicRepository { get; }
		public Repository<Work> WorkRepository { get; }
		public AuthStore AuthStore { get; }
		public Repository<TopicMember> MemberRepository { get; }
		public Repository<Post> PostRepository { get; }

		public TopicController(Repository<Topic> topicRepository, Repository<Work> workRepository, AuthStore authStore,
			Repository<TopicMember> memberRepository, Repository<Post> postRepository)
		{
			TopicRepository = topicRepository;
			WorkRepository = workRepository;
			AuthStore = authStore;
			MemberRepository = memberRepository;
			PostRepository = postRepository;
		}


		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> CreateTopic([FromBody]CreateTopicModel model)
		{
			// check related work
			var workId = XUtils.ParseId(model.RelatedWork);
			var work = await WorkRepository.FirstOrDefaultAsync(p => p.Id == workId);
			if (work == null)
				workId = null;

			// create topic and set member=1
			var topic = new Topic(model.IsGroup, model.Name, model.Description, workId, AuthStore.UserId.Value);
			topic.SetMemberCount(1);

			await TopicRepository.PutAsync(topic);

			return Ok(new IdResponse(topic.Id));
		}
		public class CreateTopicModel
		{
			[Required]
			public bool IsGroup { get; set; }

			[Required]
			[MinLength(1)]
			public string Name { get; set; }

			[Required]
			[MinLength(3)]
			public string Description { get; set; }

			public string RelatedWork { get; set; }
		}


		[HttpPost("join-topic")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> JoinTopic([FromBody]JoinTopicModel model)
		{
			var topicId = XUtils.ParseId(model.TopicId);
			if (topicId == null)
				return new ApiError(MyErrorCode.IdNotFound, "Id parse error").Wrap();

			// Check if already in topic
			var member = await MemberRepository
				.FirstOrDefaultAsync(p => p.TopicId == topicId.Value && p.UserId == AuthStore.UserId.Value);

			if (member != null)
				return new ApiError(MyErrorCode.UniqueConstraintConflict, "你已经加入过了").Wrap();

			// join topic
			member = new TopicMember(topicId.Value, AuthStore.UserId.Value, MemberRole.Normal);
			await MemberRepository.PutAsync(member);

			return Ok();
		}
		public class JoinTopicModel
		{
			[Required]
			public string TopicId { get; set; }
		}


		[HttpPost("send-post")]
		[Consumes("multipart/form-data")]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> SendPost([FromForm]SendPostModel model, [FromServices]IBlobBucket blobBucket)
		{
			// 不需要判断topic是否存在

			// should be a member of the topic

			var topicId = XUtils.ParseId(model.TopicId);
			if (topicId == null)
				return new ApiError(MyErrorCode.IdNotFound, "TopicId parse error").Wrap();

			var member = await MemberRepository.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId.Value);

			if (member == null)
				return new ApiError(MyErrorCode.PermissionDenied, "你不是成员").Wrap();

			// send post

			//	 put image
			string imageUrl = null;
			if (model.Image != null)
			{
				if (model.Image.Length > 1024 * 1024 * 5)//5mb
					return new ApiError(MyErrorCode.FileTooLarge, "图片太大，不能超过5MB").Wrap();
				using (var stream = model.Image.OpenReadStream())
				{
					imageUrl = await blobBucket.PutBlobAsync(stream, Path.GetRandomFileName());
				}
			}

			var post = new Post(AuthStore.UserId.Value, topicId.Value, model.Text, model.Title, imageUrl);
			await PostRepository.PutAsync(post);

			return Ok(new IdResponse(post.Id));
		}
		public class SendPostModel
		{
			[Required]
			public string TopicId { get; set; }

			public string Title { get; set; }

			public string Text { get; set; }

			public IFormFile Image { get; set; }
		}
	}
}