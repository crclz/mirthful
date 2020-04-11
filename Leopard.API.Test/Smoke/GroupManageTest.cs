using Leopard.Domain;
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
			Context = new OneContext(new DbContextOptions<OneContext>());
		}


		[Fact]
		async Task SendAdminRequestAndHandle()
		{
			var a = await ClientSesion.RandomInstance();
			var b = await ClientSesion.RandomInstance();

			// a create group
			var topicId = (await a.Api<TopicApi>().CreateTopicAsync(new CreateTopicModel(true, "ssss", "DDDDD", null))).Id;

			// b join group
			await b.Api<TopicApi>().JoinTopicAsync(new JoinTopicModel(topicId.ToString()));

			// b send request
			var requestId = (await b.Api<GroupManageApi>().SendAdminRequestAsync(new SendAdminRequestModel(topicId.ToString(), "hello!!!"))).Id;

			// check
			var request = await a.Api<GroupManageApi>().GetRequestByIdAsync(requestId.ToString());

			Assert.Equal(requestId, request.Id);
			Assert.Equal(b.UserId, request.SenderId);
			Assert.Equal(topicId, request.TopicId);

			var requests = await a.Api<GroupManageApi>().GetUnhandledRequestsAsync(topicId.ToString(), 0, true);
			Assert.Single(requests);


			// accept
			await a.Api<GroupManageApi>().HandleRequestAsync(new HandleRequestModel(requestId.ToString(), true));


			// check request in two ways

			var q = await a.Api<GroupManageApi>().GetRequestByIdAsyncWithHttpInfo(requestId.ToString());

			requests = await a.Api<GroupManageApi>().GetUnhandledRequestsAsync(topicId.ToString(), 0, true);
			Assert.Empty(requests);

			request = await a.Api<GroupManageApi>().GetRequestByIdAsync(requestId.ToString());
			Assert.Equal(RequestStatus.Accepted, request.Status);


			// check membership
			var membership = await Context.TopicMembers
				.Where(p => p.TopicId == XUtils.ParseId(topicId.ToString()) && p.UserId == XUtils.ParseId(b.UserId.ToString()))
				.FirstOrDefaultAsync();

			Assert.Equal(Leopard.Domain.TopicMemberAG.MemberRole.Normal, membership.Role);


		}
	}
}
