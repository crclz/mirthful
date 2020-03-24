using Dawn;
using System.IO;
using System.Threading.Tasks;

namespace Leopard.Infrastructure
{
	public class FileBucket : IBlobBucket
	{
		public FileBucket(DirectoryInfo saveTo, string requestPath)
		{
			SaveTo = saveTo;
			if (!requestPath.StartsWith("/"))
				requestPath = "/" + requestPath;
			if (!requestPath.EndsWith("/"))
				requestPath += "/";
			RequestPath = requestPath;
		}

		public string StoragePath { get; }
		public DirectoryInfo SaveTo { get; }
		public string RequestPath { get; }

		public async Task<string> PutBlobAsync(Stream stream, string name)
		{
			Guard.Argument(() => name).NotNull();

			var filename = Path.Join(SaveTo.FullName, name);
			using (var fs = File.OpenWrite(filename))
			{
				await stream.CopyToAsync(fs);
			}

			return RequestPath + name;
		}
	}
}
