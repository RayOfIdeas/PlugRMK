using System.Collections.Generic;
using UnityEngine;

namespace PlugRMK.GenericUti
{
    public static class MathUtility
    {
        public enum TwoAxises { XY, XZ, YZ }

        public struct Line
        {
            public Vector2 p1;
            public Vector2 p2;

            public Line(Vector2 p1, Vector2 p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }

        #region [Methods: Intersection]

        public static Vector2? GetIntersection(Line lineA, Line lineB)
        {
            var x =
                ((lineA.p1.x * lineA.p2.y - lineA.p1.y * lineA.p2.x) * (lineB.p1.x - lineB.p2.x) - (lineA.p1.x - lineA.p2.x) * (lineB.p1.x * lineB.p2.y - lineB.p1.y * lineB.p2.x))
                /
                ((lineA.p1.x - lineA.p2.x) * (lineB.p1.y - lineB.p2.y) - (lineA.p1.y - lineA.p2.y) * (lineB.p1.x - lineB.p2.x));

            if (float.IsNaN(x))
                return null;

            var y =
                ((lineA.p1.x * lineA.p2.y - lineA.p1.y * lineA.p2.x) * (lineB.p1.y - lineB.p2.y) - (lineA.p1.y - lineA.p2.y) * (lineB.p1.x * lineB.p2.y - lineB.p1.y * lineB.p2.x))
                /
                ((lineA.p1.x - lineA.p2.x) * (lineB.p1.y - lineB.p2.y) - (lineA.p1.y - lineA.p2.y) * (lineB.p1.x - lineB.p2.x));

            if (float.IsNaN(y))
                return null;

            return new Vector2(x, y);
        }

        public static bool IsIntersect(this Vector2 pos, Vector2 otherPosMin, Vector2 otherPosMax, float posRadius = 0f)
        {
            return pos.x + posRadius > otherPosMin.x && pos.x - posRadius < otherPosMax.x &&
                    pos.y + posRadius > otherPosMin.y && pos.y - posRadius < otherPosMax.y;
        }

        #endregion

        #region [Methods: New Vector]

        public static Vector2 NewVector2(this float value)
        {
            return new(value, value);
        }

        public static Vector2 NewVector2(this Vector3 value, TwoAxises twoAxises = TwoAxises.XZ)
        {
            return twoAxises switch
            {
                TwoAxises.XZ => new(value.x, value.z),
                TwoAxises.YZ => new(value.y, value.z),
                _ => new(value.x, value.y),
            };
        }

        public static Vector3 NewVector3(this float value)
        {
            return new(value, value, value);
        }

        #endregion

        #region [Methods: Random Vector]

        public static Vector3 GetRandomVector3(Vector3 randomRange)
        {
            return new Vector3(
                Random.Range(-randomRange.x, randomRange.x),
                Random.Range(-randomRange.y, randomRange.y),
                Random.Range(-randomRange.z, randomRange.z)
            );
        }
            
        #endregion

        #region [Methods: Angle]

        public static Vector2 ToVector2(this float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        public static float ToAngle(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        public static float GetAngle(Vector2 from, Vector2 to)
        {
            var direction = to - from;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        #endregion

        #region [Methods: Time]

        public static string SecondsToTimeString(this float seconds)
        {
            if (seconds < 3600)
            {
                int minutes = (int)seconds / 60;
                int sec = (int)seconds % 60;
                return $"{minutes:D2}:{sec:D2}";
            }
            else
            {
                int hours = (int)seconds / 3600;
                int minutes = (int)seconds % 3600 / 60;
                int sec = (int)seconds % 60;
                return $"{hours:D2}:{minutes:D2}:{sec:D2}";
            }
        }

        public static string SecondsToTimeString(this int seconds)
        {
            if (seconds < 3600)
            {
                int minutes = seconds / 60;
                int sec = seconds % 60;
                return $"{minutes:D2}:{sec:D2}";
            }
            else
            {
                int hours = seconds / 3600;
                int minutes = seconds % 3600 / 60;
                int sec = seconds % 60;
                return $"{hours:D2}:{minutes:D2}:{sec:D2}";
            }
        }

        #endregion

        #region [Methods: Cross Operation]

        public static void CrossOp(out float xCurrent, float xTotal, float yCurrent, float yTotal)
        {
            xCurrent = xTotal * yCurrent / yTotal;
        }

        public static void CrossOp(float xCurrent, out float xTotal, float yCurrent, float yTotal)
        {
            xTotal = xCurrent * yTotal / yCurrent;
        }

        #endregion

        #region [Methods: Clamp]

        public static Vector2 Clamp(this Vector2 vector2, float min, float max)
        {
            return vector2.Clamp(min.NewVector2(), max.NewVector2());
        }

        public static Vector2 Clamp(this Vector2 vector2, Vector2 min, Vector2 max)
        {
            return new(
                Mathf.Clamp(vector2.x, min.x, max.x),
                Mathf.Clamp(vector2.y, min.y, max.y)
                );
        }

        #endregion

        #region [Methods: Between]

        public static bool IsBetween(this float value, float min, float max)
        {
            return value > min && value < max;
        }

        public static float Between(float min, float max, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            var distance = max - min;
            return min + distance * ratio;
        }

        public static Vector3 Between(Vector2 min, Vector2 max, float ratio)
        {
            return new(Between(min.x, max.x, ratio), Between(min.y, max.y, ratio));
        }

        public static Vector3 Between(Vector3 min, Vector3 max, float ratio)
        {
            return new(Between(min.x, max.x, ratio), Between(min.y, max.y, ratio), Between(min.z, max.z, ratio));
        }

        #endregion

        #region [Methods: Distance]

        public static float DistanceXY(Vector3 a, Vector3 b)
        {
            var num1 = a.x - b.x;
            var num2 = a.y - b.y;
            return (float)Mathf.Sqrt(num1 * num1 + num2 * num2);
        }

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            var num1 = a.x - b.x;
            var num2 = a.z - b.z;
            return (float)Mathf.Sqrt(num1 * num1 + num2 * num2);
        }

        public static float DistanceYZ(Vector3 a, Vector3 b)
        {
            var num1 = a.y - b.y;
            var num2 = a.z - b.z;
            return (float)Mathf.Sqrt(num1 * num1 + num2 * num2);
        }

        public static float DistanceXYZ(Vector3 a, Vector3 b)
        {
            var num1 = a.x - b.x;
            var num2 = a.y - b.y;
            var num3 = a.z - b.z;
            return (float)Mathf.Sqrt(num1 * num1 + num2 * num2 + num3 * num3);
        }

        #endregion

        #region [Methods: Others]

        public static List<int> GetRatio(List<int> list)
        {
            var lowestInt = GetTheLowestNumber(list);
            if (lowestInt < 0)
                return list;

            var listResult = new List<int>(list);

            for (int i = 2; i < lowestInt; i++)
            {
                if (CanAllNumbersBeDividedBy(listResult, i))
                {
                    DivideAllNumbersBy(listResult, i);
                    i--;
                }
            }

            return listResult;

            int GetTheLowestNumber(List<int> allNumbers)
            {
                int lowestInt = allNumbers[0];
                for (int i = 1; i < allNumbers.Count; i++)
                    if (allNumbers[i] < lowestInt)
                        lowestInt = allNumbers[i];

                return lowestInt;
            }

            bool CanAllNumbersBeDividedBy(List<int> allNumbers, int divider)
            {
                foreach (var num in allNumbers)
                    if (num % divider != 0)
                        return false;
                return true;
            }

            void DivideAllNumbersBy(List<int> allNumbers, int divider)
            {
                for (int n = 0; n < allNumbers.Count; n++)
                    allNumbers[n] = allNumbers[n] / divider;
            }

        }

        public static (Vector2 min, Vector2 max) SortMinMax(Vector2 a, Vector2 b)
        {
            var minX = a.x < b.x ? a.x : b.x;
            var maxX = a.x > b.x ? a.x : b.x;
            var minY = a.y < b.y ? a.y : b.y;
            var maxY = a.y > b.y ? a.y : b.y;
            return (new(minX, minY), new(maxX, maxY));
        }

        #endregion

    }
}
