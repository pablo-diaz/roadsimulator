using System;
using System.Threading;
using System.Threading.Tasks;

using Application;
using Infrastructure;
using Infrastructure.Contracts;

using CSharpFunctionalExtensions;

namespace RoadSimulation
{
    public class Program
    {
        private static object __lockObj = new object();

        public static async Task Main(string[] args)
        {
            var cancellationToken = new CancellationToken();
            IConfig configuration = new AppConfiguration();

            await Simulation.InitializeWithLogger(LogInfoToConsole, configuration)
                .OnSuccess(simulation => simulation.Simulate(LogInfoToConsole, cancellationToken))
                .OnFailure(LogErrorToConsole);
            LogInfoToConsole("Simulation finished");
        }

        private static void LogInfoToConsole(string info)
        {
            lock(__lockObj)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(info);
                Console.ForegroundColor = currentColor;
            }
        }

        private static void LogErrorToConsole(string error)
        {
            lock(__lockObj)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = currentColor;
            }
        }
    }
}
