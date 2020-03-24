using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API
{
	public class Claims
	{
		public string UserId { get; set; }
		public int? SecurityVersion { get; set; }
		public long? Expire { get; set; }
	}
}
