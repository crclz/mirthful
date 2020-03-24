using Leopard.API.Filters;
using Leopard.Domain.Model.SessionMemberAggregate;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Chat
{
	public partial class ChatController
	{
		[HttpPost("session/set-notification-policy")]
		[ServiceFilter(typeof(AuthenticationFilter))]
		[ServiceFilter(typeof(IsSessionMemberFilter))]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task<IActionResult> SetNotificationPolicy([FromBody]SetNotificationLevelModel model)
		{
			var member = SessionStore.Member;
			member.SetNotificationPolicy((SessionNotificationPolicy)model.NotificationPolicy);
			await SessionMemberRepository.PutAsync(member);

			return Ok();
		}
	}

	public class SetNotificationLevelModel : ISessionModel
	{
		public string SessionId { get; set; }

		[Required]
		public SessionNotificationPolicy? NotificationPolicy { get; set; }
	}
}
