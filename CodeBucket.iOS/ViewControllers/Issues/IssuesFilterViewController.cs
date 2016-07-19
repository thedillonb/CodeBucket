using UIKit;
using CodeBucket.Core.Filters;
using System;
using CodeBucket.DialogElements;
using System.Collections.Generic;

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

        public IssuesFilterViewController(IssuesFilterModel currentFilter, Action<IssuesFilterModel> searchAction)
        {
            _currentFilter = currentFilter.Clone();
            _searchAction = searchAction;
            Title = "Filter & Sort";
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
			return model;
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
				},
				new Section("Order By") {
                    (_orderby = CreateEnumElement("Field", _currentFilter.OrderBy)),
				}
			};

            Root.Reset(root);
        }
    }
}

