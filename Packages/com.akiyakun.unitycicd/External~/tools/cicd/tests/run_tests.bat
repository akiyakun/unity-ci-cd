@rem ‚¤‚¦‚©‚ç‚µ‚½ (sjis”FŽ¯‚³‚¹‚éŽô•¶)
@echo off

@REM Chaange current directory to the directory of this script
cd /d %~dp0
@REM echo %cd%

@REM usage: run_tests.bat <testPlatform> <assemblyName> [<assemblyName> ...]
@REM cmd splits arguments at ';' and ',' too, so "A.Tests;B.Tests" arrives as %2=A.Tests %3=B.Tests.
@REM Pass every argument after %1 to run_tests.py, which joins them with ';'.
set "TEST_PLATFORM=%~1"
set ASSEMBLY_NAMES=
:collect_args
shift
if "%~1"=="" goto run
set ASSEMBLY_NAMES=%ASSEMBLY_NAMES% "%~1"
goto collect_args

:run
py run_tests.py --runner Windows --testPlatform "%TEST_PLATFORM%" --assemblyNames%ASSEMBLY_NAMES%
echo run_tests.py return code %ERRORLEVEL%
exit /b %ERRORLEVEL%

@REM echo end
@REM pause
