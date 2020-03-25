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
	public class LoginTest
	{
		[Fact]
		async Task LoginNormal()
		{
			var username = XUtils.GetRandomString(12);
			var password = "asda1dsas";

			// register
			var usersApi = new UsersApi(TestConfig.BaseUrl);
			var res = await usersApi.RegisterAsync(new RegisterModel(username, password, "x", ""));
			Assert.NotNull(res);
			Assert.NotNull(res.Id);

			// login
			var accessApi = new AccessApi(TestConfig.BaseUrl);
			var accessRes = await accessApi.LoginAsync(new LoginModel(username, password));
			Assert.NotNull(accessRes);
			Assert.NotNull(accessRes.AccessToken);
		}
	}
}
