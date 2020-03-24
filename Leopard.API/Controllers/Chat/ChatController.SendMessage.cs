using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.ChatMessageAggregate;
using Leopard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.Chat
{
	public partial class ChatController
	{
		[HttpPost("message")]
		[ServiceFilter(typeof(AuthenticationFilter))]
		[ServiceFilter(typeof(IsSessionMemberFilter))]
		//[Consumes("multipart/form-data")]
		public async Task<IActionResult> SendMessage([FromForm]SendMessageModel model, [FromServices]IBlobBucket bucket)
		{
			var files = model.Image;
			ChatMessage message;

			switch (model.Type)
			{
				case ChatMessageType.Text:
					if (!(model?.Text.Length <= 640))
						return new ApiError(MyErrorCode.ModelInvalid, "The length of the message should shorter than 640").Wrap();
					message = ChatMessage.CreateTextMessage(SessionStore.SessionId, (ObjectId)Store.UserId, model.Text);
					break;

				case ChatMessageType.TextAndImages:
					if (!(model?.Text.Length <= 640))
						return new ApiError(MyErrorCode.ModelInvalid, "The message length must be shorter than 640").Wrap();
					//if (files.Count >= 1 && files.Count <= 9)
					//	return new ApiError(MyErrorCode.ModelInvalid, "The count of files must between 1 and 9").Wrap();
					if (files.Length > 1024 * 1024)// 1MB
						return new ApiError(MyErrorCode.ModelInvalid, "The size of any file must smaller than 1MB").Wrap();


					var blobs = new List<string>();

					using (var stream = model.Image.OpenReadStream())
					{
						var blobUrl = await bucket.PutBlobAsync(stream, Guid.NewGuid().ToString());
						blobs.Add(blobUrl);
					}

					message = ChatMessage.CreateTextAndImageMessage(SessionStore.SessionId, (ObjectId)Store.UserId,
						model.Text, blobs);
					break;

				case ChatMessageType.Voice:
					goto case ChatMessageType.File;
				case ChatMessageType.File:
					if (model.SingleFile?.Length > 1024 * 1024)
						return new ApiError(MyErrorCode.ModelInvalid, "The size of any file must smaller than 1MB").Wrap();
					using (var stream = model.SingleFile.OpenReadStream())
					{
						var blob = await bucket.PutBlobAsync(stream, Guid.NewGuid().ToString());

						if (model.Type == ChatMessageType.Voice)
							message = ChatMessage.CreateVoiceMessage(SessionStore.SessionId, (ObjectId)Store.UserId, blob);
						else
							message = ChatMessage.CreateFileMessage(SessionStore.SessionId, (ObjectId)Store.UserId, blob);
					}
					break;

				case ChatMessageType.Invitation:
					if (!ObjectId.TryParse(model.InvitationGroupId, out ObjectId groupId))
						return new ApiError(MyErrorCode.ModelInvalid, "Field groupId cannot be parsed").Wrap();
					message = ChatMessage.CreateInvitaionMessage(SessionStore.SessionId, (ObjectId)Store.UserId, groupId);
					break;

				case ChatMessageType.BusinessCard:
					if (!ObjectId.TryParse(model.CardUserId, out ObjectId cardId))
						return new ApiError(MyErrorCode.ModelInvalid, "Field cardId cannot be parsed").Wrap();
					message = ChatMessage.CreateBusinessCardMessage(SessionStore.SessionId, (ObjectId)Store.UserId, cardId);
					break;

				default:
					return new ApiError(MyErrorCode.ModelInvalid, "Unknown ChatMessageType").Wrap();
			}

			await ChatMessageRepository.PutAsync(message);

			return Ok();
		}
	}

	public class SendMessageModel : ISessionModel
	{
		public string SessionId { get; set; }

		public ChatMessageType Type { get; set; }

		public string Text { get; set; }

		public IFormFile Image { get; set; }


		public IFormFile SingleFile { get; set; }
		public string InvitationGroupId { get; set; }
		public string CardUserId { get; set; }
	}

}
