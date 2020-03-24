using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.ResponseConvension
{
	public class ApiError
	{
		public ApiError(string code, string message)
		{
			Code = code;
			Message = message;
		}

		public string Code { get; set; }

		public string Message { get; set; }

		public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

		public BadRequestObjectResult Wrap()
		{
			return new BadRequestObjectResult(this);
		}
	}
}
