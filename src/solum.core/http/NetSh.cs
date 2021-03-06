﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solum.core.http
{
    using System;

    /// <summary>
    /// Executes NetSh commands
    /// </summary>
    public static class NetSh
    {
        private const string NETSH_COMMAND = "netsh";

        /// <summary>
        /// Add a url reservation
        /// </summary>
        /// <param name="url">Url to add</param>
        /// <param name="user">User to add the reservation for</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool AddUrlAcl(string url, string user)
        {
            try
            {
                var arguments = GetParameters(url, user);

                return UacHelper.RunElevated(NETSH_COMMAND, arguments);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetParameters(string url, string user)
        {
            return string.Format("http add urlacl url={0} user={1}", url, user);
        }
    }

}
