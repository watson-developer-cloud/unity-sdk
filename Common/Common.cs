/**
* (C) Copyright IBM Corp. 2019, 2020.
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
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson
{
    public class Common : MonoBehaviour
    {
        /// <summary>
        /// The SDK version.
        /// </summary>
        public const string Version = "watson-apis-unity-sdk-4.5.0";
        /// <summary>
        /// Tracking for onboarding.
        /// </summary>
        public const string TrackingQueryParam = "target=/developer/watson&cm_sp=WatsonPlatform-WatsonServices-_-OnPageNavLink-IBMWatson_SDKs-_-Unity";
        /// <summary>
        /// Returns a set of default headers to use with each request.
        /// </summary>
        /// <param name="serviceName">The service name to be used in X-IBMCloud-SDK-Analytics header.</param>
        /// <param name="serviceVersion">The service version to be used in X-IBMCloud-SDK-Analytics header.</param>
        /// <param name="operation">The operation name to be used in X-IBMCloud-SDK-Analytics header.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSdkHeaders(string serviceName, string serviceVersion, string operationId)
        {
            Dictionary<string, string> defaultHeaders = new Dictionary<string, string>();
            defaultHeaders.Add("X-IBMCloud-SDK-Analytics", 
                string.Format("service_name={0};service_version={1};operation_id={2}", 
                serviceName, 
                serviceVersion, 
                operationId));
            defaultHeaders.Add("User-Agent",
                string.Format(
                    "{0} {1} {2} {3}",
                    Version,
                    SystemInformation.Os,
                    SystemInformation.OsVersion,
                    Application.unityVersion
                ));
            return defaultHeaders;
        }
    }
}
