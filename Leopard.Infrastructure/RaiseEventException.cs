using System;

namespace Leopard.Infrastructure
{
	/ <summary>
	/ Should be catched by domain layer then be logged silently
	/ </summary>
	public class RaiseEventException : Exception
	{
		public RaiseEventException(string message) : base(message)
		{
		}

		public RaiseEventException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
