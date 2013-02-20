using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GoodData.API.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Specialized;
using System.Web;

namespace GoodData.API.Api {
	public class ApiWrapperBase {
		public CookieContainer CookieJar;

		public ApiWrapperBase(GoodDataConfig config) {
			CookieJar = new CookieContainer();
			Config = config;
		}

		public string ProfileId { get; set; }
		public GoodDataConfig Config { get; set; }

		public void GetToken() {
			var url = Config.ServiceUrl + Constants.TOKEN_URI;
			GetRequest(url);
		}

		public void Authenticate(string userName, string password) {
			var url = Config.ServiceUrl + Constants.LOGIN_URI;
			var payload = new AuthenticationRequest {
				PostUserLogin = new PostUserLogin {
					Login = userName,
					Password = password
				}
			};
			var response = InternalPostRequest(url, payload, false);
			var userResponse = JsonConvert.DeserializeObject(response, typeof(AuthenticationResponse)) as AuthenticationResponse;
			if (userResponse != null) {
				ProfileId = userResponse.UserLogin.State.ExtractId(Constants.LOGIN_URI);
			}
		}


		/// <summary>
		/// 	Callback used to validate the certificate in an SSL conversation
		/// </summary>
		protected bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain,
															  SslPolicyErrors policyErrors) {
			if (Convert.ToBoolean(Config.IgnoreSslErrors)) {
				// allow any old dodgy certificate...
				return true;
			}
			return policyErrors == SslPolicyErrors.None;
		}

		public string JsonPostRequest(string url, object postData) {
			return InternalPostRequest(url, postData, true);
		}

		public string FormPostRequest(string url, NameValueCollection postData) {
			return InternalPostRequest(url, postData, true);
		}

		private string InternalPostRequest(string url, object postData, bool retry) {
			return MakeRequest(url, "POST", postData, retry ? 3 : 0);
		}

		public string GetRequest(string url) {
			return MakeRequest(url, "GET", null);
		}

		public string PutRequest(string url, object postData) {
			return MakeRequest(url, "PUT", postData);
		}

		public void DeleteRequest(string url) {
			MakeRequest(url, "DELETE", null);
		}

		public byte[] DownloadFile(string url) {
			using (var client = new WebClient()) {
				return client.DownloadData(url);
			}
		}

		public WebResponse GetFileResponse(string url) {
			var webRequest = WebRequest.Create(url) as HttpWebRequest;
			// allows for skipping validation warnings such as from self-signed certs
			ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

			if (webRequest != null) {
				SetupRequest(webRequest, "GET");
			}

			try {
				return webRequest.GetResponse();
			} catch (WebException ex) {
				throw WrapWebException(ex, url, "GET", null);
			}
		}

		private string MakeRequest(string url, string method, object postData, int retries = 3) {
			var webRequest = WebRequest.Create(url) as HttpWebRequest;
			// allows for skipping validation warnings such as from self-signed certs
			ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

			if (webRequest != null) {
				SetupRequest(webRequest, method);
				if (webRequest.Method == WebRequestMethods.Http.Post || webRequest.Method == WebRequestMethods.Http.Put) {
					SetupPostData(webRequest, postData);
				}
			}

			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse)webRequest.GetResponse();
				using (var s = response.GetResponseStream()) {
					using (var sr = new StreamReader(s)) {
						return sr.ReadToEnd();
					}
				}
			} catch (WebException ex) {
				if (ex.Response is HttpWebResponse) {
					var httpResponse = (HttpWebResponse)ex.Response;
					//Retry the request by reauth and get token;
					if (retries > 0 && httpResponse.StatusCode == HttpStatusCode.Unauthorized) {
						httpResponse.Close();
						CheckAuthentication();
						return MakeRequest(url, method, postData, retries - 1);
					}
				}
				throw WrapWebException(ex, url, method, postData);
			} finally {
				if (response != null)
					response.Close();
			}
		}

		private static Exception WrapWebException(WebException x, string url, string method, object postData) {
			StringBuilder errorMessage = new StringBuilder(String.Format("Error occured on request to [{0}] using Http.[{1}]", url, method));
			if (postData != null) {
				errorMessage.Append("With data type - [");
				errorMessage.Append(postData.GetType());
				errorMessage.Append("]");
			}

			if (x.Response is HttpWebResponse) {
				var httpResponse = (HttpWebResponse)x.Response;
				errorMessage.Append(". Response status - [");
				errorMessage.Append((int)httpResponse.StatusCode);
				errorMessage.Append(',');
				errorMessage.Append(httpResponse.StatusCode);
				errorMessage.Append("]");
			}

			if (x.Response != null) {
				using (var data = x.Response.GetResponseStream()) {
					errorMessage.Append(". Response - [");
					errorMessage.Append(new StreamReader(data).ReadToEnd());
					errorMessage.Append("]");
				}
				x.Response.Close();
			}

			return new GoodDataApiException(errorMessage.ToString(), x);
		}


		private static void SetupPostData(HttpWebRequest webRequest, object postData) {
			byte[] data;
			if (postData is NameValueCollection) {
				webRequest.ContentType = "application/x-www-form-urlencoded";
				data = Encoding.ASCII.GetBytes(ConvertNameValueCollectionToPostString((NameValueCollection)postData));
			} else {
				webRequest.ContentType = "application/json; charset=utf-8";
				var jsonData = SerializeObjectToJsonString(postData);
				data = Encoding.UTF8.GetBytes(jsonData);
			}

			webRequest.ContentLength = data.LongLength;
			using (Stream dataStream = webRequest.GetRequestStream()) {
				dataStream.Write(data, 0, data.Length);
			}
		}

		private static string SerializeObjectToJsonString(object postData) {
			var jsonData = JsonConvert.SerializeObject(postData, Formatting.None,
																			 new JsonSerializerSettings {
																				 ContractResolver =
																					 new CamelCasePropertyNamesContractResolver(),
																				 NullValueHandling = NullValueHandling.Ignore
																			 });
			return jsonData;
		}

		private static String ConvertNameValueCollectionToPostString(NameValueCollection data) {
			string value = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			string[] allKeys = data.AllKeys;
			for (int i = 0; i < allKeys.Length; i++) {
				string text2 = allKeys[i];
				stringBuilder.Append(value);
				stringBuilder.Append(HttpUtility.UrlEncode(text2));
				stringBuilder.Append("=");
				stringBuilder.Append(HttpUtility.UrlEncode(data[text2]));
				value = "&";
			}
			return stringBuilder.ToString();
		}

		private void SetupRequest(HttpWebRequest webRequest, string method) {
			webRequest.CookieContainer = CookieJar;
			webRequest.Method = method.ToUpper();
			webRequest.ServicePoint.Expect100Continue = false;
			webRequest.ContentType = "application/json; charset=utf-8";
			webRequest.Accept = "application/json";
			webRequest.UserAgent = "GoodData CSharp CL/1.0";
			webRequest.Headers.Add("Accept-Charset", "utf-8");
		}

		protected void CheckAuthentication() {
			if (CookieJar.Count != 0) return;
			Authenticate(Config.Login, Config.Password);
			GetToken();
		}
	}
}