@echo off
cls
cleanup -y .\Source

dotnet build .\Source\Lib\Morris.DotNetToZod.CommandLine\Morris.DotNetToZod.CommandLine.csproj -c Release
if errorlevel 1 exit /b

dotnet build .\Source\Lib\Morris.Zod.UnitTests.Models\Morris.Zod.UnitTests.Models.csproj -c Release --no-restore
if errorlevel 1 exit /b

.\Source\Lib\Morris.DotNetToZod.CommandLine\bin\Release\net8.0\dotnet-to-zod.exe .\Source\Lib\Morris.DotNetToZod.UnitTests.Models\bin\Release\net8.0\Morris.DotNetToZod.UnitTests.Models.dll .\Source\Lib\ZodGeneratedFiles

type .\Source\Lib\ZodGeneratedFiles\*.ts