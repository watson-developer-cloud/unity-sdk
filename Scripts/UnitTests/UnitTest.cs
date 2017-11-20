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

//  Uncomment to print the current service endpoint.
//#define PRINT_ENDPOINT

using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    /// <summary>
    /// This is the base class for all UnitTest classes. Derive from this class and implement the RunTest() function.
    /// </summary>
    public abstract class UnitTest
    {
        /// <summary>
        /// Which credential should the tests use. Default is 0.
        /// </summary>
        public int TestCredentialIndex = 0;

        public bool TestFailed { get; set; }

        public abstract IEnumerator RunTest();

        protected string _url;

        /// <summary>
        /// Utility function that fails this test if false.
        /// </summary>
        /// <param name="condition">If false then the test is failed and the failure is logged.</param>
        public void Test(bool condition)
        {
            if (!condition)
            {
                Log.Error("UnitTest.Test()", "UnitTest {0} has failed, Stack:\n{1}", GetType().Name, StackTraceUtility.ExtractStackTrace());
                TestFailed = true;
            }
            else
            {
                Log.Status("UnitTest.Test()", "UnitTest {0} has passed.", GetType().Name);
#if PRINT_ENDPOINT
                Log.Status("UnitTest.Test()", "endpoint: {0}", _url);
#endif
            }
        }

        public virtual string ProjectToTest()
        {
            return null;
        }
    }
}
