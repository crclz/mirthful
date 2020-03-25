using MongoDB.Bson;

namespace Leopard.API.Controllers
{
	public class IdResponse
	{
		public ObjectId Id { get; set; }

		public IdResponse(ObjectId id)
		{
			Id = id;
		}
	}
}