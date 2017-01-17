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
    private Discovery m_Discovery = new Discovery();
    private string m_EnvironmentID;

    private void Start()
    {
        LogSystem.InstallDefaultReactors();

        ////  Get Environments
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get environments");
        //if (!m_Discovery.GetEnvironments(OnGetEnvironments))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get environments");

        ////  GetEnvironment
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get environment");
        //if(!m_Discovery.GetEnvironment(OnGetEnvironment, "6c8647b7-9dd4-42c8-9cb0-117b40b14517"))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get environment");

        //  AddEnvironment using
        Log.Debug("ExampleDiscoveryV1", "Attempting to add environment");
        if (!m_Discovery.AddEnvironment(OnAddEnvironment, "unity-testing-AddEnvironment", "Testing addEnvironment in Unity SDK", 0))
            Log.Debug("ExampleDiscoveryV1", "Failed to add environment");
        
    }

    private void TestDeleteEnvironment()
    {
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete environment");
        if (!m_Discovery.DeleteEnvironment(OnDeleteEnvironment, m_EnvironmentID))
            Log.Debug("ExampleDiscoveryV1", "Failed to delete environment");
    }

    private void OnGetEnvironments(GetEnvironmentsResponse resp, string data)
    {
        if(resp != null)
        {
            foreach (Environment environment in resp.environments)
                Log.Debug("ExampleDiscoveryV1", "environment_id: {0}", environment.environment_id);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null");
        }
    }

    private void OnGetEnvironment(Environment resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "environment_name: {0}", resp.name);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null");
        }
    }

    private void OnAddEnvironment(Environment resp, string data)
    {
        if(resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Added {0}", resp.environment_id, data);
            m_EnvironmentID = resp.environment_id;

            TestDeleteEnvironment();
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnDeleteEnvironment(bool successs, string data)
    {
        Log.Debug("ExampleDiscoveryV1", "Delete success: {0}", successs);
    }
}
