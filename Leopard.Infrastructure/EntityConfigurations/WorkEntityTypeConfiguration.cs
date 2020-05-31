using Leopard.Domain.UserAG;
using Leopard.Domain.WorkAG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Infrastructure.EntityConfigurations
{
	class WorkEntityTypeConfiguration : IEntityTypeConfiguration<Work>
	{
		public void Configure(EntityTypeBuilder<Work> builder)
		{
			builder.Property(p => p.Tsv)
				.HasComputedColumnSql(@"
				setweight(to_tsvector('testzhcfg',coalesce(""Name"",'')), 'A')    ||
				setweight(to_tsvector('testzhcfg',coalesce(""Author"",'')), 'A')  ||
				setweight(to_tsvector('testzhcfg',coalesce(""Description"",'')), 'B') ");

			builder.HasIndex(p => p.Tsv).HasMethod("GIN");

			var imageUrl = "https://i.loli.net/2020/05/31/prCLIHej56ZMOwo.jpg";

			builder.HasData(
				new Work(Guid.Parse("0a180b32-2510-406f-b0fd-1e19b6bb2697"), WorkType.Book, "testbook1", "some author", "description hello", imageUrl),
				new Work(Guid.Parse("71a710dc-aab6-4b9e-97eb-61ae1d703117"), WorkType.Film, "testfilm1", "some director", "description hello", imageUrl)
				);
		}
	}
}
