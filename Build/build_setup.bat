@echo off

IF "%INNOSETUP_PATH%" == "" (
    ECHO ERROR: environment variable INNOSETUP_PATH not set
    GOTO end
)

ECHO Inno Setup located at %INNOSETUP_PATH%
ECHO compiling script...
"%INNOSETUP_PATH%\ISCC.exe" installer.iss

:end
PAUSE