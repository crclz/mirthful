using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.CommentAG;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

		public CommentsController(Repository<Comment> commentRepository, Repository<Work> workRepository, AuthStore authStore)
		{
			CommentRepository = commentRepository;
			WorkRepository = workRepository;
			AuthStore = authStore;
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

	}
}