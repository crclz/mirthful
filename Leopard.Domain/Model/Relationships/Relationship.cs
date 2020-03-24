using System;

namespace Leopard.Domain.Model.Relationships
{
	// Note: remember that an entity is mutable while a value object is immutable. 
	public abstract class Relationship : Entity
	{
		public bool IsValid { get; private set; }

		// TODO: SessionId -- Relationship 引用的流动方向？

		public bool RemindBirthday { get; private set; }

		protected Relationship()
		{
			// Required by EF
		}

		public Relationship(bool remindBirthday)
		{
			IsValid = true;
			RemindBirthday = remindBirthday;
		}

		internal void Invalidate()
		{
			if (!IsValid)
			{
				throw new InvalidOperationException("Relationship is already invalid.");
			}
			IsValid = false;
			UpdatedAtNow();
		}

		internal void SetRemindBirthday(bool remindBirthday)
		{
			RemindBirthday = remindBirthday;
			UpdatedAtNow();
		}
	}
}
