using System;

namespace GitHubSharp.Models
{
	public class ChangeStatus
	{
		public int Deletions { get; set; }
		public int Additions { get; set; }
		public int Total { get; set; }
	}
}

