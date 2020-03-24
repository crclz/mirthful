using Leopard.Domain.Model;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Leopard.Infrastructure
{
	public static class MongoCollectionExtension
	{
		public static async Task UpsertAsync<T>(this IMongoCollection<T> collection, T item) where T : Entity
		{
			var options = new FindOneAndReplaceOptions<T, object>()
			{
				IsUpsert = true
			};
			var j = await collection.FindOneAndReplaceAsync((p) => p.Id == item.Id, item, options);
		}
	}
}
