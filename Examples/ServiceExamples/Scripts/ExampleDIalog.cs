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
using IBM.Watson.DeveloperCloud.Services.Dialog.v1;

public class ExampleDialog : MonoBehaviour
{
	private Dialog m_Dialog = new Dialog();
	private string m_DialogID = "a4015960-39c2-4d6b-9571-38c74aecfffd";

	void Start ()
	{
		Debug.Log("User: 'Hello'");
		m_Dialog.Converse(m_DialogID, "Hello", OnConverse);
	}

	private void OnConverse(ConverseResponse resp)
	{
		foreach (string r in resp.response)
			Debug.Log("Watson: " + r);
	}
}
