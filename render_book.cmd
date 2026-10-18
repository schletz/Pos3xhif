python render_pdf.py dotnet_book.adoc dotnet_book_print.pdf && ^
C:\ghostscript\bin\gswin64c.exe -q -sDEVICE=pdfwrite -dPDFSETTINGS=/printer -dColorImageResolution=150 -dNOPAUSE -dBATCH ^
-sOutputFile=dotnet_book.pdf dotnet_book_print.pdf
