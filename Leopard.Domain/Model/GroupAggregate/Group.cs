using Dawn;
using Leopard.Domain.Model.Relationships;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leopard.Domain.Model.GroupAggregate
{
	public class Group : RootEntity
	{
		private Group()
		{

		}

		public Group(string name, string description, ObjectId creatorId, ObjectId sessionId)
		{
			Guard.Argument(() => name).SatisfyGroupName();
			Guard.Argument(() => description).SatisfyGroupDescription();
			Name = name;
			Description = description;
			SessionId = sessionId;
			Creator = creatorId;
			RelationshipRequirement = RelationshipRequirement.AnyoneRequirement;

			PushDomainEvent(new GroupCreatedEvent(Id, sessionId, creatorId));
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public ObjectId SessionId { get; private set; }
		public string Avatar { get; private set; }
		public ObjectId Creator { get; private set; }
		public RelationshipRequirement RelationshipRequirement { get; private set; }

		public void SetName(string name)
		{
			Guard.Argument(() => name).SatisfyGroupName();
			Name = name;
			UpdatedAtNow();
		}

		public void SetDescription(string description)
		{
			Guard.Argument(() => description).SatisfyGroupDescription();
			Description = description;
			UpdatedAtNow();
		}

		public void SetAvatar(string avatarUrl)
		{
			Guard.Argument(() => avatarUrl).NotNull();
			Avatar = avatarUrl;
			UpdatedAtNow();
		}

		public void SetRequirement(RelationshipRequirement requirement)
		{
			RelationshipRequirement = requirement;
			UpdatedAtNow();
		}

		public bool IsAnswersCorrect(IEnumerable<string> answers)
		{
			Guard.Argument(() => answers).NotNull();

			Guard.Argument(() => answers).Count(RelationshipRequirement.QuestionAndAnswers.Count());

			if (RelationshipRequirement.Type != RelationshipRequirementType.CorrectAnswerRequired)
				throw new InvalidOperationException();

			var answerEnumerator = answers.GetEnumerator();
			var qasEnumerator = RelationshipRequirement.QuestionAndAnswers.GetEnumerator();

			while (answerEnumerator.MoveNext() && qasEnumerator.MoveNext())
			{
				var isCorrect = qasEnumerator.Current.IsCorrect(answerEnumerator.Current);
				if (!isCorrect)
					return false;
			}
			return true;
		}
	}
}
