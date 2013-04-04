using System;

namespace GoodData.API.Api.Models
{
	[Serializable]
	public class GoodDataApiException : Exception
	{
		public GoodDataErrorType ErrorType { get; protected set; } 

		public GoodDataApiException(string message)
			: base(message)
		{
		}

		public GoodDataApiException(string message, Exception inner)
			: base(message,inner) {
		}

		public GoodDataApiException(string message, GoodDataErrorType errorType)
			: base(message) {
				ErrorType = errorType;
		}

		public GoodDataApiException(string message, Exception inner, GoodDataErrorType errorType)
			: base(message, inner) {
				ErrorType = errorType;
		}
	}
}