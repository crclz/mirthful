using Leopard.Domain.Model.Relationships;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Leopard.Domain.Model.GroupshipDealerAggregate
{
	public class GroupshipRequest : RelationshipRequest
	{
		public ObjectId GroupId { get; private set; }
		public ObjectId UserId { get; private set; }

		protected GroupshipRequest()
		{
			// Required by mongo
		}

		public GroupshipRequest(ObjectId groupId, ObjectId userId, string validationMessage) : base(validationMessage)
		{
			GroupId = groupId;
			UserId = userId;


		}

		public GroupshipRequest(ObjectId groupId, ObjectId userId, IEnumerable<InvestigationAndAnswer> investigationAndAnswers)
			: base(investigationAndAnswers)
		{
			GroupId = groupId;
			UserId = userId;


		}
	}
}
