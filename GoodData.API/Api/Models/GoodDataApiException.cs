using System;

namespace GoodData.API.Api.Models
{
	[Serializable]
	public class GoodDataApiException : Exception
	{
		public GoodDataApiException(string message)
			: base(message)
		{
		}
	}
}