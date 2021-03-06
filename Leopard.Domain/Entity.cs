﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Leopard.Domain
{
	public abstract class Entity
	{
		public Guid Id { get; protected set; }
		public long CreatedAt { get; protected set; }
		public long UpdatedAt { get; protected set; }

		protected Entity()
		{
			// Safe design / convension first: Id and CreatedAt can be freely overwitten if value provided
			Id = Guid.NewGuid();
			CreatedAt = UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		protected Entity(Guid id)
		{
			Id = id;
			CreatedAt = UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		protected void UpdatedAtNow()
		{
			UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		private List<BaseNotification> _domainEvents { get; set; } = new List<BaseNotification>();

		internal void PushDomainEvent(BaseNotification notification)
		{
			_domainEvents.Add(notification);
		}

		private List<BaseNotification> PopDomainEvents()
		{
			// Make a copy
			var events = _domainEvents.ToList();
			_domainEvents.Clear();

			return events;
		}

		public List<BaseNotification> PopAllDomainEventsRecursively()
		{
			var list = this.PopDomainEvents();

			foreach (var v in this.GetSubEntities().Where(p => p != null).ToList())
			{
				var subEvents = v.PopAllDomainEventsRecursively();
				list.AddRange(subEvents);
			}
			return list;
		}

		internal IEnumerable<Entity> GetSubEntities()
		{
			// Important: p.GetType() (X) ,  p.PropertyType (√)
			var propertyInfos = GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));

			var list = new List<Entity>();

			foreach (var info in propertyInfos)
			{
				var val = info.GetValue(this);
				if (val != null)
				{
					list.Add((Entity)val);
				}
			}
			return list;
		}
	}
}
