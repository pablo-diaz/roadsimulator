using CSharpFunctionalExtensions;

namespace Core.Types 
{
    // Velocity in Kilometer per hour
    public sealed class Velocity
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

        public static Result<Velocity> operator +(Velocity velocity1, Velocity velocity2) => 
            Create(velocity1.Value + velocity2.Value);

        public static Result<Velocity> operator +(Velocity velocity1, Result<Velocity> velocity2Result)
        {
            if(velocity2Result.IsFailure)
                return velocity2Result;
            return velocity1 + velocity2Result.Value;
        }

        public static bool operator ==(Velocity velocity1, Velocity velocity2) =>
            velocity1.Value == velocity2.Value;

        public static bool operator !=(Velocity velocity1, Velocity velocity2) =>
            velocity1.Value != velocity2.Value;

        public static bool operator <(Velocity velocity1, Velocity velocity2) => 
            velocity1.Value < velocity2.Value;

        public static bool operator >(Velocity velocity1, Velocity velocity2) =>
            velocity1.Value > velocity2.Value;

        public static bool operator <=(Velocity velocity1, Velocity velocity2) => 
            velocity1.Value <= velocity2.Value;

        public static bool operator >=(Velocity velocity1, Velocity velocity2) =>
            velocity1.Value >= velocity2.Value;

        public override string ToString() => this.Value.ToString("F2").PadLeft(6, '0');

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj)
        {
            var parsedVelocity = obj as Velocity;
            if (parsedVelocity == null) 
                return false;
            return parsedVelocity.Value == this.Value; 
        }
    }
}