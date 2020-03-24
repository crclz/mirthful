using System.IO;
using System.Threading.Tasks;

namespace Leopard.Infrastructure
{
	public interface IBlobBucket
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns>The URL of the blob</returns>
		public Task<string> PutBlobAsync(Stream stream, string name);
	}
}
