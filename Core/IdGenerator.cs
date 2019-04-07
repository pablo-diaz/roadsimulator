using System.Collections.Generic;

namespace Core
{
    internal static class IdGenerator
    {
        internal enum IdType
        {
            ROAD = 1,
            TRACK = 2,
            VEHICLE = 3,
            OBSTACLE = 4
        }

        private static Dictionary<IdType, int> _idsDB = new Dictionary<IdType, int>() {
            { IdType.ROAD, 0 },
            { IdType.TRACK, 0 },
            { IdType.VEHICLE, 0 },
            { IdType.OBSTACLE, 0 }
        };
        private static object __lockObj = new object();

        internal static int GetNextIdForType(IdType type)
        {
            lock(__lockObj)
            {
                var nextId = _idsDB[type];
                nextId++;
                _idsDB[type] = nextId;
                return nextId;
            }
        }
    }
}