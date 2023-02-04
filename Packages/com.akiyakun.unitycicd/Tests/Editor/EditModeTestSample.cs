using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

public class EditModeTestSample : MonoBehaviour
{
    // Regular NUnit test (works in Edit mode and Play mode)
    [Test]
    public void Test()
    {
        Debug.Log("EditMode: Test()");
    }

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