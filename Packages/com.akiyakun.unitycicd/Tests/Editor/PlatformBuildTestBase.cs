using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using unicicd.Editor;
using UnityEngine;
using UnityEngine.TestTools;

public abstract class PlatformBuildTestBase<T> where T : IPlatformBuild, new()
{
    public T CreatePlatformBuild()
    {
        var value = new T();
        value.Initialize(new CICDBuildOptions());
        return value;
    }

    public abstract void PlatformName();
}