using System;
using BitbucketSharp.Models;
using MonoTouch;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using MonoTouch.Dialog;
using CodeFramework.UI.Elements;
using MonoTouch.Foundation;
using BitbucketBrowser.UI.Controllers.Privileges;
using System.Collections.Generic;

namespace BitbucketBrowser.UI.Controllers.Issues
{
    public class IssueEditController : BaseDialogViewController
    {
        private static readonly string[] Priorities = { "Trivial", "Minor", "Major", "Critical", "Blocker" };
        private static readonly string[] Statuses = { "New", "Opened", "Resolved", "On Hold", "Invalid", "Duplicate", "Wontfix" };
        private static readonly string[] Kinds = { "Bug", "Enhancement", "Proposal", "Task" };
        private static readonly string Unassigned = "Unassigned";
        
        public string Username { get; set; }
        public string RepoSlug { get; set; }
        public IssueModel ExistingIssue { get; set; } 

        public Action<IssueModel> Success;

        EntryElement _title;
        StyledElement _assignedTo, _issueType, _priority, _status;
        MultilinedElement _content;
         
        public IssueEditController()
            : base(true)
        {
            Title = "News Issue";
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => SaveIssue());
        }
        
        private void SaveIssue()
        {
            //Stop any editing!
            this.View.EndEditing(true);

            //Check the required fields
            if (string.IsNullOrEmpty(_title.Value))
            {
                Utilities.ShowAlert("Missing field!", "You must enter a title for this issue.");
                return;
            }
            
            var issue = new CreateIssueModel() {
                Title = _title.Value,
                Content = _content.Value,
                Kind = _issueType.Value.ToLower(),
                Priority = _priority.Value.ToLower(),
                Responsible = _assignedTo.Value.Equals(Unassigned) ? null : _assignedTo.Value.ToLower(),
                Status = _status == null ? null : _status.Value.ToLower(),
            };
            
            NavigationItem.RightBarButtonItem.Enabled = false;
            this.DoWork(() => {
                IssueModel updatedModel;
                if (ExistingIssue == null)
                    updatedModel = Application.Client.Users[Username].Repositories[RepoSlug].Issues.Create(issue);
                else
                    updatedModel = Application.Client.Users[Username].Repositories[RepoSlug].Issues[ExistingIssue.LocalId].Update(issue);


                InvokeOnMainThread(() => {
                    if (Success != null)
                        Success(updatedModel);
                    NavigationController.PopViewControllerAnimated(true);
                });
            }, 
            (ex) => {
                Utilities.ShowAlert("Unable to save issue!", ex.Message);
            }, 
            () => {
                NavigationItem.RightBarButtonItem.Enabled = true;
            });
        }

        private void DeleteIssue()
        {
            this.View.EndEditing(true);
            NavigationItem.RightBarButtonItem.Enabled = false;
            this.DoWork(() => {
                Application.Client.Users[Username].Repositories[RepoSlug].Issues[ExistingIssue.LocalId].Delete();

                InvokeOnMainThread(() => {
                    if (Success != null)
                        Success(null);
                    NavigationController.PopViewControllerAnimated(true);
                });
            },
            (ex) => {
                Utilities.ShowAlert("Unable to delete issue!", ex.Message);
            },
            () => {
                NavigationItem.RightBarButtonItem.Enabled = true;
            });
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            _title = new EntryElement("Title", string.Empty, string.Empty) { 
                TitleFont = UIFont.BoldSystemFontOfSize(15f),
                EntryFont = UIFont.SystemFontOfSize(14f),
                TitleColor = UIColor.FromRGB(41, 41, 41),
            };
            
            _assignedTo = new StyledElement("Responsible", Unassigned, UITableViewCellStyle.Value1) {
                Accessory = UITableViewCellAccessory.DisclosureIndicator,
            };
            _assignedTo.Tapped += () => {
                var privileges = new PrivilegesController() { 
                    Username = Username, 
                    RepoSlug = RepoSlug, 
                    Primary = new UserModel() { Username = Username },
                    Title = _assignedTo.Caption,
                };
                privileges.SelectedItem += (obj) => {
                    _assignedTo.Value = obj.User.Username;
                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(privileges, true);
            };
            
            _issueType = new StyledElement("Issue Type", Kinds[0], UITableViewCellStyle.Value1) {
                Accessory = UITableViewCellAccessory.DisclosureIndicator
            };
            _issueType.Tapped += () => {
                var en = new EnumViewController(_issueType.Caption, Kinds, _issueType.Value);
                en.ValueSelected += (obj) => {
                    _issueType.Value = obj;
                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(en, true);
            };
            
            _priority = new StyledElement("Priority", Priorities[0], UITableViewCellStyle.Value1) {
                Accessory = UITableViewCellAccessory.DisclosureIndicator
            };
            _priority.Tapped += () => {
                var en = new EnumViewController(_priority.Caption, Priorities, _priority.Value);
                en.ValueSelected += (obj) => {
                    _priority.Value = obj;
                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(en, true);
            };
            
            _content = new MultilinedElement("Comments");
            _content.Tapped += () => {
                var composer = new Composer() { Title = "Issue Comment", Text = _content.Value, ActionButtonText = "Save" };
                composer.NewComment(this, () => {
                    var text = composer.Text;
                    _content.Value = text;
                    composer.CloseComposer();
                });
            };

            var root = new RootElement(Title);
            root.Add(new Section() { _title, _assignedTo, _issueType, _priority });
            root.Add(new Section() { _content });

            //See if it's an existing issue or not...
            if (ExistingIssue != null)
            {
                _title.Value = ExistingIssue.Title;
                if (ExistingIssue.Responsible != null)
                    _assignedTo.Value = ExistingIssue.Responsible.Username;
                _issueType.Value = ExistingIssue.Metadata.Kind;
                _priority.Value = ExistingIssue.Priority;
                if (!string.IsNullOrEmpty(ExistingIssue.Content))
                    _content.Value = ExistingIssue.Content;

                _status = new StyledElement("Status", Statuses[0], UITableViewCellStyle.Value1) {
                    Accessory = UITableViewCellAccessory.DisclosureIndicator
                };
                _status.Tapped += () => {
                    var en = new EnumViewController(_status.Caption, Statuses, ExistingIssue.Status);
                    en.ValueSelected += (obj) => {
                        _status.Value = obj;
                        NavigationController.PopViewControllerAnimated(true);
                    };
                    NavigationController.PushViewController(en, true);
                };

                //Insert the status thing inbetween title and assigned to elements
                root[0].Insert(1, _status);

                //TODO: Add trash image
                var deleteButton = new StyledElement("Delete Issue", () => {
                    var alert = new UIAlertView() { 
                        Title = "Are you sure?",
                        Message = "You are about to permanently delete issue #" + ExistingIssue.LocalId + "."
                    };
                    alert.CancelButtonIndex = alert.AddButton("Cancel");
                    var ok = alert.AddButton("Delete");

                    alert.Clicked += (object sender, UIButtonEventArgs e) => {
                        if (e.ButtonIndex == ok)
                            DeleteIssue();
                    };

                    alert.Show();
                }, Images.BinClosed) {
                    BackgroundColor = UIColor.FromPatternImage(Images.TableCellRed),
                    TextColor = UIColor.FromRGB(0.9f, 0.30f, 0.30f)
                };
                root.Add(new Section() { deleteButton });
            }

            Root = root;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.ReloadData();
        }
        
        private class PopNavRadioElement : RadioElement
        {
            public PopNavRadioElement(string caption, string group) : base(caption, group) { }
            public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
            {
                base.Selected(dvc, tableView, indexPath);
                dvc.NavigationController.PopViewControllerAnimated(true);
            }
        }
    }
}

