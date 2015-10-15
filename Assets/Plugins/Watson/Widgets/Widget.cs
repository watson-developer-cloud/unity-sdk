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

using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.Editor
{
	public enum IOType{
		Text,
		AudioFromMicrophone,
		AudioClipData
	}

	public enum WidgetState{
		Waiting = 0,
		GettingInput,
		Working,
		SendingOutput
	}

	public static class IOTypeExtensions
	{
		public static string ToFriendlyString(this IOType me)
		{
			string stringValue = null;
			switch(me)
			{
			case IOType.Text:
				stringValue= "Text";		break;
			case IOType.AudioFromMicrophone:
				stringValue= "Microphone";	break;
			case IOType.AudioClipData:
				stringValue= "Audio Clip";	break;
			default:
				stringValue = "NaN";	break;	
			}
			return stringValue;
		}
	}

	public class WatsonIO{
		public IOType dataType;
		public string name;

		public WatsonIO(IOType dataType, string name){
			this.dataType = dataType;
			this.name = name;
		}
	}

	public class Widget : MonoBehaviour{
		private WidgetState m_state;
		private Dictionary<IOType, System.Action<WatsonIO>> m_inputList = null;
		private Dictionary<IOType, System.Action<WatsonIO>> m_outputList = null;

		public Dictionary<IOType, System.Action<WatsonIO>> inputList{
			get{
				if(m_inputList == null){
					m_inputList = new Dictionary<IOType, System.Action<WatsonIO>>();
				}
				return m_inputList;
			}
		}

		public Dictionary<IOType, System.Action<WatsonIO>> outputList{
			get{
				if(m_outputList == null){
					m_outputList = new Dictionary<IOType, System.Action<WatsonIO>>();
				}
				return m_outputList;
			}
		}

		public WidgetState State{
			get{
				return m_state;
			}
		}

		#if UNITY_EDITOR

		public virtual void SetupWidgetIOForEditor(){

		}

		public virtual void OnInput(WatsonIO watsonIO){

		}

		public virtual void OnOutput(WatsonIO watsonIO){

		}

		#endif
	}

}