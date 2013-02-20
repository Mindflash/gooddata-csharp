using System;
namespace GoodData.API.SSO {
	public interface ISsoProvider {
		string GenerateToken(string email, int validaityOffsetInMinutes = 10);
	}
}
