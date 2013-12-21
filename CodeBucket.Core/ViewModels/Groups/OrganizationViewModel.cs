//using System.Threading.Tasks;
//using System.Windows.Input;
//using Cirrious.MvvmCross.ViewModels;
//using CodeBucket.Core.ViewModels.Events;
//using CodeBucket.Core.ViewModels.Gists;
//using CodeBucket.Core.ViewModels.Repositories;
//using CodeBucket.Core.ViewModels.User;
//using GitHubSharp.Models;
//using CodeFramework.Core.ViewModels;
//
//namespace CodeBucket.Core.ViewModels.Organizations
//{
//    public class OrganizationViewModel : LoadableViewModel
//	{
//        private UserModel _userModel;
//
//        public string Name 
//        { 
//            get; 
//            private set; 
//        }
//
//		public void Init(NavObject navObject) 
//		{
//			Name = navObject.Name;
//		}
//
//        public UserModel Organization
//        {
//            get { return _userModel; }
//            private set
//            {
//                _userModel = value;
//                RaisePropertyChanged(() => Organization);
//            }
//        }
//
//        public ICommand GoToMembersCommand
//        {
//            get { return new MvxCommand(() => ShowViewModel<GroupMembersViewModel>(new GroupMembersViewModel.NavObject { GroupName = Name }));}
//        }
//
//        public ICommand GoToTeamsCommand
//        {
//            get { return new MvxCommand(() => ShowViewModel<TeamsViewModel>(new TeamsViewModel.NavObject { Name = Name })); }
//        }
//
//        public ICommand GoToFollowersCommand
//        {
//            get { return new MvxCommand(() => ShowViewModel<UserFollowersViewModel>(new UserFollowersViewModel.NavObject { Username = Name })); }
//        }
//
//        public ICommand GoToEventsCommand
//        {
//            get { return new MvxCommand(() => ShowViewModel<UserEventsViewModel>(new UserEventsViewModel.NavObject { Username = Name })); }
//        }
//
//        public ICommand GoToGistsCommand
//        {
//            get { return new MvxCommand(() => ShowViewModel<UserGistsViewModel>(new UserGistsViewModel.NavObject { Username = Name })); }
//        }
//
//        public ICommand GoToRepositoriesCommand
//        {
//            get { return new MvxCommand(() => ShowViewModel<OrganizationRepositoriesViewModel>(new OrganizationRepositoriesViewModel.NavObject { Name = Name })); }
//        }
//
//        protected override Task Load(bool forceCacheInvalidation)
//        {
//			return this.RequestModel(this.GetApplication().Client.Organizations[Name].Get(), forceCacheInvalidation, response => Organization = response.Data);
//        }
//
//        public class NavObject
//        {
//            public string Name { get; set; }
//        }
//	}
//}
//
