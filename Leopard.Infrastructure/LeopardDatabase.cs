using MongoDB.Driver;
using System;

namespace Leopard.Infrastructure
{
	public class LeopardDatabase
	{
		public IMongoClient Client { get; }
		public IMongoDatabase Database { get; }

		public LeopardDatabase()
		{
			var host = Environment.GetEnvironmentVariable("MONGO_HOST");
			if (host == null)
				throw new InvalidOperationException("MONGO_HOST null");
			var password = Environment.GetEnvironmentVariable("MONGO_PASS");
			if (password == null)
				throw new InvalidOperationException("MONGO_PASS null");

			var u = new MongoUrlBuilder();
			u.Username = "root";
			u.Server = new MongoServerAddress(host);
			u.Password = password;

			var mongoUrl = u.ToMongoUrl();
			Client = new MongoClient(mongoUrl);

			// Strong Consistency
			Client = Client.WithReadConcern(ReadConcern.Majority).WithWriteConcern(WriteConcern.WMajority);

			Database = Client.GetDatabase("mirthful-dev");
		}

		private static string GetCollectionName<T>()
		{
			var type = typeof(T);
			var typeName = type.Name;
			var collectionName = typeName + "s";

			return collectionName;
		}

		public IMongoCollection<T> GetCollection<T>()
		{
			var collectioName = GetCollectionName<T>();
			var collection = Database.GetCollection<T>(collectioName);

			return collection;
		}
	}
}
