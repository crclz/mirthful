using Dawn;

namespace Leopard.Domain.Model.Relationships
{
	public class InvestigationAndAnswer : ValueObject
	{
		public Investigation Investigation { get; private set; }

		public string Answer { get; private set; }

		protected InvestigationAndAnswer()
		{
			// Required by EF
		}

		public InvestigationAndAnswer(Investigation investigation, string answer)
		{
			Guard.Argument(() => investigation).NotNull();
			Guard.Argument(() => answer).NotNull().LengthInRange(1, 32);

			Investigation = investigation;
			Answer = answer;
		}
	}
}
