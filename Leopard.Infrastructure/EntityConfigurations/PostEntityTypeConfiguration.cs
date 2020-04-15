using Leopard.Domain.PostAG;
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
	class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
	{
		public void Configure(EntityTypeBuilder<Post> builder)
		{
			builder.Property(p => p.Tsv)
				.HasComputedColumnSql(@"
				setweight(to_tsvector('testzhcfg',coalesce(""Title"",'')), 'A')  ||
				setweight(to_tsvector('testzhcfg',coalesce(""Text"",'')), 'B') ");

			builder.HasIndex(p => p.Tsv).HasMethod("GIN");
		}
	}
}
