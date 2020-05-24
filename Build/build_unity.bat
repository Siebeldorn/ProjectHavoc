@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO searching registry for Unity installation path
REG QUERY "HKCU\Software\Unity Technologies\Installer\Unity" /v "Location x64" >nul 2>&1

IF %ERRORLEVEL% == 0 (
    FOR /f "tokens=3*" %%a in ('REG QUERY "HKCU\Software\Unity Technologies\Installer\Unity" /v "Location x64"') DO SET unity=%%b
    ECHO Unity path: "!unity!"
    
    SET currpath=%~dp0
    SET projectpath="!currpath!..\Unity"
    SET logpath=!currpath!unity.log
    SET outputpath=!currpath!unity\ProjectHavoc.exe
    
    echo starting build for Unity project at !projectpath!...
    "!unity!\Editor\Unity.exe" -quit -batchmode -projectPath !projectpath! -logFile !logpath! -buildWindows64Player !outputpath!
    IF !ERRORLEVEL! == 0 (
        echo build successful!
    )
) ELSE (
    ECHO ERROR: Unity not found!
)

pause