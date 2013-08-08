using System;
using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Bitbucket.Controllers.Repositories;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers.Changesets
{
    public class ChangesetInfoController : BaseModelDrivenController
    {
        public string Node { get; private set; }
        
        public string User { get; private set; }
        
        public string Slug { get; private set; }
        
        public RepositoryDetailedModel Repo { get; set; }
        
        private readonly HeaderView _header;
        
        public ChangesetInfoController(string user, string slug, string node)
            : base(typeof(ChangesetModel), true, false)
        {
            Node = node;
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            Title = "Commit";
            Root.UnevenRows = true;
            
            _header = new HeaderView(0f) { Title = "Commit: " + node.Substring(0, node.Length > 10 ? 10 : node.Length) };
        }

        protected override void OnRender()
        {
            var model = (ChangesetModel)Model;
            var sec = new Section();
            _header.Subtitle = "Commited " + (model.Utctimestamp).ToDaysAgo();

            var d = new MultilinedElement(model.Author, model.Message);
            sec.Add(d);

            if (Repo != null)
            {
                var repo = new StyledStringElement(Repo.Name) { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    Font = StyledStringElement.DefaultDetailFont, 
                    TextColor = StyledStringElement.DefaultDetailColor,
                    Image = Images.Repo
                };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
                sec.Add(repo);
            }

            var sec2 = new Section();

            model.Files.ForEach(x => 
                                {
                var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
                var sse = new SubcaptionElement(file, x.Type)
                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                    Lines = 1 };
                sse.Tapped += () => {
                    string parent = null;
                    if (model.Parents != null && model.Parents.Count > 0)
                        parent = model.Parents[0];

                    var type = x.Type.Trim().ToLower();
                    NavigationController.PushViewController(new ChangesetDiffController(User, Slug, model.Node, parent, x.File)
                                                            { Removed = type.Equals("removed"), Added = type.Equals("added") }, true);
                };
                sec2.Add(sse);
            });

            _header.SetNeedsDisplay();
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };
            root.Add(new [] { new Section(_header), sec, sec2 });
            Root = root;
        }

        protected override object OnUpdateModel(bool forced)
        {
            var x = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetInfo(forced);
            x.Files = x.Files.OrderBy(y => y.File.Substring(y.File.LastIndexOf('/') + 1)).ToList();
            return x;
        }
    }
}

