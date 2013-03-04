using System;

namespace GoodData.API
{
	public static class Extensions
	{
		public static DateTime UnixEpochStartDate {
			get {
				return new DateTime(1970, 1, 1);
			}
		}

		public static string ExtractId(this string value, string replacePath)
		{
			return value.Replace(replacePath, string.Empty).Replace("/", string.Empty);
		}

		public static string ExtractObjectId(this string value)
		{
			var startIndex = value.LastIndexOf('/') + 1;
			return value.Substring(startIndex, value.Length - startIndex);
		}

		public static int ToUnixTimeStamp(this DateTime date) {
			return (int)(date - UnixEpochStartDate).TotalSeconds;
		}
	}
}