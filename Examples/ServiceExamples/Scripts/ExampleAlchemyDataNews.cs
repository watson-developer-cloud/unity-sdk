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
using FullSerializer;
using System.IO;
using System;

public class ExampleAlchemyDataNews : MonoBehaviour
{
    private string _apikey;
    private string _url;
    private fsSerializer _serializer = new fsSerializer();

    private AlchemyAPI _alchemyAPI;

    private bool _getNewsTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //VcapCredentials vcapCredentials = new VcapCredentials();
        //fsData data = null;

        ////  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
        ////  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
        //var environmentalVariable = Environment.GetEnvironmentVariable("VCAP_SERVICES");
        //var fileContent = File.ReadAllText(environmentalVariable);

        ////  Add in a parent object because Unity does not like to deserialize root level collection types.
        //fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

        ////  Convert json to fsResult
        //fsResult r = fsJsonParser.Parse(fileContent, out data);
        //if (!r.Succeeded)
        //    throw new WatsonException(r.FormattedMessages);

        ////  Convert fsResult to VcapCredentials
        //object obj = vcapCredentials;
        //r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
        //if (!r.Succeeded)
        //    throw new WatsonException(r.FormattedMessages);

        ////  Set credentials from imported credntials
        //Credential credential = vcapCredentials.VCAP_SERVICES["alchemy_api"][0].Credentials;
        //_apikey = credential.Apikey.ToString();
        //_url = credential.Url.ToString();

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
