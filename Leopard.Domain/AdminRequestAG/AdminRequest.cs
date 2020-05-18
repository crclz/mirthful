using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.AdminRequestAG
{
	public class AdminRequest : RootEntity
	{
		public Guid TopicId { get; private set; }
		public Guid SenderId { get; private set; }
		public string Text { get; private set; }
		public RequestStatus Status { get; private set; }

		private AdminRequest()
		{
		}

		public AdminRequest(Guid topicId, Guid senderId, string text)
		{
			TopicId = topicId;
			SenderId = senderId;
			Text = text ?? throw new ArgumentNullException(nameof(text));
			Status = RequestStatus.Unhandled;
		}

		public void Handle(bool accept)
		{
			if (Status != RequestStatus.Unhandled)
				throw new InvalidOperationException();

			if (accept)
				Status = RequestStatus.Accepted;
			else
				Status = RequestStatus.Refused;

			UpdatedAtNow();
		}
	}

	public enum RequestStatus
	{
		/// <summary>
		/// 未处理
		/// </summary>
		Unhandled = 0,

		/// <summary>
		/// 已被接受
		/// </summary>
		Accepted = 1,

		/// <summary>
		/// 已被拒绝
		/// </summary>
		Refused = 2,
	}
}
