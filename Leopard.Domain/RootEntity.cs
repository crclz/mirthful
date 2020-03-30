using System;
using System.ComponentModel.DataAnnotations;

namespace Leopard.Domain
{
	public abstract class RootEntity : Entity
	{
		protected RootEntity()
		{

		}

		public RootEntity(Guid id) : base(id)
		{

		}
	}
}
