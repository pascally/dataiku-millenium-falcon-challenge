cd ..
dotnet build
dotnet test test\MilleniumFalcon.IntegrationTests\bin\Debug\net7.0\Backend.IntegrationTests.dll
dotnet test test\MilleniumFalcon.UnitTests\bin\Debug\net7.0\Backend.UnitTests.dll
cd bat