REM Clear folders
del NuGet.SharpUtil\lib\net40\*.* /f /q
del NuGet.SharpUtil\lib\net45\*.* /f /q
del output\*.* /f /q

REM .NET v4.0
"%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" SharpExtensionsUtil\SharpExtensionsUtil.csproj /nologo /t:Clean,Build /p:Configuration=Release;Platform=AnyCPU %*
REM copy to output folder
robocopy SharpExtensionsUtil\bin\Release NuGet.SharpUtil\lib\net40 *.dll

REM .NET v4.5
"%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" SharpExtensionsUtil\SharpExtensionsUtil.csproj /nologo /t:Clean,Build /p:Configuration=Release45;Platform=AnyCPU %*
REM copy to output folder
robocopy SharpExtensionsUtil\bin\Release45 NuGet.SharpUtil\lib\net45 *.dll

"%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" NuGet.SharpUtil\NuGet.SharpUtil.csproj /nologo /t:Clean,Build /p:Configuration=Release;Platform=AnyCPU %*
REM copy to output folder
robocopy NuGet.SharpUtil output SharpUtil.*.nupkg

pause

