using Leopard.Domain;
using Leopard.Domain.Model;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Leopard.API.Controllers.Query
{
	public static class QueryFilter
	{
		public static IMongoQueryable<T> Page<T>(this IMongoQueryable<T> query, long pt, string pid, bool earlier, int limit)
			where T : Entity
		{
			var recordId = Useful.ParseId(pid);
			pt = pt < 0 ? long.MaxValue : pt;

			if (earlier)
				query = query.Where(p => p.UpdatedAt < pt || (p.UpdatedAt == pt && p.Id < recordId));
			else
				query = query.Where(p => p.UpdatedAt > pt || (p.UpdatedAt == pt && p.Id > recordId));
			query = query.Take(limit);
			return query;
		}

		public static IMongoQueryable<T> Page<T>(this IMongoQueryable<T> query, long pt, string pid, bool earlier, int limit,
			Func<T, long> tFunc, Func<T, ObjectId> idFunc)
		{
			var recordId = Useful.ParseId(pid);
			pt = pt < 0 ? long.MaxValue : pt;

			if (earlier)
				query = query.Where(p => tFunc(p) < pt || (tFunc(p) == pt && idFunc(p) < recordId));
			else
				query = query.Where(p => tFunc(p) > pt || (tFunc(p) == pt && idFunc(p) > recordId));
			query = query.Take(limit);
			return query;
		}
	}
}
