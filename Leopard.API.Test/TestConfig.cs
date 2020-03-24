using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.API.Test
{
	static class TestConfig
	{
		public static string BaseUrl => Environment.GetEnvironmentVariable("ApiTestBaseUrl");
	}
}
