/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using IBM.Cloud.SDK.Utilities;
using System;

namespace IBM.Cloud.SDK
{
    public class BaseService
    {
        protected Credentials credentials;
        protected string url;

        public BaseService(string serviceId)
        {
            var credentialsPaths = Utility.GetCredentialsPaths();
            if (credentialsPaths.Count > 0)
            {
                foreach (string path in credentialsPaths)
                {
                    if (Utility.LoadEnvFile(path))
                    {
                        break;
                    }
                }

                string ApiKey = Environment.GetEnvironmentVariable(serviceId.ToUpper() + "_APIKEY");
                string Username = Environment.GetEnvironmentVariable(serviceId.ToUpper() + "_USERNAME");
                string Password = Environment.GetEnvironmentVariable(serviceId.ToUpper() + "_PASSWORD");
                string ServiceUrl = Environment.GetEnvironmentVariable(serviceId.ToUpper() + "_URL");

                if (string.IsNullOrEmpty(ApiKey) && (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)))
                {
                    throw new NullReferenceException(string.Format("Either {0}_APIKEY or {0}_USERNAME and {0}_PASSWORD did not exist. Please add credentials with this key in ibm-credentials.env.", serviceId.ToUpper()));
                }

                if (!string.IsNullOrEmpty(ApiKey))
                {
                    TokenOptions tokenOptions = new TokenOptions()
                    {
                        IamApiKey = ApiKey
                    };

                    credentials = new Credentials(tokenOptions, ServiceUrl);

                    if (string.IsNullOrEmpty(credentials.Url))
                    {
                        credentials.Url = url;
                    }
                }

                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    credentials = new Credentials(Username, Password, url);
                }
            }
        }

        public BaseService(string versionDate, string serviceId) : this(serviceId) { }

        public BaseService(string versionDate, Credentials credentials, string serviceId) { }

        public BaseService(Credentials credentials, string serviceId) { }
    }
}
