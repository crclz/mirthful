using Leopard.Domain.UserAG;
using MongoDB.Bson;

namespace Leopard.API.Filters
{
	public class AuthStore
	{
		public ObjectId? UserId { get; set; }
		public User User { get; set; }
	}
}
