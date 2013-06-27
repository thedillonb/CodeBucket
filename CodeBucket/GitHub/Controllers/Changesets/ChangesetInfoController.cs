using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Controllers;
using CodeBucket.Views;

namespace CodeBucket.GitHub.Controllers.Changesets
{
    public class ChangesetInfoController : Controller<CommitModel>
    {
        public string Node { get; private set; }
        
        public string User { get; private set; }
        
        public string Slug { get; private set; }
        
        //public RepositoryDetailedModel Repo { get; set; }
        
        private readonly HeaderView _header;
        
        public ChangesetInfoController(string user, string slug, string node)
            : base(true, false)
        {
            Node = node;
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            Title = "Commit";
            Root.UnevenRows = true;
            
            _header = new HeaderView(0f) { Title = "Commit: " + node.Substring(0, node.Length > 10 ? 10 : node.Length) };
            Root.Add(new Section(_header));
        }
        
        protected override void OnRefresh()
        {
            var sec = new Section();
            _header.Subtitle = "Commited " + Model.Commit.Committer.Date.ToDaysAgo();
            
//            var d = new MultilinedElement(Model.Author, Model.Message);
//            sec.Add(d);
//            
//            if (Repo != null)
//            {
//                var repo = new StyledElement(Repo.Name, Images.Repo) { 
//                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
//                    Lines = 1, 
//                    Font = StyledElement.DefaultDetailFont, 
//                    TextColor = StyledElement.DefaultDetailColor,
//                };
//                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
//                sec.Add(repo);
//            }
//            
//            var sec2 = new Section();
//            
//            Model.Files.ForEach(x => 
//                                {
//                var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
//                var sse = new SubcaptionElement(file, x.Type)
//                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
//                    LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
//                    Lines = 1 };
//                sse.Tapped += () => {
//                    string parent = null;
//                    if (Model.Parents != null && Model.Parents.Count > 0)
//                        parent = Model.Parents[0];
//                    
//                    var type = x.Type.Trim().ToLower();
//                    NavigationController.PushViewController(new ChangesetDiffController(User, Slug, Model.Node, parent, x.File)
//                                                            { Removed = type.Equals("removed"), Added = type.Equals("added") }, true);
//                };
//                sec2.Add(sse);
//            });
//            
//            
//            InvokeOnMainThread(delegate {
//                _header.SetNeedsDisplay();
//                Root.Add(new [] { sec, sec2 });
//                ReloadData();
//            });
        }

        protected override CommitModel OnUpdate(bool forced)
        {
            var x = Application.GitHubClient.API.GetCommit(User, Slug, Node);
            //x.Files = x.Files.OrderBy(y => y.File.Substring(y.File.LastIndexOf('/') + 1)).ToList();
            return x.Data;
        }
    }
}

