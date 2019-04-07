using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Core.Utils
{
    public static class Utilities
    {
        private static Random _randomizer = new Random();
        private static object __lockObj = new object();

        public static Maybe<(T chosenObject, int objectPosition)> GetRandomItem<T>(this List<T> list)
        {
            var count = list.Count;
            if(count == 0)
                return Maybe<(T, int)>.None;

            int position = 0;
            lock(__lockObj)
            {
                position = _randomizer.Next(count);
            }
            
            if(position == count)
                position--;

            return Maybe<(T, int)>.From((list[position], position));
        }

        public static List<T> ReplaceAt<T>(this List<T> list, int index, T value)
        {
            list[index] = value;
            return list;
        }

        public static double GetRandomDouble(double minValuePossible, double maxValuePossible)
        {
            var difference = maxValuePossible - minValuePossible;
            if(difference < 0)
                throw new ArgumentException($"Parameter values are not valid. Min. must be less than Max. value");
            if(difference == 0)
                return minValuePossible;

            double randomDoubleValue = 0;
            lock(__lockObj)
            {
                randomDoubleValue = _randomizer.NextDouble();
            }

            return minValuePossible + (difference * randomDoubleValue);
        }

        public static int GetRandomInteger(int minValuePossible, int maxValuePossible)
        {
            if(minValuePossible > maxValuePossible)
                throw new ArgumentException($"Parameter values are not valid. Min. must be less than Max. value");
            if(minValuePossible == maxValuePossible)
                return minValuePossible;

            var randomIntegerValue = 0;
            lock(__lockObj)
            {
                randomIntegerValue = _randomizer.Next(minValuePossible, maxValuePossible);
            }

            return randomIntegerValue;
        }
    }
}