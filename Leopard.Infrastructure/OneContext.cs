using Leopard.Domain;
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
		public DbSet<BaseNotification> Notifications { get; private set; }

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

			builder.UseNpgsql(connestionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
		}

		public async Task<bool> SaveEntitiesAsync(
			DeduplicationToken deduplicationToken = null, CancellationToken cancellationToken = default)
		{
			//// Dispatch Domain Events collection. 
			//// Choices:
			//// A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
			//// side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
			//// B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
			//// You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
			//await _mediator.DispatchDomainEventsAsync(this);


			// STAGE 1


			// STAGE 1 > Step 1: Save entites
			// (Was done automatically by EF)

			// STAGE 1 > Step 2: Save Domain Events
			var notifications = ChangeTracker.Entries<RootEntity>()
				.SelectMany(p => p.Entity.PopAllDomainEventsRecursively())
				.ToList();

			await Notifications.AddRangeAsync(notifications);

			// STAGE 1 > Step 3: Save DeduplicationToken (if provided)
			if (deduplicationToken != null)
			{
				await DeduplicationTokens.AddAsync(deduplicationToken, cancellationToken);
			}

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

					notification.AllAcknowledged();
					var rows = await SaveChangesAsync();
					if (rows != 1)
						throw new InvalidOperationException("Changes should only be: AllAck field of only one notification");
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
