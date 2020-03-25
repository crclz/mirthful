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

		public CommentsController(Repository<Comment> commentRepository, Repository<Work> workRepository, AuthStore authStore,
			LeopardDatabase db, Repository<Attitude> attitudeRepository)
		{
			CommentRepository = commentRepository;
			WorkRepository = workRepository;
			AuthStore = authStore;
			Db = db;
			AttitudeRepository = attitudeRepository;
		}

		// TODO: 重复打分

		[HttpPost("create")]
		[Consumes(Application.Json)]
		[Produces(typeof(IdResponse))]

		[TypeFilter(typeof(AuthenticationFilter))]
		public async Task<IActionResult> CreateComment([FromBody]CreateCommentModel model)
		{
			var workId = XUtils.ParseId(model.WorkId);
			var work = await WorkRepository.FirstOrDefaultAsync(p => p.Id == workId);
			if (work == null)
				return new ApiError(MyErrorCode.IdNotFound, "不存在对应id的作品").Wrap();

			var comment = new Comment((ObjectId)AuthStore.UserId, (ObjectId)workId, model.Title, model.Text, model.Rating);

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

			// TODO:
			var agreeCount = await Db.GetCollection<Attitude>().AsQueryable()
				.Where(p => p.CommentId == commentId && p.Agree == true).CountAsync();
			var disagreeCount = await Db.GetCollection<Attitude>().AsQueryable()
				.Where(p => p.CommentId == commentId && p.Agree == false).CountAsync();

			return QComment.NormalView(comment, agreeCount, disagreeCount);
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

			public static QComment NormalView(Comment c, int agreeCount, int disagreeCount)
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

					AgreeCount = agreeCount,
					DisagreeCount = disagreeCount
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

			// TODO: Unique Index

			if (attitude == null)
				attitude = new Attitude((ObjectId)AuthStore.UserId, (ObjectId)cid, agree);
			else
				attitude.SetAgree(agree);

			await AttitudeRepository.PutAsync(attitude);
			return Ok();
		}
	}
}