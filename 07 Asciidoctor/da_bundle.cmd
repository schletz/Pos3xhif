@echo off
SET ZIP="C:\Program Files\7-Zip\7z.exe"

del /F /Q "diplomarbeit_asciidoc.zip"
%ZIP% a -mx=9 "diplomarbeit_asciidoc.zip" diplomarbeit*.* kopfzeile*.*
