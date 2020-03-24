using Dawn;

namespace Leopard.Domain.Model.Relationships
{
	public class Investigation : ValueObject
	{
		public string Content { get; private set; }

		protected Investigation()
		{
			// Required by EF
		}

		public Investigation(string content)
		{
			Guard.Argument(content, nameof(content)).NotNull().LengthInRange(1, 32);
			Content = content;
		}
	}
}
