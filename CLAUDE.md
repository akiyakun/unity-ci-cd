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

## .bat の文字コード
* `.bat` は CP932(Windows の「Shift_JIS」)・CRLF で保存します。日本語の Windows の cmd は `.bat` を CP932 として読むためです。
* リポジトリの中は UTF-8・LF で、`.gitattributes` の `*.bat text=auto working-tree-encoding=CP932 eol=crlf` によりチェックアウト時に CP932・CRLF に変換されます(このリポジトリも、afl の `.gitattributes` を使う導入先のプロジェクトも同じ)。
  * 別のリポジトリへはチェックアウトしたファイル(作業ツリーのバイト)をコピーし、blob や base64 で直接渡さないでください。
* `.editorconfig` の `[*.bat]` は `charset = unset` にしておきます(消さない)。EditorConfig では CP932 を指定できず、消すと `[*]` の `charset = utf-8` が当たって VS Code(EditorConfig 拡張 v0.18 以降)や JetBrains が UTF-8 として開き、日本語が化けるためです。
* `working-tree-encoding=sjis` と書いてはいけません。Git(iconv)は sjis を厳密な Shift_JIS として扱うため、`\` が `¥`、`~` が `‾` としてリポジトリに入り、GitLab の画面や差分で化けます。Web エディタや API で普通の `\` `~` を書くとチェックアウトで失敗し、`①`・`髙` などの CP932 専用文字はコミットできません。
* `.bat` 先頭の `@rem うえからした (sjis認識させる呪文)` は、VS Code などに Shift_JIS と推測させるための行です。Git の設定とは関係なく、CP932 でもそのまま使えます(VS Code は `files.autoGuessEncoding` が有効な場合に推測します)。
* 古い Git(2.18 未満)や libgit2 系のクライアントは `working-tree-encoding` に対応しておらず、UTF-8 のまま出てくるので `.bat` が正しく動きません。

## ドキュメント
* [.claude/docs/gitlab-ci.md](.claude/docs/gitlab-ci.md) : GitLab CI テンプレートの使い方と各設定の理由
* [.claude/docs/gitlab-runner-setup.md](.claude/docs/gitlab-runner-setup.md) : Windows で gitlab-runner をセットアップする手順
* [.claude/docs/troubleshooting.md](.claude/docs/troubleshooting.md) : CI で実際に起きた症状と原因
* [.claude/docs/todo.md](.claude/docs/todo.md) : 次の機会に入れるもの(関係する変更をするときに一緒に入れる)
