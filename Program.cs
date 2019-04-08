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
            var currentColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.DarkYellow;
            System.Console.WriteLine(info);
            System.Console.ForegroundColor = currentColor;
        }

        private static void LogErrorToConsole(string error)
        {
            var currentColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine(error);
            System.Console.ForegroundColor = currentColor;
        }
    }
}
