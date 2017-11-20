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
using IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v2;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;

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
        _languageTranslation.GetTranslation(OnGetTranslation, OnFail, _pharseToTranslate, "en", "es");
    }

    private void OnGetTranslation(Translations translation, Dictionary<string, object> customData)
    {
        if (translation != null && translation.translations.Length > 0)
            Log.Debug("ExampleLangaugeTranslation.OnGetTranslation()", "Spanish Translation: " + translation.translations[0].translation);
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleLangaugeTranslation.OnFail()", "Error received: {0}", error.ToString());
    }
}
