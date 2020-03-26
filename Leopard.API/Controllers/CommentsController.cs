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
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Leopard.API.Controllers
{
	[Route("api/comments")]
	[ApiController]
	public class CommentsController : ControllerBase
	{
		public Repository<Comment> CommentRepository { get; }
		public Repository<Work> WorkRepository { get; }
		public AuthStore AuthStore { get; }
		public LeopardDatabase Db { get; }
		public Repository<Attitude> AttitudeRepository { get; }
		public Repository<Report> ReportRepository { get; }

		public CommentsController(Repository<Comment> commentRepository, Repository<Work> workRepository, AuthStore authStore,
			LeopardDatabase db, Repository<Attitude> attitudeRepository, Repository<Report> reportRepository)
		{
			CommentRepository = commentRepository;
			WorkRepository = workRepository;
			AuthStore = authStore;
			Db = db;
			AttitudeRepository = attitudeRepository;
			ReportRepository = reportRepository;
		}


		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[ServiceFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> CreateComment([FromBody]CreateCommentModel model)
		{
			var workId = XUtils.ParseId(model.WorkId);
			var work = await WorkRepository.FirstOrDefaultAsync(p => p.Id == workId);
			if (work == null)
				return new ApiError(MyErrorCode.IdNotFound, "不存在对应id的作品").Wrap();

			var comment = await CommentRepository.FirstOrDefaultAsync(p => p.WorkId == workId && p.SenderId == AuthStore.UserId);
			if (comment != null)
				return new ApiError(MyErrorCode.UniqueConstraintConflict, "你已经评价过此作品").Wrap();

			comment = new Comment((ObjectId)AuthStore.UserId, (ObjectId)workId, model.Title, model.Text, model.Rating);

			await CommentRepository.PutAsync(comment);

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


		[HttpGet("get-by-id")]
		[Produces(typeof(QComment))]
		public async Task<QComment> GetById(string id)
		{
			var commentId = XUtils.ParseId(id);
			if (commentId == null)
				return null;

			var comment = await CommentRepository.FirstOrDefaultAsync(p => p.Id == commentId);

			if (commentId == null)
				return null;

			return QComment.NormalView(comment);
		}
		public class QComment
		{
			public ObjectId Id { get; set; }
			public long CreatedAt { get; set; }
			public long UpdatedAt { get; set; }
			public ObjectId SenderId { get; set; }
			public ObjectId WorkId { get; set; }
			public string Title { get; set; }
			public string Text { get; set; }
			public int Rating { get; set; }

			public int AgreeCount { get; set; }
			public int DisagreeCount { get; set; }

			public static QComment NormalView(Comment c)
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
					DisagreeCount = c.DisagreeCount
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

			var comment = await CommentRepository.FirstOrDefaultAsync(p => p.Id == cid);
			if (comment == null)
				return new ApiError(MyErrorCode.IdNotFound, "评论的id不存在").Wrap();

			var attitude = await AttitudeRepository
				.FirstOrDefaultAsync(p => p.SenderId == AuthStore.UserId && p.CommentId == cid);

			// Modify or update attitude

			if (attitude == null)
			{
				attitude = new Attitude((ObjectId)AuthStore.UserId, (ObjectId)cid, agree);

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

			await AttitudeRepository.PutAsync(attitude);
			await CommentRepository.PutAsync(comment);

			return Ok();
		}


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

			var query = Db.GetCollection<Comment>().AsQueryable().Where(p => p.WorkId == wid);

			if (order == OrderByType.Hottest)
				query = query.OrderByDescending(p => p.AgreeCount);
			else
				query = query.OrderByDescending(p => p.CreatedAt);

			query = query.Skip(page * pageSize).Take(pageSize);

			var comments = await query.ToListAsync();

			return Ok(comments);
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

			var report = await ReportRepository.FirstOrDefaultAsync(p => p.CommentId == commentId && p.SenderId == AuthStore.UserId);
			if (report != null)
				return new ApiError(MyErrorCode.UniqueConstraintConflict, "你已经举报过了").Wrap();

			var comment = await CommentRepository.FirstOrDefaultAsync(p => p.Id == commentId);
			if (comment == null)
				return new ApiError(MyErrorCode.IdNotFound, "评论id不存在").Wrap();

			// send report
			report = new Report((ObjectId)AuthStore.UserId, (ObjectId)commentId, model.Title, model.Text);
			await ReportRepository.PutAsync(report);

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