using Leopard.API.Filters;
using Leopard.Domain;
using Leopard.Domain.Model.SessionMemberAggregate;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Chat
{
	public partial class ChatController
	{
		[HttpPost("session/set-notification-rule-for-single-user")]
		[ServiceFilter(typeof(AuthenticationFilter))]
		[ServiceFilter(typeof(IsSessionMemberFilter))]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task<IActionResult> SetNotificationPolicy([FromBody]SetNotificationRuleModel model)
		{
			var member = SessionStore.Member;
			var targetId = (ObjectId)Useful.ParseId(model.TargetId);

			if (model.Level == NotificationLevel.Normal)
			{
				member.RemoveNotificationRuleByUserId(targetId);
			}
			else
			{
				member.AddNotificationRule(targetId, (NotificationLevel)model.Level);
			}

			await SessionMemberRepository.PutAsync(member);

			return Ok();
		}
	}

	public class SetNotificationRuleModel : ISessionModel
	{
		public string SessionId { get; set; }

		public string TargetId { get; set; }

		[Required]
		public NotificationLevel? Level { get; set; }
	}
}
