using Leopard.Domain.Model.UserAggregate;
using MongoDB.Bson;

namespace Leopard.API.Filters
{
	public class MiddleStore
	{
		public ObjectId? UserId { get; set; }
		public User User { get; set; }
	}
}
