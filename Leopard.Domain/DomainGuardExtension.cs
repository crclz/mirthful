using Dawn;
using Leopard.Domain.Model.Relationships;
using System.Collections.Generic;
using static Dawn.Guard;

namespace Leopard.Domain
{
	static class DomainGuardExtension
	{
		public static ref readonly ArgumentInfo<IEnumerable<InvestigationAndAnswer>> SatisfyDomainRule
			(in this ArgumentInfo<IEnumerable<InvestigationAndAnswer>> argument)
		{
			argument.NotNull().CountInRange(1, 3);
			return ref argument;
		}

		public static ref readonly ArgumentInfo<string> SatisfyValidationMessage
			(in this ArgumentInfo<string> argument)
		{
			argument.NotNull().MaxLength(32);
			return ref argument;
		}

		public static ref readonly ArgumentInfo<string> SatisfyChatText
			(in this ArgumentInfo<string> argument)
		{
			argument.MaxLength(640);
			return ref argument;
		}

		public static ref readonly ArgumentInfo<string> SatisfyGroupName
			(in this ArgumentInfo<string> argument)
		{
			argument.NotNull().LengthInRange(1, 16);
			return ref argument;
		}

		public static ref readonly ArgumentInfo<string> SatisfyGroupDescription
			(in this ArgumentInfo<string> argument)
		{
			argument.NotNull().MaxLength(32);
			return ref argument;
		}
	}
}
