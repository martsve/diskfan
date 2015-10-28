SET framework=%windir%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%framework%
csc /out:"fan.exe" /optimize "diskfan\*.cs"
