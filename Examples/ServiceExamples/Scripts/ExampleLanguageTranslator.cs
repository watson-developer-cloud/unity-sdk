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

public class ExampleLanguageTranslator : MonoBehaviour
{
    private string m_PharseToTranslate = "Hello, welcome to IBM Watson!";
    private string _username;
    private string _password;
    private string _url;
    private fsSerializer _serializer = new fsSerializer();
    private string _token = "<authentication-token>";

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
        _username = vcapCredentials.VCAP_SERVICES["language_translator"][0].Credentials.Username.ToString();
        _password = vcapCredentials.VCAP_SERVICES["language_translator"][0].Credentials.Password.ToString();
        _url = vcapCredentials.VCAP_SERVICES["language_translator"][0].Credentials.Url.ToString();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        //  Or authenticate using token
        //Credentials credentials = new Credentials(_url)
        //{
        //    AuthenticationToken = _token
        //};

        LanguageTranslator languageTranslator = new LanguageTranslator(credentials);

        if (!languageTranslator.GetTranslation(m_PharseToTranslate, "en", "es", OnGetTranslation))
            Log.Debug("ExampleLanguageTranslator", "Failed to translate!");
    }

    private void OnGetTranslation(Translations translation, string data)
    {
        Log.Debug("ExampleLanguageTranslator", "Langauge Translator - Translate Response: {0}", data);
    }
}
