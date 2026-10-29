# gitlab-runner のセットアップ(Windows)

Unity のテストを実行する Windows PC に gitlab-runner をサービスとしてインストールし、shell(pwsh)executor で登録します。

## 1. ダウンロード
* 手順: https://docs.gitlab.com/runner/install/windows/
* バイナリ一覧: https://gitlab-runner-downloads.s3.amazonaws.com/latest/index.html

ダウンロードする exe はインストーラーではなく gitlab-runner 本体です。
好きなフォルダ(例: `C:\GitLab-Runner`)に `gitlab-runner.exe` という名前で置きます。

Runner のバージョンは GitLab 本体のバージョンに合わせます。最新版でなく特定のバージョンが必要な場合は、一覧の URL の `latest` を `v<バージョン>`(例: `v17.11.0`)に置き換えたページからダウンロードします。

## 2. サービスとしてインストール
**管理者の PowerShell** で、exe を置いたフォルダに移動して実行します。
```powershell
.\gitlab-runner.exe install --user ".\<Windowsユーザー>" --password '<パスワード>'
```
* `--user` / `--password` は GitLab ではなく **Windows のアカウント** です。
* ユーザー名の先頭の `.\`(ローカルアカウントの意味)が無いと「domain does not exist」になります。
* パスワードに `$` などが含まれるとダブルクォートでは PowerShell に展開されて壊れるので、**シングルクォート** で囲みます。
* Microsoft アカウントでサインインしているユーザーの場合、PIN ではなく Microsoft アカウントのパスワードを指定します。
* ユーザー名とパスワードが正しいかは `runas /user:<Windowsユーザー> cmd` で確認できます。

**Unity をライセンス認証したのと同じ Windows ユーザー** で動かします。別のユーザー(既定の LocalSystem など)で動かすと、Unity がライセンスを見つけられず起動できません。

## 3. 起動して確認
```powershell
.\gitlab-runner.exe start
Get-Service gitlab-runner
```
`Status` が `Running` になっていれば OK です。

## 4. GitLab に Runner を登録
1. GitLab のプロジェクトで「設定 > CI/CD > Runner」を開き、「新規プロジェクト Runner」を作成します。
   * タグに `windows` を付けます(`.gitlab-ci.yml` の `tags` と合わせます)。
2. 作成後に表示される `gitlab-runner register ...` コマンドを、exe を置いたフォルダで実行します。
   * executor は `shell` を選びます。Windows では shell は `pwsh` になります(PowerShell 7 が必要です)。
3. 登録すると同じフォルダに `config.toml` が作られます。GitLab の Runner 一覧でオンライン(緑)になっていれば完了です。

## 更新
```powershell
.\gitlab-runner.exe stop
# gitlab-runner.exe を新しいバージョンに差し替える
.\gitlab-runner.exe start
```
`config.toml` はそのまま使えるので、登録しなおす必要はありません。

## Runner の PC に必要なもの
* Unity(`cicd_config.json` の `runners > unity_path` と同じパス)。ライセンス認証済みであること
* PowerShell 7(`pwsh`)
* Git
* Python(`run_tests.bat` が `py` ランチャーで `run_tests.py` を実行します)
