using Leopard.Domain.AttitudeAG;
using Leopard.Infrastructure;
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
		public OneContext Context { get; }

		public CommentTest()
		{
			Context = new OneContext(new Microsoft.EntityFrameworkCore.DbContextOptions<OneContext>());
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

			// Duplication
			await Assert.ThrowsAnyAsync<Exception>(async () =>
			{
				await a.Api<CommentsApi>().CreateCommentAsync(
					new CreateCommentModel(Work01Id, title, text, rating));
			});
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

			var commentId = commentResponse.Id;

			// Create attitude
			await a.Api<CommentsApi>().ExpressAttitudeAsync(commentResponse.Id, true);

			// Test attitude count
			var comment = await a.Api<CommentsApi>().GetByIdAsync(commentId);
			Assert.Equal(1, comment.AgreeCount);
			Assert.Equal(0, comment.DisagreeCount);

			// Duplication attitude
			await a.Api<CommentsApi>().ExpressAttitudeAsync(commentResponse.Id, true);
			comment = await a.Api<CommentsApi>().GetByIdAsync(commentId);
			Assert.Equal(1, comment.AgreeCount);
			Assert.Equal(0, comment.DisagreeCount);

			// Change attitude
			await a.Api<CommentsApi>().ExpressAttitudeAsync(commentResponse.Id, false);
			comment = await a.Api<CommentsApi>().GetByIdAsync(commentId);
			Assert.Equal(0, comment.AgreeCount);
			Assert.Equal(1, comment.DisagreeCount);
		}


		[Fact]
		async Task GetCommentsByWork()
		{
			var a = await ClientSesion.RandomInstance();

			var comments = await a.Api<CommentsApi>().GetByWorkAsync(Work01Id, OrderByType.Hottest, 0);
			Assert.NotEmpty(comments);

			comments = await a.Api<CommentsApi>().GetByWorkAsync(Work01Id, OrderByType.Newest, 0);
			Assert.NotEmpty(comments);
		}


		[Fact]
		async Task Report()
		{
			var a = await ClientSesion.RandomInstance();

			var title = "good work";
			var text = "TESTETSTETETTEXTTTTTTTTESTETSTETETTEXTTTTTTT";
			var rating = 5;

			// Create comment
			var commentResponse = await a.Api<CommentsApi>().CreateCommentAsync(
				new CreateCommentModel(Work01Id, title, text, rating));

			var commentId = commentResponse.Id;

			// Report
			string reportTitle = "shit", reportText = "helloooooooooooooooooooooooooooo";

			await a.Api<CommentsApi>().ReportAsync(new ReportModel(commentId, reportTitle, reportText));

			// Duplicate report
			await Assert.ThrowsAnyAsync<Exception>(
				() => a.Api<CommentsApi>().ReportAsync(new ReportModel(commentId, reportTitle, reportText)));
		}
	}
}
