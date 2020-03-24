﻿using System;
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

namespace Leopard.API.Controllers
{
	[Route("users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		public Repository<User> UserRepository { get; }

		public UsersController(Repository<User> userRepository)
		{
			UserRepository = userRepository;
		}

		[HttpPost("register")]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task<IActionResult> Register([FromBody]RegisterModel model)
		{
			var user = new User(model.Username, model.Password, model.Nickname, model.Description);

			try
			{
				await UserRepository.PutAsync(user);
				return Ok();
			}
			catch (ConcurrencyConflictException)
			{
				return new ApiError(MyErrorCode.UsernameExist, "用户名已经存在").Wrap();
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