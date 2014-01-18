using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Filters;

namespace CodeBucket.iOS.Views.Filters
{
    public class IssuesFilterViewController : FilterViewController
    {
        private readonly IFilterableViewModel<IssuesFilterModel> _filterController;
		private EntryElement _assignedTo;
		private EntryElement _reportedBy;
		private MultipleChoiceElement<IssuesFilterModel.StatusModel> _statusChoice;
		private MultipleChoiceElement<IssuesFilterModel.KindModel> _kindChoice;
		private MultipleChoiceElement<IssuesFilterModel.PriorityModel> _priorityChoice;
		private EnumChoiceElement<IssuesFilterModel.Order> _orderby;

        public IssuesFilterViewController(IFilterableViewModel<IssuesFilterModel> filterController)
        {
            _filterController = filterController;
        }

        public override void ApplyButtonPressed()
        {
            _filterController.ApplyFilter(CreateFilterModel());
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
            var model = _filterController.Filter.Clone();

			//Load the root
			var root = new RootElement(Title) {
				new Section("Filter") {
					(_assignedTo = new InputElement("Assigned To", "Anybody", model.AssignedTo) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
					(_reportedBy = new InputElement("Reported By", "Anybody", model.ReportedBy) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
					(_kindChoice = CreateMultipleChoiceElement("Kind", model.Kind)),
					(_statusChoice = CreateMultipleChoiceElement("Status", model.Status)),
					(_priorityChoice = CreateMultipleChoiceElement("Priority", model.Priority)),
				},
				new Section("Order By") {
					(_orderby = CreateEnumElement<IssuesFilterModel.Order>("Field", model.OrderBy)),
				},
				new Section(string.Empty, "Saving this filter as a default will save it only for this repository.") {
					new StyledStringElement("Save as Default", () =>{
						_filterController.ApplyFilter(CreateFilterModel(), true);
						CloseViewController();
					}, Images.Size) { Accessory = UITableViewCellAccessory.None },
				}
			};

			Root = root;
        }
    }
}

