using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.DiscussionAG;
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

		/// <summary>
		/// 创建话题/小组。
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> CreateTopic([FromBody] CreateTopicModel model)
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
			/// <summary>
			/// true：小组；false：话题
			/// </summary>
			[Required]
			public bool IsGroup { get; set; }

			/// <summary>
			/// 名称
			/// </summary>
			[Required]
			[MinLength(1)]
			public string Name { get; set; }

			/// <summary>
			/// 简介
			/// </summary>
			[Required]
			[MinLength(3)]
			public string Description { get; set; }

			/// <summary>
			/// 关联的作品。如果无关联的作品，则为null。
			/// </summary>
			public string RelatedWork { get; set; }
		}


		/// <summary>
		/// 加入话题/小组。
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("join-topic")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> JoinTopic([FromBody] JoinTopicModel model)
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


		/// <summary>
		/// 在小组内发送帖子
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("send-post")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> SendPost([FromBody] SendPostModel model)
		{
			var topicId = XUtils.ParseId(model.TopicId);
			if (topicId == null)
				return new ApiError(MyErrorCode.IdNotFound, "TopicId parse error").Wrap();

			// Topic exist and is group
			var topic = await Context.Topics.FirstOrDefaultAsync(p => p.Id == topicId);

			if (topic?.IsGroup != true)
				return new ApiError(MyErrorCode.TypeMismatch, "Topic not exist or is not group").Wrap();

			// should be a member of the topic
			var member = await Context.TopicMembers.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId.Value);

			if (member == null)
				return new ApiError(MyErrorCode.PermissionDenied, "你不是成员").Wrap();

			// send post

			var post = new Post(AuthStore.UserId.Value, topicId.Value, model.Text, model.Title);

			await Context.AddAsync(post);
			await Context.GoAsync();

			return Ok(new IdResponse(post.Id));
		}
		public class SendPostModel
		{
			/// <summary>
			/// 小组id
			/// </summary>
			[Required]
			public string TopicId { get; set; }

			/// <summary>
			/// 标题
			/// </summary>
			public string Title { get; set; }

			/// <summary>
			/// 正文
			/// </summary>
			public string Text { get; set; }
		}


		/// <summary>
		/// 在话题内发送讨论
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("send-discussion")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> SendDiscussion([FromBody] SendDiscussionModel model)
		{
			var topicId = model.TopicId;
			if (topicId == null)
				return new ApiError(MyErrorCode.IdNotFound, "TopicId parse error").Wrap();

			// Topic exist and is not group
			var topic = await Context.Topics.FirstOrDefaultAsync(p => p.Id == topicId);

			if (topic?.IsGroup != false)
				return new ApiError(MyErrorCode.TypeMismatch, "Topic not exist or is group").Wrap();

			// should be a member of the topic
			var member = await Context.TopicMembers.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId.Value);

			if (member == null)
				return new ApiError(MyErrorCode.PermissionDenied, "你不是成员").Wrap();

			// Send discussion
			var discussion = new Discussion(topicId, AuthStore.UserId.Value, model.Text, model.ImageUrl);

			await Context.AddAsync(discussion);
			await Context.GoAsync();

			return Ok(new IdResponse(discussion.Id));
		}
		public class SendDiscussionModel
		{
			/// <summary>
			/// 话题id
			/// </summary>
			public Guid TopicId { get; set; }

			/// <summary>
			/// 文字
			/// </summary>
			[MaxLength(800)]
			public string Text { get; set; }

			/// <summary>
			/// 图片url。这个怎么使用呢？首先用单独的一个请求上传图片，获取id，然后再在这里附带id。
			/// </summary>
			[MaxLength(100)]
			public string ImageUrl { get; set; }
		}

		static void ZeroMin(ref int page)
		{
			page = Math.Max(page, 0);
		}

		/// <summary>
		/// 获取话题的讨论
		/// </summary>
		/// <param name="topicId">话题id</param>
		/// <param name="page">页码。start from 0</param>
		/// <returns></returns>
		[HttpGet("get-discussions")]
		[Produces(typeof(QDiscussion[]))]
		public async Task<IActionResult> GetDiscussions(Guid topicId, int page)
		{
			ZeroMin(ref page);
			const int pageSize = 20;

			var query = from p in Context.Discussions.Where(p => p.TopicId == topicId)
						join q in Context.Users
						on p.SenderId equals q.Id
						orderby p.CreatedAt descending
						select new { Discussion = p, Sender = q };

			var data = await query.Skip(pageSize * page).Take(pageSize).ToListAsync();

			var qdiscussions = data.Select(p => QDiscussion.NormalView(p.Discussion, p.Sender)).ToList();

			return Ok(qdiscussions);
		}
		public class QDiscussion
		{
			public Guid TopicId { get; set; }
			public Guid SenderId { get; set; }
			public string Text { get; set; }
			public string ImageUrl { get; set; }

			/// <summary>
			/// 发送者
			/// </summary>
			public QUser User { get; set; }

			public static QDiscussion NormalView(Discussion p, User user)
			{
				return p == null ? null : new QDiscussion
				{
					TopicId = p.TopicId,
					SenderId = p.SenderId,
					Text = p.Text,
					ImageUrl = p.Image,

					User = QUser.NormalView(user)
				};
			}
		}

		/// <summary>
		/// 搜索话题中的讨论
		/// </summary>
		/// <param name="word">关键词</param>
		/// <param name="page">页码。start from 0</param>
		/// <returns></returns>
		[HttpGet("search-discussions")]
		[Produces(typeof(QDiscussion[]))]
		public async Task<IActionResult> SearchDiscussions(string word, int page)
		{
			page = Math.Max(0, page);
			int pageSize = 20;

			var query = from p in Context.Discussions.AsNoTracking()
						where p.TextTsv.Matches(EF.Functions.WebSearchToTsQuery("testzhcfg", word))
						join q in Context.Users
						on p.SenderId equals q.Id
						orderby p.TextTsv.RankCoverDensity(EF.Functions.WebSearchToTsQuery("testzhcfg", word)) descending
						select new { Discussion = p, Sender = q };

			var data = await query.Skip(pageSize * page).Take(pageSize).ToListAsync();

			var qdiscuss = data.Select(p => QDiscussion.NormalView(p.Discussion, p.Sender)).ToList();

			return Ok(qdiscuss);
		}

		/// <summary>
		/// 上传文件。
		/// 这是一个通用的上传文件的接口。
		/// 限制大小：5mb。
		/// </summary>
		/// <param name="model"></param>
		/// <param name="blobBucket"></param>
		/// <returns></returns>
		[HttpPost("upload-file")]
		[Consumes("multipart/form-data")]
		[Produces(typeof(UploadFileResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> UploadFile([FromForm] UploadFileModel model, [FromServices] IBlobBucket blobBucket)
		{
			var file = model.File;

			if (file == null)
			{
				return new ApiError(MyErrorCode.ModelInvalid, "File is null").Wrap();
			}

			if (file.Length > 1024 * 1024 * 5)//5mb
				return new ApiError(MyErrorCode.FileTooLarge, "图片太大，不能超过5MB").Wrap();


			string blobUrl;
			using (var stream = file.OpenReadStream())
			{
				blobUrl = await blobBucket.PutBlobAsync(stream, Path.GetRandomFileName());
			}

			return Ok(new UploadFileResponse { Url = blobUrl });
		}
		public class UploadFileModel
		{
			[Required]
			public IFormFile File { get; set; }
		}
		public class UploadFileResponse
		{
			public string Url { get; set; }
		}

		/// <summary>
		/// 获取话题/小组的基本信息
		/// </summary>
		/// <param name="topicId"></param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("get-topic-profile")]
		[Produces(typeof(QTopic))]
		public async Task<IActionResult> GetTopicProfile(string topicId)
		{
			var id = XUtils.ParseId(topicId);
			var topic = await Context.Topics.FirstOrDefaultAsync(p => p.Id == id);
			var qtopic = QTopic.NormalView(topic);
			return Ok(qtopic);
		}
		public class QTopic
		{
			public Guid Id { get; set; }
			public bool IsGroup { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public Guid? RelatedWork { get; set; }

			/// <summary>
			/// 成员数量
			/// </summary>
			public int MemberCount { get; set; }

			public static QTopic NormalView(Topic p)
			{
				return p == null ? null : new QTopic
				{
					Id = p.Id,
					IsGroup = p.IsGroup,
					Name = p.Name,
					Description = p.Description,
					RelatedWork = p.RelatedWork,
					MemberCount = p.MemberCount
				};
			}
		}

		/// <summary>
		/// 获取当前登录用户和话题/小组的成员关系实体。
		/// 不是成员：返回null。
		/// 如果是话题成员，那么请忽略Role，因为Role只对小组有用。
		/// </summary>
		/// <param name="topicId"></param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("get-membership")]
		[Produces(typeof(QTopicMember))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> GetMembership(Guid topicId)
		{
			var membership = await Context.TopicMembers.FirstOrDefaultAsync(p => p.TopicId == topicId && p.UserId == AuthStore.UserId);
			var qmember = QTopicMember.NormalView(membership);
			return Ok(qmember);
		}
		public class QTopicMember
		{
			public Guid TopicId { get; set; }
			public Guid UserId { get; set; }

			/// <summary>
			/// 成员角色。只在目标是小组时有效。
			/// </summary>
			public MemberRole Role { get; set; }

			public static QTopicMember NormalView(TopicMember p)
			{
				return p == null ? null : new QTopicMember
				{
					TopicId = p.TopicId,
					UserId = p.UserId,
					Role = p.Role
				};
			}
		}

		/// <summary>
		/// 获取小组的帖子
		/// </summary>
		/// <param name="topicId">小组id</param>
		/// <param name="page">页码。从0开始。</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("get-posts")]
		[Produces(typeof(QPost[]))]
		public async Task<IActionResult> GetPosts(string topicId, int page)
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
						select new
						{
							post = p,
							user = q,
							replyCount = Context.Replies.Count(z => z.PostId == p.Id),
							lastReply = Context.Replies.Where(z => z.PostId == p.Id)
								.DefaultIfEmpty().Max(p => p == null ? -2 : p.CreatedAt)
						};

			query = query.OrderByDescending(p => p.post.IsPinned)
				.ThenByDescending(p => p.lastReply)
				.Skip(page * pageSize)
				.Take(pageSize);

			var data = await query.ToListAsync();

			var qposts = data.Select(
				p => QPost.NormalView(p.post, QUser.NormalView(p.user), p.replyCount, p.lastReply)).ToList();

			return Ok(qposts);
		}
		public class QPost
		{
			public Guid Id { get; set; }
			public long CreatedAt { get; set; }
			public long UpdatedAt { get; set; }

			public Guid SenderId { get; set; }
			public string Title { get; set; }
			public string Text { get; set; }

			/// <summary>
			/// 是否是置顶帖
			/// </summary>
			public bool IsPinned { get; set; }

			/// <summary>
			/// 是否是精华帖
			/// </summary>
			public bool IsEssense { get; set; }

			/// <summary>
			/// 发送者
			/// </summary>
			public QUser User { get; set; }

			/// <summary>
			/// 回帖数量
			/// </summary>
			public int ReplyCount { get; set; }

			/// <summary>
			/// 最新回帖时间
			/// </summary>
			public long LastReply { get; set; }

			public static QPost NormalView(Post p, QUser user, int replyCount, long lastReply)
			{
				return p == null ? null : new QPost
				{
					Id = p.Id,
					CreatedAt = p.CreatedAt,
					UpdatedAt = p.UpdatedAt,
					SenderId = p.SenderId,
					Title = p.Title,
					Text = p.Text,
					IsPinned = p.IsPinned,
					IsEssense = p.IsEssence,

					User = user,
					ReplyCount = replyCount,
					LastReply = lastReply
				};
			}
		}


		/// <summary>
		/// 搜索帖子
		/// </summary>
		/// <param name="word">关键词</param>
		/// <param name="page">页码。从0开始。</param>
		/// <returns></returns>
		[HttpGet("search-posts")]
		[Produces(typeof(QPost[]))]
		public async Task<IActionResult> SearchPosts(string word, int page)
		{
			word ??= "";
			page = Math.Max(0, page);
			const int pageSize = 20;

			var query = from p in Context.Posts
						where p.Tsv.Matches(EF.Functions.WebSearchToTsQuery("testzhcfg", word))
						join q in Context.Users
						on p.SenderId equals q.Id
						select new
						{
							post = p,
							user = q,
							replyCount = Context.Replies.Count(z => z.PostId == p.Id),
							lastReply = Context.Replies.Where(z => z.PostId == p.Id)
								.DefaultIfEmpty().Max(p => p == null ? -2 : p.CreatedAt)
						};

			query = query
				.OrderByDescending(p => p.post.Tsv.RankCoverDensity(EF.Functions.WebSearchToTsQuery("testzhcfg", word)))
				.Skip(pageSize * page).Take(pageSize);

			var data = await query.ToListAsync();

			var qposts = data.Select(
				p => QPost.NormalView(p.post, QUser.NormalView(p.user), p.replyCount, p.lastReply)).ToList();

			return Ok(qposts);
		}


		/// <summary>
		/// 根据id获取帖子
		/// </summary>
		/// <param name="id">帖子id</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("get-post-by-id")]
		[Produces(typeof(QPost))]
		public async Task<IActionResult> GetPostById(string id)
		{
			var postId = XUtils.ParseId(id);
			if (postId == null)
				return new ApiError(MyErrorCode.IdNotFound, "Id parse error").Wrap();

			var query = from p in Context.Posts
						where p.Id == postId
						join q in Context.Users
						on p.SenderId equals q.Id
						select new
						{
							post = p,
							user = q,
							replyCount = Context.Replies.Count(z => z.PostId == p.Id),
							lastReply = Context.Replies.Where(z => z.PostId == p.Id)
									.DefaultIfEmpty().Max(p => p == null ? -2 : p.CreatedAt)
						};

			var data = await query.FirstOrDefaultAsync();

			var qpost = QPost.NormalView(data.post, QUser.NormalView(data.user), data.replyCount, data.lastReply);

			return Ok(qpost);
		}

		/// <summary>
		/// 回帖
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("send-reply")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> SendReply([FromBody] SendReplyModel model)
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
			[MinLength(3)]
			public string Text { get; set; }
		}


		/// <summary>
		/// 获取某帖子的回帖
		/// </summary>
		/// <param name="postId">帖子id</param>
		/// <param name="page">页码</param>
		/// <returns></returns>
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
						 .OrderBy(p => p.reply.CreatedAt).ThenBy(p=>p.reply.Id);

			var data = await query.Skip(pageSize * page).Take(pageSize).ToListAsync();

			var repliesV = data.Select(p => QReply.NormalView(p.reply, p.user));

			return Ok(repliesV);
		}
		public class QReply
		{
			public Guid Id { get; set; }
			public Guid SenderId { get; set; }

			/// <summary>
			/// 帖子id
			/// </summary>
			public Guid PostId { get; set; }

			/// <summary>
			/// 文字
			/// </summary>
			public string Text { get; set; }

			/// <summary>
			/// 回帖者
			/// </summary>
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

		/// <summary>
		/// 搜索回帖
		/// </summary>
		/// <param name="word">关键词</param>
		/// <param name="page">页码，从0开始。</param>
		/// <returns></returns>
		[HttpGet("search-replies")]
		[Produces(typeof(QReply[]))]
		public async Task<IActionResult> SearchReplies(string word, int page)
		{
			word ??= "";
			page = Math.Max(0, page);
			const int pageSize = 20;

			var query = from p in Context.Replies
						where p.TextTsv.Matches(EF.Functions.WebSearchToTsQuery("testzhcfg", word))
						join q in Context.Users on p.SenderId equals q.Id
						orderby p.TextTsv.RankCoverDensity(EF.Functions.WebSearchToTsQuery("testzhcfg", word)) descending
						select new { reply = p, user = q };

			var data = await query.Skip(pageSize * page).Take(pageSize).ToListAsync();

			var repliesV = data.Select(p => QReply.NormalView(p.reply, p.user));

			return Ok(repliesV);
		}

		/// <summary>
		/// 管理帖子
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("do-admin")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> DoAdmin([FromBody] DoAdminModel model)
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
			if (model.Action == AdminAction.IsPinned)
				post.SetPinned(model.Status);

			if (model.Action == AdminAction.IsEssence)
				post.SetEssence(model.Status);

			if (model.Action == AdminAction.Remove)
			{
				if (model.Status)
					Context.Remove(post);
			}

			await Context.GoAsync();

			return Ok();
		}
		public class DoAdminModel
		{
			/// <summary>
			/// 帖子id
			/// </summary>
			public string PostId { get; set; }

			/// <summary>
			/// 操作
			/// </summary>
			public AdminAction Action { get; set; }

			/// <summary>
			/// 状态值。例如Action=IsPinned, Status=false，就意味着对帖子【取消精华】。
			/// </summary>
			public bool Status { get; set; }
		}

		public enum AdminAction
		{
			/// <summary>
			/// 是否置顶
			/// </summary>
			IsPinned,

			/// <summary>
			/// 是否是精华帖
			/// </summary>
			IsEssence,

			/// <summary>
			/// 删除帖子
			/// </summary>
			Remove
		}


		/// <summary>
		/// 搜索话题/小组
		/// </summary>
		/// <param name="word">关键词</param>
		/// <param name="isGroup">是否是小组</param>
		/// <param name="page">页码。start from 0</param>
		/// <returns></returns>
		[HttpGet("search-topics")]
		[Produces(typeof(QTopic[]))]
		public async Task<IActionResult> SearchTopics(string word, bool isGroup, int page)
		{
			word ??= "";
			page = Math.Max(0, page);
			const int pageSize = 20;

			var query = from p in Context.Topics
						where p.IsGroup == isGroup && p.Tsv.Matches(EF.Functions.WebSearchToTsQuery("testzhcfg", word))
						orderby p.Tsv.RankCoverDensity(EF.Functions.WebSearchToTsQuery("testzhcfg", word)) descending
						select p;

			var data = await query.Skip(page * pageSize).Take(pageSize).ToListAsync();

			var qtopics = data.Select(p => QTopic.NormalView(p));

			return Ok(qtopics);
		}


		[NotCommand]
		[HttpGet("hotest-topic-not-group")]
		[Produces(typeof(QTopic[]))]
		public async Task<IActionResult> HotestTopicsNotGroup()
		{
			var query = from p in Context.Topics.Where(p => p.IsGroup == false)
						select new
						{
							topic = p,
							discussionCount = Context.Discussions.Count(o => o.TopicId == p.Id)
						}
						into k
						orderby k.discussionCount descending
						select k;
			var topics = await query.Take(20).ToListAsync();

			var qtopics = topics.Select(p => QTopic.NormalView(p.topic));

			return Ok(qtopics);
		}

	}
}