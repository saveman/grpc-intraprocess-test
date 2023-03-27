set SCRIPT_PATH=%~dp0
set VENV_DIR=.venv
set TARGET_DIR=generated

pushd %SCRIPT_PATH%

call %VENV_DIR%\Scripts\activate.bat

rmdir /S /Q %TARGET_DIR%
mkdir %TARGET_DIR%

python -m grpc_tools.protoc ^
    -Iprotos ^
    --python_out=%TARGET_DIR% ^
    --pyi_out=%TARGET_DIR% ^
    --grpc_python_out=%TARGET_DIR% ^
    protos/greet.proto

call %VENV_DIR%\Scripts\deactivate.bat

popd
