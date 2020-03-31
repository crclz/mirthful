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

		[NotCommand]
		[HttpGet("by-id")]
		[Produces(typeof(QWork))]
		public async Task<IActionResult> GetWorkById(Guid id)
		{
			var work = await Context.Works.AsNoTracking().Where(p => p.Id == id).FirstOrDefaultAsync();

			var qwork = QWork.NormalView(work);

			return Ok(qwork);
		}

		[NotCommand]
		[HttpGet("by-keyword")]
		[Produces(typeof(QWork[]))]
		public async Task<IActionResult> GetWorkByKeyword(WorkType type, string keyword)
		{
			const int pageSize = 20;

			if (keyword == null)
				return new ApiError(MyErrorCode.ModelInvalid, "Keyword is null").Wrap();

			if (keyword.Length >= 20)
				keyword = keyword.Substring(0, 20);

			var query = from p in Context.Works.AsNoTracking()
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

			var works = await query.Take(pageSize).ToListAsync();

			var qworks = works.Select(p => QWork.NormalView(p)).ToList();

			return Ok(qworks);
		}

		public class QWork
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public string Author { get; set; }
			public string Description { get; set; }

			public static QWork NormalView(Work p)
			{
				return p == null ? null : new QWork
				{
					Id = p.Id,
					Name = p.Name,
					Author = p.Author,
					Description = p.Description
				};
			}
		}
	}
}