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
    private string m_ExampleText = "After several months of work we are happy to present the Watson Unity SDK, an SDK to enable the Unity community to access the Watson Developer Cloud and build a cognitive application in Unity.\n\nI’ve been involved in the game industry for 22+ years now, but I can tell you in all honestly working in the Watson Innovation Labs and on this project has been the highlight of my career. This SDK really represents the first phase in what we plan to bring to the community, which in the end will be a framework for building a full cognitive application.\n\nYou as a developer will find very simple C# service abstractions for accessing Dialog, Speech To Text, Text to Speech, Language Translation, and Natural Language Classification services. Additionally, we’ve implemented something we are calling a Widget, which has inputs and outputs and performs some basic function.\n\nThese widgets can be connected together to form a graph for the data that’s routed for a given cognitive application. The widgets will attempt to automatically connect to each other, or you as a developer can override that behavior and manually take control of the process. We’ve implemented widgets for all of the basic services and provide a couple of example applications showing how they can work together.\n\nWe plan to continue to expand and build on the Watson Unity SDK. We invite you to join us in contributing to and using the SDK, or by simply giving us your feedback. We’d love to have you on board for this ride!";

	void Start () {
        LogSystem.InstallDefaultReactors();
        string exampleHTMLPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/AlchemyTest.html";

        //  Get Author URL POST
//        if(!m_AlchemyLanguage.GetAuthors(OnGetAuthors, m_ExampleURL, true))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors URL POST!");

        //  Get Author URL GET
//        if(!m_AlchemyLanguage.GetAuthors(OnGetAuthors, m_ExampleURL))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors URL GET!");

        //  Get Author HTML POST
//        if(!m_AlchemyLanguage.GetAuthors(exampleHTMLPath, OnGetAuthors, m_ExampleURL))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get authors HTML POST!");

        //  Get Concepts URL GET
//        if(!m_AlchemyLanguage.GetRankedConceptsURL(OnGetConcepts, m_ExampleURL))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts URL Get!");

        //  Get Concepts Text POST
//        if(!m_AlchemyLanguage.GetRankedConceptsText(OnGetConcepts, m_ExampleText, m_ExampleURL))
//            Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts Text POST!");

        //  Get Concepts HTML POST
        if(!m_AlchemyLanguage.GetRankedConceptsHTML(OnGetConcepts, exampleHTMLPath, m_ExampleURL))
            Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts HTML POST!");
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

    private void OnGetConcepts(ConceptsData concepts, string data)
    {
        if(concepts != null)
        {
            Log.Debug("ExampleAlchemyLanguage", "status: {0}", concepts.status);
            Log.Debug("ExampleAlchemyLanguage", "url: {0}", concepts.url);
            Log.Debug("ExampleAlchemyLanguage", "totalTransactions: {0}", concepts.totalTransactions);
            Log.Debug("ExampleAlchemyLanguage", "language: {0}", concepts.language);
            if(concepts.concepts.Length == 0)
                Log.Debug("ExampleAlchemyLanguage", "No concepts found!");

            foreach(Concept concept in concepts.concepts)
                Log.Debug("ExampleAlchemyLanguage", "Concept: {0}, Relevance: {1}", concept.text, concept.relevance);
        }
        else
        {
            Log.Debug("ExampleAlchemyLanguage", "Failed to find Concepts!");
        }
    }
}
