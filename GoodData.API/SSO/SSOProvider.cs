using System;
using System.Diagnostics;
using System.Web;
using GoodData.Security.OpenPGP;
using GoodData.API.API.SSO;

namespace GoodData.API.SSO {
	public class SsoProvider {
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
			var userData = CreateUserData(email);

			lock (lockThis) {
				var signedData = pgpProcessor.Sign(userData);
				var encryptedData = pgpProcessor.Encrypt(signedData);
				return EncodeUserData(encryptedData);
			}
		}


		private static string CreateUserData(string email, int validaityOffsetInMinutes = 10) {
			return "{\"email\":\"" + email + "\",\"validity\":" +
					 Math.Round(DateTime.UtcNow.AddMinutes(validaityOffsetInMinutes).ToUnixTime()) + "}";
		}

		private static string EncodeUserData(string input) {
			return HttpUtility.UrlEncode(input);
		}
	}
}