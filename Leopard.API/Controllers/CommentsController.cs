﻿using System;
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


		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(AuthenticationFilter))]
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

			await Context.GoAsync();

			return Ok(new IdResponse(comment.Id));
		}
		public class CreateCommentModel
		{
			[Required]
			public string WorkId { get; set; }

			[Required]
			[MinLength(1)]
			public string Title { get; set; }

			[Required]
			[MinLength(25)]
			public string Text { get; set; }

			[Required]
			[Range(1, 5)]
			public int Rating { get; set; }
		}


		[NotCommand]
		[HttpGet("get-by-id")]
		[Produces(typeof(QComment))]
		public async Task<QComment> GetById(string id)
		{
			var commentId = XUtils.ParseId(id);
			if (commentId == null)
				return null;

			var query = from p in Context.Comments.Where(r => r.Id == commentId)
						join q in Context.Users
						on p.SenderId equals q.Id
						select new { comment = p, user = q };

			var data = await query.FirstOrDefaultAsync();

			var user = QUser.NormalView(data.user);
			var commentQ = QComment.NormalView(data.comment, user);

			return commentQ;
		}
		public class QComment
		{
			public Guid Id { get; set; }
			public long CreatedAt { get; set; }
			public long UpdatedAt { get; set; }
			public Guid SenderId { get; set; }
			public Guid WorkId { get; set; }
			public string Title { get; set; }
			public string Text { get; set; }
			public int Rating { get; set; }
			public int AgreeCount { get; set; }
			public int DisagreeCount { get; set; }

			public QUser User { get; set; }

			public static QComment NormalView(Comment c, QUser user)
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
					User = user
				};
			}
		}


		[HttpPost("express-attitude")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> ExpressAttitude(string commentId, bool agree)
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

			if (attitude == null)
			{
				attitude = new Attitude((Guid)AuthStore.UserId, (Guid)cid, agree);

				if (agree)
					comment.SetAgreeCount(comment.AgreeCount + 1);
				else
					comment.SetDisagreeCount(comment.DisagreeCount + 1);
			}
			else // Already exist attitude
			{
				if (attitude.Agree == agree)
					return Ok();// Unchanged

				attitude.SetAgree(agree);

				if (agree)
				{
					comment.SetAgreeCount(comment.AgreeCount + 1);
					comment.SetDisagreeCount(comment.DisagreeCount - 1);
				}
				else
				{
					comment.SetAgreeCount(comment.AgreeCount - 1);
					comment.SetDisagreeCount(comment.DisagreeCount + 1);
				}
			}

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
						join q in Context.Users
						on p.SenderId equals q.Id
						select new { comment = p, user = q };

			if (order == OrderByType.Hottest)
				query = query.OrderByDescending(p => p.comment.AgreeCount);
			else
				query = query.OrderByDescending(p => p.comment.CreatedAt);

			query = query.Skip(page * pageSize).Take(pageSize);

			var data = await query.ToListAsync();
			var commentsQ = data.Select(p => QComment.NormalView(p.comment, QUser.NormalView(p.user))).ToList();

			return Ok(commentsQ);
		}

		public enum OrderByType
		{
			Newest,
			Hottest,
		}


		[HttpPost("report")]
		[Consumes(Application.Json)]

		[ServiceFilter(typeof(AuthenticationFilter))]
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