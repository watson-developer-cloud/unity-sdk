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
using FullSerializer;
using IBM.Watson.DeveloperCloud.Utilities;

namespace IBM.Watson.DeveloperCloud.Services.Conversation.v1
{
	/// <summary>
	/// This class wraps the Watson Conversation service. 
	/// <a href="http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/conversation.html">Conversation Service</a>
	/// </summary>
	public class Conversation : IWatsonService {
		#region Public Types
		#endregion

		#region Public Properties
		#endregion

		#region Private Data
		private const string SERVICE_ID = "ConversationV1";
		private static fsSerializer sm_Serializer = new fsSerializer();
		#endregion

		#region IWatsonService implementation

		/// <exclude />
		public string GetServiceID()
		{
			return SERVICE_ID;
		}

		/// <exclude />
		public void GetServiceStatus(ServiceStatus callback)
		{
			/*if (Config.Instance.FindCredentials(SERVICE_ID) != null)
				new CheckServiceStatus(this, callback);
			else
			{
				if (callback != null && callback.Target != null)
				{
					callback(SERVICE_ID, false);
				}
			}*/
		}

		/*private class CheckServiceStatus
		{
			private Conversation m_Service = null;
			private ServiceStatus m_Callback = null;
			private int m_DialogCount = 0;

			public CheckServiceStatus(Conversation service, ServiceStatus callback)
			{
				m_Service = service;
				m_Callback = callback;

				string customServiceID = Config.Instance.GetVariableValue(SERVICE_ID + "_ID");

				//If custom classifierID is defined then we are using it to check the service health
//				if (!string.IsNullOrEmpty(customServiceID))
//				{
//
//					if (!m_Service.Converse(customServiceID, "Hello", OnDialog))
//						OnFailure("Failed to invoke Converse().");
//					else
//						m_DialogCount += 1;
//				}
//				else
//				{
//					if (!m_Service.GetDialogs(OnGetDialogs))
//						OnFailure("Failed to invoke GetDialogs().");
//				}


			}

//			private void OnGetDialogs(Dialogs dialogs)
//			{
//				if (m_Callback != null)
//				{
//					foreach (var dialog in dialogs.dialogs)
//					{
//						if (!m_Service.Converse(dialog.dialog_id, "Hello", OnDialog))
//							OnFailure("Failed to invoke Converse().");
//						else
//							m_DialogCount += 1;
//					}
//				}
//				else
//					OnFailure("GetDialogs() failed.");
//			}
//
//			private void OnDialog(ConverseResponse resp)
//			{
//				if (m_DialogCount > 0)
//				{
//					m_DialogCount -= 1;
//					if (resp != null)
//					{
//						if (m_DialogCount == 0 && m_Callback != null && m_Callback.Target != null)
//							m_Callback(SERVICE_ID, true);
//					}
//					else
//						OnFailure("ConverseResponse is null.");
//				}
//			}
//
//			private void OnFailure(string msg)
//			{
//				Log.Error("Dialog", msg);
//				m_Callback(SERVICE_ID, false);
//				m_DialogCount = 0;
//			}
		}
*/
		#endregion


	}
}
