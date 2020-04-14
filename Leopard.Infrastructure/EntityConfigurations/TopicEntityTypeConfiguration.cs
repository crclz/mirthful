using Leopard.Domain.TopicAG;
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
	class TopicEntityTypeConfiguration : IEntityTypeConfiguration<Topic>
	{
		public void Configure(EntityTypeBuilder<Topic> builder)
		{
			builder.Property(p => p.Tsv)
				.HasComputedColumnSql(@"
				setweight(to_tsvector('testzhcfg',coalesce(""Name"",'')), 'A')    ||
				setweight(to_tsvector('testzhcfg',coalesce(""Description"",'')), 'B') ");

			builder.HasIndex(p => p.Tsv).HasMethod("GIN");
		}
	}
}
