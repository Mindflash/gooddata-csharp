using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoodData.API.SSO;
using GoodData.Security.OpenPGP;

namespace GoodData.API.API.SSO {
	public class DefaultPgpProcessor : IGoodDataPgpProcessor {

		public DefaultPgpProcessor(OpenPgpProcessor processor) {
			this.processor = processor;
		}

		private OpenPgpProcessor processor;

		public string Encrypt(string unencryptedText) {
			return processor.Encrypt(unencryptedText);
		}

		public string Sign(string unencryptedText) {
			return processor.Sign(unencryptedText);
		}
	}
}
