using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeFramework.iOS.Utils;

namespace CodeBucket.iOS.Views.Issues
{
	public class IssueEditView : IssueModifyView
    {
		private IHud _hud;

		public override void ViewDidLoad()
		{
			Title = "Edit Issue";

			base.ViewDidLoad();
			var vm = (IssueEditViewModel)ViewModel;

			var state = new TrueFalseElement("Open", true);
			state.ValueChanged += (sender, e) => vm.IsOpen = state.Value;

			vm.Bind(x => x.IsOpen, x =>
			{
				state.Value = x;
					if (state.GetImmediateRootElement() != null)
					Root.Reload(state, UITableViewRowAnimation.None);
			}, true);

			Root.Insert(Root.Count - 1, UITableViewRowAnimation.None, new Section { state });
		}
    }
}

