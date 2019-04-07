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
            var trackInfoResult = road.TryInsertingVehicleToRandomTrack(vehicle);
            if(trackInfoResult.IsFailure)
                return Result.Fail<Road>(trackInfoResult.Error);
            
            newTrackList = newTrackList.ReplaceAt(trackInfoResult.Value.trackPosition, trackInfoResult.Value.newTrack);
            return Result.Ok<Road>(Road.Create(road.Id, newTrackList));
        }

        public static Result<(Track newTrack, int trackPosition)> TryInsertingVehicleToRandomTrack(this Road road, Vehicle vehicle)
        {
            while(true) // should there exist a limit ?
            {
                var maybeRandomTrackInfo = road.Tracks.ToList().GetRandomItem();
                if(maybeRandomTrackInfo.HasNoValue)
                    return Result.Fail<(Track, int)>("There are no Tracks in this Road yet");
                var newTrackResult = maybeRandomTrackInfo.Value.chosenObject.AddVehicle(vehicle);
                if(newTrackResult.IsSuccess)
                    return Result.Ok((newTrackResult.Value, maybeRandomTrackInfo.Value.objectPosition));
                if(!newTrackResult.Error.StartsWith("Cannot add more vehicles to this Track. Current limit is"))
                    return Result.Fail<(Track, int)>(newTrackResult.Error);
            }
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
            var numTracksAdded = 0;
            foreach(var randomTrackResult in CreateRandomTracksFn(minVehiclesCuota, maxVehiclesCuota))
            {
                var newRoadResult = randomTrackResult
                    .OnSuccess(randomTrack => newRoad.AddTrack(randomTrack))
                    .OnSuccessTry(nextRoad => newRoad = nextRoad);

                if(newRoadResult.IsFailure)
                    return newRoadResult;

                numTracksAdded++;
                if(numTracksAdded >= maxTracksToGenerate)
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