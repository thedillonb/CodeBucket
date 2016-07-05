using System;
using CodeBucket.DialogElements;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssueAttributesViewController : TableViewController
    {
        private readonly ISubject<string> _selectedSubject = new Subject<string>();
        private readonly Lazy<RootElement> _root;
        private readonly string[] _values;
        private readonly string _selected;

        public static readonly string[] Priorities = { "Trivial", "Minor", "Major", "Critical", "Blocker" };
        public static readonly string[] Statuses = { "New", "Open", "Resolved", "On Hold", "Invalid", "Duplicate", "Wontfix" };
        public static readonly string[] Kinds = { "Bug", "Enhancement", "Proposal", "Task" };

        public IObservable<string> SelectedObservable => _selectedSubject.AsObservable();

		public IssueAttributesViewController(string[] values, string selected)
            : base(UIKit.UITableViewStyle.Plain)
		{
			_values = values;
			_selected = selected;
            _root = new Lazy<RootElement>(() => new RootElement(TableView));
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var sec = new Section();
			foreach (var val in _values)
			{
				var capture = val;
                var el = new CheckElement(val);
                el.Checked = string.Equals(val, _selected, StringComparison.OrdinalIgnoreCase);
                el.CheckedChanged.Select(_ => capture).Subscribe(_selectedSubject);
				sec.Add(el);
			}

            TableView.Source = new DialogTableViewSource(_root.Value);
            _root.Value.Reset(sec);
		}

    }
}

