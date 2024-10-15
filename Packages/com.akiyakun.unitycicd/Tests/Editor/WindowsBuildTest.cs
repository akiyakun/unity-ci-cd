#if UNITY_STANDALONE_WIN
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using unicicd.Editor;

public class WindowsBuildTest : PlatformBuildTestBase<WindowsPlatformBuild>
{
    [Test]
    public override void PlatformName()
    {
        var test = CreatePlatformBuild();
        Assert.That(test.PlatformName, Is.EqualTo("Windows"));
    }

}
#endif