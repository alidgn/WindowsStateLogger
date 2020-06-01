if not exist "C:\Program Files\WindowsLoggerService\" mkdir "C:\Program Files\WindowsLoggerService"
setlocal
cd /d %~dp0
xcopy /y /s WindowsLoggerService.exe "C:\Program Files\WindowsLoggerService"

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" "C:\Program Files\WindowsLoggerService\WindowsLoggerService.exe"

net start WindowsLogger.MailService
sc.exe config Logger start=auto
