using System;
using System.Diagnostics;
using System.Web;
using GoodData.Security.OpenPGP;
using GoodData.API.API.SSO;

namespace GoodData.API.SSO {
	public class SsoProvider : ISsoProvider {
		private System.Object lockThis = new System.Object();
		private IGoodDataPgpProcessor pgpProcessor;

		public SsoProvider(IGoodDataPgpProcessor pgpProcessor) {
			this.pgpProcessor = pgpProcessor;
		}

		public SsoProvider(string goodDataKeyUserId, byte[] goodDataPublickKeyData, byte[] privateKeyData, char[] passphrase) {
			this.pgpProcessor = new DefaultPgpProcessor(
				new OpenPgpProcessor(goodDataKeyUserId, goodDataPublickKeyData, privateKeyData, passphrase));
		}

		public string GenerateToken(string email, int validaityOffsetInMinutes = 10) {
			var userData = CreateUserData(email, validaityOffsetInMinutes);

			lock (lockThis) {
				var signedData = pgpProcessor.Sign(userData);
				var encryptedData = pgpProcessor.Encrypt(signedData);
				return encryptedData;
			}
		}


		private static string CreateUserData(string email, int validaityOffsetInMinutes = 10) {
			return "{\"email\":\"" + email + "\",\"validity\":" +
					 DateTime.UtcNow.AddMinutes(validaityOffsetInMinutes).ToUnixTimeStamp() + "}";
		}
	}
}