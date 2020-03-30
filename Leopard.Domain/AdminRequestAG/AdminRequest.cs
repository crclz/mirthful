﻿using System;
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
		Unhandled = 0,
		Accepted = 1,
		Refused = 2,
	}
}
