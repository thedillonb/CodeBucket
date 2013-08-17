using System;
using CodeFramework.Controllers;
using CodeBucket.Filters.Models;
using MonoTouch.Dialog;
using CodeFramework.Filters.Controllers;
using CodeFramework.Filters.Models;
using MonoTouch.UIKit;

namespace CodeBucket.Filters.Controllers
{
    public class IssuesFilterController : FilterController
    {
        private EntryElement _assignedTo;
        private EntryElement _reportedBy;
        private MultipleChoiceElement<IssuesFilterModel.StatusModel> _statusChoice;
        private MultipleChoiceElement<IssuesFilterModel.KindModel> _kindChoice;
        private MultipleChoiceElement<IssuesFilterModel.PriorityModel> _priorityChoice;
        private EnumChoiceElement _orderby;

        public override FilterModel CreateFilterModel()
        {
            var model = new IssuesFilterModel();
            model.AssignedTo = _assignedTo.Value;
            model.ReportedBy = _reportedBy.Value;
            model.Status = _statusChoice.Obj;
            model.Priority = _priorityChoice.Obj;
            model.Kind = _kindChoice.Obj;
            model.OrderBy = _orderby.Obj;
            return model;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var issuesFilterModel = GetCurrentFilterModel<IssuesFilterModel>();

            //Load the root
            var root = new RootElement(Title) {
                new Section("Filter") {
                    (_assignedTo = new InputElement("Assigned To", "Anybody", issuesFilterModel.AssignedTo) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
                    (_reportedBy = new InputElement("Reported By", "Anybody", issuesFilterModel.ReportedBy) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
                    (_kindChoice = CreateMultipleChoiceElement("Kind", issuesFilterModel.Kind)),
                    (_statusChoice = CreateMultipleChoiceElement("Status", issuesFilterModel.Status)),
                    (_priorityChoice = CreateMultipleChoiceElement("Priority", issuesFilterModel.Priority)),
                },
                new Section("Order By") {
                    (_orderby = CreateEnumElement("Field", (int)issuesFilterModel.OrderBy, typeof(IssuesFilterModel.Order))),
                }
            };

            Root = root;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.ReloadData();
        }
    }
}

