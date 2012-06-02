using BitbucketSharp.Models;

namespace BitbucketSharp.Controllers
{
    /// <summary>
    /// Provides access to a list of users
    /// </summary>
    public class UsersController : Controller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">A handle to the client</param>
        public UsersController(Client client)
            : base(client)
        {
        }

        /// <summary>
        /// Provides access to a specific user via a username
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <returns></returns>
        public UserController this[string username]
        {
            get { return new UserController(Client, username); }
        }

        /// <summary>
        /// The URI of this controller
        /// </summary>
        protected override string Uri
        {
            get { return "users/"; }
        }
    }

    /// <summary>
    /// Provides access to a user
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// The username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Groups that belong to this user
        /// </summary>
        public GroupsController Groups { get; private set; }

        /// <summary>
        /// Repositories that belong to this user
        /// </summary>
        public UserRepositoriesController Repositories { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserController(Client client, string username)
            : base(client)
        {
            Username = username;
            Groups = new GroupsController(client, this);
            Repositories = new UserRepositoriesController(client, this);
        }

        /// <summary>
        /// Gets information about this user
        /// </summary>
        /// <returns>A UsersModel</returns>
        public UsersModel GetInfo()
        {
            return Client.Get<UsersModel>(Uri);
        }

        /// <summary>
        /// Gets the events for a specific user
        /// </summary>
        /// <param name="start">The start index for returned items(default: 0)</param>
        /// <param name="limit">The limit index for returned items (default: 25)</param>
        /// <returns>A EventsModel</returns>
        public EventsModel GetEvents(int start = 0, int limit = 25)
        {
            return Client.Get<EventsModel>(Uri + "/events/?start=" + start + "&limit=" + limit);
        }

        /// <summary>
        /// The URI of this controller
        /// </summary>
        protected override string Uri
        {
            get { return "users/" + Username; }
        }
    }
}
