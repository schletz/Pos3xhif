@echo off
echo Loesche bin, obj, .vs und .vscode aus allen Ordnern...
rd /S /Q ".vs" > nul 2>&1
rd /S /Q ".vscode" > nul 2>&1
  
FOR /D /R %%d IN (*) DO (
  rd /S /Q "%%d\bin" > nul 2>&1
  rd /S /Q "%%d\obj" > nul 2>&1
  rd /S /Q "%%d\.vs" > nul 2>&1
  rd /S /Q "%%d\.vscode" > nul 2>&1
)
