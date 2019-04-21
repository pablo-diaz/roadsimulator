using System;
using System.Threading;
using System.Threading.Tasks;

using Application;
using Infrastructure;
using Infrastructure.Contracts;
using Presentation;

namespace RoadSimulation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetServiceImplementation<IConfig>();
            await ConsoleApp.Present(configuration);
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
