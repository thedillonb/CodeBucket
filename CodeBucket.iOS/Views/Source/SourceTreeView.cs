using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using System.Reactive.Linq;

namespace CodeBucket.Views.Source
{
    public class SourceTreeView : ViewModelCollectionDrivenDialogViewController
    {
        public new SourceTreeViewModel ViewModel
        {
            get { return (SourceTreeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var weakVm = new WeakReference<SourceTreeViewModel>(ViewModel);
            BindCollection(ViewModel.Content, x => CreateElement(x, weakVm));
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Title = string.IsNullOrEmpty(ViewModel.Path) ? ViewModel.Repository : ViewModel.Path.Substring(ViewModel.Path.LastIndexOf('/') + 1);
		}

        private static Element CreateElement(SourceTreeViewModel.SourceModel x, WeakReference<SourceTreeViewModel> viewModel)
        {
            if (x.Type.Equals("dir", StringComparison.OrdinalIgnoreCase))
            {
                var e = new StringElement(x.Name, AtlassianIcon.Devtoolsfolderclosed.ToImage());
                e.Clicked.Select(_ => x).BindCommand(viewModel.Get()?.GoToSourceCommand);
                return e;
            }
            if (x.Type.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                var e = new StringElement(x.Name, AtlassianIcon.Devtoolsfile.ToImage());
                e.Clicked.Select(_ => x).BindCommand(viewModel.Get()?.GoToSourceCommand);
                return e;
            }
            return new StringElement(x.Name) { Image = AtlassianIcon.Devtoolsfilebinary.ToImage() };
        }
    }
}

