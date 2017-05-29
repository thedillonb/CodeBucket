using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();
                                                                              
        public TeamItemViewModel(string name)
        {
            Name = name;
        }
    }
}

