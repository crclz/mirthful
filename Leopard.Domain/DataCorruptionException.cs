using System;

namespace Leopard.Domain
{
	public class DataCorruptionException : Exception
	{
		public DataCorruptionException()
		{
		}

		public DataCorruptionException(string message) : base(message)
		{
		}

		public DataCorruptionException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
