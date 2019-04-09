using System;
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

        public static Result<Vehicle> Create(int id, Velocity maxVelocity, Velocity currentVelocity)
        {
            if(currentVelocity > maxVelocity)
                return Result.Fail<Vehicle>($"Cannot set current velocity ({currentVelocity}) to a higer value than its max. allowed one ({maxVelocity})");
            return Result.Ok(new Vehicle(id, maxVelocity, currentVelocity));
        }

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
            while(true)
            {
                var randomVelocityValue = Utilities.GetRandomDouble(minVelocity.Value, maxVelocity.Value);
                yield return Velocity.Create(randomVelocityValue)
                    .OnSuccess(velocity => Vehicle.Create(velocity))
                    .OnBoth(vehicleResult => vehicleResult);
            }
        }

        public static Result<Vehicle> Drive(this Vehicle vehicle)
        {
            var randomSpeedToAdd = Utilities.GetRandomDouble(1, 10);
            var velocityToSetResult = vehicle.CurrentVelocity + Velocity.Create(randomSpeedToAdd);
            if(velocityToSetResult.IsFailure)
                return Result.Fail<Vehicle>(velocityToSetResult.Error);

            var velocityToSet = velocityToSetResult.Value <= vehicle.MaxVelocity ? velocityToSetResult.Value : vehicle.MaxVelocity;
            return Vehicle.Create(vehicle.Id, vehicle.MaxVelocity, velocityToSet);
        }

        public static void PrintToLog(this Vehicle vehicle, Action<string> logger)
        {
            if(logger != null)
                logger($"VehicleId {vehicle.Id} - Current Speed: {vehicle.CurrentVelocity} - Max Speed: {vehicle.MaxVelocity}");
        }
    }
}