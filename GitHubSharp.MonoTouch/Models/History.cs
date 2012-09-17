using System;

namespace GitHubSharp.Models
{
	public class History
	{
		public string Url { get; set; }
	    public string Version { get; set; }
	    public BasicUser User { get; set; }
	    public ChangeStatus ChangeStatus { get; set; }
	    public DateTime CommittedAt { get; set; }

	}
}

