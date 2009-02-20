@echo off
copy C:\Bob\Round_2\Dispersal_Sim\bin\Release\Dispersal_Sim.exe W:\Zollner-lab\Dispersal_Sim.exe 
if errorlevel 1 goto CSharpReportError
goto CSharpEnd
:CSharpReportError
echo Project error: A tool returned an error code from the build event
exit 1
:CSharpEnd