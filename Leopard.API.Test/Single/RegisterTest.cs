using Leopard.Domain;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test.Single
{
	public class RegisterTest
	{
		[Fact]
		async Task NormalRegister()
		{
			var username = XUtils.GetRandomString(12);

			var usersApi = new UsersApi(TestConfig.BaseUrl);
			var res = await usersApi.RegisterAsync(new RegisterModel(username, "asda1dsas", "x", ""));
			Assert.NotNull(res);
			Assert.NotEqual(default, res.Id);
		}

		[Fact]
		async Task DuplicatedUsername()
		{
			var username = XUtils.GetRandomString(12);
			var usersApi = new UsersApi(TestConfig.BaseUrl);
			var res = await usersApi.RegisterAsync(new RegisterModel(username, "asda1dsas", "x", ""));
			Assert.NotNull(res);
			Assert.NotEqual(default, res.Id);

			// register with the same username again
			await Assert.ThrowsAnyAsync<Exception>(() => usersApi.RegisterAsync(new RegisterModel(username, "asda1dsas", "x", "")));
		}
	}
}
