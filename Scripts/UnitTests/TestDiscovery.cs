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

using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using System.Collections;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestDiscovery : UnitTest
    {
        private Discovery m_Discovery = new Discovery();
        private System.Timers.Timer m_StateTimer = new System.Timers.Timer(1000);
        private bool m_IsCheckingState = false;
        private bool IsCheckingState
        {
            get
            {
                return m_IsCheckingState;
            }
            set
            {
                m_IsCheckingState = value;
                if (m_IsCheckingState)
                    CheckState();
            }
        }

        //  Default news values
        private string m_DefaultEnvironmentID = "6c8647b7-9dd4-42c8-9cb0-117b40b14517";
        private string m_DefaultConfigurationID = "662a2032-9e2c-472b-9eaa-1a2fa098c22e";
        private string m_DefaultCollectionID = "336f2f0e-e771-424e-a7b4-331240c8f136";

        //  Environment
        private string m_CreatedEnvironmentName = "unity-sdk-integration-test";
        private string m_CreatedEnvironmentDescription = "Integration test running for Unity SDK. Please do not delete this environment until 10 minutes after the status is 'active'. The test should delete this environment.";
        private string m_CreatedEnvironmentID;
        private bool m_IsEnvironmentActive = false;

        //  Configuration
        private string m_ConfigurationJsonPath;
        private string m_CreatedConfigurationID;

        //  Collection
        private string m_CreatedCollectionName = "Unity SDK Created Collection";
        private string m_CreatedCollectionDescription = "A collection created by the Unity SDK.";
        private string m_CreatedCollectionID;

        //  Document
        private string m_FilePathToIngest;  //  File path for PreviewConfiguration
        private string m_DocumentFilePath;
        private string m_Metadata = "{\n\t\"Creator\": \"Unity SDK Integration Test\",\n\t\"Subject\": \"Discovery service\"\n}";
        private string m_CreatedDocumentID;

        //  Query
        private string m_Query = "What is the capital of china?";

        //  Add and get Environments
        private bool m_GetEnvironmentsTested = false;
        private bool m_AddEnvironmentsTested = false;
        private bool m_GetEnvironmentTested = false;

        //  Add and get Configurations
        private bool m_GetConfigurationsTested = false;
        private bool m_AddConfigurationTested = false;
        private bool m_GetConfigurationTested = false;

        //  Preview Configuration
        private bool m_PreviewConfigurationTested = false;

        //  Add and get Collections
        private bool m_GetCollectionsTested = false;
        private bool m_AddCollectionTested = false;
        private bool m_GetCollectionTested = false;

        //  Get Fields
        private bool m_GetFieldsTested = false;

        //  Add and get Documents
        private bool m_AddDocumentTested = false;
        private bool m_GetDocumentTested = false;
        private bool m_UpdateDocumentTested = false;

        //  Query
        private bool m_QueryTested = false;

        //  Delete
        private bool m_DeleteDocumentTested = false;
        private bool m_DeleteCollectionTested = false;
        private bool m_DeleteConfigurationTested = false;
        private bool m_DeleteEnvironmentTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            m_ConfigurationJsonPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/Discovery/exampleConfigurationData.json";
            m_FilePathToIngest = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
            m_DocumentFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

            #region Environments
            //  Get Environments
            Log.Debug("ExampleDiscoveryV1", "Attempting to get environments");
            if (!m_Discovery.GetEnvironments((GetEnvironmentsResponse resp, string data) =>
            {
                if (resp != null)
                {
                    foreach (Environment environment in resp.environments)
                        Log.Debug("ExampleDiscoveryV1", "environment_id: {0}", environment.environment_id);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetEnvironments(); resp is null");
                }

                Test(resp.environments != null);
                m_GetEnvironmentsTested = true;
            }))
                Log.Debug("ExampleDiscoveryV1", "Failed to get environments");

            while (!m_GetEnvironmentsTested)
                yield return null;

            //  Add Environment
            Log.Debug("ExampleDiscoveryV1", "Attempting to add environment");
            if (!m_Discovery.AddEnvironment((Environment resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Added {0}", resp.environment_id);
                    m_CreatedEnvironmentID = resp.environment_id;
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.AddEnvironment(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.environment_id));
                m_AddEnvironmentsTested = true;
            }, m_CreatedEnvironmentName, m_CreatedEnvironmentDescription, 0))
                Log.Debug("ExampleDiscoveryV1", "Failed to add environment");

            while (!m_AddEnvironmentsTested)
                yield return null;

            //  Get Environment
            Log.Debug("ExampleDiscoveryV1", "Attempting to get environment");
            if (!m_Discovery.GetEnvironment((Environment resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "environment_name: {0}", resp.name);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetEnvironment(); resp is null");
                }

                Test(!string.IsNullOrEmpty(resp.name));
                m_GetEnvironmentTested = true;
            }, m_CreatedEnvironmentID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get environment");

            while (!m_GetEnvironmentTested)
                yield return null;
            #endregion

            #region Configurations
            //  Get Configurations
            Log.Debug("ExampleDiscoveryV1", "Attempting to get configurations");
            if (!m_Discovery.GetConfigurations((GetConfigurationsResponse resp, string data) =>
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
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetConfigurations(); resp is null, {0}", data);
                }

                Test(resp.configurations != null);
                m_GetConfigurationsTested = true;
            }, m_DefaultEnvironmentID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get configurations");

            while (!m_GetConfigurationsTested)
                yield return null;

            //  Add Configuration
            Log.Debug("ExampleDiscoveryV1", "Attempting to add configuration");
            if (!m_Discovery.AddConfiguration((Configuration resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
                    m_CreatedConfigurationID = resp.configuration_id;


                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.AddConfiguration(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.configuration_id));
                m_AddConfigurationTested = true;
            }, m_CreatedEnvironmentID, m_ConfigurationJsonPath))
                Log.Debug("ExampleDiscoveryV1", "Failed to add configuration");

            while (!m_AddConfigurationTested)
                yield return null;

            //  Get Configuration
            Log.Debug("ExampleDiscoveryV1", "Attempting to get configuration");
            if (!m_Discovery.GetConfiguration((Configuration resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Configuration: {0}, {1}", resp.configuration_id, resp.name);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.GetConfiguration(); resp is null, {0}", data);
                }

                Test(!string.IsNullOrEmpty(resp.name));
                m_GetConfigurationTested = true;
            }, m_DefaultEnvironmentID, m_DefaultConfigurationID))
                Log.Debug("ExampleDiscoveryV1", "Failed to get configuration");

            while (!m_GetConfigurationTested)
                yield return null;

            //  Preview Configuration
            Log.Debug("ExampleDiscoveryV1", "Attempting to preview configuration");
            if (!m_Discovery.PreviewConfiguration((TestDocument resp, string data) =>
            {
                if (resp != null)
                {
                    Log.Debug("ExampleDiscoveryV1", "Preview succeeded: {0}", resp.status);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Discovery.PreviewConfiguration(); resp is null {0}", data);
                }

                Test(resp != null);
                m_PreviewConfigurationTested = true;
            }, m_CreatedEnvironmentID, m_CreatedConfigurationID, null, m_FilePathToIngest, m_Metadata))
                Log.Debug("ExampleDiscoveryV1", "Failed to preview configuration");

            while (!m_PreviewConfigurationTested)
                yield return null;

            IsCheckingState = true;

            while (!m_IsEnvironmentActive)
                yield return null;

            #endregion

            #region Collections
            #endregion

            #region Fields
            #endregion

            #region Documents
            #endregion

            #region Query
            #endregion

            #region Delete
            //  DeleteEnvironment
            Log.Debug("ExampleDiscoveryV1", "Attempting to delete configuration {0}", m_CreatedConfigurationID);
            if (!m_Discovery.DeleteConfiguration((bool success, string data) =>
            {
                if (success)
                {
                    Log.Debug("ExampleDiscoveryV1", "Delete configuration successful");
                    m_CreatedConfigurationID = default(string);
                }
                else
                    Log.Debug("ExampleDiscoveryV1", "Delete configuration failed");

                Test(success);
                m_DeleteConfigurationTested = true;
            }, m_CreatedEnvironmentID, m_CreatedConfigurationID))
                Log.Debug("ExampleDiscoveryV1", "Failed to delete configuration");

            while (!m_DeleteConfigurationTested)
                yield return null;

            //  Delete Environment
            Log.Debug("ExampleDiscoveryV1", "Attempting to delete environment {0}", m_CreatedEnvironmentID);
            if (!m_Discovery.DeleteEnvironment((bool success, string data) =>
            {
                if (success)
                {
                    Log.Debug("ExampleDiscoveryV1", "Delete environment successful");
                    m_CreatedEnvironmentID = default(string);
                }
                else
                {
                    Log.Debug("ExampleDiscoveryV1", "Delete environment failed");
                }

                Test(success);
                m_DeleteEnvironmentTested = true;
            }, m_CreatedEnvironmentID))
                Log.Debug("ExampleDiscoveryV1", "Failed to delete environment");

            while (!m_DeleteEnvironmentTested)
                yield return null;
            #endregion

            yield break;
        }

        private void CheckState()
        {
            Log.Debug("ExampleDiscoveryV1", "Attempting to get environment state");

            m_Discovery.GetEnvironment((Environment resp, string data) =>
            {
                Log.Debug("ExampleDiscoveryV1", "Environment {0} is {1}", resp.environment_id, resp.status);

                IsCheckingState = false;
                if (resp.status == "active")
                {
                    m_IsEnvironmentActive = true;
                }
                else
                {
                    m_StateTimer.Elapsed += OnTimer;
                    m_StateTimer.Enabled = true;
                }
            }, m_CreatedEnvironmentID);
        }

        private void OnTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            m_StateTimer.Enabled = false;
            m_StateTimer.Elapsed -= OnTimer;
            IsCheckingState = true;
        }
    }
}
