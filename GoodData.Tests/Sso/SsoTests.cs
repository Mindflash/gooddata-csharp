﻿using GoodData.API.SSO;
using NUnit.Framework;
using Starksoft.Cryptography.OpenPGP;

namespace GoodDataTests.SSO
{
	[TestFixture]
	public class SsoTests
	{
		//[Test]
		//public void GenerateToken_ValidEmail_ExpectSucces()
		//{
		//   var service = new SsoProvider();
		//   var token = service.GenerateToken(service.Config.Login);
		//   Assert.IsNotNullOrEmpty(token);
		//   Assert.Less(0, token.IndexOf("BEGIN+PGP+MESSAGE"));
		//}

		//[Test]
		//[ExpectedException(typeof (GnuPGException))]
		//public void Sign_BadPassphrase_ExpectException()
		//{
		//   var service = new GnuPgpProcessor();
		//   var token = service.Sign("dsdsfsfsdf", "dsfksldfksfd");
		//}
	}
}