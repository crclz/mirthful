using Dawn;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.WorkAG
{
	public class Work : RootEntity
	{
		public WorkType Type { get; private set; }
		public string Name { get; private set; }
		public string Author { get; private set; }
		public string Description { get; private set; }


		public NpgsqlTsVector Tsv { get; private set; }

		private Work()
		{
		}

		public Work(WorkType type, string name, string author, string description)
		{
			Guard.Argument(() => type).Defined();

			Type = type;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Author = author ?? throw new ArgumentNullException(nameof(author));
			Description = description ?? throw new ArgumentNullException(nameof(description));
		}
	}

	public enum WorkType
	{
		Book = 0,
		Film = 1,
	}
}
