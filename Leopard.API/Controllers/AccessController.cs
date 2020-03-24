using Jose;
using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain;
using Leopard.Domain.Model.UserAggregate;
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
		public MiddleStore Store { get; }

		public AccessController(Repository<User> userRepository, MiddleStore store, SecretStore secretStore)
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
		[HttpPost("login")]
		[Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
		// Should not use string type.
		[Produces(typeof(LoginResponse))]
		public async Task<IActionResult> Login([FromBody]LoginModel data)
		{
			var id = XUtils.ParseId(data.Id);
			var password = data.Password;

			var user = await UserRepository.FirstOrDefaultAsync(p => p.Id == id);
			if (user == null)
				return new ApiError(MyErrorCode.IdNotFound, "User not exist").Wrap();

			if (!user.IsPasswordCorrect(password))
				return new ApiError(MyErrorCode.WrongPassword, "Password is wrong").Wrap();

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
			public string Id { get; set; }

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