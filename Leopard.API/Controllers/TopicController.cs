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
using Leopard.Domain.ReplyAG;
using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Domain.UserAG;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Leopard.API.Controllers
{
	[Route("api/topic")]
	[ApiController]
	public class TopicController : ControllerBase
	{
		public AuthStore AuthStore { get; }
		public OneContext Context { get; }

		public TopicController(AuthStore authStore, OneContext context)
		{
			AuthStore = authStore;
			Context = context;
		}


		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> CreateTopic([FromBody]CreateTopicModel model)
		{
			// check related work
			var workId = XUtils.ParseId(model.RelatedWork);
			var work = await Context.Works.FirstOrDefaultAsync(p => p.Id == workId);
			if (work == null)
				workId = null;

			// create topic and set member=1
			var topic = new Topic(model.IsGroup, model.Name, model.Description, workId, AuthStore.UserId.Value);
			topic.SetMemberCount(1);

			await Context.AddAsync(topic);
			await Context.GoAsync();

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

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> JoinTopic([FromBody]JoinTopicModel model)
		{
			var topicId = XUtils.ParseId(model.TopicId);
			if (topicId == null)
				return new ApiError(MyErrorCode.IdNotFound, "Id parse error").Wrap();

			// Check topic exist
			var topic = await Context.Topics.FirstOrDefaultAsync(p => p.Id == topicId);
			if (topic == null)
				return new ApiError(MyErrorCode.IdNotFound, "Topic id not found").Wrap();

			// Check if already in topic
			var member = await Context.TopicMembers
				.FirstOrDefaultAsync(p => p.TopicId == topicId.Value && p.UserId == AuthStore.UserId.Value);

			if (member != null)
				return new ApiError(MyErrorCode.UniqueConstraintConflict, "你已经加入过了").Wrap();

			// join topic
			member = new TopicMember(topicId.Value, AuthStore.UserId.Value, MemberRole.Normal);

			await Context.AddAsync(member);
			await Context.GoAsync();

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

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> SendPost([FromForm]SendPostModel model, [FromServices]IBlobBucket blobBucket)
		{
			// 不需要判断topic是否存在

			// should be a member of the topic

			var topicId = XUtils.ParseId(model.TopicId);
			if (topicId == null)
				return new ApiError(MyErrorCode.IdNotFound, "TopicId parse error").Wrap();

			var member = await Context.TopicMembers.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId.Value);

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

			await Context.AddAsync(post);
			await Context.GoAsync();

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


		[NotCommand]
		[HttpGet("get-posts")]
		[Produces(typeof(QPost[]))]
		public async Task<IActionResult> GetPosts(string topicId, int page, bool newest)
		{
			const int pageSize = 20;
			page = Math.Max(0, page);

			var tid = XUtils.ParseId(topicId);
			if (tid == null)
				return new ApiError(MyErrorCode.IdNotFound, "topicId parse error").Wrap();

			var query = from p in Context.Posts
						where p.TopicId == tid
						join q in Context.Users
						on p.SenderId equals q.Id
						select new { post = p, user = q };

			var oq = query.OrderByDescending(p => p.post.IsPinned);

			if (newest)
				oq = oq.ThenByDescending(p => p.post.CreatedAt);
			else
				oq = oq.ThenBy(p => p.post.CreatedAt);

			var q2 = oq.Skip(page * pageSize).Take(pageSize);

			var posts = await q2.ToListAsync();

			var data = posts.Select(p => QPost.NormalView(p.post, QUser.NormalView(p.user))).ToList();

			return Ok(data);
		}
		public class QPost
		{
			public Guid Id { get; set; }
			public long CreatedAt { get; set; }
			public long UpdatedAt { get; set; }

			public Guid SenderId { get; set; }
			public string Image { get; set; }
			public string Title { get; set; }
			public string Text { get; set; }
			public bool IsPinned { get; set; }
			public bool IsEssense { get; set; }

			public QUser User { get; set; }

			public static QPost NormalView(Post p, QUser user)
			{
				return p == null ? null : new QPost
				{
					Id = p.Id,
					CreatedAt = p.CreatedAt,
					UpdatedAt = p.UpdatedAt,
					SenderId = p.SenderId,
					Image = p.Image,
					Title = p.Title,
					Text = p.Text,
					IsPinned = p.IsPinned,
					IsEssense = p.IsEssence,

					User = user
				};
			}
		}

		[NotCommand]
		[HttpGet("by-id")]
		[Produces(typeof(QPost))]
		public async Task<IActionResult> GetById(string id)
		{
			var postId = XUtils.ParseId(id);
			if (postId == null)
				return new ApiError(MyErrorCode.IdNotFound, "Id parse error").Wrap();

			var query = from p in Context.Posts
						where p.Id == postId
						join q in Context.Users
						on p.SenderId equals q.Id
						select new { post = p, user = q }; ;

			var data = await query.FirstOrDefaultAsync();

			var qpost = QPost.NormalView(data.post, QUser.NormalView(data.user));

			return Ok(qpost);
		}


		[HttpPost("send-reply")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> SendReply([FromBody]SendReplyModel model)
		{
			// post exist
			var postId = XUtils.ParseId(model.PostId);
			var post = await Context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
			if (post == null)
				return new ApiError(MyErrorCode.IdNotFound, "Post id not found").Wrap();

			// user is a member of post.topic
			var member = await Context.TopicMembers.FirstOrDefaultAsync(p => p.UserId == AuthStore.UserId && p.TopicId == post.TopicId);
			if (member == null)
				return new ApiError(MyErrorCode.NotAMember, "You are not a member of the topic").Wrap();

			// send reply
			var reply = new Reply(AuthStore.UserId.Value, post.Id, model.Text);

			await Context.AddAsync(reply);
			await Context.GoAsync();
			return Ok(new IdResponse(reply.Id));
		}
		public class SendReplyModel
		{
			[Required]
			public string PostId { get; set; }

			[Required]
			[MinLength(25)]
			public string Text { get; set; }
		}


		[NotCommand]
		[HttpGet("get-replies")]
		[Produces(typeof(QReply[]))]
		public async Task<IActionResult> GetReplies(string postId, int page)
		{
			const int pageSize = 20;

			// post exist
			var pid = XUtils.ParseId(postId);
			var post = await Context.Posts.FirstOrDefaultAsync(p => p.Id == pid);
			if (post == null)
				return new ApiError(MyErrorCode.IdNotFound, "Post id not found").Wrap();

			// get
			var query = (from p in Context.Replies.Where(p => p.PostId == pid)
						 join q in Context.Users
						 on p.SenderId equals q.Id
						 select new { reply = p, user = q })
						 .OrderBy(p => p.reply.CreatedAt);

			var data = await query.Skip(pageSize * page).Take(pageSize).ToListAsync();

			var repliesV = data.Select(p => QReply.NormalView(p.reply, p.user));

			return Ok(repliesV);
		}
		public class QReply
		{
			public Guid Id { get; set; }
			public Guid SenderId { get; set; }
			public Guid PostId { get; set; }
			public string Text { get; set; }

			public QUser User { get; set; }

			public static QReply NormalView(Reply p, User user)
			{
				return p == null ? null : new QReply
				{
					Id = p.Id,
					SenderId = p.SenderId,
					PostId = p.PostId,
					Text = p.Text,

					User = QUser.NormalView(user)
				};
			}
		}


		[HttpPost("do-admin")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> DoAdmin([FromBody]DoAdminModel model)
		{
			// post exist
			var postId = XUtils.ParseId(model.PostId);
			var post = await Context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
			if (post == null) return new ApiError(MyErrorCode.IdNotFound, "Post id not found").Wrap();

			// user >= post.Topic.Admin
			var member = await Context.TopicMembers.FirstOrDefaultAsync(p => p.TopicId == post.TopicId && p.UserId == AuthStore.UserId);
			if (member == null) return new ApiError(MyErrorCode.NotAMember, "You are not a member").Wrap();
			if ((int)member.Role < (int)MemberRole.Admin)
				return new ApiError(MyErrorCode.PermissionDenied, "You should be at least admin").Wrap();

			// ok
			post.SetPinned(model.IsPinned);
			post.SetEssence(model.IsEssence);

			if (model.Delete)
				Context.Remove(post);

			await Context.GoAsync();

			return Ok();
		}
		public class DoAdminModel
		{
			public string PostId { get; set; }
			public bool IsPinned { get; set; }
			public bool IsEssence { get; set; }
			public bool Delete { get; set; }
		}
	}
}