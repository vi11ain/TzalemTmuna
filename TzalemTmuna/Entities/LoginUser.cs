﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TzalemTmuna.DB;
using System.Threading.Tasks;
using TzalemTmuna.Utilities;

namespace TzalemTmuna.Entities
{
    public class LoginUser : IEntity
    {
        private string username;
        private string email;
        private string salt;
        private string password;
        private string full_name;
        private string biography;
        public bool is_private;
        public bool is_admin;
        private string external_url;
        private List<User> following;
        private List<User> followers;

        public string Username
        {
            set
            {
                if (ValidateTools.IsUsername(value))
                    username = value;
                else
                    throw new Exception("Username can only contain English letters, numbers and underscores");
            }
            get
            {
                return username;
            }
        }
        public string Email
        {
            set
            {
                if (ValidateTools.IsEmail(value))
                    email = value;
                else
                    throw new Exception("You must enter a valid email address");
            }
            get
            {
                return email;
            }
        }
        public void NewSalt()
        {
            salt = PasswordTools.GetSalt();
        }
        public string Salt
        {
            get
            {
                return salt;
            }
        }
        public string Password
        {
            set
            {
                if(ValidateTools.IsPassword(value))
                    password = PasswordTools.HashSha256(value,salt);
                else
                    throw new Exception("You must enter a valid password");
            }
            get
            {
                return password;
            }
        }
        public string Full_name
        {
            set
            {
                if (value.Length <= 30)
                    full_name = value;
                else
                    throw new Exception("Full name can not surpass 30 characters");
            }
            get
            {
                return full_name;
            }
        }
        public string Biography
        {
            set
            {
                if (value.Length <= 150)
                    biography = value;
                else
                    throw new Exception("Biography can not surapss 150 characters");
            }
            get
            {
                return biography;
            }
        }
        public string External_url
        {
            set
            {
                if (ValidateTools.IsURL(value))
                    external_url = value;
                else if (value == string.Empty)
                { }
                else
                    throw new Exception("Link provided is not a valid url");
            }
            get
            {
                return external_url;
            }
        }

        public List<User> Following
        {
            get
            {
                if (following != null)
                    return following;
                else
                {
                    var fdb = new FollowingDB();
                    following = fdb.GetFollowing(username);
                    return following;
                }
            }
        }
        public List<User> Followers
        {
            get
            {
                if (followers != null)
                    return followers;
                else
                {
                    var fdb = new FollowingDB();
                    followers = fdb.GetFollowers(username);
                    return followers;
                }
            }
        }
        public LoginUser(DataRow dr)
        {
            username = dr["username"].ToString();
            email = dr["email"].ToString();
            salt = dr["salt"].ToString();
            password = dr["password"].ToString();
            full_name = dr["full_name"].ToString();
            biography = dr["biography"].ToString();
            is_private = Convert.ToBoolean(dr["is_private"]);
            is_admin = Convert.ToBoolean(dr["is_admin"]);
            external_url = dr["external_url"].ToString();
        }
        public LoginUser(string username, string email, string password)
        {
            this.username = username;
            this.email = email;
            NewSalt();
            if (ValidateTools.IsPassword(password))
                this.password = PasswordTools.HashSha256(password, salt);
            else
                throw new Exception("You must enter a valid password");
            full_name = username;
            is_private = false;
            is_admin = false;
        }
        public LoginUser()
        {

        }
        public void Populate(DataRow dr)
        {
            dr["username"] = username;
            dr["email"] = email;
            dr["salt"] = salt;
            dr["password"] = password;
            dr["full_name"] = full_name;
            dr["biography"] = biography;
            dr["is_private"] = is_private;
            dr["is_admin"] = is_admin;
            dr["external_url"] = external_url;
        }
    }
}
