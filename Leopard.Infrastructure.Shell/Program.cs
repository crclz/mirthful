using Leopard.Domain.UserAG;
using System;
using System.Linq;

namespace Leopard.Infrastructure.Shell
{
	class Program
	{
		static void Main(string[] args)
		{
			var name = typeof(User).Assembly.GetTypes().Where(p => p.Name == "Post");
			var context = new OneContext(new Microsoft.EntityFrameworkCore.DbContextOptions<OneContext>());
		}
	}
}
