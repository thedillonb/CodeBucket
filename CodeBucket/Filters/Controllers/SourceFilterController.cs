using System;
using CodeFramework.Filters.Controllers;
using MonoTouch.Dialog;
using CodeBucket.Filters.Models;
using MonoTouch.UIKit;
using CodeFramework.Filters.Models;

namespace CodeBucket.Filters.Controllers
{
    public class SourceFilterController : FilterController
    {
        private EnumChoiceElement _orderby;
        private TrueFalseElement _ascendingElement;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var currentModel = GetCurrentFilterModel<SourceFilterModel>();

            //Load the root
            var root = new RootElement(Title) {
                new Section("Order By") {
                    (_orderby = CreateEnumElement("Type", (int)currentModel.OrderBy, typeof(SourceFilterModel.Order))),
                    (_ascendingElement = new TrueFalseElement("Ascending", currentModel.Ascending)),
                },
                new Section() {
                    new StyledStringElement("Save as Default", SaveAsDefault, Images.Size) { Accessory = UITableViewCellAccessory.None },
                }
            };

            Root = root;
        }

        public override FilterModel CreateFilterModel()
        {
            var model = new SourceFilterModel();
            model.OrderBy = _orderby.Obj;
            model.Ascending = _ascendingElement.Value;
            return model;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.ReloadData();
        }
    }
}

