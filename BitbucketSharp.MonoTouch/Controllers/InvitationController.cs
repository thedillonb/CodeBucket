using System.Collections.Generic;
using BitbucketSharp.Models;

namespace BitbucketSharp.Controllers
{
    /// <summary>
    /// Provides access to the invitations for a repository
    /// </summary>
    public class InvitationController : Controller
    {
        /// <summary>
        /// Gets the repositories these inviations belong to
        /// </summary>
        public RepositoryController Repository { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">A handle to the client</param>
        /// <param name="repository">The repositories these invitations belong to</param>
        public InvitationController(Client client, RepositoryController repository) 
            : base(client)
        {
            Repository = repository;
        }

        /// <summary>
        /// Sents an invitation to a user with a specific permission
        /// </summary>
        /// <param name="user">The user to send the invitation to. Username or email</param>
        /// <param name="permission">The permission: read, write, admin</param>
        /// <returns></returns>
        public InvitationModel SendInvitation(string user, string permission)
        {
            return Client.Post<InvitationModel>(Uri + "/" + user,
                                                new Dictionary<string, string> {{"permission", permission}});
        }

        /// <summary>
        /// The URI of this controller
        /// </summary>
        protected override string Uri
        {
            get { return "invitations/" + Repository.Owner.Username + "/" + Repository.Slug; }
        }
    }
}
