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
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using UnityEngine;

public class ExampleDiscoveryV1 : MonoBehaviour
{
    Discovery m_Discovery = new Discovery();

    private void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Get Environments
        Log.Debug("ExampleDiscovery", "Attempting to get environments!");
        if (!m_Discovery.GetEnvironments(OnGetEnvironments))
            Log.Debug("ExampleDiscovery", "Failed to get environments!");
    }

    private void OnGetEnvironments(GetEnvironmentsResponse resp, string data)
    {
        if(resp != null)
        {
            foreach (Environment environment in resp.environments)
                Log.Debug("ExampleDiscovery", "environment_id: {0}", environment.environment_id);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null");
        }
    }
}
