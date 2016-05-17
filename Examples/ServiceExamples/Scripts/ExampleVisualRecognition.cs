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
    private string m_classifierName = "integration-test-classifier";
    private string m_classifierID = "integrationtestclassifier_1745947114";
    private string m_classifierToDelete = "";
	
	void Start () {
        LogSystem.InstallDefaultReactors();

        Config.Instance.FindCredentials(m_VisualRecognition.GetServiceID());

        //  Get all classifiers
//        if(!m_VisualRecognition.GetClassifiers(OnGetClassifiers))
//            Debug.Log("Getting classifiers failed!");

        //  Find classifier by name
//        m_VisualRecognition.FindClassifier(m_classifierName, OnFindClassifier);

        //  Find classifier by ID
//        if(!m_VisualRecognition.GetClassifier(m_classifierID, OnGetClassifier))
//            Debug.Log("Getting classifier failed!");

        //  Delete classifier by ID
//        if(!m_VisualRecognition.DeleteClassifier(m_classifierToDelete, OnDeleteClassifier))
//            Debug.Log("Deleting classifier failed!");
	}

    private void OnGetClassifiers (GetClassifiersTopLevelBrief classifiers)
    {
        if(classifiers != null && classifiers.classifiers.Length > 0)
        {
            foreach(GetClassifiersPerClassifierBrief classifier in classifiers.classifiers)
            {
                Debug.Log("Classifier: " + classifier.name + ", " + classifier.classifier_id);
            }
        }
        else
        {
            Debug.Log("Failed to get classifiers!");
        }
    }

    private void OnFindClassifier(GetClassifiersPerClassifierVerbose classifier)
    {
        if(classifier != null)
        {
            Debug.Log("Classifier " + m_classifierName + " found! ClassifierID: " + classifier.classifier_id);
        }
        else
        {
            Debug.Log("Failed to find classifier by name!");
        }
    }

    private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier)
    {
        if(classifier != null)
        {
            Debug.Log("Classifier " + m_classifierID + " found! Classifier name: " + classifier.name);
        }
        else
        {
            Debug.Log("Failed to find classifier by ID!");
        }
    }

    private void OnDeleteClassifier(bool success)
    {
        if(success)
        {
            Debug.Log("Deleted classifier " + m_classifierToDelete);
        }
        else
        {
            Debug.Log("Failed ot delete classifier by ID!");
        }
    }
}
