using CodeBucket.Core.ViewModels.App;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core
{
    /// <summary>
    /// Define the App type.
    /// </summary>
    public class App : MvxApplication
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
			//Ensure this is loaded
            MvvmCross.Plugins.Messenger.PluginLoader.Instance.EnsureLoaded();
        }
    }
}