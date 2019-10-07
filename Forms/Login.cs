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

namespace TzalemTmuna.Forms
{
    public partial class Login : MetroFramework.Forms.MetroForm
    {

        public Login()
        {
            InitializeComponent();
        }

        private void LoginError()
        {
            MetroFramework.MetroMessageBox.Show(this, "Username/Email and/or password is not valid", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateTools.IsEmail(txtUsername.Text))
            {
                var edb = new EmailDB();
                if (edb.Find(txtUsername.Text))
                {
                    DoLogin(edb.GetCurrentRow());
                }
                else
                    LoginError();
            }
            else if (ValidateTools.IsUsername(txtUsername.Text))
            {
                var udb = new UserDB();
                if (udb.Find(txtUsername.Text))
                {
                    DoLogin(udb.GetCurrentRow());
                }
                else
                    LoginError();
            }
            else
                LoginError();
        }

        private void DoLogin(User user)
        {
            var pdb = new PasswordDB();
            if (pdb.Match(user, txtPassword.Text))
                MessageBox.Show("Connecting!");
            else
                LoginError();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //    nirgolan4,gayboy
            //    udirubin8,uduman
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var register = new Register();
            register.Location = Location;
            register.Show();
            Hide();
            register.Closed += (s, args) => Show();
        }
    }
}
