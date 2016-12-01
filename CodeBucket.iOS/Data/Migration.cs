using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akavache;
using CodeBucket.Core.Services;
using Splat;
using SQLite;

namespace CodeBucket.Data
{
    public static class Migration
    {
        public static void Migrate()
        {
            var application = Locator.Current.GetService<IApplicationService>();
            var accounts = Locator.Current.GetService<IAccountsService>();
            var defaultValue = Locator.Current.GetService<IDefaultValueService>();
            var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..");
            var accountsDir = Path.Combine(baseDir, "Documents/accounts");
            var accountsDbPath = Path.Combine(accountsDir, "accounts.db");

            if (!File.Exists(accountsDbPath))
            {
                try
                {
                    var accs = BlobCache.LocalMachine.GetAllObjects<Core.Data.Account>().ToTask().Result.ToList();
                    foreach (var a in accs)
                    {
                        accounts.Save(a).Wait();
                    }

                    if (accs.Count > 0)
                    {
                        BlobCache.LocalMachine.InvalidateAll().Wait();
                    }
                }
                catch
                {
                    // Do nothing.
                }

                return;
            }

            try
            {

                var defaultAccountId = -1;
                defaultValue.TryGet("DEFAULT_ACCOUNT", out defaultAccountId);

                using (var db = new SQLiteConnection(accountsDbPath))
                {
                    foreach (var account in db.Table<BitbucketAccount>())
                    {
                        var newAccount = new Core.Data.Account
                        {
                            AvatarUrl = account.AvatarUrl,
                            DefaultStartupView = account.DefaultStartupView,
                            StashAccount = false,
                            Password = account.Password,
                            Username = account.Username,
                            DontShowTeamEvents = account.DontShowTeamEvents,
                            ExpandTeamsAndGroups = account.ExpandTeamsAndGroups,
                            RefreshToken = account.RefreshToken,
                            RepositoryDescriptionInList = account.RepositoryDescriptionInList,
                            ShowTeamEvents = account.ShowTeamEvents,
                            Token = account.Token,
                            PinnedRepositories = account.PinnnedRepositories.Select(x => new Core.Data.PinnedRepository
                            {
                                ImageUri = x.ImageUri,
                                Name = x.Name,
                                Owner = x.Owner,
                                Slug = x.Slug
                            }).ToList()
                        };

                        accounts.Save(newAccount).ToBackground();

                        if (account.Id == defaultAccountId)
                        {
                            application.SetDefaultAccount(newAccount);
                        }
                    }

                    db.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error migrating: " + e.Message);
            }
            finally
            {
                File.Delete(accountsDbPath);
                Directory.Delete(accountsDir, true);
            }
        }
    }
}

