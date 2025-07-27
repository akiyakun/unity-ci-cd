# GitHub Actions



[TOC]

# Setup

## Self-Hosted Runner の設定

Self-Hosted Runner はユーザーが用意したローカルPCでGitHub Actionsを無料で実行することができるRunnerです。
1つの Self-Hosted Runner で複数のリポジトリを管理する場合は Organization で管理する必要があるようです(未テスト)
それ以外の場合はリポジトリ毎に Self-Hosted Runner を用意し設定する必要があります。
こちらの場合はフォルダを分けてセットアップするだけです。

`Settings > Actions > Runners`から追加します。
設定方法が表示されるので基本的に手順通りに実行してください。

### Windows

PowerShell用のコマンドが表示されているので必ずPowerShellから実行してください。
`Download`項目はそれぞれコマンドを実行するだけです。
`Configure`項目は最初のコマンドは全部Enterで進めて大丈夫です。

## Workflow定義ファイル(.yaml)の用意

ワークフローは、リポジトリ内の `.github/workflows` ディレクトリを作成し.yamlを配置します。
テンプレートを用意しているのでそちらをコピーして使うとよいです。
テンプレートはパッケージ内の以下の場所にあります(Unity上では表示されないのでエクスプローラで直接フォルダを開いてください)
`Packages\com.akiyakun.unitycicd\Templates~\GitHub Actions`

詳細は[テンプレートファイル](#テンプレートファイル)を参照してください。

# テンプレートファイル



# 環境変数

`Settings > Security > Secrets and variables > Actions > Variables > Repository variables`
から環境変数を追加してください。

| Name                  | Value | 説明                                                         |
| --------------------- | ----- | ------------------------------------------------------------ |
| ENABLE_WINDOWS_RUNNER | true  | WindowsのRunnerを使う場合に定義してください。                |
| ENABLE_MACOS_RUNNER   | true  | macOSのRunnerを使う場合に定義してください。                  |
| PROJECT_DIR_NAME      |       | モノレポ構成の場合の対象のUnityプロジェクトフォルダ(相対パス)を設定します。 |

