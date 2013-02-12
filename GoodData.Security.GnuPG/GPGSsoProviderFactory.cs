using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoodData.API.SSO;

namespace GoodData.Security.GPG {
	public static class GPGSsoProviderFactory {
		public static SsoProvider CreateProvider(string goodDataPubKeyUserId, string privateKeyPassphrase) {
			return new SsoProvider(new GnuPgpProcessor(goodDataPubKeyUserId, privateKeyPassphrase));
		}
	}
}
