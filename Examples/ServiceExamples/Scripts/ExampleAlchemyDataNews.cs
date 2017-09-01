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
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;

public class ExampleAlchemyDataNews : MonoBehaviour
{
    private string _apikey = null;
    private string _url = null;

    private AlchemyAPI _alchemyAPI = null;

    private bool _getNewsTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_apikey, _url);

        _alchemyAPI = new AlchemyAPI(credentials);

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        Dictionary<string, string> queryFields = new Dictionary<string, string>();
        queryFields.Add(Fields.EnrichedUrlRelationsRelationSubjectText, "Obama");
        queryFields.Add(Fields.EnrichedUrlCleanedtitle, "Washington");
        string[] returnFields = { Fields.EnrichedUrlEntities, Fields.EnrichedUrlKeywords };
        if (!_alchemyAPI.GetNews(OnGetNews, returnFields, queryFields))
            Log.Debug("ExampleAlchemyDataNews", "Failed to get news!");

        while (!_getNewsTested)
            yield return null;

        Log.Debug("ExampleAlchemyDataNews", "Alchemy data news examples complete!");
    }

    private void OnGetNews(NewsResponse newsData, string data)
    {
        Log.Debug("ExampleAlchemyDataNews", "Alchemy data news - Get news Response: {0}", data);
        _getNewsTested = true;
    }
}
