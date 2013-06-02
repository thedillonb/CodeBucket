using System;
using CodeBucket.Controllers;
using CodeBucket.Views;
using GitHubSharp.Models;
using BitbucketBrowser.Controllers;
using MonoTouch.Dialog;
using BitbucketBrowser.Data;
using BitbucketBrowser.Elements;
using MonoTouch.UIKit;
using MonoTouch;

namespace BitbucketBrowser.GitHub.Controllers.Gists
{
    public class GistInfoController : Controller<GistModel>
    {
        public string Id { get; private set; }
        
        private readonly HeaderView _header;
        
        public GistInfoController(string id)
            : base(true, false)
        {
            Id = id;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            Title = "Gist";
            Root.UnevenRows = true;
            
            _header = new HeaderView(0f) { Title = "Gist: " + id };
            Root.Add(new Section(_header));
        }
        
        protected override void OnRefresh()
        {
            var sec = new Section();
            _header.Subtitle = "Updated " + Model.UpdatedAt.ToDaysAgo();

            var str = string.IsNullOrEmpty(Model.Description) ? "Gist " + Model.Id : Model.Description;
            var d = new NameTimeStringElement() { 
                Time = Model.UpdatedAt, 
                String = str, 
                Image = Images.Anonymous,
                BackgroundColor = UIColor.White,
                UseBackgroundColor = true,
            };

            //Sometimes there's no user!
            d.Name = (Model.User == null) ? "Anonymous" : Model.User.Login;
            d.ImageUri = (Model.User == null) ? null : new Uri(Model.User.AvatarUrl);

            sec.Add(d);
            
            var sec2 = new Section();

            foreach (var file in Model.Files.Keys)
            {
                var sse = new SubcaptionElement(file, Model.Files[file].Size + " bytes") { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                    Lines = 1 
                };

                var fileSaved = file;
                sse.Tapped += () => NavigationController.PushViewController(new GistFileController(Model.Files[fileSaved]), true);
                sec2.Add(sse);
            }

            InvokeOnMainThread(delegate {
                _header.SetNeedsDisplay();
                Root.Add(new [] { sec, sec2 });
                ReloadData();
            });
        }
        
        protected override GistModel OnUpdate(bool forced)
        {
            return Application.GitHubClient.API.GetGist(Id).Data;
        }
    }
}

