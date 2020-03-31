using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.Infrastructure
{
	public static class DbSettExtension
	{
		public static async Task<T> FirstOrDefaultAsync<T>(this DbSet<T> set, Expression<Func<T, bool>> predicate) where T : class
		{
			return await set.Where(predicate).FirstOrDefaultAsync();
		}
	}
}
