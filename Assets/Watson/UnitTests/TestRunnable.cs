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
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestRunnable : UnitTest
    {
        bool m_CoroutineRan = false;

        public override IEnumerator RunTest()
        {
            Runnable.Run(TestCoroutine("Test"));
            yield return new WaitForSeconds(1.0f);

            Test(m_CoroutineRan);
            yield break;
        }

        private IEnumerator TestCoroutine(string a_Argument)
        {
            Test(a_Argument == "Test");
            m_CoroutineRan = true;
            yield break;
        }
    }
}

