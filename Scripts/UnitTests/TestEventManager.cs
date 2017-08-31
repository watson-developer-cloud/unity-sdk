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

using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestEventManager : UnitTest
    {
        EventManager m_Manager = new EventManager();
        bool m_SendTested = false;
        bool m_SendAsyncTested = false;

        public override IEnumerator RunTest()
        {
            m_Manager.RegisterEventReceiver("SendEvent", OnSendEvent);
            m_Manager.RegisterEventReceiver("SendAsyncEvent", OnSendAsyncEvent);

            m_Manager.SendEvent("SendEvent");
            while (!m_SendTested)
                yield return null;

            m_Manager.SendEventAsync("SendAsyncEvent");
            while (!m_SendAsyncTested)
                yield return null;

            yield break;
        }

        private void OnSendEvent(object[] args)
        {
            Log.Status("TestEventManager", "OnSendEvent()");
            m_SendTested = true;
        }

        private void OnSendAsyncEvent(object[] args)
        {
            Log.Status("TestEventManager", "OnSendAsyncEvent()");
            m_SendAsyncTested = true;
        }
    }
}
