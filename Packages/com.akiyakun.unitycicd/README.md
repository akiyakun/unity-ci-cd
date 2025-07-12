# Unity CI-CD

# 導入手順
Packageを導入しUnityを起動します。

プロジェクトルートに`ci-cd_config.json`ファイルが存在するのを確認します。
存在しない場合はパッケージ側にある `Packages/com.akiyakun.unitycicd/External~/ci-cd_config.json` ファイルを手動でコピーしてください。

Player設定でシンボル`__USE_UNICICD_BUILDMENU__`を追加します。

`Assets/Editor`フォルダにProjectウィンドウの右クリックメニューから`Create > App > CICD > CICDBuildSettings`を選択し、ビルド設定ファイルを作成します。


## 対応
* Android
* iOS
* Windows
* macOS

## Testについて
テストジョブは用意していますが、テスト用のライブラリやアセットはこのプロジェクトでは扱いません。

# シンボル
__DEBUG__
デバッグビルドのときに定義される。

__PUBLISH__
本番用のときに定義される。

__TESTS__
テストビルドのときに定義される。

__USE_UNICICD_BUILDMENU__
本ライブラリで用意しているビルドメニューを使用する。


