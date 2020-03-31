using Leopard.Domain.TopicAG;
using Leopard.Domain.TopicMemberAG;
using Leopard.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leopard.API.EventHandlers.TopicHandle
{
	public class JoinTopicEventHandler : INotificationHandler<JoinTopicEvent>
	{
		public JoinTopicEventHandler(OneContext context)
		{
			Context = context;
		}

		public OneContext Context { get; }

		public async Task Handle(JoinTopicEvent e, CancellationToken cancellationToken)
		{
			var topic = await Context.Topics.FirstOrDefaultAsync(p => p.Id == e.TopicId);
			topic.MemberIncr(1);
			await Context.GoAsync();
		}
	}
}
