using Jose;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.UserAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Leopard.API.Controllers
{
	[Route("api/access")]
	[ApiController]
	public class AccessController : ControllerBase
	{
		private readonly SecretStore secretStore;

		public Repository<User> UserRepository { get; }
		public AuthStore Store { get; }

		public AccessController(Repository<User> userRepository, AuthStore store, SecretStore secretStore)
		{
			UserRepository = userRepository;
			Store = store;
			this.secretStore = secretStore;
		}

		[NotCommand]
		[HttpGet("me")]
		[Produces(typeof(QUser))]
		// TODO: Not Good
		[ServiceFilter(typeof(AuthenticationFilter))]
		public IActionResult Me()
		{
			var user = Store.User;

			if (user == null)
				return Ok(null);
			else
				return Ok(QUser.NormalView(user));
		}


		[NotCommand]
		[HttpPost("Login")]
		[Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
		// Should not use string type.
		[Produces(typeof(LoginResponse))]
		public async Task<IActionResult> Login([FromBody]LoginModel data)
		{
			var username = data.Username;
			var password = data.Password;

			var user = await UserRepository.FirstOrDefaultAsync(p => p.Username == username);

			if (user == null)
				return new ApiError(MyErrorCode.UsernameNotFound, "用户名不存在").Wrap();

			if (!user.IsPasswordCorrect(password))
				return new ApiError(MyErrorCode.WrongPassword, "密码错误").Wrap();

			// Success

			var claims = new Claims
			{
				UserId = user.Id.ToString(),
				SecurityVersion = user.SecurityVersion,
				Expire = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds()
			};

			string token = JWT.Encode(claims, secretStore.SecretKey, JwsAlgorithm.HS256);

			Response.Cookies.Append("AccessToken", token);

			return Ok(new LoginResponse { AccessToken = token });
		}
		public class LoginModel
		{
			[Required]
			[MaxLength(100)]
			public string Username { get; set; }

			[Required]
			[MaxLength(100)]
			public string Password { get; set; }
		}
		public class LoginResponse
		{
			public string AccessToken { get; set; }
		}
	}

	public class QUser
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public string Nickname { get; set; }
		public string Avatar { get; set; }

		public static QUser NormalView(User user)
		{
			return new QUser
			{
				Id = user.Id.ToString(),
				Description = user.Description,
				Nickname = user.Nickname,
				Avatar = user.Avatar,
			};
		}

		public static QUser MyView(User user) => NormalView(user);
	}
}