using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

#if __TESTS__
namespace unicicd.RuntimeTests
{
    [Description("説明")]
    public class PlayModeTestSample : MonoBehaviour
    {
        // Regular NUnit test (works in Edit mode and Play mode)
        [Test]
        [Description("説明")]
        public void Test()
        {
            Debug.Log("PlayMode: Test()");
        }

        [Test]
        public void Test2()
        {
            Debug.Log("PlayMode: Test2()");
        }

        [Test]
        public void TestFailer()
        {
            Debug.Log("PlayMode: TestFailer()");
            Assert.IsTrue(true);        // 成功
            // Assert.IsTrue(false);    // 失敗
        }

        // IEnumeratorで非同期で待つ処理等ができる
        // このテストメソッドが毎フレームUpdateのあと、LastUpdateの前に呼ばれる
        // [UnityTest]
        // public IEnumerator UnityTest()
        // {
        //     while (true)
        //     {
        //         yield return null;
        //     }
        // }

        // [Test]
        // //PrebuildSetupAttribute can be skipped because it's implemented in the same class
        // [PrebuildSetup(typeof(TestsWithPrebuildStep))]
        // public void Prebuild()
        // {
        // }

        // // Regular NUnit test (works in Edit mode and Play mode)
        // [Test]
        // [UnityPlatform(RuntimePlatform.Android)]
        // void Test()
        // {
        //     var go = new GameObject("MyGameObject");
        //     Assert.AreEqual("MyGameObject", go.name);
        // }

        // // Example in Play mode
        // [UnityTest]
        // public IEnumerator GameObject_WithRigidBody_WillBeAffectedByPhysics()
        // {
        //     var go = new GameObject();
        //     go.AddComponent<Rigidbody>();
        //     var originalPosition = go.transform.position.y;

        //     yield return new WaitForFixedUpdate();

        //     Assert.AreNotEqual(originalPosition, go.transform.position.y);
        // }

        // // Example in Edit mode:
        // [UnityTest]
        // public IEnumerator EditorUtility_WhenExecuted_ReturnsSuccess()
        // {
        //     var utility = RunEditorUtilityInTheBackgroud();

        //     while (utility.isRunning)
        //     {
        //         yield return null;
        //     }

        //     Assert.IsTrue(utility.isSuccess);
        // }

    }
}
#endif