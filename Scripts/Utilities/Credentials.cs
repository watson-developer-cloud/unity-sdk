/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using FullSerializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Helper class for holding a user and password or authorization token, used by both the WSCOnnector and RESTConnector.
    /// </summary>
    public class Credentials
    {
        #region Private Data
        private string _authenticationToken;
        #endregion
        /// <summary>
        /// The user name.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// The Api Key.
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// The autheentication token
        /// </summary>
        public string AuthenticationToken
        {
            get { return _authenticationToken; }
            set { _authenticationToken = value; }
        }
        /// <summary>
        /// The service endpoint.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Constructor that takes the URL. Used for token authentication.
        /// </summary>
        public Credentials(string url = null)
        {
            Url = url;
        }

        /// <summary>
        /// Constructor that takes the user name and password.
        /// </summary>
        /// <param name="username">The string containing the user name.</param>
        /// <param name="password">A string containing the password.</param>
        /// <param name="url">The service endpoint.</param>
        public Credentials(string username, string password, string url = null)
        {
            Username = username;
            Password = password;
            Url = url;
        }

        /// <summary>
        /// Constructor that takes an authentication token created by the user or an ApiKey. If providing an ApiKey 
        /// set useApiKey to true.
        /// </summary>
        /// <param name="url">The service endpoint.</param>
        public Credentials(string apiKey, string url = null)
        {
            ApiKey = apiKey;
            Url = url;
        }

        /// <summary>
        /// Create basic authentication header data for REST requests.
        /// </summary>
        /// <returns>The authentication data base64 encoded.</returns>
        public string CreateAuthorization()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password));
        }

        /// <summary>
        /// Do we have credentials?
        /// </summary>
        /// <returns>true if the class has a username and password.</returns>
        public bool HasCredentials()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        /// <summary>
        /// Do we have an authentication token?
        /// </summary>
        /// <returns>True if the class has a Authentication Token</returns>
        public bool HasAuthorizationToken()
        {
            return !string.IsNullOrEmpty(AuthenticationToken);
        }

        /// <summary>
        /// Do we have an ApiKey?
        /// </summary>
        /// <returns>True if the class has a Authentication Token</returns>
        public bool HasApiKey()
        {
            return !string.IsNullOrEmpty(ApiKey);
        }
    }

    [fsObject]
    public class VcapCredentials
    {
        public Dictionary<string, List<VcapCredential>> VCAP_SERVICES { get; set; }
    }

    [fsObject]
    public class VcapCredential
    {
        [fsProperty("name")]
        public string Name { get; set; }
        [fsProperty("label")]
        public string Label { get; set; }
        [fsProperty("plan")]
        public string Plan { get; set; }
        [fsProperty("credentials")]
        public Credential Credentials { get; set; }
    }

    [fsObject]
    public class Credential
    {
        [fsProperty("url")]
        public string Url { get; set; }
        [fsProperty("username")]
        public string Username { get; set; }
        [fsProperty("password")]
        public string Password { get; set; }
        [fsProperty("workspaceId")]
        public string WorkspaceId { get; set; }
        [fsProperty("apikey")]
        public string Apikey { get; set; }
        [fsProperty("note")]
        public string Note { get; set; }
    }
}
