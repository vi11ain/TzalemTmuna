﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using TzalemTmuna.DB;
using TzalemTmuna.Utilities;
using TzalemTmuna.Entities;
using TzalemTmuna.Data;
using TzalemTmuna.Statics;

namespace TzalemTmuna.Forms
{
    public partial class ViewPost : MetroFramework.Forms.MetroForm
    {
        Post post;
        bool propertyOfLogin;
        bool liked;
        LikeDB likeDB;
        List<IMouseBoundable> loginCommentControls;
        private CustomMouseBoundsChecker mouseBounds;
        int scrollLocation = 0;
        protected override void OnLoad(EventArgs e)
        {
            //Add mousebound check message filter on load
            Application.AddMessageFilter(mouseBounds);

            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //Remove mousebound check message filter on load
            Application.RemoveMessageFilter(mouseBounds);

            base.OnClosing(e);
        }
        private void HandleTheme()
        {
            if (Theme == MetroFramework.MetroThemeStyle.Dark)
            {
                btnSubmit.BackgroundImage = Properties.Dark.darkSend;
                pbComment.Image = Properties.Dark.darkComment;
                pbLike.Image = Properties.Dark.darkLike;
                pbOption.Image = Properties.Dark.darkFlag;
            }
            else
            {
                btnSubmit.BackgroundImage = Properties.Light.lightSend;
                pbComment.Image = Properties.Light.lightComment;
                pbLike.Image = Properties.Light.lightLike;
                pbOption.Image = Properties.Light.lightFlag;
            }
        }
        public ViewPost(Post post)
        {
            InitializeComponent();
            Text = post.Owner.Username + "'s Post";
            StyleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = Statics.Theme.metroThemeStyle,
                Style = Statics.Theme.metroColorStyle
            };
            HandleTheme();
            //Check if login's post
            propertyOfLogin = post.Owner.Username == LoggedInUser.login.Username;
            // If login's post or login is an admin
            if (propertyOfLogin || LoggedInUser.login.is_admin)
            {
                //Delete
                if (propertyOfLogin)
                    pbOption.Click += new EventHandler(DeletePost);
                else
                    pbOption.Click += (sender,e) => new AdminRemoveForm(post).ShowDialog();
                if (Theme == MetroFramework.MetroThemeStyle.Dark)
                {
                    pbOption.Image = Properties.Dark.darkDelete;
                }
                else
                {
                    pbOption.Image = Properties.Light.lightDelete;
                }
            }
            else
            {
                //Report
                pbOption.Click += new EventHandler(ReportPost);
            }

            likeDB = new LikeDB();
            this.post = post;
            lblText.Text = post.Post_text;
            pbPhoto.Image = FileTools.getPost(post.Owner.Username, post.Post_number);
            profilePicture.Image = FileTools.getProfilePicture(post.Owner.Username);
            lblUsername.Text = post.Owner.Username;

            //Sometimes the post's description is bigger than form's size, so this fixes the issue
            //Added padding of 10px, I know hardcoded is bad but not so important as of right now
            int resize = lblText.Size.Width + 10 - (Size.Width - lblText.Location.X);
            if (resize > 0)
            {
                Size = new Size(Size.Width + resize, Size.Height);
                tableLayoutPanel1.Size = new Size(tableLayoutPanel1.Size.Width + resize, tableLayoutPanel1.Size.Height);
            }

            foreach (User x in post.Likes)
            {
                if (x.Username == LoggedInUser.login.Username)
                {
                    liked = true;
                    if (Theme == MetroFramework.MetroThemeStyle.Dark)
                    {
                        pbLike.Image = Properties.Dark.darkLikeFilled;
                    }
                    else
                    {
                        pbLike.Image = Properties.Light.lightLikeFilled;
                    }
                    break;
                }
            }

            //List for CommentControls to pass MouseBounds
            loginCommentControls = new List<IMouseBoundable>();

            foreach (Comment x in post.Comments)
            {
                var commentControl = new CommentControl(x);
                flowLayoutPanel1.Controls.Add(commentControl);
                loginCommentControls.Add(commentControl);
            }

            //Count comments and likes
            if (post.Likes.Count == 1)
                lblLikes.Text = "1 like";
            else
                lblLikes.Text = post.Likes.Count + " likes";
            if (post.Comments.Count == 1)
                lblComments.Text = "1 comment:";
            else
                lblComments.Text = post.Comments.Count + " comments:";

            #region MouseBounds Handler
            //if (loginCommentControls.Count != 0)
            mouseBounds = new CustomMouseBoundsChecker(loginCommentControls);

            #endregion
        }

        public void RemoveComment(int comment_id, CommentControl commentControl)
        {
            foreach (Comment x in post.Comments)
            {
                if (x.Comment_id == comment_id)
                {
                    post.Comments.Remove(x);
                    loginCommentControls.Remove(commentControl);
                    //Count comments
                    if (post.Comments.Count == 1)
                        lblComments.Text = "1 comment:";
                    else
                        lblComments.Text = post.Comments.Count + " comments:";
                    //mouseBounds = new MouseBounds(loginCommentControls);
                    break;
                }
            }
        }

        private void OpenProfile(object sender, EventArgs e)
        {
            if (propertyOfLogin)
            {
                LoggedInUser.profile.Show();
            }
            else
            {
                Profile newProfile = new Profile(post.Owner);
                newProfile.Show();
            }
        }

        private void pbComment_Click(object sender, EventArgs e)
        {
            //Switch between opening and closing comment adding panel
            if (tableLayoutPanel1.RowStyles[2].Height == 0)
            {
                tableLayoutPanel1.RowStyles[2].Height = 20;
                tableLayoutPanel1.RowStyles[0].Height = 74;
                txtCommentText.Enabled = true;
                btnSubmit.Enabled = true;
            }
            else
            {
                tableLayoutPanel1.RowStyles[2].Height = 0;
                tableLayoutPanel1.RowStyles[0].Height = 94;
                txtCommentText.Enabled = false;
                btnSubmit.Enabled = false;
            }
        }

        private void Like(object sender, EventArgs e)
        {
            if (liked)
            {
                if (Theme == MetroFramework.MetroThemeStyle.Dark)
                {
                    pbLike.Image = Properties.Dark.darkLike;
                }
                else
                {
                    pbLike.Image = Properties.Light.lightLike;
                }
                likeDB.Unlike(post);
                liked = false;
            }
            else
            {
                if (Theme == MetroFramework.MetroThemeStyle.Dark)
                {
                    pbLike.Image = Properties.Dark.darkLikeFilled;
                }
                else
                {
                    pbLike.Image = Properties.Light.lightLikeFilled;
                }
                likeDB.Like(post);
                liked = true;
            }
            //Count likes
            if (post.Likes.Count == 1)
                lblLikes.Text = "1 like";
            else
                lblLikes.Text = post.Likes.Count + " likes";
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
            flowLayoutPanel1.AutoScrollPosition = new Point(0, scrollLocation);
            flowLayoutPanel1.PerformLayout();
        }

        private void ViewPost_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoggedInUser.feed.RefreshFeed();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (TextTools.StripWhitespace(txtCommentText.Text) != string.Empty)
            {
                //Add Comment to Database
                DAL.GetInstance().ExecuteNonQuery("INSERT INTO comments " +
                    "([comment_text], [owner], [post_id]) " +
                    "VALUES(@comment_text, @owner, @post_id)", new OleDbParameter[]
                    {
                                    new OleDbParameter("@comment_text", txtCommentText.Text),
                                    new OleDbParameter("@owner", LoggedInUser.login.Username),
                                    new OleDbParameter("@post_id", post.Post_id)
                    });
                //Retrieve AutoNumber comment_id from Database
                int comment_id = (int)DAL.GetInstance().ExecuteScalarQuery("SELECT [comment_id] FROM comments ORDER BY [comment_id] DESC");
                //Create comment object
                var comment = new Comment(comment_id, post.Post_id, txtCommentText.Text, new User(LoggedInUser.login));
                //Add comment to DataSet
                new CommentDB().AddRow(comment);
                //Add comment to post's comment list
                post.Comments.Add(comment);
                //Send alert to post owner
                new AlertDB().NewAlert($"{LoggedInUser.login.Username} commented:\n\r\"{comment.Comment_text}\"\n\ron your post titled:\n\r\"{post.Post_text}\"", post.Owner);
                var commentControl = new CommentControl(comment);
                //Add comment to flowLayoutPanel
                flowLayoutPanel1.Controls.Add(commentControl);
                //Add comment to loginCommentControls
                loginCommentControls.Add(commentControl);
                //Clean textbox
                txtCommentText.Text = string.Empty;
                //Count comments
                if (post.Comments.Count == 1)
                    lblComments.Text = "1 comment:";
                else
                    lblComments.Text = post.Comments.Count + " comments:";
            }
        }

        private void DeletePost(object sender, EventArgs e)
        {
            if (MetroFramework.MetroMessageBox.Show(this, "Confirm deletion of post \"" + post.Post_text + "\"", "Delete Post", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new PostDB().RemovePost(post.Post_id);
                //Call to remove comments
                new CommentDB().RemoveComments(post.Post_id);
                //Call to remove likes
                new LikeDB().RemoveLikes(post.Post_id);
                LoggedInUser.login.Posts.Remove(post);
                LoggedInUser.profile.CleanThumbnailContainer();
                LoggedInUser.profile.LoadPostThumbnails();

                //Send alert about deleted post to comment holders
                if (post.Comments.Count > 0)
                {
                    var adb = new AlertDB();
                    foreach (Comment x in post.Comments)
                    {
                        adb.NewAlert($"{LoggedInUser.login.Username} deleted a post\n\ryou commented on", x.Owner);
                    }
                }

                Close();
            }
        }

        private void ReportPost(object sender, EventArgs e)
        {
            new NewReport(1, post.Post_id.ToString()).ShowDialog();
        }
    }
}
