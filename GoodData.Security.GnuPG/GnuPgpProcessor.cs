using System;
using System.IO;
using System.Text;
using Starksoft.Cryptography.OpenPGP;
using GoodData.API.SSO;

namespace GoodData.Security.GPG
{
	/// <summary>
	/// </summary>
	/// <see cref = "http://sourceforge.net/projects/starksoftopenpg/" />
	public class GnuPgpProcessor : IGoodDataPgpProcessor, IDisposable
	{


		string goodDataPubKeyUserId;
		string keyPassphrase;

		private GnuPG _gpg;
		
		public GnuPgpProcessor(string goodDataPubKeyUserId, string keyPassphrase)
		{
			this.goodDataPubKeyUserId = goodDataPubKeyUserId;
			this.keyPassphrase = keyPassphrase;

			var path = new GpgPath();
			_gpg = new GnuPG(path.GetHomeFolderPath(), path.GetExeFolder());
		}

		/// <summary>
		/// 	Will Encrypt any OpenPGP data
		/// </summary>
		/// <param name = "recipient"></param>
		/// <param name = "unencryptedText"></param>
		/// <returns></returns>
		public string Encrypt(string unencryptedText)
		{
			// if no recipient is specified then don't do the encryption
			if (string.IsNullOrWhiteSpace(goodDataPubKeyUserId))
				throw new ArgumentNullException();

			// create two memory stream - one to hold the unencrypted data and the other stream holds the encrypted data.
			using (var unencrypted = new MemoryStream(Encoding.ASCII.GetBytes(unencryptedText)))
			using (var encrypted = new MemoryStream())
			{
				// create a new GnuPG object
				_gpg.OutputType = OutputTypes.AsciiArmor;
				_gpg.Recipient = goodDataPubKeyUserId;
				_gpg.Encrypt(unencrypted, encrypted);
				using (var reader = new StreamReader(encrypted))
				{
					encrypted.Position = 0;
					return reader.ReadToEnd();
				}
			}
		}


		public string Sign(string unencryptedText)
		{
			if (string.IsNullOrWhiteSpace(keyPassphrase) || string.IsNullOrWhiteSpace(unencryptedText))
				throw new ArgumentNullException();

			using (var unsigned = new MemoryStream(Encoding.ASCII.GetBytes(unencryptedText)))
			using (var signed = new MemoryStream())
			{
				// create a new GnuPG object
				_gpg.OutputType = OutputTypes.AsciiArmor;
				_gpg.Passphrase = keyPassphrase;
				_gpg.Sign(unsigned, signed);

				using (var reader = new StreamReader(signed))
				{
					signed.Position = 0;
					return reader.ReadToEnd();
				}
			}
		}


		public void Dispose() {
			if (_gpg != null) {
				_gpg.Dispose();
				_gpg = null;
			}
		}
	}
}