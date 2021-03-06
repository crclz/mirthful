﻿using Dawn;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.TopicAG
{
	public class Topic : RootEntity
	{
		public bool IsGroup { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Guid? RelatedWork { get; private set; }
		public int MemberCount { get; private set; }

		public NpgsqlTsVector Tsv { get; private set; }

		private Topic()
		{
		}

		public Topic(bool isGroup, string name, string description, Guid? relatedWork, Guid creatorId)
		{
			IsGroup = isGroup;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Description = description ?? throw new ArgumentNullException(nameof(description));
			RelatedWork = relatedWork;
			MemberCount = 0;

			PushDomainEvent(new TopicCreatedEvent(Id, isGroup, creatorId));
		}

		public void SetMemberCount(int cnt)
		{
			Guard.Argument(() => cnt).Min(0);
			MemberCount = cnt;
			UpdatedAtNow();
		}

		public void MemberIncr(int delta)
		{
			var newCnt = MemberCount + delta;
			if (newCnt < 0)
				throw new ArgumentException("'delta' makes MemberCount negative", nameof(delta));

			SetMemberCount(newCnt);
			UpdatedAtNow();
		}
	}
}
