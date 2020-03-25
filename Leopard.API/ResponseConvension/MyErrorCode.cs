namespace Leopard.API.ResponseConvension
{
	public static class MyErrorCode
	{
		public static string ModelInvalid => "ModelInvalid";
		public static string IdNotFound => "IdNotFound";
		public static string UsernameNotFound => "UsernameNotFound";
		public static string UsernameExist => "UsernameExist";
		public static string WrongPassword => "WrongPassword";
		public static string AlreadFriends => "AlreadFriends";
		public static string NotFriends => "NotFriends";
		public static string RequirementTypeMismatch => "RequirementTypeMismatch";
		public static string IsSelf => "IsSelf";
		public static string Blocked => "Blocked";
		public static string ExistUnhandledReuqest => "ExistUnhandledReuqest";
		public static string HasUnhandledRequestMismatch => "HasUnhandledRequestMismatch";
		public static string CountMismatch => "CountMismatch";
		public static string WrongAnswer => "WrongAnswer";
		public static string NoUnhandledReuqest => "NoUnhandledReuqest";
		public static string FileTooLarge => "FileTooLarge";
		public static string InGroupMismatch => "InGroupMismatch";
		public static string Unauthorized => "Unauthorized";
		public static string NotNormalRole => "NotNormalRole";
		public static string FounderCannotQuitGroup => "FounderCannotQuitGroup";
		public static string NotAMember => "NotAMember";
		public static string PermissionDenied => "PermissionDenied";
		public static string UniqueConstraintConflict => "UniqueConstraintConflict";
	}
}
