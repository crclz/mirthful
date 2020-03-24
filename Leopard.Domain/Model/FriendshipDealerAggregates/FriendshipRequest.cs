using Dawn;
using Leopard.Domain.Model.Relationships;
using System.Collections.Generic;

namespace Leopard.Domain.Model.FriendshipDealerAggregates
{
	public class FriendshipRequest : RelationshipRequest
	{
		public DualShip Dual { get; private set; }

		protected FriendshipRequest()
		{
			// Required by EF
		}

		public FriendshipRequest(DualShip dual, string validationMessage) : base(validationMessage)
		{
			Guard.Argument(dual, nameof(dual)).NotNull();
			Dual = dual;


		}

		public FriendshipRequest(DualShip dual, IEnumerable<InvestigationAndAnswer> investigationAndAnswers)
			: base(investigationAndAnswers)
		{
			Guard.Argument(dual, nameof(dual)).NotNull();
			Dual = dual;
		}
	}
}
