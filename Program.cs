using Application;
using CSharpFunctionalExtensions;

namespace RoadSimulation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Simulation.InitializeWithLogger(LogInfoToConsole)
                .OnSuccess(simulation => simulation.Simulate())
                .OnFailure(LogErrorToConsole);
            LogInfoToConsole("Done.");
        }

        private static void LogInfoToConsole(string info)
        {
            System.Console.WriteLine(info);
        }

        private static void LogErrorToConsole(string error)
        {
            System.Console.WriteLine(error);
        }
    }
}
