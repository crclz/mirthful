using Leopard.Domain.DiscussionAG;
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
	class DiscussionEntityTypeConfiguration : IEntityTypeConfiguration<Discussion>
	{
		public void Configure(EntityTypeBuilder<Discussion> builder)
		{
			builder.Property(p => p.TextTsv)
				.HasComputedColumnSql(@"to_tsvector('testzhcfg',coalesce(""Text"",'')) ");

			builder.HasIndex(p => p.TextTsv).HasMethod("GIN");
		}
	}
}
