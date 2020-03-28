using Leopard.Domain;
using Leopard.Domain.TopicMemberAG;
using Leopard.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Leopard.API.Test.Smoke
{
	public class GroupManageTest
	{
		public LeopardDatabase Db { get; }

		public GroupManageTest()
		{
			Db = new LeopardDatabase();
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


			// accept
			await a.Api<GroupManageApi>().HandleRequestAsync(new HandleRequestModel(requestId, true));

			// check request
			var q = await a.Api<GroupManageApi>().GetRequestByIdAsyncWithHttpInfo(requestId);

			request = await a.Api<GroupManageApi>().GetRequestByIdAsync(requestId);
			Assert.Equal(RequestStatus.Accepted, request.Status);

			// check membership
			var membership = await Db.GetCollection<TopicMember>().AsQueryable()
				.Where(p => p.TopicId == XUtils.ParseId(topicId) && p.UserId == XUtils.ParseId(b.UserId))
				.FirstOrDefaultAsync();

			Assert.Equal(MemberRole.Admin, membership.Role);

		}
	}
}
