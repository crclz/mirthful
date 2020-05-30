using CsvHelper;
using Leopard.Domain.CommentAG;
using Leopard.Domain.DiscussionAG;
using Leopard.Domain.PostAG;
using Leopard.Domain.ReplyAG;
using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Domain.UserAG;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Leopard.Seed
{
	class Program
	{
		static void Main(string[] args)
		{
			var randomText = new RandomText();
			var random = new Random();

			// copy blob files
			var blobStorePath = Environment.GetEnvironmentVariable("BLOB_STORE");

			foreach (var file in new DirectoryInfo("../../../blob-seeddata").GetFiles())
			{
				file.CopyTo(Path.Combine(blobStorePath, file.Name), true);
			}

			// Clear all data in database
			using var context = new OneContext(new DbContextOptions<OneContext>());

			context.Database.ExecuteSqlRaw("CREATE EXTENSION zhparser;");
			context.Database.ExecuteSqlRaw("CREATE TEXT SEARCH CONFIGURATION testzhcfg (PARSER = zhparser);");
			context.Database.ExecuteSqlRaw("ALTER TEXT SEARCH CONFIGURATION testzhcfg ADD MAPPING FOR n,v,a,i,e,l WITH simple;");

			context.Database.Migrate();

			Console.WriteLine("Database reseted");


			// Insert work data
			List<Work> works;

			using (var reader = new StreamReader("./workseed.csv"))
			using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				works = csv.GetRecords<WorkSeedRecord>().ToList()
					.Select(p => new Work((WorkType)p.Type, p.Name, p.Author, p.Description, p.CoverUrl))
					.ToList();
			}

			context.Works.AddRange(works);

			// Insert user data
			User adminUser = new User("admin", "access-code-001", "admin", "System admin");
			context.Users.Add(adminUser);

			var users = new List<User>();
			for (int i = 0; i < 100; i++)
			{
				users.Add(new User($"user{i}", "access-code-002", $"NormalUser{i}", ""));
			}
			context.Users.AddRange(users);

			// Insert comment data
			foreach (var work in works)
			{
				int commentCount = random.Next(1, 101);

				for (int i = 0; i < commentCount; i++)
				{
					var comment = new Comment(users[i].Id, work.Id, randomText.Generate(10), randomText.Generate(50, 80), random.Next(1, 6));
					comment.SetAgreeCount(random.Next(1, 256));
					comment.SetDisagreeCount(random.Next(0, 32));
					context.Add(comment);
				}
			}

			// Insert topic data
			List<Topic> topics = new List<Topic>();

			foreach (var work in works)
			{
				int topicCount = random.Next(1, 10);

				for (int i = 0; i < topicCount; i++)
				{
					var topicName = work.Name + " " + randomText.Generate(6, 14);
					var description = randomText.Generate(50, 200);
					var topic = new Topic(false, topicName, description, work.Id, adminUser.Id);
					topic.SetMemberCount(10);
					topics.Add(topic);
				}

				for (int i = 0; i < topicCount; i++)
				{
					var topicName = work.Name + " " + randomText.Generate(3, 10);
					var description = randomText.Generate(50, 200);
					var topic = new Topic(true/*diff*/, topicName, description, work.Id, adminUser.Id);
					topic.SetMemberCount(10);
					topics.Add(topic);

					// super admin
					var member = new TopicMember(topic.Id, adminUser.Id, MemberRole.Super, true);

					context.Add(member);
				}
			}

			context.Topics.AddRange(topics);


			// Insert discussion data
			// random count discussion entry for each topic
			foreach (var topic in topics.Where(p => p.IsGroup == false).ToList())
			{
				int discussCount = random.Next(1, 10);
				for (var i = 0; i < discussCount; i++)
				{
					var discussion = new Discussion(topic.Id, users[i].Id, randomText.Generate(40, 120), null);
					context.Discussions.Add(discussion);
				}
			}

			// Insert post data
			// random count post for each topic, random count replies for each post
			foreach (var topic in topics.Where(p => p.IsGroup == true).ToList())
			{
				int postCount = random.Next(1, 50);

				for (int i = 0; i < postCount; i++)
				{
					var post = new Post(adminUser.Id, topic.Id, randomText.Generate(50, 150), randomText.Generate(6, 16));
					context.Add(post);

					int replyCount = random.Next(1, 50);
					for (int j = 0; j < replyCount; j++)
					{
						var reply = new Reply(adminUser.Id, post.Id, randomText.Generate(10, 50));
						context.Add(reply);
					}
				}
			}


			// Create no content works
			for (int i = 1; i <= 40; i++)
			{
				var work = new Work(WorkType.Book, $"example book {i}", "NULL", "HELLO", "/blob/matrix.webp");
				context.Add(work);

				work = new Work(WorkType.Film, $"example film {i}", "NULL", "HELLO", "/blob/matrix.webp");
				context.Add(work);
			}

			Console.WriteLine("Start saving changes");
			context.SaveChanges();
			Console.WriteLine("Changes saved");
		}
	}
}
