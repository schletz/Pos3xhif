REM @echo off
REM Löscht alle temporären Visual Studio Dateien
rd /S /Q ".vs"
rd /S /Q ".vscode"
  
FOR /D /R %%d IN (*) DO (
  rd /S /Q "%%d\bin"
  rd /S /Q "%%d\obj"
  rd /S /Q "%%d\.vs"
  rd /S /Q "%%d\.vscode"
)

