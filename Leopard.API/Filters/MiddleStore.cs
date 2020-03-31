using Leopard.Domain.UserAG;
using System;

namespace Leopard.API.Filters
{
	public class AuthStore
	{
		public Guid? UserId { get; set; }
		public User User { get; set; }
	}
}
