# Local deployment on IIS

These steps publish TicketGo to IIS on your own Windows PC and open it at http://localhost:8080.
Run PowerShell and IIS Manager **as administrator**.

## 1. Turn on IIS
1. Press Win + R, type `optionalfeatures`, press Enter.
2. Tick **Internet Information Services** (keep the default sub-items and also tick **Web Management Tools > IIS Management Console**).
3. Click OK and wait until it finishes.

## 2. Install the .NET Hosting Bundle
1. Download the **Hosting Bundle** from https://dotnet.microsoft.com/download/dotnet (choose .NET 8 or newer, "ASP.NET Core Runtime > Hosting Bundle").
2. Install it.
3. In an administrator PowerShell run `iisreset`.

(The Hosting Bundle must be installed after IIS. If you installed it before, run the installer again and choose Repair.)

## 3. Publish the project
In a terminal inside the project folder (the one with `TicketGo.csproj`) run:

```powershell
dotnet publish -c Release -o C:\inetpub\TicketGo
```

If you get "access denied", run `publish.bat` instead (it publishes to a `publish` folder inside the project), then copy the files from `publish` to `C:\inetpub\TicketGo`.

## 4. Create the folder for the database
The Production setting uses SQLite, so the app needs a folder it can write to:

```powershell
mkdir C:\inetpub\TicketGo\App_Data
```

## 5. Create the app pool and the website in IIS Manager
1. Open **IIS Manager** (Start menu, type "IIS").
2. Left side, right-click **Application Pools > Add Application Pool**
   - Name: `TicketGoPool`
   - .NET CLR version: **No Managed Code**
   - Click OK.
3. Right-click **Sites > Add Website**
   - Site name: `TicketGo`
   - Application pool: `TicketGoPool`
   - Physical path: `C:\inetpub\TicketGo`
   - Port: `8080`
   - Click OK.
   (If the Default Web Site already uses port 8080, stop it or pick another port.)

## 6. Give the app pool permission to write the database
```powershell
icacls "C:\inetpub\TicketGo\App_Data" /grant "IIS AppPool\TicketGoPool:(OI)(CI)M"
```

## 7. Open the site
Go to **http://localhost:8080** and log in with the demo accounts from the README.
The first request creates the database and the sample data, so it can take a few seconds.

Take screenshots of: IIS Manager (site and app pool), the site running at `localhost:8080`, and a role dashboard. They are good proof for the submission.

## Updating the site after a code change
1. In IIS Manager, select `TicketGoPool` and click **Stop**.
2. Run the publish command again.
3. Start the app pool.
(Stopping first avoids "file in use" errors. Your `App_Data` database is not touched.)

## Problems
| Problem | Fix |
|---|---|
| HTTP Error 500.19 or 404.15 | The Hosting Bundle is missing or was installed before IIS. Install or repair it, then `iisreset`. |
| HTTP Error 500.30 | The app failed at start. Open `C:\inetpub\TicketGo\web.config`, set `stdoutLogEnabled="true"`, create a folder `logs` inside `C:\inetpub\TicketGo`, give the app pool write access to it, reload the page and read the log file. |
| Database error, "unable to open database file" | Step 4 or 6 was skipped. The `App_Data` folder must exist and the app pool must have modify permission. |
| Page opens but the site has no styling | Make sure the `wwwroot` content was published (`C:\inetpub\TicketGo\wwwroot\lib\bootstrap` exists). |
| Port already in use | Pick another port in the site bindings (for example 8081). |

## Using SQL Server instead of SQLite on IIS (optional)
Remove `appsettings.Production.json` from the published folder, set a normal SQL Server connection string
in `appsettings.json`, and give the login `IIS APPPOOL\TicketGoPool` the `db_owner` role on the database.
SQL Server LocalDB does not work well with IIS, so use SQL Server Express for this.
