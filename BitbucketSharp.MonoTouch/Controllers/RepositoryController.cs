using System.Collections.Generic;
using BitbucketSharp.Models;

namespace BitbucketSharp.Controllers
{
    /// <summary>
    /// Provides access to repositories owned by a user
    /// </summary>
    public class UserRepositoriesController : Controller
    {
        /// <summary>
        /// Gets the owner of the repositories
        /// </summary>
        public UserController Owner { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">A handle to the client</param>
        /// <param name="owner">The owner of the repositories</param>
        public UserRepositoriesController(Client client, UserController owner) : base(client)
        {
            Owner = owner;
        }

        /// <summary>
        /// Access a specific repository via the slug
        /// </summary>
        /// <param name="slug">The repository slug</param>
        /// <returns></returns>
        public RepositoryController this[string slug]
        {
            get { return new RepositoryController(Client, Owner, slug); } 
        }

        /// <summary>
        /// The URI of this controller
        /// </summary>
        protected override string Uri
        {
            get { return "repositories"; }
        }
    }

    /// <summary>
    /// Provides access to 'global' repositories via a search method
    /// </summary>
    public class RepositoriesController : Controller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">A handle to the client</param>
        public RepositoriesController(Client client) : base(client)
        {
        }

        /// <summary>
        /// Search for a specific repository via the name
        /// </summary>
        /// <param name="name">The partial or full name to search for</param>
        /// <returns>A list of RepositorySimpleModel</returns>
        public IList<RepositorySimpleModel> Search(string name)
        {
            return Client.Get<List<RepositorySimpleModel>>(Uri + "/?name=" + name);
        }

        /// <summary>
        /// The URI of this controller
        /// </summary>
        protected override string Uri
        {
            get { return "repositories"; }
        }
    }

    /// <summary>
    /// Provides access to a repository
    /// </summary>
    public class RepositoryController : Controller
    {
        /// <summary>
        /// Gets a handle to the issue controller
        /// </summary>
        public IssuesController Issues { get; private set; }

        /// <summary>
        /// Gets the owner of the repository
        /// </summary>
        public UserController Owner { get; private set; }

        /// <summary>
        /// Gets the slug of the repository
        /// </summary>
        public string Slug { get; private set; }

        /// <summary>
        /// Gets the wikis of this repository
        /// </summary>
        public WikisController Wikis { get; private set; }

        /// <summary>
        /// Gets the invitations to this repository
        /// </summary>
        public InvitationController Invitations { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">The owner of this repository</param>
        /// <param name="slug">The slug of this repository</param>
        /// <param name="client">A handle to the client</param>
        public RepositoryController(Client client, UserController owner, string slug) 
            : base(client)
        {
            Owner = owner;
            Slug = slug;
            Issues = new IssuesController(client, this);
            Wikis = new WikisController(client, this);
            Invitations = new InvitationController(client, this);
        }

        /// <summary>
        /// Requests the information on a specific repository
        /// </summary>
        /// <returns>A RepositoryDetailedModel</returns>
        public RepositoryDetailedModel GetInfo()
        {
            return Client.Get<RepositoryDetailedModel>(Uri);
        }

        /// <summary>
        /// Requests the followers of a specific repository
        /// </summary>
        /// <returns>A FollowersModel</returns>
        public FollowersModel GetFollowers()
        {
            return Client.Get<FollowersModel>(Uri + "/followers");
        }

        /// <summary>
        /// Requests the events of a repository
        /// </summary>
        /// <param name="start">The start index of returned items (default: 0)</param>
        /// <param name="limit">The limit of returned items (default: 25)</param>
        /// <param name="type">The type of event to return. If null, all event types are returned</param>
        /// <returns>A EventsModel</returns>
        public EventsModel GetEvents(int start = 0, int limit = 25, string type = null)
        {
            return Client.Get<EventsModel>(Uri + "/events/?start=" + start + "&limit=" +
                                           limit + (type == null ? "" : "&type=" + type));
        }

        /// <summary>
        /// The URI of this controller
        /// </summary>
        protected override string Uri
        {
            get { return "repositories/" + Owner.Username + "/" + Slug; }
        }
    }
}
