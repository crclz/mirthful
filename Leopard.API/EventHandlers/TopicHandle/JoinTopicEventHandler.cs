using Leopard.Domain.TopicAG;
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
	public class JoinTopicEventHandler : INotificationHandler<JoinTopicEvent>
	{
		public JoinTopicEventHandler(Repository<Topic> topicRepository)
		{
			TopicRepository = topicRepository;
		}

		public Repository<Topic> TopicRepository { get; }

		public async Task Handle(JoinTopicEvent e, CancellationToken cancellationToken)
		{
			var topic = await TopicRepository.FirstOrDefaultAsync(p => p.Id == e.TopicId);
			topic.MemberIncr(1);
			await TopicRepository.PutAsync(topic);
		}
	}
}
