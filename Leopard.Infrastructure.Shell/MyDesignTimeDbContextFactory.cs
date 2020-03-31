using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Infrastructure
{
	class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<OneContext>
	{
		public MyDesignTimeDbContextFactory()
		{
		}

		public OneContext CreateDbContext(string[] args)
		{
			return new OneContext(new DbContextOptions<OneContext>());
		}
	}
}
