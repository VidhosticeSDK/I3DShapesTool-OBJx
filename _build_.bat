@echo off

taskkill /F /IM dotnet.exe


dotnet clean --configuration Release I3DShapesTool-OBJx-master


REM Download:
REM https://dotnet.microsoft.com/en-us/download/dotnet/6.0
REM dotnet-sdk-6.0.401-win-x64.zip - OK
REM dotnet-sdk-6.0.428-win-x64.zip - OK

REM https://stackoverflow.com/questions/44074121/build-net-core-console-application-to-output-an-exe

REM dotnet publish --output "{any directory}" --runtime {runtime}
REM   --configuration {Debug|Release} -p:PublishSingleFile={true|false}
REM   -p:PublishTrimmed={true|false} --self-contained {true|false}

dotnet publish --output "I3DShapesTool-OBJx-EXE" --runtime win-x64 --configuration Release I3DShapesTool-OBJx-master

echo.
taskkill /F /IM dotnet.exe

echo.
pause
