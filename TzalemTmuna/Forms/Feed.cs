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
    public partial class Feed : MetroFramework.Forms.MetroForm
    {
        UserDB udb;
        int scrollLocation = 0;
        public Feed()
        {
            InitializeComponent();
        }

        private void HandleTheme()
        {
            if (Theme==MetroFramework.MetroThemeStyle.Dark)
            {
                btnSearch.BackgroundImage = Properties.Dark.darkSearch;
                btnProfile.BackgroundImage = Properties.Dark.darkProfile;
                btnSettings.BackgroundImage = Properties.Dark.darkSettings;
                btnAlert.BackgroundImage = Properties.Dark.darkAlert;
            }
            else
            {
                btnSearch.BackgroundImage = Properties.Light.lightSearch;
                btnProfile.BackgroundImage = Properties.Light.lightProfile;
                btnSettings.BackgroundImage = Properties.Light.lightSettings;
                btnAlert.BackgroundImage = Properties.Light.lightAlert;
            }
        }

        public void ResetMe()
        {
            Refresh();
            StyleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = Statics.Theme.metroThemeStyle,
                Style = Statics.Theme.metroColorStyle
            };
            HandleTheme();
            udb = new UserDB();
            var usernameList = udb.GetUsernameList();
            txtSearch.AutoCompleteCustomSource.AddRange(usernameList);

            flowLayoutPanel1.Controls.Clear();
            foreach (Post post in LoggedInUser.login.FeedPosts())
            {
                FeedThumbnail thumbnail = new FeedThumbnail(post);
                thumbnail.Anchor = AnchorStyles.None;
                flowLayoutPanel1.Controls.Add(thumbnail);
                flowLayoutPanel1.VerticalScroll.Maximum += thumbnail.Height;
            }

            int notificationCount = LoggedInUser.login.ReceivedRequests.Count > 0 ? 1 : 0;
            notificationCount += LoggedInUser.login.Alerts.Count;

            if (notificationCount == 0)
            {
                lblNotifications.Hide();
            }
            else
            {
                lblNotifications.Show();
                lblNotifications.Text = notificationCount.ToString();
            }
        }

        public void RefreshFeed()
        {
            flowLayoutPanel1.Controls.Clear();
            foreach (Post post in LoggedInUser.login.FeedPosts())
            {
                FeedThumbnail thumbnail = new FeedThumbnail(post);
                thumbnail.Anchor = AnchorStyles.None;
                flowLayoutPanel1.Controls.Add(thumbnail);
                flowLayoutPanel1.VerticalScroll.Maximum += thumbnail.Height;
            }

            int notificationCount = LoggedInUser.login.ReceivedRequests.Count > 0 ? 1 : 0;
            notificationCount += LoggedInUser.login.Alerts.Count;

            if (notificationCount == 0)
            {
                lblNotifications.Hide();
            }
            else
            {
                lblNotifications.Show();
                lblNotifications.Text = notificationCount.ToString();
            }
        }
        private void flowLayoutPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            scrollLocation -= e.Delta;

            if (scrollLocation < flowLayoutPanel1.VerticalScroll.Minimum)
            {
                scrollLocation = flowLayoutPanel1.VerticalScroll.Minimum;
            }
            else if (scrollLocation > flowLayoutPanel1.VerticalScroll.Maximum)
            {
                scrollLocation = flowLayoutPanel1.VerticalScroll.Maximum;
            }

            flowLayoutPanel1.VerticalScroll.Value = scrollLocation;
            flowLayoutPanel1.AutoScrollPosition = new Point(0,scrollLocation);
            flowLayoutPanel1.PerformLayout();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            LoggedInUser.profile.Show();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {   if (txtSearch.Text != string.Empty)
            {
                if (udb.Find(txtSearch.Text))
                {
                    if (udb.GetCurrentRow()["username"].ToString() == LoggedInUser.login.Username)
                    {
                        LoggedInUser.profile.Show();
                        LoggedInUser.profile.redirectAfterClose = true;
                        LoggedInUser.profile.RedirectHere += (s, args) => RefreshFeed();
                    }
                    else
                    {
                        Profile newProfile = new Profile(new User(udb.GetCurrentRow()));
                        newProfile.Show();
                        newProfile.redirectAfterClose = true;
                        newProfile.RedirectHere += (s, args) => RefreshFeed();
                    }
                }
                else
                    MetroFramework.MetroMessageBox.Show(this,"User Not Found!","Searched TzalemTmuna For \""+txtSearch.Text+"\"",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
                MetroFramework.MetroMessageBox.Show(this, "Search TextBox is Empty!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lblNotifications_Click(object sender, EventArgs e)
        {
            //new Followers(2).ShowDialog();
            new Alerts().ShowDialog();
            RefreshFeed();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (LoggedInUser.login.is_admin)
                new SettingsAdmin().ShowDialog();
            else
                new Settings().ShowDialog();
        }
    }
}
