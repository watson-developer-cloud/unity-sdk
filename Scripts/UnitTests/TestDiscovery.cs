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

using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using System.Collections;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestDiscovery : UnitTest
    {
        Discovery m_Discovery = new Discovery();

        //  Default news values
        private string m_DefaultEnvironmentID = "6c8647b7-9dd4-42c8-9cb0-117b40b14517";
        private string m_DefaultConfigurationID = "662a2032-9e2c-472b-9eaa-1a2fa098c22e";
        private string m_DefaultCollectionID = "336f2f0e-e771-424e-a7b4-331240c8f136";

        //  Environment
        private string m_CreatedEnvironmentName = "unity-sdk-integration-test";
        private string m_CreatedEnvironmentDescription = "Integration test running for Unity SDK. Please do not delete this environment until 10 minutes after the status is 'active'. The test should delete this environment.";
        private string m_CreatedEnvironmentID;

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
    }
}
