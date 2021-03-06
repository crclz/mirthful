﻿using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.TopicMemberHandle
{
	public class TopicCreatedEventHandler : INotificationHandler<TopicCreatedEvent>
	{
		public TopicCreatedEventHandler(OneContext context)
		{
			Context = context;
		}

		public OneContext Context { get; }

		public async Task Handle(TopicCreatedEvent e, CancellationToken cancellationToken)
		{
			TopicMember member;

			if (e.IsGroup)
			{
				// role: super
				member = new TopicMember(e.TopicId, e.CreatorId, MemberRole.Super, true);
			}
			else
			{
				// role: normal
				member = new TopicMember(e.TopicId, e.CreatorId, MemberRole.Normal, true);
			}

			await Context.TopicMembers.AddAsync(member);

			await Context.GoAsync();
		}
	}
}
