@echo off
REM Löscht alle temporären Visual Studio Dateien
rd /S /Q .vs
rd /S /Q netcoreapp3.0
rd /S /Q bin
rd /S /Q obj
FOR /D %%d IN (*) DO (
    rd /S /Q %%d\bin
    rd /S /Q %%d\obj
    FOR /D %%s IN (%%d\*) DO (
        rd /S /Q %%s\bin
        rd /S /Q %%s\obj
    )    
)
