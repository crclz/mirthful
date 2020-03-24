using Leopard.Domain.Model.SessionMemberAggregate;
using MongoDB.Bson;

namespace Leopard.API
{
	public class SessionStore
	{
		public ObjectId SessionId { get; set; }
		public SessionMember Member { get; set; }
	}
}
