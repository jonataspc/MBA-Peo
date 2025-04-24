dotnet build ..\Peo.sln

dotCover cover-dotnet --output report.html --ReportType HTML --Filters="+:module=Peo.*" -- test  ..\Peo.sln

.\report.html