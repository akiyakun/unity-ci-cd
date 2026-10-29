import sys
import os
import argparse
import json
import subprocess


# def load_cicd_config():

def parse_args():
  parser = argparse.ArgumentParser(description ='desc')

  parser.add_argument('--runner', type=str,
    help='Target runner name.', required=True)
  parser.add_argument('--testPlatform', type=str,
    help='Unity -testPlatform', required=True)
  parser.add_argument('--assemblyNames', type=str,
    help='Unity -assemblyNames', required=True)
  # parser.add_argument('--override', action='store_true',
  #   help='既に存在する場合でも作成しなおす')

  return parser.parse_args()


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

  # Unityで実行
  return_code = 0
  try:
    result = subprocess.run(
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
        '-assemblyNames', args.assemblyNames,
        #'-testCategory', 'Project.Tests',
        # '-testResults', '../../../build/tests_playmode_results.xml',
        '-testResults', test_results,
        '-logFile', log_file
      ],
      check=True
      # , timeout=60*3
    )
    print("return code: {}".format(result.returncode))
    # print(result)
  except subprocess.TimeoutExpired as e:
    print("Timeout expired: {}".format(e.timeout))
    sys.exit(1)
  except subprocess.CalledProcessError as e:
    # Unity -runTests の終了コード: 2=テスト失敗, 3=実行エラー
    print(e)
    return_code = e.returncode

  # Unityのログの末尾を表示(全体はCIのartifactsに残す)
  if os.path.exists(log_file):
    sys.stdout.reconfigure(errors='replace')
    with open(log_file, 'r', encoding='utf-8', errors='replace') as f:
      lines = f.readlines()
    print("---- {} (last 400 lines) ----".format(log_file))
    print(''.join(lines[-400:]))

  if return_code != 0:
    sys.exit(return_code)

  print("complate.")
  sys.exit(0)
