
set dsc="..\DogeSharp\bin\Debug\dsc"

echo Building console sample
%dsc% Main.ds MyType.ds /out:ConsoleApp.exe /preservetranslated

echo Building WinForms sample
%dsc% WinForms.ds /target:winexe /out:FormsApp.exe /reference:System.Windows.Forms.dll /reference:System.dll /preservetranslated
