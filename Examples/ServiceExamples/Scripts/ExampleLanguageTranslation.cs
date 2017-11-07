﻿/**
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
using IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v2;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleLanguageTranslation : MonoBehaviour
{
    private LanguageTranslation _languageTranslation;
    private string _username = null;
    private string _password = null;
    private string _url = null;
    private string _pharseToTranslate = "How do I get to the disco?";

    void Start()
    {
        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);
        _languageTranslation = new LanguageTranslation(credentials);

        Log.Debug("ExampleLangaugeTranslation.Start()", "English Phrase to translate: " + _pharseToTranslate);
        _languageTranslation.GetTranslation(_pharseToTranslate, "en", "es", OnGetTranslation);
    }

    private void OnGetTranslation(RESTConnector.ParsedResponse<Translations> resp)
    {
        if (resp.DataObject != null && resp.DataObject.translations.Length > 0)
            Log.Debug("ExampleLangaugeTranslation.OnGetTranslation()", "Spanish Translation: " + resp.DataObject.translations[0].translation);
    }
}
