using MongoDB.Bson;

namespace Leopard.Domain.Model
{
	public class Session : RootEntity
	{
		public Session()
		{

		}

		public Session(ObjectId id) : base(id)
		{

		}
	}
}
