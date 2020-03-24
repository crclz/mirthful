using Dawn;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Leopard.Domain.Model.Relationships
{
	public class RelationshipRequirement : ValueObject
	{
		public RelationshipRequirementType Type { get; private set; }

		[BsonElement]
		private List<QuestionAndAnswer> _questionAndAnswers { get; set; }

		public IEnumerable<QuestionAndAnswer> QuestionAndAnswers => _questionAndAnswers?.AsReadOnly();

		[BsonElement]
		private List<Investigation> _investigations { get; set; }

		public IEnumerable<Investigation> Investigations => _investigations?.AsReadOnly();

		protected RelationshipRequirement()
		{
			// Required by EF
		}

		public static RelationshipRequirement AnyoneRequirement => new RelationshipRequirement
		{
			Type = RelationshipRequirementType.Anyone
		};

		public static RelationshipRequirement ValidationRequiredRequirement => new RelationshipRequirement
		{
			Type = RelationshipRequirementType.ValidationMessageRequired
		};

		public RelationshipRequirement(IEnumerable<QuestionAndAnswer> questionAndAnswers)
		{
			// TODO: Should pass scalar type as parameter?
			Guard.Argument(questionAndAnswers, nameof(questionAndAnswers)).NotNull().CountInRange(1, 3);
			Type = RelationshipRequirementType.CorrectAnswerRequired;
			_questionAndAnswers = questionAndAnswers.ToList();
		}

		public RelationshipRequirement(IEnumerable<Investigation> investigations)
		{
			Guard.Argument(investigations, nameof(investigations)).NotNull().CountInRange(1, 3);
			Type = RelationshipRequirementType.InvestigationRequired;
			_investigations = investigations.ToList();
		}
	}

	public enum RelationshipRequirementType
	{
		Anyone = 0,
		ValidationMessageRequired = 1,
		CorrectAnswerRequired = 2,
		InvestigationRequired = 3,
	}
}
