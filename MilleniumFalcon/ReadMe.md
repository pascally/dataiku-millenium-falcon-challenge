First install dotnet SDK Version 7
[https://learn.microsoft.com/en-us/dotnet/core/install/](https://learn.microsoft.com/en-us/dotnet/core/install/)

On Mac / Windows [https://dotnet.microsoft.com/en-us/download/dotnet](https://dotnet.microsoft.com/en-us/download/dotnet)

Also you ll need to install a docker [Engine](https://docs.docker.com/engine/install/)

You can then either build the solution with [Visual Studio](https://visualstudio.microsoft.com/fr/vs/community/)
or build it by command line with dotnet build at the solution level. 

Check your version by typing
dotnet --version
You should see 7.0.202

I placed some .bat helper whose name speak for themselves. 
you will need to go to the bat [directory](https://github.com/pascally/dataiku-millenium-falcon-challenge/tree/master/MilleniumFalcon/bat)
cd bat and launch them either manually or by command


* build_and_test.bat => launch build and run test
* build_and_install_from_local_source_CLI.bat => build, pack and install CLI (you can then run the CLI by calling give-me-the-odds)
* install_CLI_from_OfficialNuggetSource.bat => download (need a internet connection) and install the CLI (you can then run the CLI by calling give-me-the-odds)
the package was pushed on Nugget official repository
[https://www.nuget.org/packages/PascalLy.MilleniumFalcon.CLI/](https://www.nuget.org/packages/PascalLy.MilleniumFalcon.CLI/)
* uninstall_CLI.bat => uninstall the CLI
* launch_WebApi_locally.bat => launch the backend WebApi locally (should be accessible through http://localhost:5000)
* launch_WebApi_on_Docker.bat => launch the backend WebApi through Docker (should be accessible through http://localhost:5000) => issue with reading a host file inside Docker container  
* launch_Backend_And_Front.bat => launch the backend WebApi (should be accessible through http://localhost:5000) and the frontend locally (should be accessible through http://localhost:5260)

For the WebApi, when launched you can access a Swagger page [http://localhost:5000/swagger/index.html
](
http://localhost:5000/swagger/index.html
)

You can see Dockerfile on the [CLI](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/MilleniumFalcon.CLI/Dockerfile) project, the [WebApi](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.WebApi/Dockerfile) project and the [FrontEnd](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/MilleniumFalcon.Front/Dockerfile) project
A better way to deploy the application for the WebApi and Front end project at least
(for the CLI on Mac and Windows, I m not sure that using a Docker container would be a better
way as it means starting a Container everytime) 
would be to use a docker-compose file and launch both of them, but i was stuck
with an issue regarding trying to read a host file within a docker container so i didn't keep up
on that way.

You will find a project presentation in [projectpresentation.md](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/projectpresentation.md)
and a list of [ThingsToCompleteToBeProdReady](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/ThingsToCompleteToBeProdReady.md) of thing that would be good to do to truly reach prod ready situation
