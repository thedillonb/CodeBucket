using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamItemViewModel : ReactiveObject
    {
        public string Name { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();
                                                                              
        public TeamItemViewModel(string name)
        {
            Name = name;
        }
    }
}

