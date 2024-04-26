cls
@echo **** 0.1.0-Alpha5 : UPDATED THE VERSION NUMBER IN THE PROJECT *AND* BATCH FILE? ****
pause

cls
@call BuildAndTest.bat

@echo ======================

set /p ShouldPublish=Publish 0.1.0-Alpha5 [yes]?
@if "%ShouldPublish%" == "yes" (
	@echo PUBLISHING
	dotnet nuget push .\Source\Lib\Morris.Zod\bin\Release\Morris.Zod.0.1.0-Alpha5.nupkg -k %MORRIS.NUGET.KEY% -s https://api.nuget.org/v3/index.json
)

