using Leopard.Domain.Interfaces;
using Leopard.Domain.Model;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Leopard.Infrastructure
{
	public class Repository<T> : IRepository<T> where T : RootEntity
	{
		private readonly IMediator mediator;
		private LeopardDatabase context { get; }

		public Repository(LeopardDatabase context, IMediator mediator)
		{
			this.context = context;
			this.mediator = mediator;
		}

		public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
			var collection = context.GetCollection<T>();
			var entity = await collection.Find(predicate).FirstOrDefaultAsync();

			return entity;
		}

		public async Task PutAsync(T entity)
		{
			await PutAsync(entity, null);
		}

		public async Task PutAsync(T entity, DeduplicationToken token)
		{
			IList<BaseNotification> notifications;

			var entityCollection = context.GetCollection<T>();
			var eventCollection = context.GetCollection<BaseNotification>();
			var deduplicationCollection = context.GetCollection<DeduplicationToken>();

			// Phase I

			using (var session = await context.Client.StartSessionAsync())
			{
				// Begin transaction with strong consistency
				session.StartTransaction(new TransactionOptions(ReadConcern.Majority, writeConcern: WriteConcern.WMajority));

				// Step1: Save DeduplicationToken (if provided)

				if (token != null)
				{
					await deduplicationCollection.InsertOneAsync(session, token);
				}

				// Step2: Save entity

				entity.IncreaseRowVersion();

				if (entity.DeletionMark == true)
				{
					await entityCollection.DeleteOneAsync(p => p.Id == entity.Id);
				}
				else if (entity.RowVersion == 0)// (newly created) -1 ---increase--> 0
				{
					// Newly created root entity, INSERT.
					await entityCollection.InsertOneAsync(session, entity);
				}
				else
				{
					// Use optimistic concurrency, UPDATE.
					var options = new UpdateOptions()
					{
						IsUpsert = false
					};
					// TODO: Test Optimistic Concurrency
					var result = await entityCollection.ReplaceOneAsync(session,
						p => p.Id == entity.Id && p.RowVersion == entity.RowVersion - 1, entity, options);

					if (result.ModifiedCount != 1)
					{
						// Concurrency conflict detected
						throw new ConcurrencyConflictException(
							$"Concurrency conflict detected when upserting root entity. ModifiedCount:{result.ModifiedCount}");
					}
				}

				// Step3: Save events
				notifications = entity.PopAllDomainEventsRecursively();
				foreach (var notification in notifications)
				{
					await eventCollection.InsertOneAsync(session, notification);
				}

				// Commit
				await session.CommitTransactionAsync();
			}


			// Phase II: Foreach event, publish and wait for consumer all ack and update status of event

			// TODO: consider doing the following things in a new thead pool thread and do not await.
			// await = easy to debug in development environment
			foreach (var notification in notifications)
			{
				try
				{
					// Publish and await consumers to ack

					await mediator.Publish(notification);// Successfully returns == All handlers Ack

					// UPDATE SET notification.AllAck = true 

					notification.AllAcknowledged();
					var options = new UpdateOptions() { IsUpsert = false };
					// TODO: directly Update (instead of replacing)
					// Precondition: strong consistency guarantee of the store
					await eventCollection.ReplaceOneAsync((p) => p.Id == notification.Id, notification, options);
				}
				catch (Exception e)
				{
					// The exception e doesn't matter: transient
					Console.WriteLine(e);
					// TODO: Log or push to poisonous queue
				}
			}
		}
	}
}