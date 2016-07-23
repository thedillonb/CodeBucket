using UIKit;
using CodeBucket.Core.Filters;
using System;
using CodeBucket.DialogElements;
using System.Collections.Generic;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssuesFilterViewController : FilterViewController
    {
        private readonly IssuesFilterModel _currentFilter;
        private readonly Action<IssuesFilterModel> _searchAction;
		private EntryElement _assignedTo;
		private EntryElement _reportedBy;
		private MultipleChoiceElement<IssuesFilterModel.StatusModel> _statusChoice;
		private MultipleChoiceElement<IssuesFilterModel.KindModel> _kindChoice;
		private MultipleChoiceElement<IssuesFilterModel.PriorityModel> _priorityChoice;
		private EnumChoiceElement<IssuesFilterModel.Order> _orderby;
        private ButtonElement _milestoneElement = new ButtonElement("Milestone", string.Empty, UITableViewCellStyle.Value1);
        private ButtonElement _verisonElement = new ButtonElement("Version", string.Empty, UITableViewCellStyle.Value1);
        private ButtonElement _componentElement = new ButtonElement("Component", string.Empty, UITableViewCellStyle.Value1);

        public IssuesFilterViewController(
            string username, string repository,
            IssuesFilterModel currentFilter, Action<IssuesFilterModel> searchAction)
        {
            _currentFilter = currentFilter.Clone();
            _searchAction = searchAction;

            OnActivation(disposable =>
            {
                _milestoneElement
                    .Clicked
                    .Select(_ =>
                    {
                        var vc = new IssueMilestonesViewController(username, repository);
                        vc.ViewModel.SelectedValue = _currentFilter.Milestone;
                        vc.ViewModel.WhenAnyValue(x => x.SelectedValue)
                          .Subscribe(x => _currentFilter.Milestone = x);
                        return vc;
                    })
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(disposable);

                _verisonElement
                    .Clicked
                    .Select(_ =>
                    {
                        var vc = new IssueVersionsViewController(username, repository);
                        vc.ViewModel.SelectedValue = _currentFilter.Version;
                        vc.ViewModel.WhenAnyValue(x => x.SelectedValue)
                          .Subscribe(x => _currentFilter.Version = x);
                        return vc;
                    })
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(disposable);

                _componentElement
                    .Clicked
                    .Select(_ =>
                    {
                    var vc = new IssueComponentsViewController(username, repository);
                        vc.ViewModel.SelectedValue = _currentFilter.Component;
                        vc.ViewModel.WhenAnyValue(x => x.SelectedValue)
                          .Subscribe(x => _currentFilter.Component = x);
                        return vc;
                    })
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(disposable);
            });
        }

        public override void ApplyButtonPressed()
        {
            _searchAction(CreateFilterModel());
        }

        private IssuesFilterModel CreateFilterModel()
        {
			var model = new IssuesFilterModel();
			model.AssignedTo = _assignedTo.Value;
			model.ReportedBy = _reportedBy.Value;
			model.Status = _statusChoice.Obj;
			model.Priority = _priorityChoice.Obj;
			model.Kind = _kindChoice.Obj;
			model.OrderBy = _orderby.Value;
            model.Milestone = _currentFilter.Milestone;
            model.Version = _currentFilter.Version;
            model.Component = _currentFilter.Component;
			return model;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _milestoneElement.Value = _currentFilter.Milestone ?? "Any";
            _verisonElement.Value = _currentFilter.Version ?? "Any";
            _componentElement.Value = _currentFilter.Component ?? "Any";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			//Load the root
            var root = new List<Section> {
				new Section("Filter") {
                    (_assignedTo = new EntryElement("Assigned To", "Anybody", _currentFilter.AssignedTo) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
                    (_reportedBy = new EntryElement("Reported By", "Anybody", _currentFilter.ReportedBy) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
                    (_kindChoice = CreateMultipleChoiceElement("Kind", _currentFilter.Kind)),
                    (_statusChoice = CreateMultipleChoiceElement("Status", _currentFilter.Status)),
                    (_priorityChoice = CreateMultipleChoiceElement("Priority", _currentFilter.Priority)),
                    _milestoneElement,
                    _verisonElement,
                    _componentElement
				},
				new Section("Order By") {
                    (_orderby = CreateEnumElement("Field", _currentFilter.OrderBy)),
				}
			};

            Root.Reset(root);
        }
    }
}

