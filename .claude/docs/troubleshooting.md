# トラブルシューティング

GitLab CI(Windows の shell Runner)で実際に起きた症状と原因です。

## ジョブがログ空のまま「待機中」になり、1〜2時間ほどで失敗する
* 原因: ジョブの `tags` に合う Runner がオンラインになっていない。
* 対処: GitLab の「設定 > CI/CD > Runner」で Runner がオンラインか、タグ(`windows`)が付いているかを確認します。Runner の PC で `Get-Service gitlab-runner` がRunning か確認します。

## run_tests.py で FileNotFoundError になる
* 原因: `cicd_config.json` の `runners > unity_path` が古い(Unity のバージョンを上げたときの更新漏れ)。
* 対処: `unity_path` を Runner の PC にインストールされている Unity のパスに直します。1.2.1 以降の `run_tests.py` では「Unity not found」と表示して終了するようになっています。

## Unity が数秒で終了コード1になり、ログが何も出ない
* 原因: Windows の `Unity.exe` は `-logFile -` を指定しても標準出力にログを出さないので、何が起きたのか分からない。
* 対処: 1.2.1 以降の `run_tests.py` は `-logFile Logs/unity_tests.log` でファイルに書き、終了後に末尾400行を表示します。全体は artifacts に残ります。ログを見て原因を調べます。

## 「could not read Username for 'https://...': terminal prompts disabled」
* 原因: `Packages/manifest.json` で参照している private な git パッケージを、Unity Package Manager が認証なしで取得しようとしている。
* 対処: テンプレートの `GIT_CONFIG_COUNT` / `GIT_CONFIG_KEY_0` / `GIT_CONFIG_VALUE_0` でジョブトークンを使うようにします([gitlab-ci.md](gitlab-ci.md) 参照)。

## 「Authentication by CI/CD job token not allowed」で 403 になる
* 原因: 取得先のプロジェクトのジョブトークン許可リストに、CI を動かすプロジェクトが入っていない。
* 対処: **取得先のプロジェクト** の「設定 > CI/CD > ジョブトークンの権限」に、CI を動かすプロジェクトを追加します。

## Library が空の初回だけ「Scene(s) Have Been Modified」ダイアログで終了コード3になる
* 原因: 初回インポートの後処理が空のシーンを変更済みにしてしまい、`-runTests` の実行時に保存確認のダイアログが出て止まる。
* 2回目以降(`Library/` が残っている状態)は通ります。`GIT_CLEAN_FLAGS: "-ffd"` で `Library/` を残しているので、初回だけ再実行すれば済みます。
* 対策: 1.2.2 から `run_tests.py` が `-runTests` の前に `-batchmode -quit` でインポートだけの起動を挟みます(ログは `Logs/unity_import.log`)。インポートが失敗したときはテストを実行せず、その終了コードで終わります。

## テストが止まったままジョブのタイムアウトで失敗し、その後のジョブが起動直後に終了コード1で失敗する
* 原因: テスト中に Error Pause(例外でエディタが一時停止する)などで止まり、ジョブのタイムアウトで打ち切られた。そのとき `Unity.exe` が終了せずに残り、同じプロジェクトを開いたままなので、次のジョブの Unity が起動直後に失敗する。
* 対処: Runner の PC で残っている `Unity.exe` を終了させてから再実行します。
* 対策: 1.2.3 から `run_tests.py` に制限時間(インポート既定30分 / テスト既定20分。CI の変数 `CICD_IMPORT_TIMEOUT` / `CICD_TEST_TIMEOUT` で変えられる)があり、過ぎたら Unity を子プロセスごと終了させて終了コード `124` で終わります。CI の Windows では起動前に残った Unity も終了させます([gitlab-ci.md](gitlab-ci.md) の「終了コード」参照)。
* 止まった原因のほうも直します。batchmode(CI の PlayMode テスト)では `Keyboard.current` などの入力デバイスが null になるので、毎フレーム触るコードでは null を確認します(onihime では afl の `DebuggerService` の `NullReferenceException` で Error Pause になっていた)。

## Unity 起動時に「cicd_config.json : Current v1.2.1, File v1.2.0」のエラーログが出る
* 原因: パッケージを更新したが、プロジェクト直下の `cicd_config.json` の `"version"` が古いまま。
* 対処: `"version"` をパッケージのバージョンに書き換えます(パッケージの README の「更新手順」参照)。

## パイプラインを削除できない
* パイプラインの削除はプロジェクトの Owner 権限が必要です。Maintainer では削除できません。
