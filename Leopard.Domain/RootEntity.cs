using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Leopard.Domain.Model
{
	public abstract class RootEntity : Entity
	{
		protected RootEntity()
		{

		}

		public RootEntity(ObjectId id) : base(id)
		{

		}

		/// <summary>
		/// If an RootEntity is newly created by factory, its RowVersion is -1.
		/// It should be used to identify whether to insert or upsert (Optimistic).
		/// </summary>
		public int RowVersion { get; private set; } = -1;

		[BsonIgnore]
		public bool DeletionMark { get; private set; } = false;

		public void IncreaseRowVersion()
		{
			RowVersion++;
		}

		public void MarkForDeletion()
		{
			DeletionMark = true;
		}
	}
}
