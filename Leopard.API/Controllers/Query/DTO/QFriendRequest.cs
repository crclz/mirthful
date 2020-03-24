using Leopard.Domain.Model.FriendshipDealerAggregates;
using Leopard.Domain.Model.Relationships;
using System.Collections.Generic;

namespace Leopard.API.Controllers.Query.DTO
{
	public class QFriendRequest
	{
		public string Id { get; set; }
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
		public IEnumerable<InvestigationAndAnswer> InvestigationAndAnswers { get; set; }
		public string RefuseMessage { get; set; }
		public RelationshipRequestType Type { get; set; }
		public RelationshipRequestStatus Status { get; set; }
		public string ValidationMessage { get; set; }
		public long CreatedAt { get; set; }
		public long UpdatedAt { get; set; }

		public QFriendRequest()
		{

		}

		public static QFriendRequest NormalView(FriendshipRequest r)
		{
			if (r == null)
				return null;

			return new QFriendRequest
			{
				Id = r.Id.ToString(),
				SenderId = r.Dual.SenderId.ToString(),
				ReceiverId = r.Dual.ReceiverId.ToString(),
				InvestigationAndAnswers = r.InvestigationAndAnswers,
				RefuseMessage = r.RefuseMessage,
				Type = r.Type,
				Status = r.Status,
				ValidationMessage = r.ValidationMessage,
				CreatedAt = r.CreatedAt,
				UpdatedAt = r.UpdatedAt
			};
		}
	}
}