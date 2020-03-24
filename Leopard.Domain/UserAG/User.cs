using Crucialize.LangExt;
using Dawn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Domain.UserAG
{
	public class User : RootEntity
	{
		// TODO: Unique Index
		public string Username { get; private set; }
		public string Description { get; private set; }
		public string Nickname { get; private set; }
		public string Salt { get; private set; }
		public string PasswordHash { get; private set; }
		public int SecurityVersion { get; private set; } = 0;
		public string Avatar { get; private set; }

		private User()
		{

		}

		public User(string username, string password, string nickname, string description)
		{
			Username = username ?? throw new ArgumentNullException(nameof(username));
			Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
			Description = description ?? throw new ArgumentNullException(nameof(description));

			SetPassword(password);
		}

		public bool IsPasswordCorrect(string password)
		{
			Guard.Argument(() => password).NotNull();
			var passAndSalt = password + Salt;
			var sha256 = passAndSalt.ToUTF8().GetSHA256();
			return sha256 == PasswordHash;
		}

		public void SetPassword(string password)
		{
			Guard.Argument(() => password).NotNull().LengthInRange(8, 32);
			Salt = XUtils.GetRandomBytes(64).ToBase64();
			var passAndSalt = password + Salt;
			PasswordHash = passAndSalt.ToUTF8().GetSHA256();

			SecurityVersion++;
		}
	}
}
