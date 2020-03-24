using Leopard.Domain.Model;
using Leopard.Domain.Model.UserAggregate;
using Leopard.Infrastructure;
using System;
using System.Linq;

namespace Leopard.DBInit
{
	static class Initializer
	{
		public static void EnsureCollectionsCreated()
		{
			var db = new LeopardDatabase();
			var assembly = typeof(User).Assembly;
			var rootEntityTypes = assembly.GetTypes().Where(p => p.IsSubclassOf(typeof(RootEntity))).ToList();

			rootEntityTypes.Add(typeof(BaseNotification));
			rootEntityTypes.Add(typeof(DeduplicationToken));

			foreach (var r in rootEntityTypes)
			{
				var name = r.Name + "s";
				try
				{
					db.Database.CreateCollection(name);
					Console.WriteLine($"{r.FullName}: Created collection: {name}");
				}
				catch
				{
					Console.WriteLine($"{r.FullName}: Collection exists");
				}
			}
		}
	}
}
