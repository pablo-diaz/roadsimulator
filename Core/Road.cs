using System;
using System.Linq;
using System.Collections.Generic;

using Core.Utils;
using CSharpFunctionalExtensions;
using Core.Types;

namespace Core
{
    public class Road
    {
        public int Id { get; }
        public IEnumerable<Track> Tracks { get; }

        private Road(int id, IEnumerable<Track> tracks)
        {
            this.Id = id;
            this.Tracks = tracks;
        }

        public static Road Create(int id, IEnumerable<Track> tracks) =>
            new Road(id, tracks);

        public static Road CreateEmpty()
        {
            var id = IdGenerator.GetNextIdForType(IdGenerator.IdType.ROAD);
            var emptyList = new List<Track>();
            return Create(id, emptyList);
        }
    }

    public static class RoadExtensions
    {
        public static Road AddTrack(this Road road, Track track)
        {
            var newTrackList = new List<Track>(road.Tracks) { track };
            return Road.Create(road.Id, newTrackList);
        }

        public static Result<Road> AddVehicleToRandomTrack(this Road road, Vehicle vehicle)
        {
            if(road.Tracks.Count() == 0)
                return Result.Fail<Road>("There are no Tracks in this Road yet");
            
            var newTrackList = new List<Track>(road.Tracks);
            var (trackChosen, trackPosition) = road.Tracks.ToList().GetRandomItem().Value;
            var newTrackResult = trackChosen.AddVehicle(vehicle);
            if(newTrackResult.IsFailure)
                return Result.Fail<Road>(newTrackResult.Error);
            newTrackList = newTrackList.ReplaceAt(trackPosition, newTrackResult.Value);
            return Result.Ok<Road>(Road.Create(road.Id, newTrackList));
        }

        public static int GetTotalVehiclesCountPlacedInTracks(this Road road)
        {
            return road.Tracks.Sum(t => t.Vehicles.Count());
        }

        public static Result<Road> AddRandomTracks(this Road road, int maxTracksToGenerate, 
            int minVehiclesCuota, int maxVehiclesCuota, 
            Func<int, int, IEnumerable<Result<Track>>> CreateRandomTracksFn)
        {
            var newRoad = road;
            foreach(var randomTrackResult in CreateRandomTracksFn(minVehiclesCuota, maxVehiclesCuota))
            {
                if(randomTrackResult.IsFailure)
                    return Result.Fail<Road>(randomTrackResult.Error);
                newRoad = newRoad.AddTrack(randomTrackResult.Value);

                if(newRoad.Tracks.Count() >= maxTracksToGenerate)
                    break;
            }
            return Result.Ok(newRoad);
        }

        public static Result<Road> AddRandomVehiclesToRoad(this Road road, Velocity minVelocity, Velocity maxVelocity, 
            int maxVehiclesToCreate, Func<Velocity, Velocity, IEnumerable<Result<Vehicle>>> CreateRandomVehiclesFn)
        {
            var newRoad = road;
            int numVehiclesAdded = 0;
            foreach(var randomVehicleResult in CreateRandomVehiclesFn(minVelocity, maxVelocity))
            {
                var newRoadResult = randomVehicleResult
                    .OnSuccess(randomVehicle => newRoad.AddVehicleToRandomTrack(randomVehicle))
                    .OnSuccessTry(nextRoad => newRoad = nextRoad);

                if(newRoadResult.IsFailure)
                    return newRoadResult;

                numVehiclesAdded++;
                if(numVehiclesAdded >= maxVehiclesToCreate)
                    break;
            }

            return Result.Ok<Road>(newRoad);
        }
    }
}