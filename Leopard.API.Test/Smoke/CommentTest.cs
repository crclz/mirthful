using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test.Smoke
{
	public class CommentTest
	{
		[Fact]
		async Task LeaveCommentUnauthorized()
		{
			var api = new CommentsApi(TestConfig.BaseUrl);

			await Assert.ThrowsAnyAsync<Exception>(async () =>
			{
				var res = await api.CreateCommentAsyncWithHttpInfo(
					new CreateCommentModel("123", "title", "TEXTTEXTTEXTTEXTTEXTTEXTTEXTTEXT", 5));
			});
		}

		public static string Work01Id = "5e7ac5057108f920d4bd3c37";

		[Fact]
		async Task LeaveComment()
		{
			var a = await ClientSesion.RandomInstance();
			await a.Api<CommentsApi>().CreateCommentAsync(
				new CreateCommentModel(Work01Id, "good work", "TESTETSTETETTEXTTTTTTTTESTETSTETETTEXTTTTTTT", 5));
		}
	}
}
