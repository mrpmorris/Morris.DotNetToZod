cls
@echo **** 0.1.0-Beta1 : UPDATED THE VERSION NUMBER IN THE PROJECT *AND* BATCH FILE? ****
pause

cls
@call BuildAndTest.bat

@echo ======================

set /p ShouldPublish=Publish 0.1.0-Beta1 [yes]?
@if "%ShouldPublish%" == "yes" (
	@echo PUBLISHING
	dotnet nuget push .\Source\Lib\Morris.DotNetToZod.CommandLine\bin\Release\Morris.DotNetToZod.CommandLine.0.1.0-Beta1.nupkg -k %MORRIS.NUGET.KEY% -s https://api.nuget.org/v3/index.json
	dotnet nuget push .\Source\Lib\Morris.DotNetToZod\bin\Release\Morris.DotNetToZod.0.1.0-Beta1.nupkg -k %MORRIS.NUGET.KEY% -s https://api.nuget.org/v3/index.json
)

