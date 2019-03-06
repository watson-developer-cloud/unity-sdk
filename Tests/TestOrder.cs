using System.Collections;
using IBM.Cloud.SDK;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class TestOrder
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return new WaitForSeconds(0.1f);
            LogSystem.InstallDefaultReactors();
            Log.Debug("TestOrder", "Setup");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new WaitForSeconds(0.1f);
            Log.Debug("TestOrder", "TearDown");
        }

        [UnityTest, Order(0)]
        public IEnumerator Test0()
        {
            yield return new WaitForSeconds(0.1f);
            Log.Debug("TestOrder", "Test0");
            Assert.IsTrue(true);
        }

        [UnityTest, Order(1)]
        public IEnumerator Test1()
        {
            yield return new WaitForSeconds(0.1f);
            Log.Debug("TestOrder", "Test1");
            Assert.IsTrue(true);
        }

        [UnityTest, Order(2)]
        public IEnumerator Test2()
        {
            yield return new WaitForSeconds(0.1f);
            Log.Debug("TestOrder", "Test2");
            Assert.IsTrue(true);
        }

        [UnityTest, Order(3)]
        public IEnumerator Test3()
        {
            yield return new WaitForSeconds(0.1f);
            Log.Debug("TestOrder", "Test3");
            Assert.IsTrue(true);
        }
    }
}
