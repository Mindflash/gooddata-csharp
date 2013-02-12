using System.Configuration;
using System.Diagnostics;
using GoodData.API.Api;
using GoodData.API.Api.Models;
using GoodData.API;
using System;

namespace GoodDataTests.Api
{
	public class BaseTest
	{
		public ApiWrapper ReportingService { get; private set; }
		public BaseTest()
		{
			throw new Exception("Please fix configuraiton in GoodDataTests.Api.BaseTest constructor.");

			ReportingService = new ApiWrapper(new GoodDataConfig() {
				 Domain = "yourdomain",
				 IgnoreSslErrors = false,
				 Login = "yourlogin",
				 Password = "yourpassword",
				 ServiceUrl = "https://secure.gooddata.com"
			});
		}

		public string TestProjectName
		{
			get { return ConfigurationManager.AppSettings["TestProject"]; }
		}

		[DebuggerStepThrough]
		public Project GetTestProject()
		{
			return ReportingService.FindProjectByTitle(TestProjectName);
		}

		[DebuggerStepThrough]
		public string GetTestProjectId()
		{
			var project = GetTestProject();
			return project.ProjectId;
		}

		public string CreateTestUser(string email)
		{
			var password = "password";
			var firstName = "sso";
			var lastName = "admin";

			return ReportingService.CreateUser(email, password, password, firstName, lastName, ReportingService.Config.Domain + ".com");
		}
	}
}