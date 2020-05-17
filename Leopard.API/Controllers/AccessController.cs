using Jose;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.UserAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		public OneContext Context { get; }
		public AuthStore Store { get; }

		public AccessController(OneContext context, AuthStore store, SecretStore secretStore)
		{
			Context = context;
			Store = store;
			this.secretStore = secretStore;
		}

		/// <summary>
		/// 获取当前登录的用户信息
		/// </summary>
		/// <response code="200">返回当前登录的用户信息，如果未登录，则返回null。</response>
		/// 
		[NotCommand]
		[HttpGet("me")]
		[Produces(typeof(QUser))]
		// TODO: Not Good
		public IActionResult Me()
		{
			var user = Store.User;

			// TODO: OK(NULL)
			if (user == null)
				return new JsonResult(null);
			else
				return Ok(QUser.NormalView(user));
		}

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[NotCommand]
		[HttpPost("login")]
		[Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
		// Should not use string type.
		[Produces(typeof(LoginResponse))]
		public async Task<IActionResult> Login([FromBody]LoginModel data)
		{
			var username = data.Username;
			var password = data.Password;

			var user = await Context.Users.FirstOrDefaultAsync(p => p.Username == username);

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

			var options = new CookieOptions
			{
				MaxAge = TimeSpan.FromDays(120),
			};

			Response.Cookies.Append("AccessToken", token, options);

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
		public Guid Id { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 昵称
		/// </summary>
		public string Nickname { get; set; }
		/// <summary>
		/// 头像图片url
		/// </summary>
		public string Avatar { get; set; }

		public static QUser NormalView(User user)
		{
			return new QUser
			{
				Id = user.Id,
				Description = user.Description,
				Nickname = user.Nickname,
				Avatar = user.Avatar,
			};
		}

		public static QUser MyView(User user) => NormalView(user);
	}
}