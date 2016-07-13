using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.DialogElements;
using CodeBucket.TableViewSources;
using CodeBucket.ViewControllers.Source;
using UIKit;

namespace CodeBucket.ViewControllers.Commits
{
    public class CommitFileChangesViewController : TableViewController
    {
        private readonly IList<CommitFileItemViewModel> _files;
        private readonly Lazy<RootElement> _rootElement;

        public CommitFileChangesViewController(IEnumerable<CommitFileItemViewModel> files)
            : base(UIKit.UITableViewStyle.Plain)
        {
            _files = files.ToList();
            _rootElement = new Lazy<RootElement>(() => new RootElement(TableView));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var root = _rootElement.Value;
            TableView.Source = new DialogTableViewSource(root);

            var sections = _files.GroupBy(x => x.Parent).Select(x =>
            {
                var elements = x.Select(y =>
                {
                    var element = new ButtonElement(y.Name, y.Type.ToString(), UITableViewCellStyle.Subtitle);
                    element.Image = AtlassianIcon.PageDefault.ToImage();
                    element.Clicked.Subscribe(_ =>
                    {
                        var viewCtrl = new ChangesetDiffViewController(y.Username, y.Repository, y.Node, y.ChangesetFile);
                        NavigationController.PushViewController(viewCtrl, true);
                    });
                    return element;
                });

                return new Section(x.Key) { elements };
            });

            root.Reset(sections);
        }
    }
}

