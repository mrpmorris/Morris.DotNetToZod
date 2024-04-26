@echo off
cls
cleanup -y .\Source
rd /s /Q "%USERPROFILE%\.nuget\packages\morris.zod"
del /s *.nupkg

dotnet build .\Source\Lib\Morris.Zod\Morris.Zod.csproj -c Release
if errorlevel 1 exit /b

dotnet restore .\Source\Tests\Morris.Zod.UnitTests.Models\Morris.Zod.UnitTests.Models.csproj
dotnet build .\Source\Tests\Morris.Zod.UnitTests.Models\Morris.Zod.UnitTests.Models.csproj -c Release --no-restore
if errorlevel 1 exit /b

type .\Source\Tests\ZodGeneratedFiles\*.ts