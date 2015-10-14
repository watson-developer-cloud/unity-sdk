using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBM.Watson.Utilities
{
    /// <summary>
    /// Helper class for holding a user & password, used by both the WSCOnnector & RESTConnector.
    /// </summary>
    public class Credentials
    {
        public Credentials()
        { }
        public Credentials( string user, string password )
        {
            User = user;
            Password = password;
        }

        public string User { get; set; }
        public string Password { get; set; }
    };
}
