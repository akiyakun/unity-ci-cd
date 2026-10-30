import sys
import os
import re
import signal
import argparse
import json
import subprocess


# 時間切れで run_tests.py が Unity を終了させたときの終了コード
TIMEOUT_EXIT_CODE = 124


# def load_cicd_config():

def parse_args():
  parser = argparse.ArgumentParser(description ='desc')

  parser.add_argument('--runner', type=str,
    help='Target runner name.', required=True)
  parser.add_argument('--testPlatform', type=str,
    help='Unity -testPlatform', required=True)
  # 複数指定可。run_tests.bat 経由だと cmd が ; で引数を分けるので、受け取った後に ; で連結して Unity に渡す
  parser.add_argument('--assemblyNames', type=str, nargs='+',
    help='Unity -assemblyNames', required=True)
  parser.add_argument('--skipImport', action='store_true',
    help='テスト前のインポートだけの起動を省略する')
  # CI のジョブのタイムアウト(GitLab の既定は 1 時間)より先に切れるようにしておく
  # ジョブのタイムアウトで打ち切られると Unity が残り、次のジョブが起動できなくなるため
  parser.add_argument('--importTimeout', type=int, default=30,
    help='インポートだけの起動の制限時間(分)。0 で無制限')
  parser.add_argument('--testTimeout', type=int, default=20,
    help='テストの起動の制限時間(分)。0 で無制限')
  # parser.add_argument('--override', action='store_true',
  #   help='既に存在する場合でも作成しなおす')

  return parser.parse_args()


# Unityのログの末尾を表示(全体はCIのartifactsに残す)
def print_log_tail(log_file):
  if not os.path.exists(log_file): return
  sys.stdout.reconfigure(errors='replace')
  with open(log_file, 'r', encoding='utf-8', errors='replace') as f:
    lines = f.readlines()
  print("---- {} (last 400 lines) ----".format(log_file))
  print(''.join(lines[-400:]))


# Unity.exe は子プロセス(シェーダーのコンパイラなど)を持つので、まとめて終了させる
def kill_process_tree(pid):
  if os.name == 'nt':
    subprocess.run(['taskkill', '/PID', str(pid), '/T', '/F'],
      stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
  else:
    os.kill(pid, signal.SIGKILL)


# 前のジョブで残った Unity(このプロジェクトを開いたまま)を終了させる
# 残っていると、別の Unity がプロジェクトを開いているので、起動してすぐ終了コード 1 で終わる
# 開発者の PC で開いているエディタを閉じないように、CI(環境変数 CI がある)の Windows のときだけ行う
def kill_stale_unity(project_path):
  if os.name != 'nt' or not os.environ.get('CI'): return
  try:
    result = subprocess.run(
      [
        'powershell', '-NoProfile', '-Command',
        "Get-CimInstance Win32_Process -Filter \"Name='Unity.exe'\" | Select-Object ProcessId,CommandLine | ConvertTo-Json -Compress"
      ],
      capture_output=True, text=True, errors='replace', check=True
    )
    processes = json.loads(result.stdout) if result.stdout.strip() else []
  except Exception as e:
    print("Unity のプロセスを確認できませんでした: {}".format(e))
    return
  # 1 件だけのときは配列にならない
  if isinstance(processes, dict): processes = [processes]

  # コマンドラインにこのプロジェクトのパスがあるもの(別のプロジェクトのパスの先頭が一致しただけのものは除く)
  path_pattern = re.compile(re.escape(os.path.normcase(project_path)) + r'\\?(?=["\s]|$)')
  for process in processes:
    command_line = os.path.normcase((process.get('CommandLine') or '').replace('/', '\\'))
    if path_pattern.search(command_line):
      print("前のジョブで残った Unity を終了させます(PID {})".format(process['ProcessId']), flush=True)
      kill_process_tree(process['ProcessId'])


# Unity を起動して終了コードを返す。制限時間を過ぎたら Unity を終了させて TIMEOUT_EXIT_CODE を返す
def run_unity(command, timeout_minutes):
  process = subprocess.Popen(command)
  try:
    return_code = process.wait(timeout=timeout_minutes * 60 if timeout_minutes > 0 else None)
  except subprocess.TimeoutExpired:
    print("{} 分たっても Unity が終わらないので終了させます(テストやエディタの一時停止で止まった可能性があります)。".format(timeout_minutes), flush=True)
    kill_process_tree(process.pid)
    process.wait()
    return_code = TIMEOUT_EXIT_CODE
  print("return code: {}".format(return_code), flush=True)
  return return_code


if __name__ == "__main__":
  args = parse_args()
  # if args.params is None: args.params = ''
  # src = os.path.abspath(args.src)
  # dest = os.path.abspath(args.dest)

  # if args.override and os.path.exists(dest):
  #   os.remove(dest)

  # モノレポかポリレポか判断
  up_pass = None
  if os.path.exists('../../../cicd_config.json'):
    up_pass = '../../../'
  elif os.path.exists('../../cicd_config.json'):
    up_pass = '../../'
  else:
    print("cicd_config.json not found.")
    sys.exit(1)

  # cicd_config.jsonからrunner情報を取得
  runner = None
  try:
    with open('{}cicd_config.json'.format(up_pass), 'r', encoding='utf-8') as f:
      cicd_config = json.load(f)
    # print(cicd_config['runners'])

    # name が args.runner の要素を取得(見つからない場合はNone)
    runner = next((r for r in cicd_config['runners'] if r['name'] == args.runner), None)
    if runner is None:
      print("runner '{}' not found in cicd_config.json.".format(args.runner))
      sys.exit(1)
    # print(runner['name'])
    # print(runner)
  except Exception as e:
    print(e)
    sys.exit(1)

  # Unityが見つからない場合はパスを表示して終了(Unityのバージョンを上げたときにcicd_config.jsonの更新漏れが多い)
  unity_path = runner['unity_path']
  if not os.path.exists(unity_path):
    print("Unity not found: {}".format(unity_path))
    print("cicd_config.json の runners > unity_path を確認してください。")
    sys.exit(1)

  project_path = os.path.abspath(up_pass)
  test_results = os.path.join(project_path, 'Logs', 'tests_results.xml')
  # WindowsのUnity.exeは -logFile - でも標準出力に出ないため、ファイルに書いて終了後に表示する
  log_file = os.path.join(project_path, 'Logs', 'unity_tests.log')
  import_log_file = os.path.join(project_path, 'Logs', 'unity_import.log')
  # Library/ と一緒に Logs/ も消された状態で起動されることがあるので作っておく
  os.makedirs(os.path.join(project_path, 'Logs'), exist_ok=True)

  kill_stale_unity(project_path)

  # インポートだけ先に済ませる
  # Library/ が空の初回は、インポートの後処理でシーンが変更扱いになり、
  # -runTests の開始時に保存確認のダイアログで止まる(終了コード3)ことがあるため
  if not args.skipImport:
    print("---- import ----", flush=True)
    import_code = run_unity(
      [
        unity_path,
        '-projectPath', project_path,
        '-forgetProjectPath',
        '-batchmode',
        '-quit',
        '-logFile', import_log_file
      ],
      args.importTimeout
    )

    # インポートに失敗したらテストは実行しない
    if import_code != 0:
      print_log_tail(import_log_file)
      print("インポート段階で失敗しました(終了コード {})。テストは実行していません。".format(import_code))
      sys.exit(import_code)

  # Unityで実行
  print("---- tests ----", flush=True)
  # Unity -runTests の終了コード: 2=テスト失敗, 3=実行エラー
  return_code = run_unity(
    [
      unity_path,
      # "/Applications/Unity/Hub/Editor/6000.0.50f1-x86_64/Unity.app/Contents/MacOS/Unity",
      # '-projectPath', "C:/local/dev/itomon/survivors/survivors",
      '-projectPath', project_path,
      '-forgetProjectPath',
      '-batchmode',
      '-runTests',
      # '-testPlatform', 'PlayMode',
      '-testPlatform', args.testPlatform,
      # '-playerHeartbeatTimeout', '1',
      # '-assemblyNames', 'Project.Tests',
      '-assemblyNames', ';'.join(name for name in args.assemblyNames if name),
      #'-testCategory', 'Project.Tests',
      # '-testResults', '../../../build/tests_playmode_results.xml',
      '-testResults', test_results,
      '-logFile', log_file
    ],
    args.testTimeout
  )

  print_log_tail(log_file)

  if return_code != 0:
    sys.exit(return_code)

  print("complate.")
  sys.exit(0)
