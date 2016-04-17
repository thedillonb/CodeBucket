using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using CodeBucket.Core.Services;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels
{
    /// <summary>
    ///    Defines the BaseViewModel type.
    /// </summary>
    public abstract class BaseViewModel : MvxViewModel
    {
        /// <summary>
        /// Gets the go to URL command.
        /// </summary>
        /// <value>The go to URL command.</value>
        public ICommand GoToUrlCommand
        {
            get { return new MvxCommand<string>(x => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = x })); }
        }

        /// <summary>
        /// Gets the ViewModelTxService
        /// </summary>
        /// <value>The tx sevice.</value>
        protected IViewModelTxService TxSevice
        {
            get { return GetService<IViewModelTxService>(); }
        }

        /// <summary>
        /// Gets the messenger service
        /// </summary>
        /// <value>The messenger.</value>
        protected IMvxMessenger Messenger
        {
            get { return GetService<IMvxMessenger>(); }
        }

        /// <summary>
        /// Gets the alert service
        /// </summary>
        /// <value>The alert service.</value>
        protected IAlertDialogService AlertService
        {
            get { return GetService<IAlertDialogService>(); }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service.</returns>
        protected TService GetService<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        /// <summary>
        /// Display an error message to the user
        /// </summary>
        /// <param name="message">Message.</param>
        protected Task DisplayAlert(string message)
        {
            return AlertService.Alert("Error!", message);
        }
    }
}
