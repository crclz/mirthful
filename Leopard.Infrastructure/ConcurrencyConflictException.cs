using System;

namespace Leopard.Infrastructure
{
	public class ConcurrencyConflictException : Exception
	{
		public ConcurrencyConflictException(string message) : base(message)
		{
		}
	}
}
