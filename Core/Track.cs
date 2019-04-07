using System.Collections.Generic;
using Core.Utils;
using CSharpFunctionalExtensions;

namespace Core
{
    public class Track
    {
        public int Id { get; }
        public int MaxVehiclesCuota { get; }
        
        public IEnumerable<Vehicle> Vehicles { get; }
        public IEnumerable<Obstacle> Obstacles { get; }

        private Track(int id, int maxVehiclesCuota, IEnumerable<Vehicle> vehicles, IEnumerable<Obstacle> obstacles)
        {
            this.Id = id;
            this.MaxVehiclesCuota = maxVehiclesCuota;
            this.Vehicles = vehicles;
            this.Obstacles = obstacles;
        }

        public static Track Create(int id, int maxVehiclesCuota, IEnumerable<Vehicle> vehicles, IEnumerable<Obstacle> obstacles) => 
            new Track(id, maxVehiclesCuota, vehicles, obstacles);

        public static Result<Track> CreateEmpty(int maxVehiclesCuota)
        {
            if(maxVehiclesCuota <= 0)
                return Result.Fail<Track>($"maxVehiclesCuota must be > 0. Current value is: {maxVehiclesCuota}");

            var id = IdGenerator.GetNextIdForType(IdGenerator.IdType.TRACK);
            var vehicleList = new List<Vehicle>();
            var obstacleList = new List<Obstacle>();
            return Result.Ok(Track.Create(id, maxVehiclesCuota, vehicleList, obstacleList));
        }
    }

    public static class TrackExtensions
    {
        public static Result<Track> AddVehicle(this Track track, Vehicle vehicle)
        {
            var newVehicleList = new List<Vehicle>(track.Vehicles) { vehicle };
            return Result.Ok(Track.Create(track.Id, track.MaxVehiclesCuota, newVehicleList, track.Obstacles));
        }

        public static Track AddObstacle(this Track track, Obstacle obstacle)
        {
            var newObstacleList = new List<Obstacle>(track.Obstacles) { obstacle };
            return Track.Create(track.Id, track.MaxVehiclesCuota, track.Vehicles, newObstacleList);
        }

        public static IEnumerable<Result<Track>> CreateRandomTracks(int minVehiclesCuota, int maxVehiclesCuota)
        {
            var maxVehiclesCuotaRandomValue = Utilities.GetRandomInteger(minVehiclesCuota, maxVehiclesCuota);
            yield return Track.CreateEmpty(maxVehiclesCuotaRandomValue)
                .OnBoth(trackResult => trackResult);
        }
    }
}