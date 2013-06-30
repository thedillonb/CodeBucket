using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Controllers;
using CodeBucket.Views;
using System.Linq;
using CodeBucket.Elements;
using CodeBucket.GitHub.Controllers.Repositories;

namespace CodeBucket.GitHub.Controllers.Changesets
{
    public class ChangesetInfoController : Controller<CommitModel>
    {
        public string Node { get; private set; }
        public string User { get; private set; }
        public string Slug { get; private set; }
        public bool ShowRepo { get; set; }        
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

            var d = new MultilinedElement(Model.Author.Login, Model.Commit.Message);
            sec.Add(d);

            if (ShowRepo)
            {
                var repo = new StyledElement(Slug, Images.Repo) { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    Font = StyledElement.DefaultDetailFont, 
                    TextColor = StyledElement.DefaultDetailColor,
                };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(User, Slug), true);
                sec.Add(repo);
            }

            var sec2 = new Section();

            Model.Files.ForEach(x => 
                                {
                var file = x.Filename.Substring(x.Filename.LastIndexOf('/') + 1);
                var sse = new SubcaptionElement(file, x.Status)
                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                    Lines = 1 };
                sse.Tapped += () => {
                    string parent = null;
                    if (Model.Parents != null && Model.Parents.Count > 0)
                        parent = Model.Parents[0].Sha;

                    var type = x.Status.Trim().ToLower();
                    NavigationController.PushViewController(new ChangesetDiffController(User, Slug, Node, parent, x.Filename)
                                                            { Removed = type.Equals("removed"), Added = type.Equals("added") }, true);
                };
                sec2.Add(sse);
            });


            InvokeOnMainThread(delegate {
                _header.SetNeedsDisplay();
                Root.Add(new [] { sec, sec2 });
                ReloadData();
            });
        }

        protected override CommitModel OnUpdate(bool forced)
        {
            var x = Application.GitHubClient.API.GetCommit(User, Slug, Node).Data;
            x.Files = x.Files.OrderBy(y => y.Filename.Substring(y.Filename.LastIndexOf('/') + 1)).ToList();
            return x;
        }
    }
}

