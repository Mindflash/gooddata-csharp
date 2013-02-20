﻿using System;
namespace GoodData.API.Api {
	public interface IApiWrapper {
		void AddUserToProjectWithRoleByTitle(string projectId, string userId, string roleName = SystemRoles.DashboardOnly);
		void AddUserToProjectWithRoleByUri(string projectId, string userId, string roleUri);
		GoodData.API.Api.Models.AssignUserFiltersUpdateResult AssignUserFilters(string projectId, System.Collections.Generic.List<string> userprofileIds, System.Collections.Generic.List<string> userFilterUris);
		string CreateProject(string title, string summary, string template = null, string driver = SystemPlatforms.PostGres);
		string CreateUser(string login, string password, string verfiyPassword, string firstName, string lastName, string ssoProvider = null, string country = "US");
		string CreateUserFilterUsingAttributeTitles(string projectId, string filterTitle, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> attributeTitlesWithElementTitles, bool inclusive = true);
		string CreateUserFilterUsingAttributeUris(string projectId, string filterTitle, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> attributeUrisWithElementTitles, bool inclusive = true);
		void DeleteObject(string projectId, string relativeUri);
		void DeleteObjectByTitle(string projectId, string title, GoodData.API.ObjectTypes objectType);
		void DeleteProject(string projectId);
		void DeleteUser(string profileId);
		void DeleteUserFilterByTitle(string projectId, string filterTitle);
		string ExecuteReport(string reportUri, GoodData.API.ExportFormatTypes exportFormatType = ExportFormatTypes.csv);
		GoodData.API.Api.Models.ExportArtifact ExportPartials(string projectId, System.Collections.Generic.List<string> uris);
		GoodData.API.Api.Models.ExportArtifact ExportProject(string projectId, bool exportUsers = false, bool exportData = false);
		string ExportReport(string reportUri, GoodData.API.ExportFormatTypes exportFormatType = ExportFormatTypes.csv);
		GoodData.API.Api.Models.Attribute FindAttributeByTitle(string projectId, string attributeTitle, System.Collections.Generic.List<GoodData.API.Api.Models.Entry> attributes = null);
		GoodData.API.Api.Models.Element FindAttributeElementByTitle(string projectId, GoodData.API.Api.Models.Attribute attribute, string elementTitle);
		GoodData.API.Api.Models.AccountResponseSetting FindDomainUsersByLogin(string email, string domain = "");
		System.Collections.Generic.List<GoodData.API.Api.Models.Entry> FindObjectByTitle(string projectId, string title, GoodData.API.ObjectTypes objectType);
		GoodData.API.Api.Models.Project FindProjectByTitle(string title);
		GoodData.API.Api.Models.User FindProjectUsersByEmail(string projectId, string email);
		GoodData.API.Api.Models.ProjectRole FindRoleByTitle(string projectId, string systemRole = SystemRoles.DashboardOnly);
		System.Collections.Generic.List<GoodData.API.Api.Models.ObjectMeta> GetActiveObjects(string projectId, GoodData.API.ObjectTypes objectTypes);
		GoodData.API.Api.Models.Attribute GetAttributeByUri(string attributeUri);
		System.Collections.Generic.List<GoodData.API.Api.Models.Element> GetAttributeElements(string projectId, GoodData.API.Api.Models.Attribute attribute);
		GoodData.API.Api.Models.UsingResponse GetDependancies(string projectId, string objectId, bool? filterByReport = null);
		System.Collections.Generic.List<GoodData.API.Api.Models.AccountResponseSettingWrapper> GetDomainUsers(string domain = "");
		System.Net.WebResponse GetFile(string uri);
		byte[] GetFileContents(System.Net.WebResponse response);
		System.Collections.Generic.List<GoodData.API.Api.Models.User> GetFullProjectUsers(string projectId);
		GoodData.API.Api.Models.User GetFullProjectUsersByEmail(string projectId, string email);
		dynamic GetObject(string objectLink);
		string GetObjectIdentifier(string objectLink);
		System.Collections.Generic.List<GoodData.API.Api.Models.ObjectMeta> GetObjectMetaData(string projectId, GoodData.API.ObjectTypes objectTypes);
		System.Collections.Generic.List<GoodData.API.Api.Models.Project> GetProjects();
		GoodData.API.Api.Models.ProjectUserFilters GetProjectUserFilters(string projectId, int count = 1000, int offset = 0);
		System.Collections.Generic.List<GoodData.API.Api.Models.UserWrapper> GetProjectUsers(string projectId);
		System.Collections.Generic.List<string> GetQueryLinks(string projectId, GoodData.API.ObjectTypes objectTypes);
		System.Collections.Generic.List<GoodData.API.Api.Models.ProjectRole> GetRoles(string projectId);
		GoodData.API.Api.Models.IdentifiersResponse GetUris(string projectId, System.Collections.Generic.List<string> identifiers);
		string ImportPartials(string projectId, string token, bool overwriteNewer = true, bool updateLdmObjects = false);
		string ImportProject(string projectId, string token);
		bool PollPartialStatus(string uri);
		bool PollStatus(string uri);
		System.Collections.Generic.List<GoodData.API.Api.Models.Entry> Query(string projectId, GoodData.API.ObjectTypes objectTypes);
		void UpdateProfileSettings(string projectId, string profileId);
		void UpdateProjectUserAccess(string projectId, string profileId, bool enabled, string roleName = SystemRoles.DashboardOnly);
		void UpdateSSOProvider(string profileId);
	}
}