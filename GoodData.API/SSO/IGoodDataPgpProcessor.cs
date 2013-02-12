using System;
namespace GoodData.API.SSO {
	public interface IGoodDataPgpProcessor {
		string Encrypt(string unencryptedText);
		string Sign(string unencryptedText);
	}
}
