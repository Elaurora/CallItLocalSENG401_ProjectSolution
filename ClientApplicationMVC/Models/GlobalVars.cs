using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientApplicationMVC.Models
{
    public static class GlobalVars
    {
        /// <summary>
        /// The name of the user currently logged in. Defaults to "Log In" if no one is logged in
        /// </summary>
        public static string user { get; set; } = "Log In";

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
            if("Log In".Equals(user))
            {
                return false;
            }
            return true;
        }

    }
}