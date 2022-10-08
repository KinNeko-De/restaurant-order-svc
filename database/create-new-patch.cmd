@echo off
set TARGETDIR=%~dp0\patch
set /p DESC="Enter short description (without blanks and with minus): "

for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set ldt=%%j
set iso=%ldt:~0,4%%ldt:~4,2%%ldt:~6,2%.%ldt:~8,2%%ldt:~10,2%.%ldt:~12,2%%ldt:~15,3%
set FILENAME=V%iso%__%DESC%.sql
echo. 2>%TARGETDIR%\%FILENAME%
echo patch created successful: [%FILENAME%]
call %TARGETDIR%\%FILENAME%
