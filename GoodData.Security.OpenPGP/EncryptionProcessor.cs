using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System.Text;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples {

	public sealed class EncryptionProcessor {

		private PgpSecretKey secretKeyForSigning;
		private char[] secretKeyPassword;
		private PgpPublicKey publicKeyForEncryption;


		/**
		 * Constructor is private and only for internal usage - use {@link com.gooddata.security.pgp.PgpEncryptor.Builder} instead.
		 * @param encryptionKey public key which will be used for encryption of messages
		 * @param signingKey private key which will be used for making messages' signatures.
		 * @param secretKeyPassword password for secret key
		 */
		public EncryptionProcessor(PgpPublicKey encryptionKey, PgpSecretKey signingKey, char[] secretKeyPassword) {
			this.publicKeyForEncryption = encryptionKey;
			this.secretKeyForSigning = signingKey;
			this.secretKeyPassword = secretKeyPassword;
		}


		/**
		 * Generates an encapsulated signed file.
		 */
		public void signMessage(Stream unsignedContent, Stream signedContent, bool armor) {

			if (armor) {
				// output will be BASE64 encoded
				signedContent = new ArmoredOutputStream(signedContent);
			}

			PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.ZLib);
			PgpLiteralDataGenerator literalDataGenerator = new PgpLiteralDataGenerator();
			try {
				BcpgOutputStream bcpgSignedContentOut = new BcpgOutputStream(compressedDataGenerator.Open(signedContent));

				PgpPrivateKey pgpPrivateKey = secretKeyForSigning.ExtractPrivateKey(secretKeyPassword);
				PgpSignatureGenerator signatureGenerator = createSignatureGenerator(pgpPrivateKey);

				signatureGenerator.GenerateOnePassVersion(false).Encode(bcpgSignedContentOut);

				Stream literalDataOut = literalDataGenerator.Open(bcpgSignedContentOut, PgpLiteralData.Binary, "_CONSOLE", unsignedContent.Length, DateTime.Now);

				updateSignatureGeneratorWithInputBytes(unsignedContent, signatureGenerator, literalDataOut);
				signatureGenerator.Generate().Encode(bcpgSignedContentOut);
			} finally {
				literalDataGenerator.Close();
				compressedDataGenerator.Close();
				signedContent.Close();
			}
		}


		/**
		 * Encrypts given {@code message}. Output is written ot the {@code encryptedMessage} output stream.
		 * Message is encrypted usign symmetric algorithm, default is {@link SymmetricKeyAlgorithmTags#TRIPLE_DES}.
		 *
		 * @param encryptedMessage
		 * @param message
		 * @param armor                  if output should be armored (BASE64 encoding of binary data)
		 * @throws java.io.IOException
		 * @throws java.security.NoSuchProviderException
		 * @throws PgpException
		 */
		public void encryptMessage(Stream encryptedMessage, string message, bool armor) {

			PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
			PgpLiteralDataGenerator literalDataGenerator = new PgpLiteralDataGenerator();

			// we want to generate compressed data
			MemoryStream byteOut = new MemoryStream();

			writeClearDataToByteOut(compressedDataGenerator, literalDataGenerator, Encoding.ASCII.GetBytes(message), byteOut);

			compressedDataGenerator.Close();

			MemoryStream encryptedOut = new MemoryStream();

			Stream outStream = encryptedOut;
			if (armor) {
				// output will be BASE64 encoded
				outStream = new ArmoredOutputStream(outStream);
			}

			Stream encryptedDataOut = null;
			try {
				byte[] bytes = byteOut.ToArray();
				encryptedDataOut = createEncryptedDataGenerator().Open(outStream, bytes.Length);
				encryptedDataOut.Write(bytes, 0, bytes.Length);  // obtain the actual bytes from the compressed stream
			} finally {
				if (encryptedDataOut != null) {
					encryptedDataOut.Close();
				}
				outStream.Close();
			}

			var encData = encryptedOut.ToArray();
			encryptedMessage.Write(encData, 0, encData.Length);

		}


		////--------------------------------------------------- BUILDER ------------------------------------------------------
		//private  class Builder {


		//   private static char[] EMPTY_PASSWORD = new char[0];

		//   private PgpPublicKey publicKeyForEncryption = null;
		//   private PgpSecretKey secretKeyForSigning = null;
		//   private char[] secretKeyPassword = EMPTY_PASSWORD;

		//   public Builder setPublicKeyForEncryption(InputStream pgpPublicKeyIn) {
		//      Validate.notNull(pgpPublicKeyIn, "If you do not want to set public key, then simply do not call this method!");
		//      try {
		//         PgpPublicKey publicKey = KeyUtils.findPublicKeyForEncryption(pgpPublicKeyIn);
		//         if (publicKey == null) {
		//            throw new IllegalArgumentException("Cannot load public key from given input stream");
		//         }
		//         this.publicKeyForEncryption = publicKey;
		//      } catch (Exception e) {
		//         throw new IllegalArgumentException("Cannot load public key from given input stream");
		//      }

		//      return this;
		//   }

		//   public Builder setPublicKeyForEncryption(PgpPublicKey pgpPublicKey) {
		//      Validate.notNull(pgpPublicKey, "If you do not want to set public key, then simply do not call this method!");
		//      this.publicKeyForEncryption = pgpPublicKey;
		//      return this;
		//   }

		//   public Builder setSecretKeyForSigning(InputStream pgpSecretKeyIn) {
		//      Validate.notNull(pgpSecretKeyIn, "If you do not want to set private key, then simply do not call this method!");
		//      try {
		//         PgpSecretKey secretKey = KeyUtils.findSecretKeyForSigning(pgpSecretKeyIn);
		//         if (secretKey == null) {
		//            throw new IllegalArgumentException("Cannot load secret key from given input stream");
		//         }
		//         this.secretKeyForSigning = secretKey;
		//      } catch (Exception e) {
		//         throw new IllegalArgumentException("Cannot load secret key from given input stream");
		//      }

		//      return this;
		//   }

		//   public Builder setSecretKeyForSigning(PgpSecretKey pgpSecretKey) {
		//      Validate.notNull(pgpSecretKey, "If you do not want to set private key, then simply do not call this method!");
		//      this.secretKeyForSigning = pgpSecretKey;
		//      return this;
		//   }

		//   public Builder setSecretKeyPassword(char[] secretKeyPassword) {
		//      if (secretKeyPassword == null) {
		//         this.secretKeyPassword = EMPTY_PASSWORD;
		//      } else {
		//         this.secretKeyPassword = Arrays.copyOf(secretKeyPassword, secretKeyPassword.length);
		//      }
		//      return this;
		//   }

		//   public PgpEncryptor createPgpEncryptor() {
		//      return new PgpEncryptor(publicKeyForEncryption, secretKeyForSigning, secretKeyPassword);
		//   }

		//}


		//--------------------------------------------------- HELPER METHODS -----------------------------------------------

		private PgpSignatureGenerator createSignatureGenerator(PgpPrivateKey pgpPrivateKey) {
			PgpSignatureGenerator signatureGenerator = new PgpSignatureGenerator(
				secretKeyForSigning.PublicKey.Algorithm, HashAlgorithmTag.Sha1);

			signatureGenerator.InitSign(PgpSignature.BinaryDocument, pgpPrivateKey);

			setSignatureSubpackets(secretKeyForSigning, signatureGenerator);
			return signatureGenerator;
		}


		private static void setSignatureSubpackets(PgpSecretKey pgpSec, PgpSignatureGenerator signatureGenerator) {
			IEnumerable it = pgpSec.PublicKey.GetUserIds();
			var e = it.GetEnumerator();

			if (e.MoveNext()) {
				PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();

				spGen.SetSignerUserId(false, (String)e.Current);
				signatureGenerator.SetHashedSubpackets(spGen.Generate());
			}
		}


		private static void updateSignatureGeneratorWithInputBytes(Stream unsigendContent,
				  PgpSignatureGenerator signatureGenerator, Stream lOut) {
			int ch;

			while ((ch = unsigendContent.ReadByte()) >= 0) {
				lOut.WriteByte((byte)ch);
				signatureGenerator.Update((byte)ch);
			}
		}

		private void writeClearDataToByteOut(PgpCompressedDataGenerator compressedDataGenerator, PgpLiteralDataGenerator literalDataGenerator, byte[] clearData, Stream byteOut) {
			try {
				Stream pOut = literalDataGenerator.Open(compressedDataGenerator.Open(byteOut), // the compressed output stream
						  PgpLiteralData.Binary,
						  PgpLiteralData.Text.ToString(),  // "filename" to store
						  clearData.Length, // length of clear data
						  DateTime.Now // current time
				);
				pOut.Write(clearData, 0, clearData.Length);
			} finally {
				literalDataGenerator.Close();
			}
		}


		private PgpEncryptedDataGenerator createEncryptedDataGenerator() {
			PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes,
				false,
				new SecureRandom());
			encryptedDataGenerator.AddMethod(this.publicKeyForEncryption);

			return encryptedDataGenerator;
		}

	}

}
