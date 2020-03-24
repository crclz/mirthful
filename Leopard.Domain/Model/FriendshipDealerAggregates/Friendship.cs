using Dawn;
using Leopard.Domain.Model.Relationships;
using MongoDB.Bson;

namespace Leopard.Domain.Model.FriendshipDealerAggregates
{
	public class Friendship : Relationship
	{
		public DualShip Dual { get; private set; }

		public ObjectId SessionId { get; private set; }

		public string Nickname { get; private set; }

		protected Friendship()
		{
			// Required by EF
		}

		public Friendship(DualShip dual, ObjectId sessionId, bool remindBirthday) : base(remindBirthday)
		{
			Guard.Argument(dual, nameof(dual)).NotNull();

			Dual = dual;
			SessionId = sessionId;
		}

		internal void SetNickname(string nickname)
		{
			Guard.Argument(() => nickname).MaxLength(16);
			Nickname = nickname;
			UpdatedAtNow();
		}
	}
}
