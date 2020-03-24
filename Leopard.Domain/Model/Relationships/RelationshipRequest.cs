using Dawn;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leopard.Domain.Model.Relationships
{
	// TODO: What the fuck can be an entity or a value object?
	public abstract class RelationshipRequest : Entity
	{
		public RelationshipRequestType Type { get; private set; }

		public string ValidationMessage { get; private set; }

		[BsonElement]
		protected List<InvestigationAndAnswer> _investigationAndAnswer { get; private set; }

		public IEnumerable<InvestigationAndAnswer> InvestigationAndAnswers => _investigationAndAnswer?.AsReadOnly();

		public RelationshipRequestStatus Status { get; private set; }

		public string RefuseMessage { get; private set; }

		protected RelationshipRequest()
		{
			// Required by EF
		}

		public RelationshipRequest(IEnumerable<InvestigationAndAnswer> investigationAndAnswers)
		{
			Guard.Argument(() => investigationAndAnswers).SatisfyDomainRule();

			Type = RelationshipRequestType.InvestigationAndAnswer;
			_investigationAndAnswer = investigationAndAnswers.ToList();
			Status = RelationshipRequestStatus.Unhandled;
		}

		public RelationshipRequest(string validationMessage)
		{
			Guard.Argument(() => validationMessage).SatisfyValidationMessage();

			Type = RelationshipRequestType.ValidationMessage;
			ValidationMessage = validationMessage;
			Status = RelationshipRequestStatus.Unhandled;
		}

		internal void Expire()
		{
			if (Status != RelationshipRequestStatus.Unhandled)
			{
				throw new InvalidOperationException($"Request should be unhandled. Status: {Status}");
			}

			Status = RelationshipRequestStatus.Expired;

			UpdatedAtNow();
		}

		internal void Accept()
		{
			if (Status != RelationshipRequestStatus.Unhandled)
			{
				throw new InvalidOperationException($"Request should be unhandled. Status: {Status}");
			}
			Status = RelationshipRequestStatus.Accepted;

			UpdatedAtNow();
		}

		internal void Refuse(string message, bool block)
		{
			if (Status != RelationshipRequestStatus.Unhandled)
			{
				throw new InvalidOperationException($"Request should be unhandled. Status: {Status}");
			}

			RefuseMessage = message;

			if (block)
			{
				Status = RelationshipRequestStatus.RefusedAndBlocked;
			}
			else
			{
				Status = RelationshipRequestStatus.Refused;
			}

			UpdatedAtNow();
		}

		internal void Abandon()
		{
			if (Status == RelationshipRequestStatus.Unhandled)
			{
				Status = RelationshipRequestStatus.Abandoned;
			}
			else
			{
				throw new InvalidOperationException("Request should be unhandled");
			}
			UpdatedAtNow();
		}
	}

	public enum RelationshipRequestType
	{
		ValidationMessage = 0,
		InvestigationAndAnswer = 1,
	}

	public enum RelationshipRequestStatus
	{
		Unhandled = 0,
		Accepted = 1,
		Refused = 2,
		Expired = 3,
		RefusedAndBlocked = 4,
		/// <summary>
		/// If user want to send another request when there's already a request, 
		/// user should first abandon the previous request
		/// </summary>
		Abandoned = 5,
	}
}
