using Leopard.Domain;
using Leopard.Domain.TopicMemberAG;
using Leopard.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test.Smoke
{
	public class GroupManageTest
	{
		OneContext Context { get; }
		public GroupManageTest()
		{
			Context = new OneContext(null);
		}


		[Fact]
		async Task SendAdminRequestAndHandle()
		{
			var a = await ClientSesion.RandomInstance();
			var b = await ClientSesion.RandomInstance();

			// a create group
			var topicId = (await a.Api<TopicApi>().CreateTopicAsync(new CreateTopicModel(true, "ssss", "DDDDD", null))).Id;

			// b join group
			await b.Api<TopicApi>().JoinTopicAsync(new JoinTopicModel(topicId));

			// b send request
			var requestId = (await b.Api<GroupManageApi>().SendAdminRequestAsync(new SendAdminRequestModel(topicId, "hello!!!"))).Id;

			// check
			var request = await a.Api<GroupManageApi>().GetRequestByIdAsync(requestId);

			Assert.Equal(requestId, request.Id);
			Assert.Equal(b.UserId, request.SenderId);
			Assert.Equal(topicId, request.TopicId);

			var requests = await a.Api<GroupManageApi>().GetUnhandledRequestsAsync(topicId, 0, true);
			Assert.Single(requests);


			// accept
			await a.Api<GroupManageApi>().HandleRequestAsync(new HandleRequestModel(requestId, true));


			// check request in two ways

			var q = await a.Api<GroupManageApi>().GetRequestByIdAsyncWithHttpInfo(requestId);

			requests = await a.Api<GroupManageApi>().GetUnhandledRequestsAsync(topicId, 0, true);
			Assert.Empty(requests);

			request = await a.Api<GroupManageApi>().GetRequestByIdAsync(requestId);
			Assert.Equal(RequestStatus.Accepted, request.Status);


			// check membership
			var membership = await Context.TopicMembers
				.Where(p => p.TopicId == XUtils.ParseId(topicId) && p.UserId == XUtils.ParseId(b.UserId))
				.FirstOrDefaultAsync();

			Assert.Equal(MemberRole.Admin, membership.Role);


		}
	}
}
