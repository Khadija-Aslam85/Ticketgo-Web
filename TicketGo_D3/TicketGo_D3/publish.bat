@echo off
echo Publishing TicketGo to the publish folder...
dotnet publish -c Release -o publish
echo.
echo Done. Copy the files from the publish folder to C:\inetpub\TicketGo
pause
