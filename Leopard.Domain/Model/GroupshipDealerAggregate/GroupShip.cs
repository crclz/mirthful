using Dawn;
using Leopard.Domain.Model.Relationships;
using MongoDB.Bson;

namespace Leopard.Domain.Model.GroupshipDealerAggregate
{
	public class GroupShip : Relationship
	{
		public ObjectId GroupId { get; private set; }
		public ObjectId UserId { get; private set; }
		public GroupRole Role { get; private set; }
		public string UserDisplayName { get; private set; }
		public string GroupDisplayName { get; private set; }

		private GroupShip()
		{
			// Required by EF
		}

		internal GroupShip(ObjectId groupId, ObjectId userId, GroupRole role, bool remindBirthday) : base(remindBirthday)
		{
			Guard.Argument(() => role).Defined();
			GroupId = groupId;
			UserId = userId;
			Role = role;
		}

		internal void SetUserDisplayName(string userDisplayName)
		{
			Guard.Argument(() => userDisplayName).NotNull().LengthInRange(1, 16);
			UserDisplayName = userDisplayName;
			UpdatedAtNow();
		}

		internal void SetGroupDisplayName(string groupDisplayName)
		{
			Guard.Argument(() => groupDisplayName).NotNull().LengthInRange(1, 16);
			GroupDisplayName = groupDisplayName;
			UpdatedAtNow();
		}

		internal void SetRole(GroupRole role)
		{
			Guard.Argument(() => role).Defined();
			Role = role;
			UpdatedAtNow();
		}
	}

	public enum GroupRole
	{
		Normal = 0,
		Admin = 1,
		Founder = 2,
	}
}
