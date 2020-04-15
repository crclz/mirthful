using Leopard.Domain.ReplyAG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Infrastructure.EntityConfigurations
{
	class ReplyEntityTypeConfiguration : IEntityTypeConfiguration<Reply>
	{
		public void Configure(EntityTypeBuilder<Reply> builder)
		{
			builder.Property(p => p.TextTsv)
				.HasComputedColumnSql(@" to_tsvector('testzhcfg',coalesce(""Text"",'')) ");

			builder.HasIndex(p => p.TextTsv).HasMethod("GIN");
		}
	}
}
