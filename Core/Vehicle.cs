using System.Collections.Generic;
using Core.Types;
using Core.Utils;
using CSharpFunctionalExtensions;

namespace Core
{
    public class Vehicle
    {
        public int Id { get; }
        public Velocity MaxVelocity { get; }
        public Velocity CurrentVelocity { get; }

        private Vehicle(int id, Velocity maxVelocity, Velocity currentVelocity)
        {
            this.Id = id;
            this.MaxVelocity = maxVelocity;
            this.CurrentVelocity = currentVelocity;
        }

        public static Vehicle Create(int id, Velocity maxVelocity, Velocity currentVelocity) =>
            new Vehicle(id, maxVelocity, currentVelocity);

        public static Result<Vehicle> Create(Velocity maxVelocity)
        {
            var id = IdGenerator.GetNextIdForType(IdGenerator.IdType.VEHICLE);
            return Velocity.Create(0)
                .OnSuccess(velocity => Create(id, maxVelocity, velocity))
                .OnBoth(vehicleResult => vehicleResult);
        }
    }

    public static class VehicleExtensions
    {
        public static IEnumerable<Result<Vehicle>> CreateRandomVehicles(Velocity minVelocity, Velocity maxVelocity)
        {
            var randomVelocityValue = Utilities.GetRandomDouble(minVelocity.Value, maxVelocity.Value);
            yield return Velocity.Create(randomVelocityValue)
                .OnSuccess(velocity => Vehicle.Create(velocity))
                .OnBoth(vehicleResult => vehicleResult);
        }
    }
}