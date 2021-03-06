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
    public class User : IEntity
    {
        private string username;
        private string email;
        private string full_name;
        private string biography;
        public bool is_private;
        public bool is_admin;
        public bool is_verified;
        private string external_url;
        private List<User> following;
        private List<User> followers;
        private List<Post> posts;
        private List<Report> reports;
        private string ban_text;

        public string Username
        {
            set
            {
                if (TextTools.IsUsername(value))
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
                if (TextTools.IsEmail(value))
                    email = value;
                else
                    throw new Exception("You must enter a valid email address");
            }
            get
            {
                return email;
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
                    throw new Exception("Biography can not surpass 150 characters");
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
                if (TextTools.IsURL(value))
                    external_url = value;
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
        public List<Post> Posts
        {
            get
            {
                if (posts != null)
                    return posts;
                else
                {
                    var pdb = new PostDB();
                    posts = pdb.GetPosts(username);
                    return posts;
                }
            }
        }

        public List<Report> Reports
        {
            get
            {
                if (reports != null)
                    return reports;
                else
                {
                    var rdb = new ReportDB();
                    reports = rdb.GetReports(username);
                    return reports;
                }
            }
        }

        public string Ban_text
        {
            get
            {
                return ban_text;
            }
        }

        public User(DataRow dr)
        {
            username = dr["username"].ToString();
            email = dr["email"].ToString();
            full_name = dr["full_name"].ToString();
            biography = dr["biography"].ToString();
            is_private = Convert.ToBoolean(dr["is_private"]);
            is_admin = Convert.ToBoolean(dr["is_admin"]);
            is_verified = Convert.ToBoolean(dr["is_verified"]);
            external_url = dr["external_url"].ToString();
            ban_text = dr["ban_text"].ToString();
        }
        public User(LoginUser login)
        {
            username = login.Username;
            email = login.Email;
            full_name = login.Full_name;
            biography = login.Biography;
            is_private = login.is_private;
            is_admin = login.is_admin;
            is_verified = login.is_verified;
            external_url = login.External_url;
            //ban_text is not needed while converting LoginUser to User
            //because login cannot exist while being banned
        }
        public User()
        {

        }
        public void Populate(DataRow dr)
        {
            dr["username"] = username;
            dr["email"] = email;
            dr["full_name"] = full_name;
            dr["biography"] = biography;
            dr["is_private"] = is_private;
            dr["is_admin"] = is_admin;
            dr["is_verified"] = is_verified;
            dr["external_url"] = external_url;
            dr["ban_text"] = ban_text;
        }
    }
}
