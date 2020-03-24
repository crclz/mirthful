using Org.OpenAPITools.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.API.Test
{
	public class AccessTokenAttacher
	{
		public AccessTokenAttacher(string accessToken)
		{
			AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
		}

		public string AccessToken { get; }

		public void Attach(IApiAccessor api)
		{
			api.Configuration.DefaultHeaders.Add("AccessToken", AccessToken);
		}
	}
}
