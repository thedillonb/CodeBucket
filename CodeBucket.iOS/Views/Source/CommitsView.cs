using System;
using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels;
using MonoTouch.Dialog;

namespace CodeBucket.iOS.Views.Source
{
	public abstract class CommitsView : ViewModelCollectionDrivenDialogViewController
	{
		public override void ViewDidLoad()
		{
			Title = "Commits".t();
			Root.UnevenRows = true;

			base.ViewDidLoad();

			var vm = (CommitsViewModel) ViewModel;
			BindCollection(vm.Commits, x =>
				{
					var msg = x.Message ?? string.Empty;
					var firstLine = msg.IndexOf("\n", StringComparison.Ordinal);
					var desc = firstLine > 0 ? msg.Substring(0, firstLine) : msg;
					var el = new NameTimeStringElement { Name = x.Author, Time = x.Utctimestamp.ToDaysAgo(), String = desc, Lines = 4 };
					el.Tapped += () => vm.GoToChangesetCommand.Execute(x);
					return el;
				});
		}
	}
}

