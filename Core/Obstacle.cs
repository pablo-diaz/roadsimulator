namespace Core
{
    public class Obstacle
    {
        public enum ObstacleType
        {
            SEMAPHORE = 1
        }

        public int Id { get; }
        public ObstacleType Type { get; }

        private Obstacle(int id, ObstacleType type)
        {
            this.Id = id;
            this.Type = type;
        }

        public static Obstacle CreateForType(ObstacleType type)
        {
            var id = IdGenerator.GetNextIdForType(IdGenerator.IdType.OBSTACLE);
            return new Obstacle(id, type);
        }
    }

    public static class ObstacleExtensions
    {

    }
}