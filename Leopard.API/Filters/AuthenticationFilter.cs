using Jose;
using Leopard.Domain;
using Leopard.Domain.Model.UserAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Filters
{
	public class AuthenticationFilter : IAsyncActionFilter
	{
		public AuthenticationFilter(Repository<User> userRepository, MiddleStore store, SecretStore secretStore)
		{
			UserRepository = userRepository;
			Store = store;
			SecretStore = secretStore;
		}

		public Repository<User> UserRepository { get; }
		public MiddleStore Store { get; }
		public SecretStore SecretStore { get; }

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			string token = null;

			// If not provided AccessToken in header, go for cookie

			if (context.HttpContext.Request.Headers.TryGetValue("AccessToken", out StringValues sv))
			{
				token = sv.ToArray()?.FirstOrDefault();
			}

			token = token ?? context.HttpContext.Request.Cookies["AccessToken"];

			if (token == null)
				goto failure;

			Claims claims;
			try { claims = JWT.Decode<Claims>(token, SecretStore.SecretKey); }
			catch (JoseException)
			{
				goto failure;
			}

			// Model validation (due to schema migration)
			if (claims.UserId == null || claims.Expire == null || claims.SecurityVersion == null)
				goto failure;

			// Test expire
			if (claims.Expire < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
				goto failure;

			var userId = XUtils.ParseId(claims.UserId);
			if (userId == null)
				goto failure;

			// Success

			Store.UserId = userId;

			// TODO: Not this filter's responsibility. Do this in another filter
			Store.User = await UserRepository.FirstOrDefaultAsync(p => p.Id == userId);

			// Important!
			await next();
			return;

		failure:
			// Let Store.UserId remain null and delete AccessToken in cookie
			context.HttpContext.Response.Cookies.Delete("AccessToken");
		}
	}
}
