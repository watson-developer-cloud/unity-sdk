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
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using FullSerializer;
using System.IO;
using System;
using System.Collections;

public class ExampleLanguageTranslator : MonoBehaviour
{
    private string _pharseToTranslate = "Hello, welcome to IBM Watson!";
    private string _username;
    private string _password;
    private string _url;
    //private string _token = "<authentication-token>";
    private fsSerializer _serializer = new fsSerializer();

    private LanguageTranslator _languageTranslator;

    private bool _getTranslationTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        VcapCredentials vcapCredentials = new VcapCredentials();
        fsData data = null;

        //  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
        //  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
        var environmentalVariable = Environment.GetEnvironmentVariable("VCAP_SERVICES");
        var fileContent = File.ReadAllText(environmentalVariable);

        //  Add in a parent object because Unity does not like to deserialize root level collection types.
        fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

        //  Convert json to fsResult
        fsResult r = fsJsonParser.Parse(fileContent, out data);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsResult to VcapCredentials
        object obj = vcapCredentials;
        r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Set credentials from imported credntials
        Credential credential = vcapCredentials.VCAP_SERVICES["language_translator"][0].Credentials;
        _username = credential.Username.ToString();
        _password = credential.Password.ToString();
        _url = credential.Url.ToString();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        //  Or authenticate using token
        //Credentials credentials = new Credentials(_url)
        //{
        //    AuthenticationToken = _token
        //};

        _languageTranslator = new LanguageTranslator(credentials);

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        if (!_languageTranslator.GetTranslation(_pharseToTranslate, "en", "es", OnGetTranslation))
            Log.Debug("ExampleLanguageTranslator", "Failed to translate!");
        while (!_getTranslationTested)
            yield return null;

        Log.Debug("ExampleLanguageTranslator", "Language Translator examples complete.");
    }

    private void OnGetTranslation(Translations translation, string data)
    {
        Log.Debug("ExampleLanguageTranslator", "Langauge Translator - Translate Response: {0}", data);
        _getTranslationTested = true;
    }
}
