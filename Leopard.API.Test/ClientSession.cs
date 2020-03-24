//using Org.OpenAPITools.Api;
//using Org.OpenAPITools.Client;
//using Org.OpenAPITools.Model;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Leopard.API.Test
//{
//	public class ClientSesion
//	{
//		public AccessTokenAttacher AccessTokenAttacher { get; }

//		public string UserId { get; }

//		public ClientSesion(AccessTokenAttacher accessTokenAttacher, string userId)
//		{
//			AccessTokenAttacher = accessTokenAttacher;
//			UserId = userId;
//		}

//		public static async Task<ClientSesion> InstanceByRegisterAndLogin(string username, string nickname, string password, string desciption)
//		{
//			var userApi = new UsersApi(TestConfig.BaseUrl);
//			var res = await userApi.RegisterAsyncWithHttpInfo(new RegisterModel(username, password, nickname, desciption));

//			Assert.Equal(HttpStatusCode.OK, res.StatusCode);

//			return await InstanceByLogin(res.Id, password);
//		}

//		public static async Task<ClientSesion> InstanceByLogin(string userId, string password)
//		{
//			var accessApi = new AccessApi(TestConfig.BaseUrl);
//			var loginRes = await accessApi.LoginAsync(new LoginModel(userId, password));
//			Assert.NotNull(loginRes);
//			Assert.NotNull(loginRes.AccessToken);

//			var attacher = new AccessTokenAttacher(loginRes.AccessToken);

//			return new ClientSesion(attacher, userId);
//		}

//		public T Api<T>() where T : IApiAccessor
//		{
//			var api = (T)Activator.CreateInstance(typeof(T), args: TestConfig.BaseUrl);
//			AccessTokenAttacher.Attach(api);
//			return api;
//		}
//	}
//}
