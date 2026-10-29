# unity-ci-cd

Unity 用の CI/CD パッケージです。目的は、Unity プロジェクトに導入するだけで簡単にビルド・テストの CI/CD をセットアップして使えるようにすることです。
このリポジトリ自体も Unity プロジェクトで、パッケージを組み込んで動作確認できるようになっています。

## 構成
* `Packages/com.akiyakun.unitycicd` : パッケージ本体
  * `Editor/` : ビルドメニュー、コマンドラインビルド、設定読み込みなど
  * `External~/tools` : ビルド・テスト用スクリプト。Editor 起動時に `CICDSetup` がプロジェクト直下の `tools/` にコピーします
    * プロジェクトの `tools/cicd/cicd_version` と `CICDConfig.CurrentVersion` が異なるとき(またはファイルが無いとき)に上書きコピーします
  * `External~/cicd_config.json` : プロジェクト直下に `cicd_config.json` が無いときだけコピーされる設定ファイルの雛形
  * `Templates~/` : CI 設定のテンプレート(GitHub Actions、GitLab CI)
* `tools/`、`cicd_config.json`(リポジトリ直下) : このリポジトリ自身の Unity プロジェクト用にコピーされたもの
* `doc/` : GitHub Actions のセットアップ手順

`~` の付いたフォルダは Unity からは見えません。

## バージョンを上げるとき
`External~/tools` の中身を変えたら、既存プロジェクトに配布されるようにバージョンを上げます。次をすべて同じ値にそろえます。
* `Packages/com.akiyakun.unitycicd/Editor/CICDConfig.cs` の `CurrentVersion`
* `Packages/com.akiyakun.unitycicd/External~/tools/cicd/cicd_version`(改行なし)
* `Packages/com.akiyakun.unitycicd/External~/cicd_config.json` の `"version"`
* リポジトリ直下の `cicd_config.json` と `tools/cicd/cicd_version`

あわせて `package.json` の `version` も上げ、パッケージの README に変更点を書きます。
`CICDConfig.Load()` はプロジェクトの `cicd_config.json` の `"version"` とも比較してエラーログを出すので、既存プロジェクトには README の「更新手順」で書き換えてもらいます。

## ドキュメント
* [.claude/docs/gitlab-ci.md](.claude/docs/gitlab-ci.md) : GitLab CI テンプレートの使い方と各設定の理由
* [.claude/docs/gitlab-runner-setup.md](.claude/docs/gitlab-runner-setup.md) : Windows で gitlab-runner をセットアップする手順
* [.claude/docs/troubleshooting.md](.claude/docs/troubleshooting.md) : CI で実際に起きた症状と原因
