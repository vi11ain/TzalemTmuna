﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TzalemTmuna.DB;
using TzalemTmuna.Utilities;
using TzalemTmuna.Entities;
using TzalemTmuna.Data;
using TzalemTmuna.Statics;

namespace TzalemTmuna.Forms
{
    public partial class Profile : MetroFramework.Forms.MetroForm
    {
        public bool isMainProfile;
        public bool redirectAfterClose;
        public User user;
        private bool needsRefresh = false;

        //login's profile
        public Profile()
        {
            InitializeComponent();
            Text = "My Profile";
            StyleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = Statics.Theme.metroThemeStyle,
                Style = Statics.Theme.metroColorStyle
            };
            Controls.Remove(btnMenu);
            btnMenu.Dispose();
            btnUpload.Image =
                Theme == MetroFramework.MetroThemeStyle.Dark
                ? Properties.Dark.darkUpload : Properties.Light.lightUpload;
            //Check whether I show special badge or dispose badge picture box
            if (LoggedInUser.login.is_admin)
            {
                new ToolTip().SetToolTip(pbBadge, "Administrator");
                pbBadge.Image =
                    Theme == MetroFramework.MetroThemeStyle.Dark
                    ? Properties.Dark.darkAdmin : Properties.Light.lightAdmin;
            }
            else if (LoggedInUser.login.is_verified)
            {
                new ToolTip().SetToolTip(pbBadge, "Verified User");
                pbBadge.Image =
                    Theme == MetroFramework.MetroThemeStyle.Dark
                    ? Properties.Dark.darkVerified : Properties.Light.lightVerified;
            }
            else
            {
                Controls.Remove(pbBadge);
                pbBadge.Dispose();
            }
            isMainProfile = true;
            ProfilePicture.BackColor = BackColor;
            ProfilePicture.Image = FileTools.getProfilePicture(LoggedInUser.login.Username);
            if (LoggedInUser.login.Biography != null)
                lblBio.Text = LoggedInUser.login.Biography.Replace("\\n", Environment.NewLine);
            else
                lblBio.Text = string.Empty;
            lblName.Text = LoggedInUser.login.Full_name;
            lblUsername.Text = LoggedInUser.login.Username;
            lblWebsite.Text = LoggedInUser.login.External_url;
            lblFollowers.Text = LoggedInUser.login.Followers.Count.ToString();
            lblFollowing.Text = LoggedInUser.login.Following.Count.ToString();
            lblPosts.Text = LoggedInUser.login.Posts.Count.ToString();

            btnOption.Text = "Edit Profile";
            LoadPostThumbnails();
        }
        //Edit Profile button - for login
        private void EditProfile()
        {
            new EditProfile().ShowDialog();
            //Refresh profile
            ProfilePicture.Image = FileTools.getProfilePicture(LoggedInUser.login.Username);
            if (LoggedInUser.login.Biography != null)
                lblBio.Text = LoggedInUser.login.Biography.Replace("\\n", Environment.NewLine);
            else
                lblBio.Text = string.Empty;
            lblName.Text = LoggedInUser.login.Full_name;
            lblUsername.Text = LoggedInUser.login.Username;
            lblWebsite.Text = LoggedInUser.login.External_url;
        }
        //user's profile
        public Profile(User user)
        {
            InitializeComponent();
            Text = user.Username + "'s Profile";
            StyleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = Statics.Theme.metroThemeStyle,
                Style = Statics.Theme.metroColorStyle
            };
            //Check whether I show special badge or dispose badge picture box
            if (user.is_admin)
            {
                new ToolTip().SetToolTip(pbBadge, "Administrator");
                pbBadge.Image =
                    Theme == MetroFramework.MetroThemeStyle.Dark
                    ? Properties.Dark.darkAdmin : Properties.Light.lightAdmin;
            }
            else if (user.Ban_text != string.Empty)
            {
                new ToolTip().SetToolTip(pbBadge, user.Ban_text);
                pbBadge.Image =
                    Theme == MetroFramework.MetroThemeStyle.Dark
                    ? Properties.Dark.darkBan : Properties.Light.lightBan;
            }
            else if (user.is_verified)
            {
                new ToolTip().SetToolTip(pbBadge, "Verified User");
                pbBadge.Image =
                    Theme == MetroFramework.MetroThemeStyle.Dark
                    ? Properties.Dark.darkVerified : Properties.Light.lightVerified;
            }
            else
            {
                Controls.Remove(pbBadge);
                pbBadge.Dispose();
            }

            if (LoggedInUser.login.is_admin && !user.is_admin)
            {
                reportToolStripMenuItem.Text = user.Ban_text == "" ? "Ban" : "Unban";
                reportToolStripMenuItem.Click += (sender, e) => new AdminRemoveForm(user, user.Ban_text == "").ShowDialog();
            }
            else
                reportToolStripMenuItem.Click += new EventHandler(reportToolStripMenuItem_Click);

            isMainProfile = false;
            this.user = user;
            ProfilePicture.BackColor = BackColor;
            ProfilePicture.Image = FileTools.getProfilePicture(user.Username);
            if (user.Biography != null)
                lblBio.Text = user.Biography.Replace("\\n", Environment.NewLine);
            else
                lblBio.Text = string.Empty;
            lblName.Text = user.Full_name;
            lblUsername.Text = user.Username;
            lblWebsite.Text = user.External_url;
            lblFollowers.Text = user.Followers.Count.ToString();
            lblFollowing.Text = user.Following.Count.ToString();
            lblPosts.Text = user.Posts.Count.ToString();
            Controls.Remove(btnLogout);
            btnLogout.Dispose();
            Controls.Remove(btnUpload);
            btnUpload.Dispose();
            btnOption.Text = "Follow";
            bool searchIfLoginFollowsUser = true;
            //Used to determine if Login can view User's pics
            bool profileAccessGranted = false;
            if (user.is_private)
            {
                foreach (User x in LoggedInUser.login.SentRequests)
                {
                    if (x.Username == user.Username)
                    {
                        btnOption.Text = "Requested";
                        searchIfLoginFollowsUser = false;
                        if (StyleManager.Theme == MetroFramework.MetroThemeStyle.Dark)
                            btnOption.Theme = MetroFramework.MetroThemeStyle.Light;
                        else
                            btnOption.Theme = MetroFramework.MetroThemeStyle.Dark;
                        break;
                    }
                }
            }
            else
                profileAccessGranted = true;
            if (searchIfLoginFollowsUser)
            {
                foreach (User x in LoggedInUser.login.Following)
                {
                    if (x.Username == user.Username)
                    {
                        btnOption.Text = "Following";
                        profileAccessGranted = true;
                        if (StyleManager.Theme == MetroFramework.MetroThemeStyle.Dark)
                            btnOption.Theme = MetroFramework.MetroThemeStyle.Light;
                        else
                            btnOption.Theme = MetroFramework.MetroThemeStyle.Dark;
                        break;
                    }
                }
            }
            if (profileAccessGranted)
                LoadPostThumbnails();
        }
        private void Unfollow()
        {
            var fdb = new FollowingDB();
            fdb.Unfollow(user);
            btnOption.Text = "Follow";
            btnOption.Theme = StyleManager.Theme;
            //Refresh followers
            lblFollowers.Text = user.Followers.Count.ToString();
        }
        private void Follow()
        {
            if (user.is_private)
            {
                var rdb = new RequestsDB();
                rdb.SendRequest(LoggedInUser.login, user);
                btnOption.Text = "Requested";
                if (StyleManager.Theme == MetroFramework.MetroThemeStyle.Dark)
                    btnOption.Theme = MetroFramework.MetroThemeStyle.Light;
                else
                    btnOption.Theme = MetroFramework.MetroThemeStyle.Dark;
            }
            else
            {
                var fdb = new FollowingDB();
                fdb.Follow(user);
                btnOption.Text = "Following";
                if (StyleManager.Theme == MetroFramework.MetroThemeStyle.Dark)
                    btnOption.Theme = MetroFramework.MetroThemeStyle.Light;
                else
                    btnOption.Theme = MetroFramework.MetroThemeStyle.Dark;
                //Refresh followers
                lblFollowers.Text = user.Followers.Count.ToString();
                //Refresh feed
                LoggedInUser.feed.RefreshFeed();
            }
        }
        private void RemoveRequest()
        {
            var rdb = new RequestsDB();
            rdb.RemoveRequest(LoggedInUser.login, user);
            btnOption.Text = "Follow";
            btnOption.Theme = StyleManager.Theme;
        }
        private void btnOption_Click(object sender, EventArgs e)
        {
            switch (btnOption.Text)
            {
                case "Follow":
                    Follow();
                    break;
                case "Following":
                    Unfollow();
                    break;
                case "Edit Profile":
                    EditProfile();
                    break;
                case "Requested":
                    RemoveRequest();
                    break;
            }
        }
        public void CleanThumbnailContainer()
        {
            flowLayoutPanel4.Controls.Clear();
        }
        private void ShowList(int Mode)
        {
            if (isMainProfile)
            {
                new Followers(Mode).ShowDialog();
                //Refresh following
                lblFollowing.Text = LoggedInUser.login.Following.Count.ToString();
                lblFollowers.Text = LoggedInUser.login.Followers.Count.ToString();
            }
            else
            {
                new Followers(this, Mode, user).ShowDialog();
                //Refresh following
                lblFollowing.Text = user.Following.Count.ToString();
                lblFollowers.Text = user.Followers.Count.ToString();
            }
        }

        public void RefreshFollowingAndFollowers()
        {
            if (isMainProfile)
            {
                //Refresh following
                lblFollowing.Text = LoggedInUser.login.Following.Count.ToString();
            }
            else
            {
                //Refresh following
                lblFollowing.Text = user.Following.Count.ToString();
            }
        }

        public void LoadPostThumbnails()
        {
            if (isMainProfile)
            {
                foreach (Post x in LoggedInUser.login.Posts)
                {
                    ProfileThumbnail thumbnail = new ProfileThumbnail(x);
                    flowLayoutPanel4.Controls.Add(thumbnail);
                }
            }
            else
            {
                foreach (Post x in user.Posts)
                {
                    ProfileThumbnail thumbnail = new ProfileThumbnail(x);
                    flowLayoutPanel4.Controls.Add(thumbnail);
                }
            }
        }
        private void lblTextFollowers_Click(object sender, EventArgs e)
        {
            ShowList(0);
        }

        private void lblFollowers_Click(object sender, EventArgs e)
        {
            ShowList(0);
        }

        private void lblFollowingText_Click(object sender, EventArgs e)
        {
            ShowList(1);
        }

        private void lblFollowing_Click(object sender, EventArgs e)
        {
            ShowList(1);
        }

        private void btnFollowRequests_Click(object sender, EventArgs e)
        {
            ShowList(2);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.username = string.Empty;
            Properties.Settings.Default.password = string.Empty;
            Properties.Settings.Default.Save();
            LoggedInUser.loginPage = new Login();
            LoggedInUser.loginPage.Show();
            redirectAfterClose = false;
            Close();
        }

        private void Profile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (redirectAfterClose)
                RedirectHere(sender, e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // If login closes his window, then only hide it because we need it alive for static calls.
            // THIS IS HIDE FOR LOGIN'S PROFILE!
            // check feed is visible tho, cause if feed is hidden then we need to terminate
            if (isMainProfile && LoggedInUser.feed.Visible)
            {
                //Environment.Exit(0);
                Hide();
                redirectAfterClose = false;
                needsRefresh = true;
                e.Cancel = true;
            }
        }

        //Ment to hold all redirections to previous open profiles and windows, depends on redirectAfterClose Boolean.
        public event EventHandler RedirectHere;

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            if (lblWebsite.Text != "")
                System.Diagnostics.Process.Start(lblWebsite.Text);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (new NewPost().ShowDialog() == DialogResult.OK)
            {
                ProfileThumbnail thumbnail = new ProfileThumbnail(LoggedInUser.login.Posts.Last());
                flowLayoutPanel4.Controls.Add(thumbnail);
            }
        }

        private void Profile_VisibleChanged(object sender, EventArgs e)
        {
            //Refresh followers every hide & show
            if (needsRefresh)
            {
                needsRefresh = false;
                lblFollowing.Text = LoggedInUser.login.Following.Count.ToString();
                lblFollowers.Text = LoggedInUser.login.Followers.Count.ToString();
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            btnMenu.ContextMenuStrip.Show(btnMenu, new Point(0, btnMenu.Height));
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new NewReport(3, user.Username).ShowDialog();
        }
    }
}
