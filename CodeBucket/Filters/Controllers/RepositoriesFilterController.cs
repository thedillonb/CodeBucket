using System;
using CodeFramework.Filters.Controllers;
using MonoTouch.Dialog;
using CodeFramework.Filters.Models;
using CodeBucket.Filters.Models;
using MonoTouch.UIKit;

namespace CodeBucket.Filters.Controllers
{
    public class RepositoriesFilterController : FilterController
    {
        private EnumChoiceElement _orderby;
        private TrueFalseElement _ascendingElement;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var currentModel = GetCurrentFilterModel<RepositoriesFilterModel>();

            //Load the root
            var root = new RootElement(Title) {
                new Section("Order By") {
                    (_orderby = CreateEnumElement("Field", (int)currentModel.OrderBy, typeof(RepositoriesFilterModel.Order))),
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
            var model = new RepositoriesFilterModel();
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

