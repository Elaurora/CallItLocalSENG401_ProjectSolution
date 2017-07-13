using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientApplicationMVC.Models
{
    public static class Globals
    {
        /// <summary>
        /// The amount of time the client should wait for a response from the server
        /// Value is 10mins
        /// </summary>
        public const int patienceLevel_ms = 600000;

        /// <summary>
        /// Returns true if the client is currently logged in.
        /// </summary>
        /// <returns>True if logged in. False otherwise.</returns>
        public static bool isLoggedIn()
        {
            if("Log In".Equals(getUser()))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the name of the current user
        /// </summary>
        /// <param name="user">The user name</param>
        public static void setUser(string user)
        {
            HttpContext.Current.Session["user"] = user;
        }

        /// <summary>
        /// gets the name of the current user
        /// </summary>
        /// <returns>The name of the current user</returns>
        public static string getUser()
        {
            return (string)HttpContext.Current.Session["user"];
        }
    }
}