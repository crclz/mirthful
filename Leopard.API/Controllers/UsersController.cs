using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Leopard.Domain.UserAG;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Leopard.API.ResponseConvension;
using Npgsql;

namespace Leopard.API.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		public UsersController(OneContext context)
		{
			Context = context;
		}

		public OneContext Context { get; }

		[HttpPost("register")]
		[Consumes(MediaTypeNames.Application.Json)]
		[Produces(typeof(IdResponse))]
		public async Task<IActionResult> Register([FromBody]RegisterModel model)
		{
			var user = new User(model.Username, model.Password, model.Nickname, model.Description);

			try
			{
				await Context.AddAsync(user);
				await Context.GoAsync();
				return Ok(new IdResponse(user.Id));
			}
			catch (PostgresException e)
			{
				if (e.SqlState == PostgresErrorCodes.UniqueViolation)
					return new ApiError(MyErrorCode.UsernameExist, "用户名已经存在").Wrap();
				else
					throw;
			}
		}
		public class RegisterModel
		{
			[Required]
			[MinLength(3)]
			[MaxLength(12)]
			public string Username { get; set; }

			[Required]
			[MinLength(6)]
			[MaxLength(32)]
			public string Password { get; set; }

			[Required]
			[MinLength(1)]
			[MaxLength(16)]
			public string Nickname { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Description { get; set; }
		}
	}
}