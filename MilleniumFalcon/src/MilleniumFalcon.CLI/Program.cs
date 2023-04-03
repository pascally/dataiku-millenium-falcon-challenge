// See https://aka.ms/new-console-template for more information

using Backend.Application.Services;
using Backend.Domain.UseCases;
using Backend.Infrastructure.Common;
using Backend.Infrastructure.Repository;

OnboardComputerUsecases onboardComputerService = new OnboardComputerService(new RoutesRepository(), new JsonFileReader());

switch (args.Count())
{
    case 1:
        Console.WriteLine("Lacking empire configs files");
        Console.WriteLine("give-me-the-odds example1/millennium-falcon.json example1/empire.json");
        return;
    case 2:
        if (!onboardComputerService.LoadMilleniumFalconDatas(args[0]))
        {
            Console.WriteLine($"Failed to load file {args[0]}");
            return;
        }
        if (!onboardComputerService.LoadEmpireDatas(args[1]))
        {
            Console.WriteLine($"Failed to load file {args[1]}");
            return;
        }
        Console.WriteLine($"{onboardComputerService.ComputeOddsToDestination()}");
        return;
    default:
        Console.WriteLine("Please enter filepath to millenium-falcon and empire configs files");
        Console.WriteLine("give-me-the-odds example1/millennium-falcon.json example1/empire.json");
        return;
}

