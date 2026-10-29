# UnityCICD

# 導入手順
Packageを導入しUnityを起動します。

プロジェクトルートに`cicd_config.json`ファイルが存在するのを確認します。
存在しない場合はパッケージ側にある `Packages/com.akiyakun.unitycicd/External~/cicd_config.json` ファイルを手動でコピーしてください。

Player設定でシンボル`__USE_UNICICD_BUILDMENU__`を追加します。

`Assets/Editor`フォルダにProjectウィンドウの右クリックメニューから`Create > App > CICD > CICDBuildSettings`を選択し、ビルド設定ファイルを作成します。


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


