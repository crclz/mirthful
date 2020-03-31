using Leopard.Domain.UserAG;
using System;
using System.Linq;

namespace Leopard.Infrastructure.Shell
{
	class Program
	{
		static void Main(string[] args)
		{
			var context = new OneContext(new Microsoft.EntityFrameworkCore.DbContextOptions<OneContext>());
		}
	}
}
