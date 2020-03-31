
using System;

namespace Leopard.API.Controllers
{
	public class IdResponse
	{
		public Guid Id { get; set; }

		public IdResponse(Guid id)
		{
			Id = id;
		}
	}
}