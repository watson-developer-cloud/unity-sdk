


using IBM.Watson.DeveloperCloud.Utilities;
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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services
{
    /// <summary>
    /// Callback for the GetServiceStatus() function.
    /// </summary>
    /// <param name="serviceID">The ID of the service.</param>
    /// <param name="active">The status of the service, true is up, false is down.</param>
    public delegate void ServiceStatus(string serviceID, bool active);

    /// <summary>
    /// This interface defines common interface for all watson services.
    /// </summary>
    public interface IWatsonService
    {
        /// <summary>
        /// Returns the service ID.
        /// </summary>
        /// <returns>A string containing the service ID.</returns>
        string GetServiceID();
        /// <summary>
        /// This should check if the service is up or down, and invoke the callback with the current
        /// state of the service once determined.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        void GetServiceStatus(ServiceStatus callback);
    }

    /// <summary>
    /// Service helper class.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// This returns a instance of all services.
        /// </summary>
        /// <returns>An array of IWatsonService instances.</returns>
        public static IWatsonService[] GetAllServices(bool reqCredentials = false)
        {
            List<IWatsonService> services = new List<IWatsonService>();

            Type[] types = Utilities.Utility.FindAllDerivedTypes(typeof(IWatsonService));
            foreach (var type in types)
            {
                try
                {
                    IWatsonService serviceObject = Activator.CreateInstance(type) as IWatsonService;
                    if (reqCredentials && Config.Instance.FindCredentials(serviceObject.GetServiceID()) == null)
                        continue;       // skip services that don't have credential data..
                    services.Add(serviceObject as IWatsonService);
                }
                catch (Exception)
                { }
            }

            return services.ToArray();
        }
    }
}
