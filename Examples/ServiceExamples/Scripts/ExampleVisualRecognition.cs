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
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

public class ExampleVisualRecognition : MonoBehaviour {
    private VisualRecognition m_VisualRecognition = new VisualRecognition();

	
	void Start () {
        LogSystem.InstallDefaultReactors();

        Config.Instance.FindCredentials(m_VisualRecognition.GetServiceID());
        
        if(!m_VisualRecognition.GetClassifiers(OnGetClassifiers))
            Debug.Log("Getting classifiers failed!");
	}

    private void OnGetClassifiers (GetClassifiersTopLevelBrief classifiers)
    {
        if(classifiers != null && classifiers.classifiers.Length > 0)
        {
//            for(int i = 0; i < classifiers.classifiers.Length; 
            foreach(GetClassifiersPerClassifierBrief classifier in classifiers.classifiers)
            {
                Debug.Log("Classifier: " + classifier.name + ", " + classifier.classifier_id);
            }
        }
        else
        {
            Debug.Log("Request failed!");
        }
    }
}
