cls
cleanup -y .\Source
rd /s /Q "%USERPROFILE%\.nuget\packages\Morris.DotNetToZod"
del /s *.nupkg

dotnet build .\Source\Lib\Morris.DotNetToZod\Morris.DotNetToZod.csproj -c Release
if errorlevel 1 exit /b

dotnet restore .\Source\Lib\Morris.DotNetToZod.UnitTests.Models\Morris.DotNetToZod.UnitTests.Models.csproj
dotnet build .\Source\Lib\Morris.DotNetToZod.UnitTests.Models\Morris.DotNetToZod.UnitTests.Models.csproj -c Release --no-restore
if errorlevel 1 exit /b

type .\Source\Lib\ZodGeneratedFiles\*.ts