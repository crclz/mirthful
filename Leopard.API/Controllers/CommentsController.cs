using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.AttitudeAG;
using Leopard.Domain.CommentAG;
using Leopard.Domain.ReportAG;
using Leopard.Domain.UserAG;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Leopard.API.Controllers
{
	[Route("api/comments")]
	[ApiController]
	public class CommentsController : ControllerBase
	{
		public AuthStore AuthStore { get; }
		public OneContext Context { get; }

		public CommentsController(AuthStore authStore, OneContext context)
		{
			AuthStore = authStore;
			Context = context;
		}


		/// <summary>
		/// 对作品发送评论
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> CreateComment([FromBody]CreateCommentModel model)
		{
			var workId = XUtils.ParseId(model.WorkId);
			var work = await Context.Works.FirstOrDefaultAsync(p => p.Id == workId);
			if (work == null)
				return new ApiError(MyErrorCode.IdNotFound, "不存在对应id的作品").Wrap();

			var comment = await Context.Comments.FirstOrDefaultAsync(p => p.WorkId == workId && p.SenderId == AuthStore.UserId);
			if (comment != null)
				return new ApiError(MyErrorCode.UniqueConstraintConflict, "你已经评价过此作品").Wrap();

			comment = new Comment(AuthStore.UserId.Value, workId.Value, model.Title, model.Text, model.Rating);

			await Context.AddAsync(comment);
			await Context.GoAsync();

			return Ok(new IdResponse(comment.Id));
		}
		public class CreateCommentModel
		{
			/// <summary>
			/// 作品Id
			/// </summary>
			[Required]
			public string WorkId { get; set; }

			/// <summary>
			/// 评论的标题
			/// </summary>
			[Required]
			[MinLength(1)]
			public string Title { get; set; }

			/// <summary>
			/// 评论的正文
			/// </summary>
			[Required]
			[MinLength(25)]
			public string Text { get; set; }

			/// <summary>
			/// 打分
			/// </summary>
			[Required]
			[Range(1, 5)]
			public int Rating { get; set; }
		}

		/// <summary>
		/// 查询评论
		/// </summary>
		/// <param name="id">评论Id</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("get-by-id")]
		[Produces(typeof(QComment))]
		public async Task<QComment> GetById(string id)
		{
			var commentId = XUtils.ParseId(id);
			if (commentId == null)
				return null;

			var query = from p in Context.Comments.Where(r => r.Id == commentId)
						join q in Context.Users on p.SenderId equals q.Id
						select new
						{
							comment = p,
							user = q,
							myAtt = Context.Attitudes.Where(o => o.CommentId == p.Id && o.SenderId == AuthStore.UserId).FirstOrDefault()
						};

			var data = await query.FirstOrDefaultAsync();

			var user = QUser.NormalView(data.user);
			var commentQ = QComment.NormalView(data.comment, user, data.myAtt?.Agree);

			return commentQ;
		}
		public class QComment
		{
			public Guid Id { get; set; }
			public long CreatedAt { get; set; }
			public long UpdatedAt { get; set; }
			/// <summary>
			/// 评论发送者id
			/// </summary>
			public Guid SenderId { get; set; }

			/// <summary>
			/// 作品id
			/// </summary>
			public Guid WorkId { get; set; }

			/// <summary>
			/// 评论标题
			/// </summary>
			public string Title { get; set; }
			public string Text { get; set; }
			public int Rating { get; set; }
			public int AgreeCount { get; set; }
			public int DisagreeCount { get; set; }

			public QUser User { get; set; }

			public bool? MyAttitude { get; set; }

			public static QComment NormalView(Comment c, QUser user, bool? myAttitude)
			{
				return c == null ? null : new QComment
				{
					Id = c.Id,
					CreatedAt = c.CreatedAt,
					UpdatedAt = c.UpdatedAt,
					SenderId = c.SenderId,
					WorkId = c.WorkId,
					Title = c.Title,
					Text = c.Text,
					Rating = c.Rating,
					AgreeCount = c.AgreeCount,
					DisagreeCount = c.DisagreeCount,
					User = user,
					MyAttitude = myAttitude
				};
			}
		}


		[HttpPost("express-attitude")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> ExpressAttitude(string commentId, bool? agree)
		{
			var cid = XUtils.ParseId(commentId);
			if (cid == null)
				return new ApiError(MyErrorCode.IdNotFound, "评论的id不存在（解析错误）").Wrap();

			var comment = await Context.Comments.FirstOrDefaultAsync(p => p.Id == cid);
			if (comment == null)
				return new ApiError(MyErrorCode.IdNotFound, "评论的id不存在").Wrap();

			var attitude = await Context.Attitudes
				.FirstOrDefaultAsync(p => p.SenderId == AuthStore.UserId && p.CommentId == cid);

			// Modify or update attitude

			if (attitude == null)// No attitude before
			{
				if (agree != null)// if agree==null do nothing
				{
					attitude = new Attitude((Guid)AuthStore.UserId, (Guid)cid, agree.Value);

					if (agree == true)
						comment.SetAgreeCount(comment.AgreeCount + 1);
					else if (agree == false)
						comment.SetDisagreeCount(comment.DisagreeCount + 1);

					await Context.AddAsync(attitude);
				}
			}
			else // Already exist attitude
			{
				if (attitude.Agree == agree)
					return Ok();// Unchanged

				if (agree == true)
				{
					comment.SetAgreeCount(comment.AgreeCount + 1);
					comment.SetDisagreeCount(comment.DisagreeCount - 1);
					attitude.SetAgree(true);
				}
				else if (agree == false)
				{
					comment.SetAgreeCount(comment.AgreeCount - 1);
					comment.SetDisagreeCount(comment.DisagreeCount + 1);
					attitude.SetAgree(false);
				}
				else
				{
					// Remove attitude
					Context.Remove(attitude);
					if (attitude.Agree)
						comment.SetAgreeCount(comment.AgreeCount - 1);
					else
						comment.SetDisagreeCount(comment.DisagreeCount - 1);
				}
			}

			Console.WriteLine($"AAAAA {comment.AgreeCount} {comment.DisagreeCount}");

			await Context.GoAsync();

			return Ok();
		}


		[NotCommand]
		[HttpGet("by-work")]
		[Produces(typeof(QComment[]))]
		public async Task<IActionResult> GetByWork(string workId, OrderByType order, int page)
		{
			if (!Enum.IsDefined(typeof(OrderByType), order))
				return new ApiError(MyErrorCode.ModelInvalid, "Invalid 'order'").Wrap();

			page = Math.Max(page, 0);

			const int pageSize = 20;

			var wid = XUtils.ParseId(workId);
			if (wid == null)
				return new ApiError(MyErrorCode.ModelInvalid, "wordId parse error").Wrap();

			var query = from p in Context.Comments
						where p.WorkId == wid
						join q in Context.Users on p.SenderId equals q.Id
						join o in Context.Attitudes.Where(z => z.SenderId == AuthStore.UserId) on p.Id equals o.CommentId
						into xx
						from x in xx.DefaultIfEmpty()
						select new
						{
							comment = p,
							user = q,
							myatt = x
						};

			if (order == OrderByType.Hottest)
				query = query.OrderByDescending(p => p.comment.AgreeCount);
			else
				query = query.OrderByDescending(p => p.comment.CreatedAt);

			var ll = await query.Take(10).ToListAsync();


			query = query.Skip(page * pageSize).Take(pageSize);

			var data = await query.ToListAsync();

			var commentsQ = data.Select(p => QComment.NormalView(p.comment, QUser.NormalView(p.user), p.myatt?.Agree)).ToList();

			return Ok(commentsQ);
		}

		public enum OrderByType
		{
			Newest,
			Hottest,
		}


		[HttpPost("report")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(RequireLoginFilter))]
		public async Task<IActionResult> Report([FromBody]ReportModel model)
		{
			var commentId = XUtils.ParseId(model.CommentId);
			if (commentId == null)
				return new ApiError(MyErrorCode.IdNotFound, "CommentId parse error").Wrap();

			var report = await Context.Reports.FirstOrDefaultAsync(p => p.CommentId == commentId && p.SenderId == AuthStore.UserId);
			if (report != null)
				return new ApiError(MyErrorCode.UniqueConstraintConflict, "你已经举报过了").Wrap();

			var comment = await Context.Comments.FirstOrDefaultAsync(p => p.Id == commentId);
			if (comment == null)
				return new ApiError(MyErrorCode.IdNotFound, "评论id不存在").Wrap();

			// send report
			report = new Report((Guid)AuthStore.UserId, (Guid)commentId, model.Title, model.Text);

			await Context.AddAsync(report);
			await Context.GoAsync();

			return Ok();
		}
		public class ReportModel
		{
			[Required]
			public string CommentId { get; set; }

			[Required]
			[MinLength(1)]
			public string Title { get; set; }

			[Required]
			[MinLength(15)]
			public string Text { get; set; }
		}
	}
}