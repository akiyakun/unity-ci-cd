using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditModeTestSample// : MonoBehaviour
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
        Debug.Log("EditMode: Test()");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
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