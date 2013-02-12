using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
	internal class OpenPgpUtilities
	{
		internal static byte[] CompressStream(Stream inputStream, string fileName, CompressionAlgorithmTag algorithm)
		{
			
			using (MemoryStream bOut = new MemoryStream()) {
				//inputStream.CopyTo(bOut);

				PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(algorithm);

				PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
				Stream pOut = lData.Open(comData.Open(bOut), PgpLiteralData.Binary, fileName, inputStream.Length, DateTime.Now);

				inputStream.CopyTo(pOut);
				inputStream.Close();

				return bOut.ToArray();
			}
		}

		/**
		 * Search a secret key ring collection for a secret key corresponding to keyID if it
		 * exists.
		 * 
		 * @param pgpSec a secret key ring collection.
		 * @param keyID keyID we want.
		 * @param pass passphrase to decrypt secret key with.
		 * @return
		 * @throws PGPException
		 * @throws NoSuchProviderException
		 */
		internal static PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyID, char[] pass)
		{
			PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyID);

			if (pgpSecKey == null)
			{
				return null;
			}

			return pgpSecKey.ExtractPrivateKey(pass);
		}

		internal static PgpPublicKey ReadPublicKey(string fileName, string userId)
		{
			using (Stream keyIn = File.OpenRead(fileName))
			{
				return ReadPublicKey(keyIn, userId);
			}
		}

		internal static PgpPublicKey ReadPublicKey(byte[] keyData, string userId) {
			using (Stream keyIn = new MemoryStream(keyData)) {
				return ReadPublicKey(keyIn, userId);
			}
		}

		/**
		 * A simple routine that opens a key ring file and loads the first available key
		 * suitable for encryption.
		 * 
		 * @param input
		 * @return
		 * @throws IOException
		 * @throws PGPException
		 */
		internal static PgpPublicKey ReadPublicKey(Stream input, string userId)
		{
			PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(
				PgpUtilities.GetDecoderStream(input));

			foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
			{
				foreach (PgpPublicKey key in keyRing.GetPublicKeys())
				{
					
					if (key.IsEncryptionKey &&  key.GetUserIds().OfType<string>().Any(x => x != null && x.Contains(userId)))
					{
						return key;
					}
				}
			}

			throw new ArgumentException("Can't find encryption key in key ring.");
		}

		internal static PgpSecretKey ReadSecretKey(string fileName)
		{
			using (Stream keyIn = File.OpenRead(fileName))
			{
				return ReadSecretKey(keyIn);
			}
		}

		internal static PgpSecretKey ReadSecretKey(byte[] keyData) {
			using (Stream keyIn = new MemoryStream(keyData)) {
				return ReadSecretKey(keyIn);
			}
		}

		/**
		 * A simple routine that opens a key ring file and loads the first available key
		 * suitable for signature generation.
		 * 
		 * @param input stream to read the secret key ring collection from.
		 * @return a secret key.
		 * @throws IOException on a problem with using the input stream.
		 * @throws PGPException if there is an issue parsing the input stream.
		 */
		internal static PgpSecretKey ReadSecretKey(Stream input)
		{
			PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(
				PgpUtilities.GetDecoderStream(input));

			//
			// we just loop through the collection till we find a key suitable for encryption, in the real
			// world you would probably want to be a bit smarter about this.
			//

			foreach (PgpSecretKeyRing keyRing in pgpSec.GetKeyRings())
			{
				foreach (PgpSecretKey key in keyRing.GetSecretKeys())
				{
					if (key.IsSigningKey)
					{
						return key;
					}
				}
			}

			throw new ArgumentException("Can't find signing key in key ring.");
		}
	}
}
