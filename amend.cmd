@echo off
setlocal

cd /d "%~dp0"

git add -A
if errorlevel 1 goto error

git commit --amend --no-edit
if errorlevel 1 goto error

git push --force
if errorlevel 1 goto error

echo Done.
pause
exit /b 0

:error
echo.
echo An error occurred - see output above.
pause
exit /b 1
