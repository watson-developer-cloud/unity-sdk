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

using IBM.Watson.NaturalLanguageClassifier.V1;
using IBM.Watson.NaturalLanguageClassifier.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;


namespace IBM.Watson.Examples
{
    public class ExampleNaturalLanguageClassifierV1 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey;
		[Tooltip("The service URL (optional). This defaults to \"https://api.us-south.natural-language-classifier.watson.cloud.ibm.com\"")]
        [SerializeField]
        private string serviceUrl;
        #endregion

        private NaturalLanguageClassifierService service;
        private string nluText = "IBM is an American multinational technology company headquartered in Armonk, New York, United States with operations in over 170 countries.";
        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(iamApikey))
            {
                throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
            }

            IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

            while (!authenticator.CanAuthenticate())
            {
                yield return null;
            }

			service = new NaturalLanguageClassifierService(authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                service.SetServiceUrl(serviceUrl);
            }

            Runnable.Run(ExampleListClassifiers());
        }

        private IEnumerator ExampleListClassifiers()
        {
            ClassifierList listClassifiersResponse = null;
            service.ListClassifiers(
                callback: (DetailedResponse<ClassifierList> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageClassifierServiceV1", "ListClassifiers result: {0}", response.Response);
                    listClassifiersResponse = response.Result;
                }
            );

            while (listClassifiersResponse == null)
                yield return null;
        }
    }
}
