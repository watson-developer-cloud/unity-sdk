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
    private string m_CreatedEnvironmentID;
    private string m_DefaultEnvironmentID = "6c8647b7-9dd4-42c8-9cb0-117b40b14517";
    private string m_DefaultConfigurationID = "662a2032-9e2c-472b-9eaa-1a2fa098c22e";
    private string m_ConfigurationJsonPath;

    private void Start()
    {
        LogSystem.InstallDefaultReactors();

        m_ConfigurationJsonPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/exampleConfigurationData.json";

        ////  Get Environments
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get environments");
        //if (!m_Discovery.GetEnvironments(OnGetEnvironments))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get environments");

        ////  GetEnvironment
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get environment");
        //if(!m_Discovery.GetEnvironment(OnGetEnvironment, "6c8647b7-9dd4-42c8-9cb0-117b40b14517"))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get environment");

        //  AddEnvironment
        Log.Debug("ExampleDiscoveryV1", "Attempting to add environment");
        if (!m_Discovery.AddEnvironment(OnAddEnvironment, "unity-testing-AddEnvironment", "Testing addEnvironment in Unity SDK", 0))
            Log.Debug("ExampleDiscoveryV1", "Failed to add environment");

        ////  Get Configurations
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get configurations");
        //if(!m_Discovery.GetConfigurations(OnGetConfigurations, m_DefaultEnvironmentID))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get configurations");

        ////  Get Configuration
        //Log.Debug("ExampleDiscoveryV1", "Attempting to get configuration");
        //if (!m_Discovery.GetConfiguration(OnGetConfiguration, m_DefaultEnvironmentID, m_DefaultConfigurationID))
        //    Log.Debug("ExampleDiscoveryV1", "Failed to get configuration");
    }

    private void TestDeleteEnvironment()
    {
        //  DeleteEnvironment
        Log.Debug("ExampleDiscoveryV1", "Attempting to delete environment");
        if (!m_Discovery.DeleteEnvironment(OnDeleteEnvironment, m_CreatedEnvironmentID))
            Log.Debug("ExampleDiscoveryV1", "Failed to delete environment");
    }

    private void TestAddConfiguration()
    {
        //  Add Configuration
        Log.Debug("ExampleDiscoveryV1", "Attempting to add configuration");
        if (!m_Discovery.AddConfiguration(OnAddConfiguration, m_CreatedEnvironmentID, m_ConfigurationJsonPath))
            Log.Debug("ExampleDiscoveryV1", "Failed to add configuration");
    }
    private void OnGetEnvironments(GetEnvironmentsResponse resp, string data)
    {
        if (resp != null)
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
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Added {0}", resp.environment_id, data);
            m_CreatedEnvironmentID = resp.environment_id;

            //TestDeleteEnvironment();
            TestAddConfiguration();
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

    private void OnGetConfigurations(GetConfigurationsResponse resp, string data)
    {
        if (resp != null)
        {
            if (resp.configurations != null && resp.configurations.Length > 0)
            {
                foreach (ConfigurationRef configuration in resp.configurations)
                {
                    Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", configuration.configuration_id, configuration.name);
                }
            }
            else
            {
                Log.Debug("ExampleDiscoveryV1", "There are no configurations for this environment.");
            }
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnGetConfiguration(Configuration resp, string data)
    {
        if (resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }

    private void OnAddConfiguration(Configuration resp, string data)
    {
        if(resp != null)
        {
            Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
        }
        else
        {
            Log.Debug("ExampleDiscoveryV1", "resp is null, {0}", data);
        }
    }
}
