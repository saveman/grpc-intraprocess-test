set SCRIPT_PATH=%~dp0
set VENV_DIR=.venv

pushd %SCRIPT_PATH%

rmdir /S /Q %VENV_DIR%

python -m venv %VENV_DIR%

call %VENV_DIR%\Scripts\activate.bat
python -m pip install -r requirements.txt
python -m pip list
call %VENV_DIR%\Scripts\deactivate.bat

popd
