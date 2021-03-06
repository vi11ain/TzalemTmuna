﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TzalemTmuna.DB;
using TzalemTmuna.Utilities;
using TzalemTmuna.Entities;
using TzalemTmuna.Statics;

namespace TzalemTmuna.Forms
{
    public partial class Login : MetroFramework.Forms.MetroForm
    {
        public Login()
        {
            InitializeComponent();
            StyleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = Statics.Theme.metroThemeStyle,
                Style = Statics.Theme.metroColorStyle
            };
        }
        private void LoginError()
        {
            MetroFramework.MetroMessageBox.Show(this, "Username/Email and/or password is not valid", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (TextTools.IsEmail(txtUsername.Text))
            {
                var edb = new EmailDB();
                if (edb.Find(txtUsername.Text))
                {
                    DoLogin(new LoginUser(edb.GetCurrentRow()));
                }
                else
                    LoginError();
            }
            else if (TextTools.IsUsername(txtUsername.Text))
            {
                var udb = new UserDB();
                if (udb.Find(txtUsername.Text))
                {
                    DoLogin(new LoginUser(udb.GetCurrentRow()));
                }
                else
                    LoginError();
            }
            else
                LoginError();
        }

        private void DoLogin(LoginUser user)
        {
            if (PasswordTools.Match(user, txtPassword.Text))
            {
                //Verify user is not banned
                if (user.GetBanText() == string.Empty)
                {
                    if (chkRememberMe.Checked)
                    {
                        Properties.Settings.Default.username = user.Username;
                        Properties.Settings.Default.password = user.Password;
                        Properties.Settings.Default.Save();
                    }
                    LoggedInUser.login = user;
                    LoggedInUser.profile = new Profile();
                    LoggedInUser.feed.ResetMe();
                    LoggedInUser.feed.Show();
                    Close();
                    //profile.Closed += (s, args) => Location = profile.Location;
                    //profile.Closed += (s, args) => Show();
                }
                //User is banned -> represent user with ban text
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Sorry "+user.Username+",\rBut your account is banned.\r\rBan reason:\r"+user.GetBanText(), "User is banned", MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
                LoginError();
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            LoggedInUser.feed.Hide();
            //  A few creds for login:
            //    nirgolan4,gayboy
            //    udirubin8,uduman
            //    dvir_derbi,12345dvir
            //    yakubovkarina2002@gmail.com,kiki2002
            //    eden_kaushansky_34,eden212616692
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var register = new Register();
            register.Location = Location;
            register.Show();
            Hide();
            register.Closed += (s, args) => Location = register.Location;
            register.Closed += (s, args) => Show();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (LoggedInUser.feed.Visible == false)
            {
                Application.Exit();
            }
        }

        private void btnForgotPassword_Click(object sender, EventArgs e)
        {
            var forgotPassword = new ForgotPassword();
            forgotPassword.Location = Location;
            forgotPassword.Show();
            Hide();
            forgotPassword.Closed += (s, args) => Location = forgotPassword.Location;
            forgotPassword.Closed += (s, args) => Show();
        }
    }
}
