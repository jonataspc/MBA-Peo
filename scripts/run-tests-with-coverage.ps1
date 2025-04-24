dotnet build ..\Peo.sln

dotCover cover-dotnet --output report.html --ReportType HTML --Filters="-:class=*Migrations*;+:module=Peo.*;" -- test  ..\Peo.sln

.\report.html