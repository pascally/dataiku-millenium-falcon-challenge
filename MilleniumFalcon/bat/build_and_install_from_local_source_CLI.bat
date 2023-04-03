cd ..
dotnet build
cd src\MilleniumFalcon.CLI
dotnet tool uninstall --global pascally.milleniumfalcon.cli
dotnet pack
dotnet tool install --global --add-source ./nupkg pascally.milleniumfalcon.cli
cd ..\..\bat