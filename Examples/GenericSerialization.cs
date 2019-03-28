/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;
using Newtonsoft.Json;
using IBM.Watson.Assistant.V2.Model;

namespace IBM.Watson.Examples
{
    public class GenericSerialization : MonoBehaviour
    {
        private string responseJson = "{\"output\":{\"generic\":[{\"response_type\":\"text\",\"text\":\"Let me confirm: You want an appointment for Friday at 12 PM. Is this correct?\"}],\"intents\":[{\"intent\":\"Customer_Care_Appointments\",\"confidence\":0.642203044891357}],\"entities\":[{\"entity\":\"sys-date\",\"location\":[0,9],\"value\":\"2019-01-11\",\"confidence\":1,\"metadata\":{\"calendar_type\":\"GREGORIAN\",\"timezone\":\"GMT\"}},{\"entity\":\"reply\",\"location\":[10,16],\"value\":\"yes\",\"confidence\":1}]},\"context\":{\"global\":{\"system\":{\"turn_count\":5,\"skill_reference_id\":\"dc762f48-e10c-4418-92c1-c7be6feadbc0\"}},\"skills\":{\"main skill\":{\"user_defined\":{\"no_reservation\":true,\"time\":\"12:00:00\",\"name\":\"Watson\",\"date\":\"2019-01-11\"}}}}}";

        void Start()
        {
            LogSystem.InstallDefaultReactors();

            MessageResponse messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseJson);
            //Dictionary<string, object> e = Json.Deserialize(responseJson) as Dictionary<string, object>;
            //Dictionary<string, object> context = e["context"] as Dictionary<string, object>;
            //Dictionary<string, object> skills = context["skills"] as Dictionary<string, object>;
            //Dictionary<string, object> main_skill = skills["main skill"] as Dictionary<string, object>;
            //Dictionary<string, object> user_defined = main_skill["user_defined"] as Dictionary<string, object>;

            //string name = user_defined["name"] as string;

            //var user_defined = messageResponse.Context.Skills["main skill"]["user_defined"].ToString();
            //var uDefinedObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(user_defined);

            //Log.Debug("GenericSerialization", "main skill: {0}", uDefinedObject["name"]);
            //Log.Debug("GenericSerialization", "test: {0}", messageResponse);

            var name = messageResponse.Context.Skills["main skill"]["user_defined"]["name"].ToString();
            Log.Debug("GenericSerialization", "name: {0}", name);

        }

        public T GetValueFromGeneric<T>(Dictionary<string, object> obj)
        {
            T response = default(T);
            return response;
        }
    }
}
