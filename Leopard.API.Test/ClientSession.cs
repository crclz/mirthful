using Leopard.Domain;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test
{
	public class ClientSesion
	{
		public AccessTokenAttacher AccessTokenAttacher { get; }

		public string UserId { get; }

		public ClientSesion(AccessTokenAttacher accessTokenAttacher, string userId)
		{
			AccessTokenAttacher = accessTokenAttacher;
			UserId = userId;
		}

		public static async Task<ClientSesion> InstanceByRegisterAndLogin(string username, string nickname, string password, string desciption)
		{
			var userApi = new UsersApi(TestConfig.BaseUrl);
			var res = await userApi.RegisterAsync(new RegisterModel(username, password, nickname, desciption));

			return await InstanceByLogin(username, password);
		}

		public static async Task<ClientSesion> InstanceByLogin(string username, string password)
		{
			var accessApi = new AccessApi(TestConfig.BaseUrl);
			var loginRes = await accessApi.LoginAsync(new LoginModel(username, password));
			Assert.NotNull(loginRes);
			Assert.NotNull(loginRes.AccessToken);

			var attacher = new AccessTokenAttacher(loginRes.AccessToken);

			return new ClientSesion(attacher, username);
		}

		public static async Task<ClientSesion> RandomInstance()
		{
			var username = XUtils.GetRandomString(12);
			return await InstanceByRegisterAndLogin(username, "test-user", "testtest123123", "This is a test user");
		}

		public T Api<T>() where T : IApiAccessor
		{
			var api = (T)Activator.CreateInstance(typeof(T), args: TestConfig.BaseUrl);
			AccessTokenAttacher.Attach(api);
			return api;
		}
	}
}
