using System;
using System.Collections.Generic;
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
        private Dictionary<int, Vehicle> _vehicleStatusesDb = new Dictionary<int, Vehicle>();
        private static object __lockObj = new object();

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

        public async Task<Result> Simulate(Action<string> loggerFn, CancellationToken cancellationToken)
        {
            this._logInfoFn("Starting simulation set up ....");
            var simulationThreads = new List<Task>();
            var result = SetupRoad()
                .OnSuccess(road => {
                    var vehiclesCount = road.GetTotalVehiclesCountPlacedInTracks();
                    var numOfWatcherThreads = 1;
                    var numOfThreadsToAllocate = vehiclesCount + numOfWatcherThreads;
                    ThreadPool.SetMinThreads(numOfThreadsToAllocate, numOfThreadsToAllocate);
                    return road;
                })
                .OnSuccess(road => StartSimulationThreadsForRoad(road, null, KeepTrackOfVehicleState, cancellationToken))
                .OnSuccess(vehicleThreads => simulationThreads.AddRange(vehicleThreads))
                .OnSuccess(() => RunVehicleStatusWatcherThread(loggerFn, cancellationToken))
                .OnSuccess(vehicleStatusWatcherThread => simulationThreads.Add(vehicleStatusWatcherThread));

            await Task.WhenAll(simulationThreads);

            return result;
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

        private Result<List<Task>> StartSimulationThreadsForRoad(Road road, Action<string> loggerFn, Action<Vehicle> statusFn, CancellationToken cancellationToken)
        {
            var trackThreads = new List<Task>();
            foreach(var track in road.Tracks)
            {
                var vehicleThreads = StartSimulationThreadsForTrack(track, loggerFn, statusFn, cancellationToken);
                trackThreads.AddRange(vehicleThreads);
            }

            trackThreads.ForEach(thread => thread.Start());

            return Result.Ok(trackThreads);
        }

        private IEnumerable<Task<Result>> StartSimulationThreadsForTrack(Track track, Action<string> loggerFn, Action<Vehicle> statusFn, CancellationToken cancellationToken)
        {
            return track.Vehicles
                .Select(vehicle => new Task<Result>(() => DoVehicleThreadWork(vehicle, loggerFn, statusFn, cancellationToken)));
        }

        private Result<Vehicle> DoVehicleThreadWork(Vehicle vehicle, Action<string> loggerFn, Action<Vehicle> statusFn, CancellationToken cancellationToken)
        {
            return vehicle.Drive(loggerFn, statusFn, cancellationToken);
        }

        private void KeepTrackOfVehicleState(Vehicle vehicle)
        {
            lock(__lockObj)
            {
                this._vehicleStatusesDb[vehicle.Id] = vehicle;
            }
        }

        private Result<Task> RunVehicleStatusWatcherThread(Action<string> loggerFn, CancellationToken cancellationToken)
        {
            var task = SetupVehicleStatusWatcherThread(loggerFn, cancellationToken);
            task.Start();
            return Result.Ok(task);
        }

        private Task SetupVehicleStatusWatcherThread(Action<string> loggerFn, CancellationToken cancellationToken)
        {
            return new Task(() => {
                do
                {
                    lock(__lockObj)
                    {
                        foreach(var vehicleEntry in this._vehicleStatusesDb)
                            vehicleEntry.Value.PrintToLog(loggerFn);
                        loggerFn("----------------------------------------------------");
                    }
                    Thread.Sleep(500);
                } while(!cancellationToken.IsCancellationRequested);
            });
        }
    }
}