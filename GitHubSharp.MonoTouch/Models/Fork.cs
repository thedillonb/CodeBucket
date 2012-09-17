using System;

namespace GitHubSharp.Models
{
	public class Fork
	{
		public BasicUser User {get;set;}
	    public string Url { get; set; }
	    public DateTime CreatedAt { get; set; }
	}
}

