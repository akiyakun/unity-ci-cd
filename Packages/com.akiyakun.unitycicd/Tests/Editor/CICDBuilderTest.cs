using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using unicicd.Editor;

public class CICDBuilderTest
{
    // テスト用のビルドオプションを生成
    public static CICDBuildOptions CreateTestBuildOptions()
    {
        return new CICDBuildOptions();
    }

    // テスト用のビルダーを生成
    public static CICDBuilder CreateTestBuilder()
    {
        var builder = new CICDBuilder();
        if (builder.Initialize(CreateTestBuildOptions()) == false)
        {
            Assert.Fail("Failed to Initialize CICDBuilder");
        }
        return builder;
    }

    // ビルダーの初期化テスト
    [Test]
    public void InitializeTest()
    {
        var builder = new CICDBuilder();
        Assert.IsTrue(builder.Initialize(CreateTestBuildOptions()));
    }

    // 各ビルドモードのルートパス確認テスト
    [Test]
    public void WorkingBuildModeRootDirectoryTest()
    {
        foreach (object value in System.Enum.GetValues(typeof(CICDBuildMode)))
        {
            var str = CICDBuilder.GetWorkingBuildModeRootDirectory((CICDBuildMode)value);
            Assert.IsFalse(string.IsNullOrEmpty(str), $"BuildMode.{value.ToString()} is Null or Empty");
        }
    }

    // ワーキングビルドディレクトリのPathテスト
    [Test]
    public void WorkingBuildDirectoryTest()
    {
        var builder = CreateTestBuilder();
        Assert.That(builder.WorkingBuildDirectory, Is.EqualTo(
            CICDBuilder.BuildRootDirectory + "/current/Mock_Test"
        ));
    }

    // 必須シーンテスト
    [Test]
    public void RequiredSceneListTest()
    {
        var builder = new CICDBuilder();
        var options = CreateTestBuildOptions();
        bool isCalled = false;

        options.InAppDebug = false;

        options.OnBeforeBuildProcess += (CICDBuilder builder, BuildPlayerOptions bpo) => {
            // Debug.Log("OnBeforeBuildProcess");
            isCalled = true;

            Assert.That(bpo.scenes.Length, Is.EqualTo(3));

            Assert.That(bpo.scenes[0], Is.EqualTo("Assets/Scenes/Required_01_Scene.unity"));
            Assert.That(bpo.scenes[1], Is.EqualTo("Assets/Scenes/Required_02_Scene.unity"));
            Assert.That(bpo.scenes[2], Is.EqualTo("Assets/Scenes/Required_03_Scene.unity"));

            // ビルドする必要は無いのでキャンセル
            return false;
        };

        Assert.IsTrue(builder.Initialize(options));
        builder.Build();

        Assert.IsTrue(isCalled);
    }

    // InAppDebugシーンテスト
    [Test]
    public void InAppDebugSceneListTest()
    {
        var builder = new CICDBuilder();
        var options = CreateTestBuildOptions();
        bool isCalled = false;

        options.InAppDebug = true;

        options.OnBeforeBuildProcess += (CICDBuilder builder, BuildPlayerOptions bpo) =>
        {
            // Debug.Log("OnBeforeBuildProcess");
            isCalled = true;

            Assert.That(bpo.scenes.Length, Is.EqualTo(5));

            Assert.That(bpo.scenes[0], Is.EqualTo("Assets/Scenes/Required_01_Scene.unity"));
            Assert.That(bpo.scenes[1], Is.EqualTo("Assets/Scenes/Required_02_Scene.unity"));
            Assert.That(bpo.scenes[2], Is.EqualTo("Assets/Scenes/Required_03_Scene.unity"));

            Assert.That(bpo.scenes[3], Is.EqualTo("Assets/Scenes/InAppDebug_01_Scene.unity"));
            Assert.That(bpo.scenes[4], Is.EqualTo("Assets/Scenes/InAppDebug_02_Scene.unity"));

            // ビルドする必要は無いのでキャンセル
            return false;
        };

        Assert.IsTrue(builder.Initialize(options));
        builder.Build();

        Assert.IsTrue(isCalled);
    }

}