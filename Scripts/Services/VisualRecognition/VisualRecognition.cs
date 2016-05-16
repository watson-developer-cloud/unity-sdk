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

using UnityEngine;
using IBM.Watson.DeveloperCloud.Services;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3
{
    public class VisualRecognition : IWatsonService {
        #region Public Types
        #endregion

        #region Private Data
        private const string SERVICE_ID = "VisualRecognitionV3";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region IWatsonService implementation

        public string GetServiceID()
        {
            throw new System.NotImplementedException();
        }

        public void GetServiceStatus(ServiceStatus callback)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    	
    }
}
