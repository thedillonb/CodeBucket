using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.DialogElements;
using CodeBucket.TableViewSources;
using ReactiveUI;

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
                    var element = new StringElement(y.Name, y.Type.ToString(), UIKit.UITableViewCellStyle.Subtitle);
                    element.Image = AtlassianIcon.PageDefault.ToImage();
                    element.Clicked.InvokeCommand(y.GoToCommand);
                    return element;
                });

                return new Section(x.Key) { elements };
            });

            root.Reset(sections);
        }
    }
}

