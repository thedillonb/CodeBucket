using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.DialogElements;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Commits
{
    public class CommitFileChangesViewController : DialogViewController
    {
        private readonly IList<CommitFileViewModel> _files;
        private readonly bool _showDetails;

        public CommitFileChangesViewController(IEnumerable<CommitFileViewModel> files)
            : base(UIKit.UITableViewStyle.Plain)
        {
            _files = files.ToList();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sections = 
                _files
                .GroupBy(x => x.Parent)
                .Select(x =>
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

            Root.Reset(sections);
        }
    }
}

