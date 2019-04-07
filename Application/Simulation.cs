using System;

using Core;
using Core.Types;

using CSharpFunctionalExtensions;

namespace Application
{
    public class Simulation
    {
        private readonly Action<string> _logInfoFn;

        private readonly Velocity _minVelocitySetting;
        private readonly Velocity _maxVelocitySetting;
        private readonly int _minVehiclesCuotaSetting = 10;
        private readonly int _maxVehiclesCuotaSetting = 30;
        private readonly int _maxTracksToCreate = 10;
        private readonly int _maxVehiclesToCreate = 50;

        public Simulation(Action<string> logInfoFn)
        {
            this._logInfoFn = logInfoFn;
            var (minVelocity, maxVelocity) = SetupMinAndMaxVelocities();
            this._minVelocitySetting = minVelocity;
            this._maxVelocitySetting = maxVelocity;
        }

        public Result Simulate()
        {
            this._logInfoFn("Starting simulation set up ....");
            var roadSetupResult = SetupRoad();
            if(roadSetupResult.IsFailure)
                return Result.Fail($"Error setting up road. Reason: {roadSetupResult.Error}");

            return Result.Ok();
        }

        private (Velocity minVelocity, Velocity maxVelocity) SetupMinAndMaxVelocities()
        {
            var minVelocityResult = Velocity.Create(30);
            var maxVelocityResult = Velocity.Create(80);
            if (minVelocityResult.IsFailure)
                throw new ApplicationException($"Error setting min. velocity. Reason: {minVelocityResult.Error}");
            if (maxVelocityResult.IsFailure)
                throw new ApplicationException($"Error setting max. velocity. Reason: {maxVelocityResult.Error}");
            return (minVelocityResult.Value, maxVelocityResult.Value);
        }

        private Result<Road> SetupRoad()
        {
            return Road.CreateEmpty()
                .AddRandomTracks(this._maxTracksToCreate, this._minVehiclesCuotaSetting, this._maxVehiclesCuotaSetting, TrackExtensions.CreateRandomTracks)
                .OnSuccess(road => road.AddRandomVehiclesToRoad(this._minVelocitySetting, this._maxVelocitySetting, this._maxVehiclesToCreate, VehicleExtensions.CreateRandomVehicles))
                .OnBoth(roadResult => roadResult);
        }
    }
}