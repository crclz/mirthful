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
using Microsoft.EntityFrameworkCore;

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

		/// <summary>
		/// 注册用户
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
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
			catch (DbUpdateException e)
			when (e.InnerException is PostgresException pe && pe.SqlState == PostgresErrorCodes.UniqueViolation)
			{
				return new ApiError(MyErrorCode.UsernameExist, "用户名已经存在").Wrap();
			}
		}
		public class RegisterModel
		{
			/// <summary>
			/// 用户名。仅仅用于登录
			/// </summary>
			[Required]
			[MinLength(3)]
			[MaxLength(12)]
			public string Username { get; set; }

			/// <summary>
			/// 密码
			/// </summary>
			[Required]
			[MinLength(8)]
			[MaxLength(32)]
			public string Password { get; set; }

			/// <summary>
			/// 昵称，用于展示。
			/// </summary>
			[Required]
			[MinLength(1)]
			[MaxLength(16)]
			public string Nickname { get; set; }

			/// <summary>
			/// 简介
			/// </summary>
			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Description { get; set; }
		}
	}
}