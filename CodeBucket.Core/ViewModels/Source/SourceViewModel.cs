using System.Threading.Tasks;
using System;

namespace CodeBucket.Core.ViewModels.Source
{
	public class SourceViewModel : FileSourceViewModel
    {
		private string _user;
		private string _repository;
		private string _branch;
		private string _path;
		private string _name;

		protected override async Task Load(bool forceCacheInvalidation)
        {
			var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(_name));
            var source = this.GetApplication().Client.Users[_user].Repositories[_repository].Branches[_branch].Source;
			var mime = await Task.Run<string>(() =>
			{
				using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
                    return source.GetFileRaw(_path, stream);
				}
			});

			FilePath = filepath;
            HtmlUrl = "http://bitbucket.org/" + source.Branch.Branches.Repository.Owner.Username + "/" + source.Branch.Branches.Repository.Slug + "/src/" + source.Branch.UrlSafeName + "/" + _path;
			var isText = mime.Contains("text");
			if (isText)
			{
				ContentPath = CreateContentFile();
			}
        }

		public void Init(NavObject navObject)
		{
			_path = navObject.Path;
			_name = navObject.Name;
			_user = navObject.User;
			_repository = navObject.Repository;
			_branch = navObject.Branch;

			//Create the filename
			var fileName = System.IO.Path.GetFileName(_path);
			if (fileName == null)
				fileName = _path.Substring(_path.LastIndexOf('/') + 1);

			//Create the temp file path
			Title = fileName;
		}

		public class NavObject
		{
			public string Path { get; set; }
			public string Name { get; set; }
			public string User { get; set; }
			public string Repository { get; set; }
			public string Branch { get; set; }
		}
    }
}