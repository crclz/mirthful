using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Infrastructure.EntityConfigurations
{
	class DeduplicationTokenEntityTypeConfiguration : IEntityTypeConfiguration<DeduplicationToken>
	{
		public void Configure(EntityTypeBuilder<DeduplicationToken> builder)
		{
			builder.HasIndex(p => new { p.EventId, p.HandlerType, p.Additional }).IsUnique();
		}
	}
}
