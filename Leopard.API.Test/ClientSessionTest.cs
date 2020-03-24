//using Org.OpenAPITools.Api;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Leopard.API.Test
//{
//	public class ClientSessionTest
//	{
//		[Theory]
//		[InlineData("chr", "p1ds1123asd1", "I am a teapot.")]
//		public async Task InstanceByRegisterAndLoginTest(string nickname, string password, string description)
//		{
//			var session = await ClientSesion.InstanceByRegisterAndLogin(nickname, password, description);
//			var user = await session.Api<AccessApi>().MeAsync();

//			Assert.NotNull(user);
//			Assert.Equal(nickname, user.Nickname);
//			Assert.Equal(description, user.Description);
//		}
//	}
//}
