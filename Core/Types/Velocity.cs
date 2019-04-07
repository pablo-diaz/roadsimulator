using CSharpFunctionalExtensions;

namespace Core.Types 
{
    // Velocity in Kilometer per hour
    public class Velocity
    {
        public double Value { get; }
        
        private Velocity(double value)
        {
            this.Value = value;
        }

        public static Result<Velocity> Create(double value)
        {
            if(value < 0)
                return Result.Fail<Velocity>($"Velocity must be greather or equal than 0. Current value was {value}");

            if(value > 80)
                return Result.Fail<Velocity>($"Velocity must be less or equal than 80. Current value was {value}");
            
            var obj = new Velocity(value);
            return Result.Ok<Velocity>(obj);
        }
    }
}