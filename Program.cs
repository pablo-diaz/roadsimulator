using System;
using System.Threading;
using System.Threading.Tasks;

using Application;
using CSharpFunctionalExtensions;

namespace RoadSimulation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var cancellationToken = new CancellationToken();

            await Simulation.InitializeWithLogger(LogInfoToConsole)
                .OnSuccess(simulation => simulation.Simulate(LogInfoToConsole, cancellationToken))
                .OnFailure(LogErrorToConsole);
            LogInfoToConsole("Simulation finished");
        }

        private static void LogInfoToConsole(string info)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(info);
            Console.ForegroundColor = currentColor;
        }

        private static void LogErrorToConsole(string error)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = currentColor;
        }
    }
}
