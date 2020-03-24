using Leopard.Domain.Model;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Leopard.Domain
{
	public interface IRepository<T> where T : RootEntity
	{
		public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

		public Task PutAsync(T item);
	}
}
