set SCRIPT_PATH=%~dp0
set VENV_DIR=.venv

pushd %SCRIPT_PATH%

call %VENV_DIR%\Scripts\activate.bat

set OLD_PYTHONPATH=%PYTHONPATH%
set PYTHONPATH=%PYTHONPATH%;%SCRIPT_PATH%\src;$PYTHONPATH;%SCRIPT_PATH%\generated

python grpc-app.py

set PYTHONPATH=%OLD_PYTHONPATH%

call %VENV_DIR%\Scripts\deactivate.bat

popd
