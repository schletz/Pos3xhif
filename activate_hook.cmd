@echo off
rem Activates the versioned git hooks in .githooks for this clone. Run it once after cloning.
rem The pre-commit hook packs every changed exercise in <chapter>\Uebungen into <exercise>.7z.
cd /d "%~dp0"
git config core.hooksPath .githooks || exit /b 1
echo Git hooks activated: core.hooksPath = .githooks
where python >nul 2>nul || echo WARNING: python not found on PATH. The pre-commit hook needs it.
if not exist "C:\Program Files\7-Zip\7z.exe" echo WARNING: 7-Zip not found in "C:\Program Files\7-Zip". The pre-commit hook needs it.
