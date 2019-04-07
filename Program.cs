using Application;

namespace RoadSimulation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var simulation = new Simulation(LogInfoToConsole);
            var simulationInfo = simulation.Simulate();
            if(simulationInfo.IsFailure)
                LogErrorToConsole(simulationInfo.Error);

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
