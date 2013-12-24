using System.Threading.Tasks;
using System;
using CodeFramework.Core.ViewModels;

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

			var mime = await Task.Run<string>(() =>
			{
				using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					return this.GetApplication().Client.Users[_user].Repositories[_repository].Branches[_branch].Source.GetFileRaw(_path, stream);

//
//					//There is a bug in the Bitbucket server that says everything returned is text. Content Type: text/plain
//					//Attempt to load this the normal way... If we fail then we'll fall back. If that fails then just display an error.
//					try 
//					{
//						//If this is successful there will be no exception. Just exit out!
//
//						//If the encoding is a base64 then assume it is a binary
//						if (d.Encoding != null && d.Encoding.Equals("base64"))
//						{
//							//Save the data to the disk
//							var decodedData = System.Convert.FromBase64String(d.Data);
//							stream.Write(decodedData, 0, decodedData.Length);
//						}
//						//If there is no encoding, or it's not base64 then don't worry about it. It's most likely text.
//						else
//						{
//								//var data = System.Security.SecurityElement.Escape(d.Data);
//								var bytes = System.Text.Encoding.UTF8.GetBytes(d.Data);
//							stream.Write(bytes, 0, bytes.Length);
//						}
//
//						//Nothing else to do!
//						return string.Empty;
//					}
//					catch (InternalServerException ex)
//					{
//						Console.WriteLine("Could not grab file the bitbucket way: " + ex.Message);
//					}
//
//					throw new Exception("shit!");
				}
			});

			FilePath = filepath;

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