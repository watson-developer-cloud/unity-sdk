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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using IBM.Watson.Logging;
using System.Collections.Generic;

namespace IBM.Watson.Utilities
{
    /// <summary>
    /// Singleton class for sending and receiving events.
    /// </summary>
    public class EventManager
    {
        #region Public Properties
        public static EventManager Instance { get { return Singleton<EventManager>.Instance; } }
        #endregion

        #region Public Types
        public delegate void OnReceiveEvent(object[] args);
        #endregion

        #region Public Functions
        public void RegisterEventReceiver(string eventName, OnReceiveEvent callback)
        {
			if (!m_EventMap.ContainsKey (eventName))
				m_EventMap.Add (eventName, new List<OnReceiveEvent> (){callback});
			else	
            	m_EventMap[eventName].Add(callback);
        }

        public void UnregisterAllEventReceivers()
        {
            m_EventMap.Clear();
        }

        public void UnregisterEventReceivers(string eventName)
        {
            m_EventMap.Remove(eventName);
        }

        public void UnregisterEventReceiver(string eventName, OnReceiveEvent callback)
        {
            if (m_EventMap.ContainsKey(eventName))
                m_EventMap[eventName].Remove(callback);
        }

        public bool SendEvent(string eventName, params object[] args)
        {
            List<OnReceiveEvent> receivers = null;
            if (m_EventMap.TryGetValue(eventName, out receivers))
            {
                for (int i = 0; i < receivers.Count; ++i)
                {
                    if (receivers[i] == null)
                    {
                        Log.Warning("EventManager", "Removing invalid event receiver.");
                        receivers.RemoveAt(i--);
                        continue;
                    }
                    receivers[i](args);
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Private Data
        private Dictionary<string, List<OnReceiveEvent>> m_EventMap = new Dictionary<string, List<OnReceiveEvent>>();
        #endregion

		#region Public Static - Event Names

		//Mood Changes
		public static string onMoodChange = "onMoodChange";
		public static string onMoodChangeFinish = "onMoodChangeFinish";

		//Behavior Changes
		public static string onBehaviorChange = "onBehaviorChange";
		public static string onBehaviorChangeFinish = "onBehaviorChangeFinish";

		#endregion
    }
	
}
