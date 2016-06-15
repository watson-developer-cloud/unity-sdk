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
using IBM.Watson.DeveloperCloud.Services.AlchemyLanguage.v1;
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleAlchemyLanguage : MonoBehaviour {
    private AlchemyLanguage m_AlchemyLanguage = new AlchemyLanguage();
    private string m_ExampleURL = "https://developer.ibm.com/open/2016/01/21/introducing-watson-unity-sdk/";

	void Start () {
        LogSystem.InstallDefaultReactors();

	    //  Get Author URL POST
//        if(!m_AlchemyLanguage.GetAuthors(OnGetAuthors, m_ExampleURL, true))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors URL POST!");

        //  Get Author URL GET
//        if(!m_AlchemyLanguage.GetAuthors(OnGetAuthors, m_ExampleURL))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors URL GET!");

        //  Get Author HTML POST
//        string exampleHTMLPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/AlchemyTest.html";
//        if(!m_AlchemyLanguage.GetAuthors(exampleHTMLPath, OnGetAuthors, "http://www.google.com", true))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors HTML POST!");

        //  Get Author HTML GET
        string exampleHTMLPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/AlchemyTest.html";
        if(!m_AlchemyLanguage.GetAuthors(exampleHTMLPath, OnGetAuthors, "http://www.cnn.com", false))
            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors HTML GET!");
	}
	
    private void OnGetAuthors(AuthorsData authors, string data)
    {
        if(authors != null)
        {
            Log.Debug("ExampleAlchemyLanguage", "data: {0}", data);
            if(authors.authors.names.Length == 0)
                Log.Debug("ExampleAlchemyLanguage", "No authors found!");

            foreach(string name in authors.authors.names)
                Log.Debug("ExampleAlchemyLanguage", "Author " + name + " found!");
        }
        else
        {
            Log.Debug("ExampleAlchemyLanguage", "Failed to find Author!");
        }
    }
}
