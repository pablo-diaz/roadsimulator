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
            var cancellationTokenSource = new CancellationTokenSource();
            IConfig configuration = GetServiceImplementation<IConfig>();

            var finishTriggerWatcherTask = StayPendingForFinishTrigger(cancellationTokenSource);
            var simulationTask = Simulation.InitializeWithLogger(LogInfoToConsole, configuration)
                .OnSuccess(simulation => simulation.Simulate(LogInfoToConsole, cancellationTokenSource.Token))
                .OnFailure(LogErrorToConsole);

            await Task.WhenAll(new Task[] { simulationTask, finishTriggerWatcherTask });

            LogInfoToConsole("Simulation finished");
        }

        private static Task StayPendingForFinishTrigger(CancellationTokenSource cancellationTokenSource)
        {
            var task = new Task(() => {
                do
                {
                    Console.WriteLine("Press Q to end this simulation");
                    var keyPressed = Console.ReadKey();
                    Console.WriteLine();
                    if(keyPressed.Key == ConsoleKey.Q)
                        break;
                } while(true);

                Console.WriteLine("Ending this simulation");
                cancellationTokenSource.Cancel();
            });
            
            task.Start();
            return task;
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

        private static TService GetServiceImplementation<TService>() where TService: class
        {
            Type serviceType = typeof(TService);
            if(serviceType == typeof(IConfig))
                return new AppConfiguration() as TService;

            throw new Exception($"There is no implementation setup for service {nameof(serviceType)}");
        }
    }
}
