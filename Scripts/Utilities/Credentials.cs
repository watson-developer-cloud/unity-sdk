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

using System;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  /// <summary>
  /// Helper class for holding a user and password, used by both the WSCOnnector and RESTConnector.
  /// </summary>
  public class Credentials
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Credentials()
    { }
    /// <summary>
    /// Constructor that takes the user name and password.
    /// </summary>
    /// <param name="user">The string containing the user name.</param>
    /// <param name="password">A string containing the password.</param>
    public Credentials(string user, string password)
    {
      User = user;
      Password = password;
    }

    /// <summary>
    /// The user name.
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// The password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Create basic authentication header data for REST requests.
    /// </summary>
    /// <returns>The authentication data base64 encoded.</returns>
    public string CreateAuthorization()
    {
      return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(User + ":" + Password));
    }

    /// <summary>
    /// Do we have credentials?
    /// </summary>
    /// <returns></returns>
    public bool HasCredentials()
    {
      return !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password);
    }
  };
}
