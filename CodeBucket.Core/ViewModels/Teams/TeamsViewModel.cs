using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamsViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<string> _teams = new CollectionViewModel<string>();

		public CollectionViewModel<string> Teams
        {
            get { return _teams; }
        }

        public ICommand GoToTeamCommand
        {
            get { return new MvxCommand<string>(x => ShowViewModel<TeamViewModel>(new TeamViewModel.NavObject { Name = x })); }
        }

        protected override async Task Load()
        {
            Teams.Items.Clear();
            await this.GetApplication().Client.ForAllItems(
                x => x.Teams.GetAll(), 
                x => Teams.Items.AddRange(x.Select(y => y.Username)));
        }
    }
}