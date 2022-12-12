@echo off

REM https://stackoverflow.com/questions/44074121/build-net-core-console-application-to-output-an-exe

REM dotnet publish --output "{any directory}" --runtime {runtime}
REM   --configuration {Debug|Release} -p:PublishSingleFile={true|false}
REM   -p:PublishTrimmed={true|false} --self-contained {true|false}

dotnet publish --output "I3DShapesTool-OBJx-EXE" --runtime win-x64 --configuration Release I3DShapesTool-OBJx-master

echo.
pause
