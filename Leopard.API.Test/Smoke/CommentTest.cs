using Leopard.Domain.AttitudeAG;
using Leopard.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test.Smoke
{
	public class CommentTest
	{
		public static string Work01Id = "5e7ac5057108f920d4bd3c37";
		public LeopardDatabase Db { get; }

		public CommentTest()
		{
			Db = new LeopardDatabase();
		}

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


		[Fact]
		async Task LeaveCommentAndGetById()
		{
			var a = await ClientSesion.RandomInstance();

			var title = "good work";
			var text = "TESTETSTETETTEXTTTTTTTTESTETSTETETTEXTTTTTTT";
			var rating = 5;

			// Create comment
			var commentResponse = await a.Api<CommentsApi>().CreateCommentAsync(
				new CreateCommentModel(Work01Id, title, text, rating));

			Assert.NotNull(commentResponse);
			Assert.NotNull(commentResponse.Id);

			// Get by id
			var comment = await a.Api<CommentsApi>().GetByIdAsync(commentResponse.Id);
			Assert.Equal(title, comment.Title);
			Assert.Equal(text, comment.Text);
			Assert.Equal(a.UserId, comment.SenderId);
			Assert.Equal(rating, comment.Rating);
		}

		[Fact]
		async Task ExpressAttidude()
		{
			var a = await ClientSesion.RandomInstance();

			var title = "good work";
			var text = "TESTETSTETETTEXTTTTTTTTESTETSTETETTEXTTTTTTT";
			var rating = 5;

			// Create comment
			var commentResponse = await a.Api<CommentsApi>().CreateCommentAsync(
				new CreateCommentModel(Work01Id, title, text, rating));

			// Create attitude
			await a.Api<CommentsApi>().ExpressAttitudeAsync(commentResponse.Id, true);
			var atts = await Db.GetCollection<Attitude>().AsQueryable()
				.Where(p => p.CommentId == ObjectId.Parse(commentResponse.Id))
				.ToListAsync();

			var ok = atts.Any(p => p.SenderId.ToString() == a.UserId && p.CommentId.ToString() == commentResponse.Id);
			Assert.True(ok);

			// attitude count
			var comment = await a.Api<CommentsApi>().GetByIdAsync(commentResponse.Id);
			Assert.Equal(1, comment.AgreeCount);
			Assert.Equal(0, comment.DisagreeCount);
		}
	}
}
