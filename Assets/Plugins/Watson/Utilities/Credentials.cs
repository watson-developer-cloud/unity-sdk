using System;
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

        public string CreateAuthorization()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(User + ":" + Password));
        }
    };
}
