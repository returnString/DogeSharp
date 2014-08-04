set dsc="..\DogeSharp\bin\Debug\dsc"

xcopy /y Libs\*.dll

echo Building console sample
%dsc% Main.ds MyType.ds /out:ConsoleApp.exe /preservetranslated

echo Building WinForms sample
%dsc% WinForms.ds /target:winexe /out:FormsApp.exe /reference:System.Windows.Forms.dll /reference:System.dll /preservetranslated

echo Building Servya sample
%dsc% Server.ds /out:ServyaApp.exe /reference:Libs\Servya.dll /reference:Libs\Servya.Http.dll /reference:System.dll /preservetranslated
