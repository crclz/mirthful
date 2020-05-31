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
		public string CoverUrl { get; private set; }

		public NpgsqlTsVector Tsv { get; private set; }


		private Work()
		{
		}

		public Work(Guid id, WorkType type, string name, string author, string description, string coverUrl) : base(id)
		{
			Guard.Argument(() => type).Defined();

			Type = type;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Author = author ?? throw new ArgumentNullException(nameof(author));
			Description = description ?? throw new ArgumentNullException(nameof(description));
			CoverUrl = coverUrl;
		}

		public Work(WorkType type, string name, string author, string description, string coverUrl)
			: this(Guid.NewGuid(), type, name, author, description, coverUrl)
		{
		}
	}

	public enum WorkType
	{
		/// <summary>
		/// 书籍
		/// </summary>
		Book = 0,

		/// <summary>
		/// 电影
		/// </summary>
		Film = 1,
	}
}
