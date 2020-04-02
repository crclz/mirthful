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
	public class AuthenticationFilter : IAsyncActionFilter
	{
		public AuthenticationFilter(OneContext context, AuthStore store, SecretStore secretStore)
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

			var userId = XUtils.ParseId(claims.UserId);
			if (userId == null)
				goto failure;

			User user = null;

			// Test expire
			if (claims.Expire < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
			{
				// Test security version
				user = await Context.Users.FirstOrDefaultAsync(p => p.Id == userId);
				if (user.SecurityVersion == claims.SecurityVersion)
				{
					// Issue a new token
					var newClaims = new Claims
					{
						UserId = user.Id.ToString(),
						SecurityVersion = user.SecurityVersion,
						Expire = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds()
					};

					var newToken = JWT.Encode(claims, SecretStore.SecretKey, JwsAlgorithm.HS256);

					var options = new CookieOptions
					{
						MaxAge = TimeSpan.FromDays(120),
					};

					context.HttpContext.Response.Cookies.Append("AccessToken", token, options);
				}
				else
				{
					goto failure;
				}
			}

			// Success

			Store.UserId = userId;

			// TODO: Not this filter's responsibility. Do this in another filter
			Store.User = user ?? await Context.Users.FirstOrDefaultAsync(p => p.Id == userId);

			// Important!
			await next();
			return;

		failure:
			// Let Store.UserId remain null and delete AccessToken in cookie
			context.HttpContext.Response.Cookies.Delete("AccessToken");
			context.Result = new UnauthorizedResult();
		}
	}
}
