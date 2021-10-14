namespace SerializationInterceptor.Extensions
{
	internal static class StringExtensions
	{
		public static string LowerFirstLetter(this string s) => string.IsNullOrEmpty(s) ? s : char.ToLower(s[0]) + s[1..];

		public static string DoubleQuote(this string s) => $"\"{s}\"";
	}
}
