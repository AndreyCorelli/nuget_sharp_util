REM Clear folders
del NuGet.SharpUtil\lib\net45\*.* /f /q
del output\*.* /f /q

REM .NET v4.5
"%PROGRAMFILES(x86)%\MSBuild\14.0\Bin\MsBuild.exe" SharpExtensionsUtil\SharpExtensionsUtil.csproj /nologo /t:Clean,Build /p:Configuration=Release45;Platform=AnyCPU %*
REM copy to output folder
robocopy SharpExtensionsUtil\bin\Release45 NuGet.SharpUtil\lib\net45 SharpExtensionsUtil.dll

"%PROGRAMFILES(x86)%\MSBuild\14.0\Bin\MsBuild.exe" NuGet.SharpUtil\NuGet.SharpUtil.csproj /nologo /t:Clean,Build /p:Configuration=Release;Platform=AnyCPU %*
REM copy to output folder
robocopy NuGet.SharpUtil output SharpUtil.*.nupkg

pause

