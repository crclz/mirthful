using Jose;
using Leopard.Domain;
using Leopard.Domain.UserAG;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Filters
{
	public class RequireLoginFilter : IAsyncActionFilter
	{
		public RequireLoginFilter(OneContext context, AuthStore store, SecretStore secretStore)
		{
			Context = context;
			Store = store;
			SecretStore = secretStore;
		}

		public OneContext Context { get; }
		public AuthStore Store { get; }
		public SecretStore SecretStore { get; }

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (Store.UserId == null)
			{
				context.Result = new UnauthorizedResult();
				return;
			}
			else
			{
				await next();
			}
		}
	}
}
