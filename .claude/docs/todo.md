# 次の機会に入れるもの

単独で入れるほどではないので、関係する変更をするときに一緒に入れるものです。入れたらここから消します。

## run_tests.py
* 制限時間(`--importTimeout` / `--testTimeout`)を CI の設定から変えられるようにする(環境変数や `cicd_config.json` の `"tests"` など)。
  * `run_tests.bat` は2つ目以降の引数をすべて `--assemblyNames` に渡すので、今はテンプレートから指定できない。既定の合計50分は GitLab の既定のタイムアウト(1時間)より短いので急ぎではない。
* macOS では `kill_process_tree` が Unity 本体しか終了させない。`Popen(start_new_session=True)` で起動し `os.killpg` で終了させる。
