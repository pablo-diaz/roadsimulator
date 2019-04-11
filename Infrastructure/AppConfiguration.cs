using Infrastructure.Contracts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace Infrastructure
{
    public sealed class AppConfiguration : IConfig
    {
        private readonly IConfiguration _config;

        public AppConfiguration()
        {
            this._config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public int GetMaxAllowedVelocity() => int.Parse(this._config["SimulationSettings:MaxAllowedVelocity"]);

        public int GetMaxTracksToCreate() => int.Parse(this._config["SimulationSettings:MaxTracksToCreate"]);

        public int GetMaxVehiclesCuota() => int.Parse(this._config["SimulationSettings:MaxVehiclesCuota"]);

        public int GetMaxVehiclesToCreate() => int.Parse(this._config["SimulationSettings:MaxVehiclesToCreate"]);

        public int GetMinAllowedVelocity() => int.Parse(this._config["SimulationSettings:MinAllowedVelocity"]);

        public int GetMinVehiclesCuota() => int.Parse(this._config["SimulationSettings:MinVehiclesCuota"]);
    }
}