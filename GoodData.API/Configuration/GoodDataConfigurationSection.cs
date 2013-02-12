using System.Configuration;

namespace GoodData.API {
	public class GoodDataConfig {
		public string ServiceUrl { get; set; }
		public string Domain { get; set; }
		public bool IgnoreSslErrors { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
	}
}