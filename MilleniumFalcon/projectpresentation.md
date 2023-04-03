# Millenium Falcon Project structure

## Source Overview

![This is a alt text.](https://i0.wp.com/jasontaylor.dev/wp-content/uploads/2020/01/Figure-01-2.png?w=531&ssl=1 "This is a sample image.")

### Backend.Domain
###### Contain entities, usecase interfaces, types... specific to the domain layer (business model / star wars story in that case).
##### Model
* [Planet](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Domain/Domain/Models/Planet.cs)
* [Route](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Domain/Domain/Models/Route.cs)

##### Application Usecases interface
* [OnboardComputerUsecases](https://github.com/pascally/dataiku-millenium-falcon-challenge/tree/master/MilleniumFalcon/src/Backend.Domain/Domain/UseCases)
    * LoadMilleniumFalconDatas
    * LoadEmpireDatas
    * ComputeOddsToDestination

### Backend.Application
###### This layer contains all application logic. Dependent on the domain layer, but no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers.

##### Config
* [EmpireConfig](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Application/Application/Config/EmpireConfig.cs)
* [MilleniumFalconConfig](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Application/Application/Config/MilleniumFalconConfig.cs)

##### Interfaces
* [IConfigFileReader](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Application/Application/Interfaces/Common/IConfigFileReader.cs)
* [IRouteRepository](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Application/Application/Interfaces/Repository/IRoutesRepository.cs)

##### Services
* [OnboardComputerService (implements OnboardComputerUsecases)](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Application/Application/Services/OnboardComputerService.cs)

### Backend.Infrastructure
###### This layer contains classes for accessing external resources such as file Databases, File etc
##### Common
* [JsonFileReader (implements IConfigFileReader)](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Infrastructure/Infrastructure/Common/JsonFileReader.cs)

##### Repository
* [RouteRepository (implements IRouteRepository)](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/src/Backend.Infrastructure/Infrastructure/Repository/RoutesRepository.cs)
        
### Backend.WebApi
###### This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection.
* API Endpoints
    * /milleniumfalcon
        * Post
    * /empire
        * Post
    * /successodds
        * Get


### MilleniumFalcon.CLI
Contains the CLI project, when launched you can access a Swagger page [http://localhost:5000/swagger/index.html
](
http://localhost:5000/swagger/index.html
)


### MilleniumFalcon.Front
Contains the Frond end project

## Test Overview


### What is and what should be
*The tests are separated between __Unit and Integration tests__. **To be more complete the project should contain a end-to-end test project for the Millenium front and CLI projects*** . The test coverage should also be displayed.

### Backend.UnitTests
* [PlanetTests](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/test/MilleniumFalcon.UnitTests/Domain/Models/PlanetTests.cs)
* [RouteTests](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/test/MilleniumFalcon.UnitTests/Domain/Models/RouteTests.cs)
* [OnboardComputerServiceTests](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/test/MilleniumFalcon.UnitTests/Services/OnboardComputerServiceTests.cs)


### Backend.IntegrationTests
* [JsonFileReaderTests](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/test/MilleniumFalcon.IntegrationTests/Common/JsonFileReaderTests.cs)
* [RoutesRepositoryTests](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/test/MilleniumFalcon.IntegrationTests/Repository/RoutesRepositoryTests.cs)
* [OnboardComputerServiceTests](https://github.com/pascally/dataiku-millenium-falcon-challenge/blob/master/MilleniumFalcon/test/MilleniumFalcon.IntegrationTests/Services/OnboardComputerServiceTests.cs)
###### ComputeOddsToDestination

| Input         | Output        |
| ------------- |:-------------:|
| millennium-falcon1.json + empire1.json      | 0.0     |
| millennium-falcon1.json + empire2.json      | 81.0    |
| millennium-falcon1.json + empire3.json      | 90.0    |
| millennium-falcon1.json + empire4.json      | 100.0   |


