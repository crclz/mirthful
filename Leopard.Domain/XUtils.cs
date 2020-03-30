using Crucialize.LangExt;
using System;
using System.Security.Cryptography;

namespace Leopard.Domain
{
	public static class XUtils
	{
		public static byte[] GetRandomBytes(int byteCount)
		{
			using (var r = new RNGCryptoServiceProvider())
			{
				var data = new byte[byteCount];
				r.GetBytes(data);

				return data;
			}
		}

		public static string GetRandomString(int length)
		{
			var data = GetRandomBytes(length * 2);
			var s = data.ToBase64();
			return s.Substring(0, length);
		}

		public static Guid? ParseId(string guidString)
		{
			if (!Guid.TryParse(guidString, out Guid id))
				return null;
			return id;
		}
	}
}
