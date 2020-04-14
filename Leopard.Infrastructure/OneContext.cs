using Leopard.Domain;
using Leopard.Domain.AdminRequestAG;
using Leopard.Domain.AttitudeAG;
using Leopard.Domain.CommentAG;
using Leopard.Domain.DiscussionAG;
using Leopard.Domain.PostAG;
using Leopard.Domain.ReplyAG;
using Leopard.Domain.ReportAG;
using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Domain.UserAG;
using Leopard.Domain.WorkAG;
using Leopard.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.Infrastructure
{
	public class OneContext : DbContext
	{
		private readonly IMediator _mediator;

		public DbSet<DeduplicationToken> DeduplicationTokens { get; private set; }
		//public DbSet<BaseNotification> Notifications { get; private set; }

		public DbSet<AdminRequest> AdminRequests { get; private set; }
		public DbSet<Attitude> Attitudes { get; private set; }
		public DbSet<Comment> Comments { get; private set; }
		public DbSet<Post> Posts { get; private set; }
		public DbSet<Reply> Replies { get; private set; }
		public DbSet<Discussion> Discussions { get; private set; }
		public DbSet<Report> Reports { get; private set; }
		public DbSet<Topic> Topics { get; private set; }
		public DbSet<TopicMember> TopicMembers { get; private set; }
		public DbSet<User> Users { get; private set; }
		public DbSet<Work> Works { get; private set; }

		public OneContext(DbContextOptions<OneContext> options) : base(options)
		{
		}

		public OneContext(DbContextOptions<OneContext> options, IMediator mediator) : base(options)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			var host = Environment.GetEnvironmentVariable("PostgresHost");
			if (host == null)
				throw new InvalidOperationException("Unable to find $PostgresHost in environmental variables.");

			var password = Environment.GetEnvironmentVariable("PostgresPassword");
			if (password == null)
				throw new InvalidOperationException("Unable to find $PostgresPassword in environmental variables.");

			var connBuilder = new NpgsqlConnectionStringBuilder
			{
				Host = host,
				Username = "postgres",
				Password = password,
				Database = "mirthful",
				CommandTimeout = 10,
				Timeout = 5,
			};

			var connestionString = connBuilder.ToString();

			builder.UseNpgsql(connestionString, b => b.MigrationsAssembly("Leopard.Infrastructure.Shell"));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new DeduplicationTokenEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new WorkEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new DiscussionEntityTypeConfiguration());
		}

		public async Task<bool> GoAsync(
			DeduplicationToken deduplicationToken = null, CancellationToken cancellationToken = default)
		{
			// STAGE 1


			// STAGE 1 > Step 1: Save entites
			// (Was done automatically by EF)

			// STAGE 1 > Step 2: Save Domain Events
			var notifications = ChangeTracker.Entries<RootEntity>()
				.SelectMany(p => p.Entity.PopAllDomainEventsRecursively())
				.ToList();
			/*
			await Notifications.AddRangeAsync(notifications);

			// STAGE 1 > Step 3: Save DeduplicationToken (if provided)
			if (deduplicationToken != null)
			{
				await DeduplicationTokens.AddAsync(deduplicationToken, cancellationToken);
			}
			*/

			// STAGE 1 > Commit
			// Throws if conflict (DeduplicationToken Unique Index conflict / Optimistic concrrency conflict)
			var result = await SaveChangesAsync(cancellationToken);



			// STAGE 2

			//  Notice: now, each notification in notifications is tracked by change tracker
			//  Rules:
			//  Not permitted: "Modify - Other Procedures - Save". Because OtherProcedure may call SaveChanges
			//  OK: "Modify - Save - Other Procedures"

			// Stage 2 > Step 1: dispatch events and update ack status
			foreach (var notification in notifications)
			{
				try
				{
					await _mediator.Publish(notification);// Successfully returns == All handlers Ack

					// UPDATE SET notification.AllAck = true, and commit the change of notification

					/*
					notification.AllAcknowledged();
					var rows = await SaveChangesAsync();
					if (rows != 1)
						throw new InvalidOperationException("Changes should only be: AllAck field of only one notification");
					*/
				}
				catch
				{
					// Production environment: may be transient exceptions (due to concurrency conflicts).
					// Replace rethrow with log / push to poisonous quque.

					// Development & test environment: rethrow can expose problems

					throw;
				}
			}

			return true;
		}
	}
}
