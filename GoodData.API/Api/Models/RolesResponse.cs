using System.Collections.Generic;

namespace GoodData.API.Api.Models
{
	public class RolesResponse
	{
		public ProjectRoles ProjectRoles { get; set; }
	}

	public class ProjectRoles
	{
		public List<string> Roles { get; set; }
	}
}