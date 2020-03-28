using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test.Smoke
{
	public class TopicTest
	{
		public LeopardDatabase Db { get; }

		public TopicTest()
		{
			Db = new LeopardDatabase();
		}

		[Fact]
		async Task CreateTopic()
		{
			await CreateByModel(new CreateTopicModel(false, "some-topic", "haha", null));

			await CreateByModel(new CreateTopicModel(false, "some-topic2", "aaaahaha", null));

			await CreateByModel(new CreateTopicModel(false, "some-topic", "haha", CommentTest.Work01Id));
		}


		async Task CreateByModel(CreateTopicModel model)
		{
			var a = await ClientSesion.RandomInstance();

			var idRes = await a.Api<TopicApi>().CreateTopicAsync(model);
			var topicId = idRes.Id;

			// Check topic

			var topic = await Db.GetCollection<Topic>().AsQueryable()
				.Where(p => p.Id == ObjectId.Parse(topicId)).FirstOrDefaultAsync();

			Assert.NotNull(topic);
			Assert.Equal(model.IsGroup, topic.IsGroup);
			Assert.Equal(model.Name, topic.Name);
			Assert.Equal(model.IsGroup, topic.IsGroup);
			Assert.Equal(1, topic.MemberCount);

			if (model.RelatedWork != null)
				Assert.Equal(model.RelatedWork, topic.RelatedWork.ToString());


			// Check member

			var members = await Db.GetCollection<TopicMember>().AsQueryable()
				.Where(p => p.TopicId == ObjectId.Parse(topicId)).ToListAsync();

			Assert.Single(members);

			var member = members[0];

			if (model.IsGroup == true)
				Assert.Equal(MemberRole.Super, member.Role);
			else
				Assert.Equal(MemberRole.Normal, member.Role);
		}


		[Fact]
		async Task JoinTopicAndSendPostAndGetPosts()
		{
			var a = await ClientSesion.RandomInstance();
			var b = await ClientSesion.RandomInstance();

			// b create topic
			var model = new CreateTopicModel(false, "some-topic", "haha", null);
			var idRes = await b.Api<TopicApi>().CreateTopicAsync(model);
			var topicId = idRes.Id;

			// a send post without join topic
			await Assert.ThrowsAnyAsync<Exception>(() => a.Api<TopicApi>().SendPostAsync(topicId, "hello", "Hello everyone!"));

			// a join topic
			await a.Api<TopicApi>().JoinTopicAsync(new JoinTopicModel(topicId));

			// Check member
			var members = await Db.GetCollection<TopicMember>().AsQueryable()
				.Where(p => p.TopicId == ObjectId.Parse(topicId)).ToListAsync();

			Assert.Equal(2, members.Count);

			var member = members.Where(p => p.UserId.ToString() == a.UserId).FirstOrDefault();
			Assert.NotNull(member);
			Assert.Equal(MemberRole.Normal, member.Role);

			// a send post 
			idRes = await a.Api<TopicApi>().SendPostAsync(topicId, "hello1", "Hello everyone!");
			var post = await a.Api<TopicApi>().GetByIdAsync(idRes.Id);
			Assert.Equal("hello1", post.Title);

			// b send post
			idRes = await b.Api<TopicApi>().SendPostAsync(topicId, "hello2", "Hello everyone!");
			post = await b.Api<TopicApi>().GetByIdAsync(idRes.Id);
			Assert.Equal("hello2", post.Title);

			// get posts
			var posts = await a.Api<TopicApi>().GetPostsAsync(topicId, 0, true);

			Assert.Equal(2, posts.Count);


			// Send reply
			var postId = posts[0].Id;
			idRes = await a.Api<TopicApi>().SendReplyAsync(new SendReplyModel(postId, "123123asd1dasd25字数asd12e12312asd"));
			var replyId = idRes.Id;

			// Check reply
			var replies = await a.Api<TopicApi>().GetRepliesAsync(postId, 0);
			Assert.Single(replies);
			Assert.Equal(replyId, replies[0].Id);
		}


		[Fact]
		async Task DoAdmin()
		{
			var a = await ClientSesion.RandomInstance();

			// Create topic (IsGroup=true)
			var model = new CreateTopicModel(true, "pin-test-topic", "haha", null);
			var idRes = await a.Api<TopicApi>().CreateTopicAsync(model);
			var topicId = idRes.Id;

			// Send Posts

			await a.Api<TopicApi>().SendPostAsync(topicId, "123", "123123asd1dasd25");

			await a.Api<TopicApi>().SendPostAsync(topicId, "123", "123123asd1dasd25");

			idRes = await a.Api<TopicApi>().SendPostAsync(topicId, "123", "123123asd1dasd25");
			var pinnedId = idRes.Id;

			await a.Api<TopicApi>().SendPostAsync(topicId, "123", "123123asd1dasd25");

			var deleteId = (await a.Api<TopicApi>().SendPostAsync(topicId, "123", "123123asd1dasd25")).Id;

			var essenseId = (await a.Api<TopicApi>().SendPostAsync(topicId, "123", "123123asd1dasd25")).Id;

			// do admin
			await a.Api<TopicApi>().DoAdminAsync(new DoAdminModel(pinnedId, true, false, false));
			await a.Api<TopicApi>().DoAdminAsync(new DoAdminModel(deleteId, false, false, true));
			await a.Api<TopicApi>().DoAdminAsync(new DoAdminModel(essenseId, false, true, false));

			// check
			var posts = await a.Api<TopicApi>().GetPostsAsync(topicId, 0, true);
			Assert.Equal(5, posts.Count);
			Assert.Equal(pinnedId, posts[0].Id);
			Assert.DoesNotContain(posts, p => p.Id == deleteId);
			Assert.Contains(posts, p => p.Id == essenseId && p.IsEssense);
		}
	}
}
