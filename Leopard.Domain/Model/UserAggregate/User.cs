using Crucialize.LangExt;
using Dawn;
using Leopard.Domain.Model.Relationships;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leopard.Domain.Model.UserAggregate
{
	public class User : RootEntity
	{
		public string Description { get; private set; }

		public string Nickname { get; private set; }

		public string Salt { get; private set; }

		public string PasswordHash { get; private set; }

		public string Avatar { get; private set; }

		[BsonElement]
		public DateTimeOffset? Birthday { get; private set; }

		public RelationshipRequirement RelationshipRequirement { get; private set; }

		public int SecurityVersion { get; private set; } = 0;

		private User()
		{
			// Required by EF
		}

		public User(string password, string nickname, string description)
		{
			SetPassword(password);

			SetDescription(description);
			SetNickname(nickname);

			RelationshipRequirement = RelationshipRequirement.AnyoneRequirement;
		}

		public bool IsPasswordCorrect(string password)
		{
			Guard.Argument(() => password).NotNull();
			var passAndSalt = password + Salt;
			var sha256 = passAndSalt.ToUTF8().GetSHA256();
			return sha256 == PasswordHash;
		}

		public void SetPassword(string password)
		{
			Guard.Argument(() => password).NotNull().LengthInRange(8, 32);
			Salt = Useful.GetRandomBytes(64).ToBase64();
			var passAndSalt = password + Salt;
			PasswordHash = passAndSalt.ToUTF8().GetSHA256();

			SecurityVersion++;
		}

		public void SetDescription(string description)
		{
			Guard.Argument(description, nameof(description)).MaxLength(32);
			Description = description;

			UpdatedAtNow();
		}

		public void SetNickname(string nickname)
		{
			Guard.Argument(nickname, nameof(nickname)).LengthInRange(1, 16);
			Nickname = nickname;

			UpdatedAtNow();
		}

		public void SetAvatar(string url)
		{
			Avatar = url;

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

		public void SetBirthday(DateTimeOffset birthday)
		{
			Birthday = birthday;
			UpdatedAtNow();
		}
	}
}
