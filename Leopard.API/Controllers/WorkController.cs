using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leopard.API.ResponseConvension;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leopard.API.Controllers
{
	[Route("api/work")]
	[ApiController]
	public class WorkController : ControllerBase
	{
		public OneContext Context { get; }

		public WorkController(OneContext context)
		{
			Context = context;
		}

		/// <summary>
		/// 根据id获取作品。
		/// </summary>
		/// <param name="id">作品id</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("by-id")]
		[Produces(typeof(QWork))]
		public async Task<IActionResult> GetWorkById(Guid id)
		{
			var work = await Context.Works.AsNoTracking().Where(p => p.Id == id).FirstOrDefaultAsync();

			var qwork = QWork.NormalView(work);

			return Ok(qwork);
		}

		/// <summary>
		/// 搜索作品
		/// </summary>
		/// <param name="type">作品类型</param>
		/// <param name="keyword">关键词</param>
		/// <param name="page">页码。从0开始。</param>
		/// <returns></returns>
		[NotCommand]
		[HttpGet("by-keyword")]
		[Produces(typeof(QWork[]))]
		public async Task<IActionResult> GetWorkByKeyword(WorkType type, string keyword, int page)
		{
			page = Math.Max(0, page);
			const int pageSize = 20;

			if (keyword == null)
				return new ApiError(MyErrorCode.ModelInvalid, "Keyword is null").Wrap();

			if (keyword.Length >= 20)
				keyword = keyword.Substring(0, 20);

			var query = from p in Context.Works.AsNoTracking().Where(p => p.Type == type)
						select new
						{
							work = p,
							qr = EF.Functions.WebSearchToTsQuery("testzhcfg", keyword)
						} into p
						select new
						{
							p.work,
							p.qr,
							rank = p.work.Tsv.RankCoverDensity(p.qr, NpgsqlTsRankingNormalization.DivideBy1PlusLogLength)
						} into p
						where p.work.Tsv.Matches(p.qr)
						orderby p.rank descending
						select p.work;

			var works = await query.Skip(page * pageSize).Take(pageSize).ToListAsync();

			var qworks = works.Select(p => QWork.NormalView(p)).ToList();

			return Ok(qworks);
		}

		public class QWork
		{
			public Guid Id { get; set; }

			/// <summary>
			/// 作品名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 作者
			/// </summary>
			public string Author { get; set; }

			/// <summary>
			/// 描述
			/// </summary>
			public string Description { get; set; }

			/// <summary>
			/// 作品类型
			/// </summary>
			public WorkType Type { get; set; }

			public int CommentCount { get; set; }

			public static QWork NormalView(Work p)
			{
				return p == null ? null : new QWork
				{
					Id = p.Id,
					Name = p.Name,
					Author = p.Author,
					Description = p.Description,
					Type = p.Type,
					CommentCount = 0
				};
			}

			public static QWork NormalView(Work p, int commentCount)
			{
				return p == null ? null : new QWork
				{
					Id = p.Id,
					Name = p.Name,
					Author = p.Author,
					Description = p.Description,
					Type = p.Type,
					CommentCount = commentCount
				};
			}
		}


		[NotCommand]
		[HttpGet("hotest-works")]
		[Produces(typeof(QWork[]))]
		public async Task<IActionResult> HotestWorks(WorkType type)
		{
			var query = from p in Context.Works.Where(p => p.Type == type)
						select new
						{
							work = p,
							commentCount = Context.Comments.Where(o => o.WorkId == p.Id).Count()
						}
						into k
						orderby k.commentCount descending
						select k;

			var works = await query.Take(20).ToListAsync();
			var qworks = works.Select(p => QWork.NormalView(p.work, p.commentCount));

			return Ok(qworks);
		}
	}
}