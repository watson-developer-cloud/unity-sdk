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

using IBM.Watson.Assistant.V2;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class AssistantV2IntegrationTests
    {
        public AssistantService service;

        //// A Test behaves as an ordinary method
        //[Test]
        //public void AssistantV2IntegrationTestsSimplePasses()
        //{
        //    // Use the Assert class to test conditions
        //}

        //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        //// `yield return null;` to skip a frame.
        //[UnityTest, Order(0)]
        //public IEnumerator AssistantV2IntegrationTestsWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
        [SetUp]
        public void TestSetup()
        {
            LogSystem.InstallDefaultReactors();
            service = new AssistantService("2019-02-18");
        }

        [UnityTest, Order(0)]
        public void TestCreateSession()
        {

        }

        [TearDown]
        public void TestTearDown()
        {

        }
    }
}
