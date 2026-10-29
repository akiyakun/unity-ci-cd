# GitLab CI テンプレート

テンプレート: `Packages/com.akiyakun.unitycicd/Templates~/GitLab CI/gitlab-ci.yml`

Windows の shell Runner で Unity の Test Runner(PlayMode / EditMode)を実行します。
Runner の用意は [gitlab-runner-setup.md](gitlab-runner-setup.md) を参照してください。

## 使い方
1. テンプレートをリポジトリ直下に `.gitlab-ci.yml` としてコピーします。
2. 変数を合わせます。
   | 変数 | 既定値 | 説明 |
   | --- | --- | --- |
   | `PROJECT_DIR_NAME` | `"."` | Unity プロジェクトのフォルダ(リポジトリ直下からの相対パス)。モノレポならサブフォルダ名 |
   | `TEST_PLATFORM` | `"PlayMode"` | `PlayMode` か `EditMode` |
   | `TEST_ASSEMBLY_NAMES` | `"Project.Tests"` | 自分のテストアセンブリ名に変えます。複数は `;` 区切り |
3. `tags` を Runner に付けたタグに合わせます(既定は `windows`)。
4. プロジェクトの `cicd_config.json` の `runners` に `name` が `"Windows"` のものがあり、`unity_path` が Runner の PC にインストールされている Unity を指しているか確認します。
5. `tools/` がコミットされているか確認します(パッケージ導入後に Unity Editor を一度起動するとコピーされます)。

テストは `tools/cicd/tests/run_tests.bat` → `run_tests.py` 経由で実行されます。`run_tests.bat` は `py`(Python ランチャー)を呼ぶので、Runner の PC に Python が必要です。

## 各設定の理由

### GIT_CLEAN_FLAGS: "-ffd"
shell Runner は作業フォルダを使い回します。既定の `-ffdx` だと `.gitignore` で無視している `Library/` まで毎回消えて、毎回フルインポートになり非常に遅くなります。`-x` を外して無視ファイルを残します。

### rules(develop への push と web / api)
`develop` への push で実行します。加えて `CI_PIPELINE_SOURCE` が `web`(GitLab の画面の「パイプラインを実行」)か `api` のときはブランチに関係なく実行できるので、作業ブランチで試したいときに使います。

### etc/symlinks.txt からジャンクションを作る
`Packages/` 以下をシンボリックリンクでほかのフォルダ(共通ライブラリなど)に向けているプロジェクト向けです。
Windows のシンボリックリンク作成は管理者権限か開発者モードが必要なので、CI では権限のいらないジャンクションで代用します。
`etc/symlinks.txt` が無いプロジェクトでは何もしません。

書式は1行に `リンクのパス|リンク先のパス` です。パスはリポジトリ直下からの相対パスか絶対パスで、`#` で始まる行と空行は無視します。
```
# Packages/ に共通ライブラリをリンクする
Packages/com.example.lib|libs/com.example.lib
```
既にリンクのパスが存在する場合は作り直しません。

### private な git パッケージの取得(ジョブトークン)
`Packages/manifest.json` で同じ GitLab 上の private リポジトリを git の URL(`https://<GitLabのホスト>/...`)で参照していると、Unity Package Manager が取得するときに認証が必要になります。
そこで `GIT_CONFIG_COUNT` / `GIT_CONFIG_KEY_0` / `GIT_CONFIG_VALUE_0` 環境変数で git の `url.<base>.insteadOf` を設定し、`https://<ホスト>/` を `https://gitlab-ci-token:<CI_JOB_TOKEN>@<ホスト>/` に置き換えます。
環境変数なのでこのジョブのプロセスの間だけ有効で、Runner のユーザーのグローバルな git 設定は変えません。

ジョブトークンで他のプロジェクトを読むには、**取得先のプロジェクト**の「設定 > CI/CD > ジョブトークンの権限」に、CI を動かすプロジェクトを追加しておく必要があります(許可リスト)。追加していないと 403 になります。

### artifacts(テスト結果と Unity のログ)
`when: always` で、テストが失敗したときも次を残します(1週間)。
* `Logs/tests_results.xml` : Test Runner の結果(NUnit 形式)
* `Logs/unity_tests.log` : Unity のログ全体

ジョブのログには `run_tests.py` が Unity のログの末尾400行を表示します。全体を見たいときは artifacts からダウンロードします。

## 終了コード
`run_tests.py` は Unity の終了コードをそのまま返します。
* `0` : 成功
* `2` : テスト失敗
* `3` : 実行エラー(コンパイルエラー、ダイアログで止まったなど)
* `1` : Unity の起動に失敗した(ログを確認)、または `cicd_config.json` や Unity が見つからないなど Unity を起動する前のエラー

## テストアセンブリの複数指定
`TEST_ASSEMBLY_NAMES` は `A.Tests;B.Tests` のように `;` 区切りで複数指定できます。
cmd は `;` や `,` も引数の区切りとして扱うので、`run_tests.bat` には `%2=A.Tests %3=B.Tests` のように分かれて届きます。
`run_tests.bat` は `%1` をテストプラットフォームとし、残りの引数をすべて `run_tests.py --assemblyNames` に渡します。`run_tests.py` はそれを `;` で連結して Unity の `-assemblyNames` に渡します。
