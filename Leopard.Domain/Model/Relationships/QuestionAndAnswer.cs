using Dawn;

namespace Leopard.Domain.Model.Relationships
{
	public class QuestionAndAnswer : ValueObject
	{
		public string Question { get; private set; }

		public string Answer { get; private set; }

		protected QuestionAndAnswer()
		{
			// Required by EF
		}

		public QuestionAndAnswer(string question, string answer)
		{
			// TODO: WhiteSpace? 
			Guard.Argument(question, nameof(question)).NotNull().NotEmpty().NotWhiteSpace().LengthInRange(1, 32);
			Guard.Argument(answer, nameof(answer)).NotNull().NotEmpty().NotWhiteSpace().LengthInRange(1, 32);
			Question = question;
			Answer = answer;
		}

		public bool IsCorrect(string answer)
		{
			return answer == Answer;
		}
	}
}
