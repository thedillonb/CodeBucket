using System;
using System.Threading.Tasks;
using System.Net;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface ILoadableViewModel
    {
        IReactiveCommand LoadCommand { get; }
    }

    public abstract class LoadableViewModel : BaseViewModel, ILoadableViewModel
    {
        public IReactiveCommand LoadCommand { get; }

        protected abstract Task Load();

        private async Task LoadResource()
        {
            var retry = false;
            while (true)
            {
                if (retry)
                    await Task.Delay(100);

                try
                {
                    await Load();
                    return;
                }
                catch (WebException)
                {
                    if (!retry)
                        retry = true;
                    else
                        throw;
                }
            }
        }

        protected LoadableViewModel()
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => LoadResource());
        }

    }
}

