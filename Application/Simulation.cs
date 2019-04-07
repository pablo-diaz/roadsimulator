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

        private Simulation(Action<string> logInfoFn, Velocity minVelocity, Velocity maxVelocity)
        {
            this._logInfoFn = logInfoFn;
            this._minVelocitySetting = minVelocity;
            this._maxVelocitySetting = maxVelocity;
        }

        public static Result<Simulation> InitializeWithLogger(Action<string> logInfoFn)
        {
            var minAndMaxVelocityResult = SetupMinAndMaxVelocities();
            if(minAndMaxVelocityResult.IsFailure)
                return Result.Fail<Simulation>(minAndMaxVelocityResult.Error);
            return Result.Ok(new Simulation(logInfoFn, minAndMaxVelocityResult.Value.minVelocity, 
                minAndMaxVelocityResult.Value.maxVelocity));
        }

        public Result Simulate()
        {
            this._logInfoFn("Starting simulation set up ....");
            return SetupRoad();
        }

        private static Result<(Velocity minVelocity, Velocity maxVelocity)> SetupMinAndMaxVelocities()
        {
            var minVelocityResult = Velocity.Create(30);
            var maxVelocityResult = Velocity.Create(80);
            if (minVelocityResult.IsFailure)
                Result.Fail<(Velocity, Velocity)>($"Error setting min. velocity. Reason: {minVelocityResult.Error}");
            if (maxVelocityResult.IsFailure)
                Result.Fail<(Velocity, Velocity)>($"Error setting max. velocity. Reason: {maxVelocityResult.Error}");
            return Result.Ok((minVelocityResult.Value, maxVelocityResult.Value));
        }

        private Result<Road> SetupRoad()
        {
            return Road.CreateEmpty()
                .AddRandomTracks(this._maxTracksToCreate, this._minVehiclesCuotaSetting, 
                    this._maxVehiclesCuotaSetting, TrackExtensions.CreateRandomTracks)
                .OnSuccess(road => road.AddRandomVehiclesToRoad(this._minVelocitySetting, 
                    this._maxVelocitySetting, this._maxVehiclesToCreate, VehicleExtensions.CreateRandomVehicles));
        }
    }
}