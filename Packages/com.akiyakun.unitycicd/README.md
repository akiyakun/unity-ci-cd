# UnityCICD

# 導入手順
Packageを導入しUnityを起動します。

プロジェクトルートに`cicd_config.json`ファイルが存在するのを確認します。
存在しない場合はパッケージ側にある `Packages/com.akiyakun.unitycicd/External~/cicd_config.json` ファイルを手動でコピーしてください。

Player設定でシンボル`__USE_UNICICD_BUILDMENU__`を追加します。

`Assets/Editor`フォルダにProjectウィンドウの右クリックメニューから`Create > App > CICD > CICDBuildSettings`を選択し、ビルド設定ファイルを作成します。

# 更新手順(既存プロジェクト)
パッケージを更新したら以下を行います。
1. プロジェクト直下の`cicd_config.json`の`"version"`をパッケージのバージョン(`CICDConfig.CurrentVersion`)に書き換えます。書き換えるまではUnity起動時にエラーログが出ます。
2. `tools/`フォルダはUnity Editor起動時に自動で上書きコピーされます(`tools/cicd/cicd_version`がパッケージのバージョンと異なる場合)。プロジェクト側で変更したファイルは上書きされるので注意してください。
3. `cicd_config.json`の`runners > unity_path`を現在使っているUnityのパスに合わせます。

## 1.2.1
* `run_tests.py` : Unityのログを`Logs/unity_tests.log`に出力して末尾を表示、Unityの終了コードをそのまま返す、`unity_path`が無い場合に案内を表示、引数のカンマ抜けを修正
* `run_tests.bat` / `run_tests.py` : テストアセンブリを複数指定できるようにした(`run_tests.bat PlayMode A.Tests;B.Tests`)
* GitLab CI 用のテンプレート`Templates~/GitLab CI/gitlab-ci.yml`を追加


## 対応
* Android
* iOS
* Windows
* macOS

## Testについて
テストジョブは用意していますが、テスト用のライブラリやアセットはこのプロジェクトでは扱いません。

## CIテンプレート
`Templates~` フォルダにCI設定のテンプレートがあります(Unity上では表示されないのでエクスプローラで直接開いてください)。
* `Templates~/GitHub Actions` : GitHub Actions 用（詳細は `doc/GitHub Actions.md`）
* `Templates~/GitLab CI/gitlab-ci.yml` : GitLab CI/CD 用（Windows の shell Runner で PlayMode テストを実行。リポジトリ直下に `.gitlab-ci.yml` としてコピーして使う）

# シンボル
__DEBUG__
デバッグビルドのときに定義される。

__PUBLISH__
本番用のときに定義される。

__TESTS__
テストビルドのときに定義される。

__USE_UNICICD_BUILDMENU__
本ライブラリで用意しているビルドメニューを使用する。


