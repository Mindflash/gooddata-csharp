﻿using Newtonsoft.Json;

namespace GoodData.API.Api.Models
{
	public class RoleResponse
	{
		public ProjectRole ProjectRole { get; set; }
	}

	public class ProjectRole
	{
		[JsonIgnore]
		public string RoleId { get; set; }
		public ObjectMeta Meta { get; set; }
	}
}