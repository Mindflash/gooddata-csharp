using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Org.BouncyCastle.Bcpg.OpenPgp.Examples;
using Org.BouncyCastle.Bcpg.OpenPgp;
using log4net;

namespace GoodData.Security.OpenPGP {
	public class OpenPgpProcessor {

		private static readonly ILog log = LogManager.GetLogger(typeof(OpenPgpProcessor)); 

		public OpenPgpProcessor(string goodDataKeyUserId, byte[] goodDataPublickKeyData, byte[] privateKeyData, char[] passphrase) {

			var publickKey = ParsePublicKey(goodDataKeyUserId, goodDataPublickKeyData);

			var secretKey = OpenPgpUtilities.ReadSecretKey(privateKeyData);

			processor = new EncryptionProcessor(
					publickKey,
					secretKey,
					passphrase);
		}

		private static PgpPublicKey ParsePublicKey(string goodDataKeyUserId, byte[] goodDataPublickKeyData) {
			var publickKey = OpenPgpUtilities.ReadPublicKey(goodDataPublickKeyData, goodDataKeyUserId);

			long secondsToExpire = publickKey.GetValidSeconds();
			if (secondsToExpire > 0) {
				TimeSpan publickKeyValidTimeLeft = publickKey.CreationTime.AddSeconds(secondsToExpire).Subtract(DateTime.UtcNow);
				if (publickKeyValidTimeLeft.TotalDays < 14) {
					log.Warn("GoodData public key used for SSO will expire in 14 days.");
				}
			}
			return publickKey;
		}

		private EncryptionProcessor processor;


		public string Encrypt(string unencryptedText) {
			using (var streamOut = new MemoryStream()) {
				processor.encryptMessage(streamOut,
					unencryptedText,
					true);

				streamOut.Position = 0;
				var sr = new StreamReader(streamOut);
				return sr.ReadToEnd();
			}
		}

		public string Sign(string unencryptedText) {
			using (var streamOut = new MemoryStream()) {
				using (var unsigned = new MemoryStream(Encoding.ASCII.GetBytes(unencryptedText))) {
					processor.signMessage(unsigned, streamOut, true);
				}
				streamOut.Position = 0;
				var sr = new StreamReader(streamOut);
				return sr.ReadToEnd();
			}
		}

		public void Dispose() {
		}
	}
}
