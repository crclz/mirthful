using MongoDB.Bson;
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

		public static (ObjectId, ObjectId) SmallerBigger(ObjectId a, ObjectId b)
		{
			if (a > b)
				return (b, a);
			else
				return (a, b);
		}

		public static ObjectId? ParseId(string objectIdString)
		{
			if (!ObjectId.TryParse(objectIdString, out ObjectId lastRecordId))
				return null;
			return lastRecordId;
		}
	}
}
