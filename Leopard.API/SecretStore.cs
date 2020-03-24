using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API
{
	public class SecretStore
	{
		private byte[] secretKey { get; }

		// Tested
		public byte[] SecretKey => secretKey.ToArray();

		public SecretStore(byte[] secretKey)
		{
			this.secretKey = secretKey;
		}
	}
}
