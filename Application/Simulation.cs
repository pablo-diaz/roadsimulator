using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly int _minVehiclesCuotaSetting = 5;
        private readonly int _maxVehiclesCuotaSetting = 10;
        private readonly int _maxTracksToCreate = 20;
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

        public async Task<Result> Simulate(Action<string> logger, CancellationToken cancellationToken)
        {
            this._logInfoFn("Starting simulation set up ....");
            return await SetupRoad()
                .OnSuccess(road => StartSimulationThreadsForRoad(road, logger, cancellationToken));
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
                    this._maxVelocitySetting, this._maxVehiclesToCreate, 
                    VehicleExtensions.CreateRandomVehicles, this._logInfoFn));
        }

        private async Task<Result> StartSimulationThreadsForRoad(Road road, Action<string> logger, CancellationToken cancellationToken)
        {
            foreach(var track in road.Tracks)
                await StartSimulationThreadsForTrack(track, logger, cancellationToken);
            return Result.Ok();
        }

        private async Task<Result> StartSimulationThreadsForTrack(Track track, Action<string> logger, CancellationToken cancellationToken)
        {
            foreach(var vehicle in track.Vehicles)
            {
                var newVehicleResult = await Task.Factory.StartNew(() => { 
                    return vehicle.Drive()
                        .OnSuccess(newVehicle => newVehicle.PrintToLog(logger));
                }, cancellationToken);
            }

            return Result.Ok();
        }
    }
}