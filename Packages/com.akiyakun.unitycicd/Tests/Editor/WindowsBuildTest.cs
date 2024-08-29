using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using unicicd.Editor.Build;

public class WindowsBuildTest : PlatformBuildTestBase<WinPlatformBuild>
{
    [Test]
    public override void PlatformName()
    {
        var test = CreatePlatformBuild();
        Assert.That(test.PlatformName, Is.EqualTo("Win"));
    }

}