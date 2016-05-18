@echo off

cd %~dp0
set PATH=%PATH%;%windir%\Microsoft.NET\Framework\v4.0.30319\

csc /t:exe TestCode.cs
