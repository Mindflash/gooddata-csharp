using System.Collections.Generic;

namespace GoodData.API.Api.Models
{
	public class ProjectUserRequest
	{
		public UserRequest User { get; set; }

		#region Nested type: UserRequest

		public class UserRequest
		{
			public ContentRequest Content { get; set; }
			public LinksRequest Links { get; set; }

			#region Nested type: ContentRequest

			public class ContentRequest
			{
				public ContentRequest()
				{
					Status = "ENABLED";
				}

				public string Status { get; set; }
				public List<string> UserRoles { get; set; }
			}

			#endregion

			#region Nested type: LinksRequest

			public class LinksRequest
			{
				public string Self { get; set; }
			}

			#endregion
		}

		#endregion

	}


	public class ProjectUserRequestResponse {
		public ProjectUsersUpdateResult ProjectUsersUpdateResult { get; set; }
	}

	public class ProjectUsersUpdateResult {
		public List<ProjectUsersUpdateResultUserFailed> Failed { get; set; }
	}

	public class ProjectUsersUpdateResultUserFailed {
		public string User { get; set; }
		public string Message { get; set; }
	}
}