using System;

namespace Leopard.DBInit
{
	class Program
	{
		static void Main(string[] args)
		{
			Initializer.EnsureCollectionsCreated();
		}
	}
}
