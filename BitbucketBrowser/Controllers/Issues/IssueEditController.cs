using System;
using System.Linq;
using BitbucketSharp.Models;
using MonoTouch;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using MonoTouch.Dialog;
using CodeFramework.UI.Elements;
using MonoTouch.Foundation;
using BitbucketBrowser.Controllers.Privileges;
using System.Collections.Generic;

namespace BitbucketBrowser.Controllers.Issues
{
    public class IssueEditController : BaseDialogViewController
    {
        private static readonly string[] Priorities = { "Trivial", "Minor", "Major", "Critical", "Blocker" };
        private static readonly string[] Statuses = { "New", "Open", "Resolved", "On Hold", "Invalid", "Duplicate", "Wontfix" };
        private static readonly string[] Kinds = { "Bug", "Enhancement", "Proposal", "Task" };
        private const string Unassigned = "Unassigned";
        private const string None = "None";

        public string Username { get; set; }
        public string RepoSlug { get; set; }
        public IssueModel ExistingIssue { get; set; }

        bool _extrasDone;
        public List<MilestoneModel> Milestones { get; set; }
        public List<ComponentModel> Components { get; set; }
        public List<VersionModel> Versions { get; set; }

        public Action<IssueModel> Success;

        EntryElement _title;
        StyledElement _assignedTo, _issueType, _priority, _status, _milestone, _component, _version;
        MultilinedElement _content;

        public IssueEditController()
            : base(true)
        {
            Title = "New Issue";
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => SaveIssue());
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!_extrasDone)
                LoadExtras();
            _extrasDone = true;
        }

        private void LoadExtras()
        {
            if (Milestones == null || Components == null || Versions == null)
            {
                this.DoWork(() =>
                {
                    try
                    {
                        if (Milestones == null)
                            Milestones = Application.Client.Users[Username].Repositories[RepoSlug].Issues.GetMilestones();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (Components == null)
                            Components = Application.Client.Users[Username].Repositories[RepoSlug].Issues.GetComponents();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (Versions == null)
                            Versions = Application.Client.Users[Username].Repositories[RepoSlug].Issues.GetVersions();
                    }
                    catch (Exception)
                    {
                    }
                },
                ex => { }, AddExtrasToRoot);
            }
            else
            {
                AddExtrasToRoot();
            }
        }

        private void AddExtrasToRoot()
        {
            var sec = new Section();
            if (Milestones != null && Milestones.Count > 0)
            {
                var elements = new List<string> { None };
                elements.AddRange(from s in Milestones select s.Name);
                string defaultValue = None;
                if (ExistingIssue != null && !string.IsNullOrEmpty(ExistingIssue.Metadata.Milestone))
                    defaultValue = ExistingIssue.Metadata.Milestone;
                _milestone = CreateEnumElement("Milestone", defaultValue, elements);
                sec.Add(_milestone);
            }

            if (Components != null && Components.Count > 0)
            {
                var elements = new List<string> { None };
                elements.AddRange(from s in Components select s.Name);
                string defaultValue = None;
                if (ExistingIssue != null && !string.IsNullOrEmpty(ExistingIssue.Metadata.Component))
                    defaultValue = ExistingIssue.Metadata.Component;
                _component = CreateEnumElement("Components", defaultValue, elements);
                sec.Add(_component);
            }

            if (Versions != null && Versions.Count > 0)
            {
                var elements = new List<string> { None };
                elements.AddRange(from s in Versions select s.Name);
                string defaultValue = None;
                if (ExistingIssue != null && !string.IsNullOrEmpty(ExistingIssue.Metadata.Version))
                    defaultValue = ExistingIssue.Metadata.Version;
                _version = CreateEnumElement("Version", defaultValue, elements);
                sec.Add(_version);
            }

            if (sec.Count == 0)
                return;

            Root.Insert(1, sec);
        }

        private CreateIssueModel CreateRequest()
        {
            var issue = new CreateIssueModel
                            {
                                Title = _title.Value,
                                Content = _content.Value,
                                Kind = _issueType.Value.ToLower(),
                                Priority = _priority.Value.ToLower(),
                                Responsible = _assignedTo.Value.Equals(Unassigned) ? null : _assignedTo.Value.ToLower(),
                                Status = _status == null ? null : _status.Value.ToLower(),
                                Version = (_version == null || _version.Value.Equals(None)) ? null : _version.Value,
                                Component = (_component == null || _component.Value.Equals(None)) ? null : _component.Value,
                                Milestone = (_milestone == null || _milestone.Value.Equals(None)) ? null : _milestone.Value,
                            };

            //Nullify them if they are the same...
            if (ExistingIssue != null)
            {
                if (Equals(issue.Title, ExistingIssue.Title)) issue.Title = null;
                if (Equals(issue.Content, ExistingIssue.Content)) issue.Content = null;
                if (Equals(issue.Kind, ExistingIssue.Metadata.Kind)) issue.Kind = null;
                if (Equals(issue.Priority, ExistingIssue.Priority)) issue.Priority = null;
                if (Equals(issue.Responsible, ExistingIssue.Responsible)) issue.Responsible = null;
                if (Equals(issue.Status, ExistingIssue.Status)) issue.Status = null;

                //Component shit
                if (Equals(issue.Version, ExistingIssue.Metadata.Version))
                    issue.Version = null;
                else if (issue.Version == null)
                    issue.Version = string.Empty;

                if (Equals(issue.Component, ExistingIssue.Metadata.Component))
                    issue.Component = null;
                else if (issue.Component == null)
                    issue.Component = string.Empty;

                if (Equals(issue.Milestone, ExistingIssue.Metadata.Milestone))
                    issue.Milestone = null;
                else if (issue.Milestone == null)
                    issue.Milestone = string.Empty;
            }

            return issue;
        }

        private void SaveIssue()
        {
            //Stop any editing!
            View.EndEditing(true);

            //Check the required fields
            if (string.IsNullOrEmpty(_title.Value))
            {
                Utilities.ShowAlert("Missing field!", "You must enter a title for this issue.");
                return;
            }

            var issue = CreateRequest();

            //Check to see if there is any change...
            if (issue.CheckNoChange())
            {
                NavigationController.PopViewControllerAnimated(true);
                return;
            }

            NavigationItem.RightBarButtonItem.Enabled = false;
            this.DoWork(() =>
            {
                var updatedModel = ExistingIssue == null ? Application.Client.Users[Username].Repositories[RepoSlug].Issues.Create(issue) : 
                    Application.Client.Users[Username].Repositories[RepoSlug].Issues[ExistingIssue.LocalId].Update(issue);
                InvokeOnMainThread(() =>
                {
                    if (Success != null)
                        Success(updatedModel);

                    if (NavigationController != null)
                        NavigationController.PopViewControllerAnimated(true);
                });
            },
            ex => Utilities.ShowAlert("Unable to save issue!", ex.Message),
            () =>
            {
                NavigationItem.RightBarButtonItem.Enabled = true;
            });
        }

        private void DeleteIssue()
        {
            View.EndEditing(true);
            NavigationItem.RightBarButtonItem.Enabled = false;
            this.DoWork(() =>
            {
                Application.Client.Users[Username].Repositories[RepoSlug].Issues[ExistingIssue.LocalId].Delete();

                InvokeOnMainThread(() =>
                {
                    if (Success != null)
                        Success(null);

                    if (NavigationController != null)
                        NavigationController.PopViewControllerAnimated(true);
                });
            },
            ex => Utilities.ShowAlert("Unable to delete issue!", ex.Message),
            () =>
            {
                NavigationItem.RightBarButtonItem.Enabled = true;
            });
        }

        private StyledElement CreateEnumElement(string title, string defaultVal, IEnumerable<string> values)
        {
            var element = new StyledElement(title, defaultVal, UITableViewCellStyle.Value1)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator
            };
            element.Tapped += () =>
            {
                var en = new RadioChoiceViewController(element.Caption, values, element.Value);
                en.ValueSelected += obj =>
                {
                    element.Value = obj;
                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(en, true);
            };
            
            return element;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PopulateRoot();
        }

        private void PopulateRoot()
        {
            _title = new InputElement("Title", string.Empty, string.Empty);

            _assignedTo = new StyledElement("Responsible", Unassigned, UITableViewCellStyle.Value1)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator,
            };
            _assignedTo.Tapped += () =>
            {
                var privileges = new PrivilegesController
                                     {
                                         Username = Username,
                                         RepoSlug = RepoSlug,
                                         Primary = new UserModel { Username = Username },
                                         Title = _assignedTo.Caption,
                                     };
                privileges.SelectedItem += obj =>
                {
                    _assignedTo.Value = obj.Username;
                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(privileges, true);
            };

            _issueType = CreateEnumElement("Issue Type", Kinds[0], Kinds);
            _priority = CreateEnumElement("Priority", Priorities[0], Priorities);

            _content = new MultilinedElement("Description");
            _content.Tapped += () =>
            {
                var composer = new Composer { Title = "Issue Description", Text = _content.Value, ActionButtonText = "Save" };
                composer.NewComment(this, () =>
                {
                    var text = composer.Text;
                    _content.Value = text;
                    composer.CloseComposer();
                });
            };

            var root = new RootElement(Title) { new Section { _title, _assignedTo, _issueType, _priority }, new Section { _content } };

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

                _status = CreateEnumElement("Status", ExistingIssue.Status, Statuses);

                //Insert the status thing inbetween title and assigned to elements
                root[0].Insert(1, _status);

                var deleteButton = new StyledElement("Delete Issue", () =>
                {
                    var alert = new UIAlertView
                                    {
                                        Title = "Are you sure?",
                                        Message = "You are about to permanently delete issue #" + ExistingIssue.LocalId + "."
                                    };
                    alert.CancelButtonIndex = alert.AddButton("Cancel");
                    var ok = alert.AddButton("Delete");

                    alert.Clicked += (sender, e) =>
                    {
                        if (e.ButtonIndex == ok)
                            DeleteIssue();
                    };

                    alert.Show();
                }, Images.BinClosed)
                {
                    Accessory = UITableViewCellAccessory.None,
                    BackgroundColor = UIColor.FromPatternImage(Images.TableCellRed),
                    TextColor = UIColor.FromRGB(0.9f, 0.30f, 0.30f)
                };
                root.Add(new Section { deleteButton });
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

