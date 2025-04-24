dotnet build ..\Peo.sln

dotCover cover-dotnet --output report.html --ReportType HTML -- test  ..\Peo.sln

.\report.html