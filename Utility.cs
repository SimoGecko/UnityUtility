// (c) Simone Guggiari 2020-2022

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;

////////// PURPOSE: Various Utility functions //////////

namespace sxg
{
/*
    private static __TYPE__ instance;
    public static __TYPE__ Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<__TYPE__>();
            return instance;
        }
    }


    [CustomEditor(typeof(__TYPE__))]
    public class __TYPE__ Editor : Editor
    {
        private __TYPE__ myTarget;

        private void OnEnable()
        {
            myTarget = (__TYPE__)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // code
        }
    }
*/

    public enum Direction { Up, Left, Down, Right };

    // Naming: Get / Find / Compute / 
    public static class Utility
    {

        ////////////////////////// MATH ////////////////////////////////
        public static int          Mod                  (this int value, int mod)
        {
            return ((value % mod) + mod) % mod;
        }
        public static float        ModF                 (this float value, float mod)
        {
            return ((value % mod) + mod) % mod;
        }
        public static int          Sign                 (this int value)
        {
            return value == 0 ? 0 : value > 0 ? 1 : -1;
        }
        public static float        Sign                 (this float value)
        {
            return Mathf.Abs(value) < 0.0001f ? 0f : value > 0f ? 1f : -1f;
        }
        public static int          SignNo0              (this int value)
        {
            return value >= 0 ? 1 : -1;
        }
        public static float        SignNo0              (this float value)
        {
            return value >= 0f ? 1f : -1f;
        }
        public static float        Remap01_11           (this float value)
        {
            return value * 2f - 1f;
        }
        public static float        Remap11_01           (this float value)
        {
            return (value + 1f) / 2f;
        }
        public static float        Remap                (this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (toMax - toMin) * ((value - fromMin) / (fromMax - fromMin));
        }
        public static float        RemapClamp           (this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (toMax - toMin) * Mathf.Clamp01((value - fromMin) / (fromMax - fromMin));
        }
        public static float        Pow2                 (this float value)
        {
            return value * value;
        }
        public static float        Truncate             (this float value, float max)
        {
            return Mathf.Sign(value) * Mathf.Min(value, max);
        }
        public static float        Truncate             (this float value, float min, float max)
        {
            return Mathf.Sign(value) * Mathf.Clamp(value, min, max);
        }


        ////////////////////////// ANGLE ////////////////////////////////
        public static float        Canonicalize         (float angleDeg)
        {
            // returns the angle in the canonical range ]-180, 180]
            return (angleDeg + 180f).ModF(360f) - 180f;
        }
        public static float        CanonicalizePI       (float angleRad)
        {
            // returns the angle in the canonical range [0, 2pi[
            return (angleRad).ModF(Mathf.PI * 2f);
        }
        public static Vector2      DirectionFromAngle   (float angleDeg)
        {
            float angleRad = angleDeg * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        public static float        AngleFromDirection   (Vector2 direction)
        {
            float angleRad = Mathf.Atan2(direction.y, direction.x);
            return angleRad * Mathf.Rad2Deg;
        }
        public static Vector3      SmoothDampAngle      (Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            return new Vector3(
                Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, smoothTime),
                Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, smoothTime),
                Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, smoothTime));
        }


        ////////////////////////// ROUND ////////////////////////////////
        public static float        Round                (float value, int amount)
        {
            return Mathf.Round(value / amount) * amount;
        }
        public static float        RoundDown            (float value, int amount)
        {
            return Mathf.Floor(value / amount) * amount;
        }
        public static float        RoundUp              (float value, int amount)
        {
            return Mathf.Ceil(value / amount) * amount;
        }
        public static float        Round                (float value, float amount)
        {
            return Mathf.Round(value / amount) * amount;
        }
        public static float        RoundDown            (float value, float amount)
        {
            return Mathf.Floor(value / amount) * amount;
        }
        public static float        RoundUp              (float value, float amount)
        {
            return Mathf.Ceil(value / amount) * amount;
        }

        public static Vector2      Round                (this Vector2 vector, int amount)
        {
            return new Vector2(Mathf.Round(vector.x / amount) * amount, Mathf.Round(vector.y / amount) * amount);
        }
        public static Vector2      RoundDown            (this Vector2 vector, int amount)
        {
            return new Vector2(Mathf.Floor(vector.x / amount) * amount, Mathf.Floor(vector.y / amount) * amount);
        }
        public static Vector2      RoundUp              (this Vector2 vector, int amount)
        {
            return new Vector2(Mathf.Ceil(vector.x / amount) * amount, Mathf.Ceil(vector.y / amount) * amount);
        }
        public static Vector2      RoundF               (this Vector2 vector, float amount)
        {
            return new Vector2(Mathf.Round(vector.x / amount) * amount, Mathf.Round(vector.y / amount) * amount);
        }

        public static Vector3      Round                (this Vector3 vector, int amount)
        {
            return new Vector3(Mathf.Round(vector.x / amount) * amount, Mathf.Round(vector.y / amount) * amount, Mathf.Round(vector.z / amount) * amount);
        }
        public static Vector3      RoundDown            (this Vector3 vector, int amount)
        {
            return new Vector3(Mathf.Floor(vector.x / amount) * amount, Mathf.Floor(vector.y / amount) * amount, Mathf.Round(vector.z / amount) * amount);
        }
        public static Vector3      RoundUp              (this Vector3 vector, int amount)
        {
            return new Vector3(Mathf.Ceil(vector.x / amount) * amount, Mathf.Ceil(vector.y / amount) * amount, Mathf.Round(vector.z / amount) * amount);
        }
        public static Vector3      RoundF               (this Vector3 vector, float amount)
        {
            return new Vector3(Mathf.Round(vector.x / amount) * amount, Mathf.Round(vector.y / amount) * amount, Mathf.Round(vector.z / amount) * amount);
        }

        public static void         RoundTo              (this Transform transform, float amount)
        {
            transform.localPosition = RoundF(transform.localPosition, amount);
        }


        ////////////////////////// RECT ////////////////////////////////
        public static Vector2      GetPoint             (this Rect rect, float x, float y)
        {
            return rect.position + new Vector2(rect.size.x * x, rect.size.y * y);
        }
        public static Vector2      GetPoint             (this Rect rect, Vector2 vector)
        {
            return rect.position + new Vector2(rect.size.x * vector.x, rect.size.y * vector.y);
        }
        public static Vector2      GetInversePoint      (this Rect rect, Vector2 point)
        {
            //return new Vector2(
            //    Mathf.InverseLerp(rect.xMin, rect.xMax, point.x),
            //    Mathf.InverseLerp(rect.yMin, rect.yMax, point.y));
            return new Vector2(
                (point.x - rect.xMin) / rect.width,
                (point.y - rect.yMin) / rect.height);
        }
        public static Vector2      GetRandomPoint       (this Rect rect)
        {
            return rect.position + new Vector2(rect.size.x * UnityEngine.Random.value, rect.size.y * UnityEngine.Random.value);
        }
        public static Vector2      Clamp                (this Rect rect, Vector2 vector)
        {
            vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax);
            vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax);
            return vector;
        }
        public static Vector2      Clamp                (this Rect rect, Vector3 vector)
        {
            vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax);
            vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax);
            return vector;
        }
        public static float        Area                 (this Rect rect)
        {
            return rect.width * rect.height;
        }
        public static Rect         Expand               (this Rect rect, float amount)
        {
            return new Rect(rect.position - Vector2.one * amount, rect.size + 2f * amount * Vector2.one);
        }
        public static Rect         ExpandToInclude      (this Rect rect, Vector2 point)
        {
            rect.xMin = Mathf.Min(rect.xMin, point.x);
            rect.xMax = Mathf.Max(rect.xMax, point.x);
            rect.yMin = Mathf.Min(rect.yMin, point.y);
            rect.yMax = Mathf.Max(rect.yMax, point.y);
            return rect;
        }
        public static Rect         Rect                 (this Transform transform)
        {
            return new Rect(transform.position, transform.localScale);
        }


        ////////////////////////// VECTOR2 ////////////////////////////////
        public static Vector2      Truncate             (this Vector2 vector, float max)
        {
            return vector.normalized * Mathf.Min(vector.magnitude, max);
        }
        public static Vector2      Clamp                (this Vector2 vector, float min, float max)
        {
            return vector.normalized * Mathf.Clamp(vector.magnitude, min, max);
        }
        public static Vector2      ClampComponents      (this Vector2 vector, float min, float max)
        {
            return new Vector2(Mathf.Clamp(vector.x, min, max), Mathf.Clamp(vector.y, min, max));
        }
        public static Vector2      Quantize             (this Vector2 vector, int angleQuantization, int magnitudeQuantization)
        {
            // quantizes a vector inside the unit circle
            float angle = Mathf.Atan2(vector.x, vector.y);
            float magnitude = vector.magnitude;
            {
                angle /= (2f * Mathf.PI);// + 0.5f / angleQuantization;
                //angle = angle - Mathf.Floor(angle);
                angle = Mathf.Round(angle * angleQuantization) / angleQuantization;
                angle *= (2f * Mathf.PI);
                angle = Mathf.PI / 2f - angle;
            }
            {
                // magnitude is already 0-1
                magnitude = Mathf.Clamp01(Mathf.Round(magnitude * magnitudeQuantization) / magnitudeQuantization);
            }
            return new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        }
        public static Vector2      Rotate               (this Vector2 vector, float angleDegrees)
        {
            float sin = Mathf.Sin(angleDegrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angleDegrees * Mathf.Deg2Rad);

            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }
        public static Vector2      Rotate90             (this Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }
        public static Vector2      RotateNeg90          (this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }
        public static float        Lerp                 (this Vector2 vector, float t)
        {
            return Mathf.Lerp(vector.x, vector.y, t);
        }
        public static float        GetAngleRad          (this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }
        public static float        GetAngleDeg          (this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }
        public static Vector3      To3                  (this Vector2 vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }
        public static Vector3Int   To3                  (this Vector2Int vector)
        {
            return new Vector3Int(vector.x, 0, vector.y);
        }
        public static float        Min                  (this Vector2 vector)
        {
            Debug.Assert(vector.x <= vector.y);
            return vector.x;
        }
        public static float        Max                  (this Vector2 vector)
        {
            Debug.Assert(vector.x <= vector.y);
            return vector.y;
        }
        public static Vector2      ToV2                 (this Direction direction)
        {
            switch (direction)
            {
            case Direction.Up:    return Vector2.up;
            case Direction.Left:  return Vector2.left;
            case Direction.Down:  return Vector2.down;
            case Direction.Right: return Vector2.right;
            }
            Debug.LogWarning("Invalid direction");
            return Vector2.zero;
        }

        ////////////////////////// VECTOR3 ////////////////////////////////
        public static Vector2      To2                  (this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
        public static Vector2Int   To2                  (this Vector3Int vector)
        {
            return new Vector2Int(vector.x, vector.z);
        }


        ////////////////////////// RANDOM ////////////////////////////////
        public static float        GetRandom            (this Vector2 v)
        {
            Debug.Assert(v.x <= v.y, $"Vector used for randomization should have x<y, {v}");
            return UnityEngine.Random.Range(v.x, v.y);
        }
        public static int          GetRandom            (this Vector2Int v)
        {
            Debug.Assert(v.x <= v.y, $"Vector used for randomization should have x<y, {v}");
            return UnityEngine.Random.Range(v.x, v.y+1);
        }
        public static Vector2      RandomInside         (this Rect rect)
        {
            return rect.position + new Vector2(rect.size.x * UnityEngine.Random.value, rect.size.y * UnityEngine.Random.value);
        }
        public static Quaternion   RandomRotZ           ()
        {
            return Quaternion.Euler(0, 0, UnityEngine.Random.value * 360f);
        }
        public static int          WheelOfFortune       (float[] a, float randValue = -1f)
        {
            //returns a UnityEngine.Random.index with probability proportional to a
            float sum = a.Sum();

            if (sum == 0f)
            { // equiprobable
                for (int j = 0; j < a.Length; j++) a[j] = 1;
                sum = a.Length;
            }

            float x = 0f;
            float r;
            if (randValue == -1f)
            {
                r = UnityEngine.Random.Range(0f, sum);
            }
            else
            {
                r = randValue * sum;
            }

            for (int i = 0; i < a.Length; i++)
            {
                x += a[i];
                if (r <= x)
                {
                    return i;
                }
            }
            Debug.LogError("wheel of fortune didn't finish");
            return -1;
        }
        public static float        Normal               ()
        {
            // PURPOSE: Normal distribution with avg=0 and std=1
            float result = 0f;
            for (int i = 0; i < 12; i++) result += UnityEngine.Random.value;
            return result - 6f;
        }
        public static float        Normal               (float mu, float sigma)
        {
            return mu + Normal() * sigma;
        }
        public static float        NormalFromTo         (float a, float b)
        {
            float cutoff = 3.3f;
            float n01 = Mathf.Clamp01(Normal() / (2f * cutoff) + 0.5f); // BUG: it was + 1f but it should have been +0.5f
            return a + n01 * (b - a);
        }
        public static float        NormalSkewed         (float min, float max, float avg)
        {
            // PURPOSE: Returns a random value sampled from a distribution that is in the range [min,max] and has average avg (similar to a skewed distribution)
            // TODO: this has to be fixed as it's not continuous and the expected value is not the avg
            float cutoff = 3.3f;
            float n01 = Mathf.Clamp01(Normal() / (2f * cutoff) + 0.5f);
            if (n01 < 0.5f) return Mathf.Lerp(min, avg, n01*2f);
            else return Mathf.Lerp(avg, max, (n01-0.5f)*2f);
        }
        public static float        SampleRandomDistribution(float average, float deviation)
        {
            return Mathf.Max(0f, UnityEngine.Random.Range(average - deviation, average + deviation));
        }
        /* public static void TestDistributions()
        {
            const int numSamples = 1000;
            string s = "skewed:\n";
            for (int i = 0; i < numSamples; ++i)
            {
                s += Utility.NormalSkewed(17, 50, 45) + "\n";
            }
            Debug.Log(s);
            
            s = "fromto:\n";
            for (int i = 0; i < numSamples; ++i)
            {
                s += Utility.NormalFromTo(2, 4) + "\n";
            }
            Debug.Log(s);
            
            s = "normal:\n";
            for (int i = 0; i < numSamples; ++i)
            {
                s += Utility.Normal() + "\n";
            }
            Debug.Log(s);
            
            s = "normal2:\n";
            for (int i = 0; i < numSamples; ++i)
            {
                s += Utility.Normal(6.5f, 1.5f) + "\n";
            }
            Debug.Log(s);
        } */


        ////////////////////////// SEQUENCES / COLLECTIONS ////////////////////////////////
        public static T            Last<T>              (this IEnumerable<T> enumerable, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            return enumerable.ElementAt(enumerable.Count() - 1);
        }
        public static T            SafeGet<T>           (this IEnumerable<T> enumerable, int index, T defaultT = default)
        {
            if (enumerable == null || 0 > index || index >= enumerable.Count()) return defaultT;
            return enumerable.ElementAt(index);
        }
        public static T            Get<T>               (this IEnumerable<T> enumerable, int index, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            return enumerable.ElementAt(index.Mod(enumerable.Count()));
        }
        public static T            GetClamp<T>          (this IEnumerable<T> enumerable, int index, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            return enumerable.ElementAt(Mathf.Clamp(index, 0, enumerable.Count()-1));
        }
        public static T            GetRandom<T>         (this IEnumerable<T> enumerable, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            int index = UnityEngine.Random.Range(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
        public static T            Find<T>              (this IEnumerable<T> enumerable, Predicate<T> predicate, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            foreach(T elem in enumerable)
            {
                if (predicate(elem)) return elem;
            }
            return defaultT;
        }
        public static int          FindIndex<T>         (this IEnumerable<T> enumerable, T element)
        {
            if (enumerable == null || enumerable.Count() == 0) return -1;
            for (int i = 0; i < enumerable.Count(); i++)
            {
                if (enumerable.ElementAt(i).Equals(element)) return i;
            }
            return -1;
        }
        public static int          FindIndex<T>         (this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            if (enumerable == null || enumerable.Count() == 0) return -1;
            for (int i = 0; i < enumerable.Count(); i++)
            {
                if (predicate(enumerable.ElementAt(i))) return i;
            }
            return -1;
        }
        public static T            FindBest<T>          (this IEnumerable<T> enumerable, Func<T, float> costFunction, Predicate<T> filter = null, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            float minCost = float.MaxValue;
            T bestElement = defaultT;
            for (int i = 0; i < enumerable.Count(); i++)
            {
                T element = enumerable.ElementAt(i);
                if (filter == null || filter(element)) // pass filter
                {
                    float elementCost = costFunction(element);
                    if (elementCost < minCost)
                    {
                        minCost = elementCost;
                        bestElement = element;
                    }
                }
            }
            return bestElement;
        }
        public static T[]          MyAppend<T>          (this T[] array, T newElement)
        {
            List<T> tmpList = array.ToList();
            tmpList.Add(newElement);
            array = tmpList.ToArray();
            return array;
        }
        public static T[]          MyAppend<T>          (this T[] array, IEnumerable<T> newElements)
        {
            List<T> tmpList = array.ToList();
            tmpList.AddRange(newElements);
            T[] result = tmpList.ToArray();// this doesn't assign
            return result;
        }
        public static bool         IsNullOrEmpty<T>     (this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.Count() == 0;
        }
        public static bool         Has                  (this Array array, int index)
        {
            return array != null && 0 <= index && index < array.Length;
        }
        public static bool         IsAnyNull            (params object[] o)
        {
            for (int i = 0; i < o.Length; i++)
            {
                if (o == null) return true;
            }
            return false;
        }
        public static int          IndexOfMaxElement    (this int[] array)
        {
            int maxValue = array.Max();
            int maxIndex = Array.IndexOf(array, maxValue);
            return maxIndex;
        }
        public static int          IndexOfMaxElement    (this float[] array)
        {
            float maxValue = array.Max();
            int maxIndex = Array.IndexOf(array, maxValue);
            return maxIndex;
        }
        public static int          IndexOfMaxElementNotAllEqual(this float[] array)
        {
            float maxValue = array.Max();
            bool oneDifferent = false;
            foreach(float f in array)
            {
                if(f != maxValue)
                {
                    oneDifferent = true;
                    break;
                }
            }
            if(oneDifferent == false)
            {
                return -1; // the array is exactly the same for all
            }
            int maxIndex = Array.IndexOf(array, maxValue);
            return maxIndex;
        }
        public static int[]        Generate             (int num, int start, int increment = 1)
        {
            int[] result = new int[num];
            for (int i = 0; i < num; i++)
            {
                result[i] = start + i * increment;
            }
            return result;
        }
        public static float[]      Generate             (int num, float start, float increment = 1f)
        {
            float[] result = new float[num];
            for (int i = 0; i < num; i++)
            {
                result[i] = start + i * increment;
            }
            return result;
        }
        public static int[]        Repeat               (int num, int value)
        {
            return Generate(num, value, 0);
        }
        public static float[]      Repeat               (int num, float value)
        {
            return Generate(num, value, 0);
        }
        public static int[]        Sequence             (int from, int to /*exclusive*/)
        {
            return Generate(to - from, from, 1);
        }
        public static List<T>      Rotate<T>            (this List<T> list, int offset)
        {
            return list.Skip(offset).Concat(list.Take(offset)).ToList();
        }
        public static int          FindClosestValue     (this float[] array, float value)
        {
            int bestIndex = -1;
            float minDist = float.MaxValue;
            for (int i = 0; i < array.Length; i++)
            {
                float abs = Mathf.Abs(array[i] - value);
                if (abs < minDist)
                {
                    minDist = abs;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
        public static int          FindClosestValue     (this int[] array, int value)
        {
            int bestIndex = -1;
            int minDist = int.MaxValue;
            for (int i = 0; i < array.Length; i++)
            {
                int abs = Mathf.Abs(array[i] - value);
                if (abs < minDist)
                {
                    minDist = abs;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
        public static void         RemoveWhere<K,V>     (this Dictionary<K,V> dictionary, System.Predicate<KeyValuePair<K,V>> pred)
        {
            var toRemove = dictionary.Where(pair => pred(pair)).Select(pair => pair.Key).ToList();

            foreach (var key in toRemove)
            {
                dictionary.Remove(key);
            }
        }


        ////////////////////////// GEOMETRY ////////////////////////////////
        public static bool         IsPointInsideTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            // taken from https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
            static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
            {
                return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
            }
            //float d1, d2, d3;
            //bool has_neg, has_pos;

            float d1 = sign(p, a, b);
            float d2 = sign(p, b, c);
            float d3 = sign(p, c, a);

            bool has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
        public static Vector2      GetTriangleCenter    (Vector2 a, Vector2 b, Vector2 c)
        {
            // TODO: add special case if they are very close (at least 2 points)
            Vector2 pab = (a + b) / 2f;
            Vector2 pac = (a + c) / 2f;
            if(FindSegmentIntersection(pab, c, pac, b, out Vector2 ans))
            {
                return ans;
            }
            // TODO: This can happen sometimes (most likely because of numeric issues)
            Debug.LogWarning("Failed to compute triangle center");
            return Vector2.zero;
        }
        public static float        GetTriangleArea      (Vector2 a, Vector2 b, Vector2 c)
        {
            return Mathf.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f;
        }

        public static Vector2      GetPolygonCenter     (Vector2[] pts)
        {
            Vector2 off = pts[0];
            float twicearea = 0;
            float x = 0;
            float y = 0;
            Vector2 p1, p2;
            float f;
            for (int i = 0, j = pts.Length - 1; i < pts.Length; j = i++)
            {
                p1 = pts[i];
                p2 = pts[j];
                f = (p1.x - off.x) * (p2.y - off.y) - (p2.x - off.x) * (p1.y - off.y);
                twicearea += f;
                x += (p1.x + p2.x - 2 * off.x) * f;
                y += (p1.y + p2.y - 2 * off.y) * f;
            }

            f = twicearea * 3;

            return new Vector2(x / f + off.x, y / f + off.y);
        }

        public static Vector2      Intersect            (Ray2D r0, Ray2D r1)
        {
            if (FindSegmentIntersection(r0.origin, r0.GetPoint(10000f), r1.origin, r1.GetPoint(10000f), out Vector2 output))
            {
                return output;
            }
            Debug.Assert(false, "The rays don't intersect");
            return Vector2.zero;
        }
        public static Vector2      FindIntersectionPointWithOffset(Vector2 p0, Vector2 p1, Vector2 p2, float offset)
        {
            Vector2 d0 = (p1 - p0).normalized;
            Vector2 d1 = (p2 - p1).normalized;
            const float tolerance = 1f - 0.01f;
            float dot = Vector2.Dot(d0, d1);
            if (dot >= tolerance) // collinear
            {
                return p1 + d0.Rotate90() * offset;
            }
            else if (dot <= -tolerance) // opposite
            {
                return p1 - d0 * offset;
            }
            else
            {
                Vector2 n = FindNormalVectorFast(d0, d1);
                float s = Vector2.SignedAngle(d0, d1) * Mathf.Deg2Rad;
                float w = offset / Mathf.Cos(s / 2f) * Mathf.Sign(-s);
                return p1 + n.normalized * w;
            }
        }
        public static Vector2      FindNormalVector     (Vector2 d, Vector2 r)
        {
            // NOTES: assumes that d, r, are normalized
            // NOTES: d is coming TOWARDS the center, r is going AWAY from it (like raycasting)
            // thanks wolframalpha
            float dot = Vector2.Dot(d, r);
            const float tolerance = 0.001f;
            if (dot > 1f - tolerance) // collinear
            {
                return d.Rotate90();
            }
            else if (dot < -1f + tolerance) // opposite
            {
                return -d;
            }

            float den = Mathf.Sqrt(2f * (d.x * d.x - d.x * r.x + d.y * d.y - d.y * r.y));
            Debug.Assert(Mathf.Abs(den) > Mathf.Epsilon, "FindNormalVector divide by 0");
            float deninv = 1f / den;
            return new Vector2((d.x - r.x) * deninv, (d.y - r.y) * deninv);
        }
        public static Vector2      FindNormalVectorFast (Vector2 d, Vector2 r)
        {
            // NOTES: assumes that d, r, are normalized
            // ATTENTION: Assumes that the vectors are not collinear
            // NOTES: d is coming TOWARDS the center, r is going AWAY from it (like raycasting)
            // thanks wolframalpha
            float den = Mathf.Sqrt(2f * (d.x * d.x - d.x * r.x + d.y * d.y - d.y * r.y));
            float deninv = 1f / den;
            return new Vector2((d.x - r.x) * deninv, (d.y - r.y) * deninv);
        }

        public static Vector2      FindSegmentsIntersectionExpected   (Segment s1, Segment s2)
        {
            return FindSegmentsIntersectionExpected(s1.a, s1.b, s2.a, s2.b);
        }
        public static Vector2      FindSegmentsIntersectionExpected   (Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            bool success = FindSegmentIntersection(a, b, c, d, out Vector2 ans);
            Debug.Assert(success, "Could not find an intersection even though it was expected");
            return ans;
        }
        public static bool         FindSegmentIntersection            (Segment s1, Segment s2, out Vector2 intersection)
        {
            return FindSegmentIntersection(s1.a, s1.b, s2.a, s2.b, out intersection);
        }
        public static bool         FindSegmentIntersection            (Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersection)
        {
            // returns true if there is an intersection between the 2d segments (a,b) and (c,d)
            // from https://github.com/setchi/Unity-LineSegmentsIntersection/blob/master/Assets/LineSegmentIntersection/Scripts/Math2d.cs
            intersection = Vector2.zero;

            float e = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);

            if (e == 0.0f)
            {
                return false;
            }

            float u = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / e;
            float v = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / e;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
            {
                return false;
            }

            intersection.x = a.x + u * (b.x - a.x);
            intersection.y = a.y + u * (b.y - a.y);

            return true;
        }
        public static bool         SegmentsIntersect                  (Segment s1, Segment s2)
        {
            return SegmentsIntersect(s1.a, s1.b, s2.a, s2.b);
        }
        public static bool         SegmentsIntersect                  (Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            // from https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
            static bool ccw(Vector2 A, Vector2 B, Vector2 C)
            {
                return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
            }
            return ccw(a, c, d) != ccw(b, c, d) && ccw(a, b, c) != ccw(a, b, d);
        }
        public static float        FindSegmentDistanceToPoint         (Segment s, Vector2 p)
        {
            return FindSegmentDistanceToPoint(s.a, s.b, p);
        }
        public static float        FindSegmentDistanceToPoint         (Vector2 a, Vector2 b, Vector2 p)
        {
            // NOTES: a, b are the segment endpoints
            return Mathf.Sqrt(FindSegmentDistanceToPointSquared(a, b, p));
        }
        public static float        FindSegmentDistanceToPointSquared  (Segment s, Vector2 p)
        {
            return FindSegmentDistanceToPointSquared(s.a, s.b, p);
        }
        public static float        FindSegmentDistanceToPointSquared  (Vector2 a, Vector2 b, Vector2 p)
        {
            float l2 = (a - b).sqrMagnitude;
            if (Mathf.Abs(l2) < 0.0001f) return (p - a).sqrMagnitude;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, b - a) / l2);
            Vector2 projection = a + t * (b - a);
            return (p - projection).sqrMagnitude;
        }
        public static float        FindSegmentDistanceToSegment       (Segment s1, Segment s2)
        {
            return FindSegmentDistanceToSegment(s1.a, s1.b, s2.a, s2.b);
        }
        public static float        FindSegmentDistanceToSegment       (Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            return Mathf.Sqrt(FindSegmentDistanceToSegmentSquared(a, b, c, d));
        }
        public static float        FindSegmentDistanceToSegmentSquared(Segment s1, Segment s2)
        {
            return FindSegmentDistanceToSegmentSquared(s1.a, s1.b, s2.a, s2.b);
        }
        public static float        FindSegmentDistanceToSegmentSquared(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            if (SegmentsIntersect(a, b, c, d)) return 0f;
            return Mathf.Min
            (
                FindSegmentDistanceToPointSquared(a, b, c),
                FindSegmentDistanceToPointSquared(a, b, d),
                FindSegmentDistanceToPointSquared(c, d, a),
                FindSegmentDistanceToPointSquared(c, d, b)
            );
        }
        public static bool         SegmentsAreCloserThan              (Segment s1, Segment s2, float dist)
        {
            if (SegmentsIntersect(s1, s2)) return true;
            float dist2 = dist * dist;
            if (FindSegmentDistanceToPointSquared(s1, s2.a) <= dist2) return true;
            if (FindSegmentDistanceToPointSquared(s1, s2.b) <= dist2) return true;
            if (FindSegmentDistanceToPointSquared(s2, s1.a) <= dist2) return true;
            if (FindSegmentDistanceToPointSquared(s2, s1.b) <= dist2) return true;
            return false;
        }
        //public static Vector2      FindIntersectionPointWithOffset_Old(Vector2 p0, Vector2 p1, Vector2 p2, float offset)
        //{
        //    Vector2 d0 = (p1 - p0).normalized;
        //    Vector2 d1 = (p2 - p1).normalized;
        //    Vector2 t0 = d0.RotateNeg90();
        //    Vector2 t1 = d1.RotateNeg90();
        //    float w = offset;
        //    Ray2D r0 = new Ray2D(p0 + t0 * w, d0);
        //    Ray2D r1 = new Ray2D(p2 + t1 * w, -d1);
        //    return Intersect(r0, r1);
        //}


        ////////////////////////// ENUM ////////////////////////////////
        public static int          EnumLength<Enum>     ()
        {
            return System.Enum.GetValues(typeof(Enum)).Length;
        }
        public static string[]     EnumNames<Enum>      ()
        {
            return System.Enum.GetNames(typeof(Enum));
        }


        ////////////////////////// TRANSFORM ////////////////////////////////
        public static float        Heading              (this Transform transform)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) return 0f;
            float heading = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);
            return heading;
        }
        public static void         MakeNChildren        (this Transform transform, int N)
        {
            Debug.Assert(N >= 0 && transform != null, "MakeChildren has invalid parameters");
            Debug.Assert(N == 0 || transform.childCount > 0, "MakeChildren transform doesn't have a child to instantiate");
            if (transform.childCount == N) return;
            if (transform.childCount > N)
            {
                for (int i = transform.childCount - 1; i >= N; --i)
                {
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
            else if (transform.childCount < N)
            {
                int toSpawn = N - transform.childCount;
                for (int i = 0; i < toSpawn; i++)
                {
                    GameObject.Instantiate(transform.GetChild(0).gameObject, transform);
                }
            }
        }
        public static void         DestroyAllChildren   (this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
        public static void         SortChildren         (this Transform transform, bool recursive = false)
        {
            // PURPOSE: Sorts the children of the given gameobject by name
            Transform[] ts = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                ts[i] = transform.GetChild(i);
                if (recursive)
                {
                    SortChildren(ts[i], recursive);
                }
            }
            System.Array.Sort(ts, (t1, t2) => (t1.name.CompareTo(t2.name))); // transform.name == transform.gameobject.name

            for (int i = 0; i < ts.Length; i++)
            {
                ts[i].SetSiblingIndex(i);
            }
        }
        public static void         PrintChildren         (this Transform transform, StringBuilder sb, bool recursive = true, int depth = 0)
        {
            // PURPOSE: prints the children of the given gameobject by name
            sb.Append(new string('\t', depth));
            sb.Append(transform.gameObject.name);
            sb.Append('\n');

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (recursive)
                {
                    PrintChildren(child, sb, recursive, depth+1);
                }
            }
        }

        ////////////////////////// STRING ////////////////////////////////
        public static string       RemoveSpaceAndTabs   (this string s)
        {
            return s.Replace(" ", string.Empty).Replace("\t", string.Empty);
        }
        public static string       RemoveCommaAndTabs   (this string s)
        {
            return s.Replace(",", string.Empty).Replace("\t", string.Empty);
        }
        public static string       RemoveAll            (this string s, string characters)
        {
            foreach(char c in characters)
            {
                s = s.Replace(c.ToString(), string.Empty);
            }
            return s;
        }
        public static string       ToReadableTime       (float timeInSeconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(timeInSeconds);
            //if(ts.Days > 0)
            //{
            //    return string.Format("{0} days {1:D1}:{2:D2}:{3:D2}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            //}
            if (ts.Hours > 0)
            {
                return string.Format("{0:D1}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
            }
            else
            {
                return string.Format("{0:D1}:{1:D2}", ts.Minutes, ts.Seconds);
            }
        }
        public static string       FormatTimestamp      (this DateTime value)
        {
            const string timestampFormat = "yyyy-MM-dd_HH:mm:ss";
            return value.ToString(timestampFormat);
        }
        public static string       GetTimestampNow      ()
        {
            return FormatTimestamp(DateTime.Now);
        }
        public static string[]     SplitBy              (this string s, params char[] chars)
        {
            return s.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string       GetTimestampNowPrecise()
        {
            const string timestampFormat = "yyyy/MM/dd HH:mm:ss.fff";
            return DateTime.Now.ToString(timestampFormat);
        }

        public static string       AddSpacesBeforeCapitalLetters(string text, bool preserveAcronyms = true)
        {
            // from: https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            StringBuilder newText = new(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string[][]   SplitCsv             (TextAsset textAsset)
        {
            return SplitCsv(textAsset.text, textAsset.name);
        }
        public static string[][]   SplitCsv             (string content, string filename = "")
        {
            string[] lines = content.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[][] cells = new string[lines.Length][];
            int cols = 0;
            for (int i = 0; i < lines.Length; i++)
            {

                cells[i] = lines[i].Split(new char[] { '\t' }, StringSplitOptions.None);
                if (i == 0) cols = cells[i].Length;
                else { Debug.Assert(cells[i].Length == cols, $"The provided CSV {filename} file to split in not square (line {i})"); }
            }
            return cells;
        }
        public static string[][]   Transpose            (this string[][] array)
        {
            int rows = array.Length;
            int cols = array[0].Length;
            string[][] ans = new string[cols][];
            for (int c = 0; c < cols; c++)
            {
                ans[c] = new string[rows];
                for (int r = 0; r < rows; r++)
                {
                    ans[c][r] = array[r][c];
                }
            }
            return ans;
        }

        public static string       IntToAlphaNumeric    (int number, int N)
        {
            // [A-Z0-9]{N}
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int L = chars.Length;

            string ans = "";
            for (int i = 0; i < N; i++)
            {
                int index = number.Mod(L);
                number /= L;
                ans += chars[index];
            }
            return ans;
        }
        public static bool         Like                 (this string input, string pattern)
        {
            return Regex.IsMatch(input, $"^{pattern}$");
            //return Regex.IsMatch(input, pattern);
        }
        public static bool         IsRegexMatch         (this string input, string pattern, out string[] tokens)
        {
            Regex regex = new(pattern);
            if(regex.IsMatch(input))
            {
                GroupCollection gc = regex.Match(input).Groups;
                tokens = new string[gc.Count];
                for (int i = 0; i < gc.Count; i++)
                {
                    tokens[i] = gc[i].ToString();
                }
                return true;
            }
            tokens = null;
            return false;
        }

        ////////////////////////// ROUTINE / INVOKE ////////////////////////////////
        public static IEnumerator  SimpleRoutine        (Action<float> function, float duration, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);
            float speed = 1f / duration;
            float percent = 0f;
            function(0f);
            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                function(percent);
                yield return null;
            }
            function(1f);
        }
        public static void         Invoke               (this MonoBehaviour mb, Action action, float delay)
        {
            Debug.Assert(mb != null, "Trying to call Invoke on a null monobehavior");
            Debug.Assert(delay >= 0f, "Delay to invoke is <0");
            if (delay <= 0f) action();
            else
            {
                mb.StartCoroutine(PrivateInvokeRoutine(action, delay));
            }
        }
        public static void         Invoke               (Action action, float delay)
        {
            Debug.LogWarning("Invoke without mb is disabled");
            //Invoke(CoroutineManager.Instance, action, delay);
        }
        private static IEnumerator PrivateInvokeRoutine (Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
        /*public static IEnumerator Invoke(Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }*/


        ////////////////////////// GAMEOBJECT / COMPONENT ////////////////////////////////
        public static void         Bind<T>              (this Component mb, out T result, bool includeChildren = true, bool canBeNull = false) where T : Component
        {
            if (includeChildren)
            {
                result = mb.GetComponentInChildren<T>();
            }
            else
            {
                result = mb.GetComponent<T>();
            }

            if (!canBeNull)
            {
                Debug.Assert(result != null, $"{mb.gameObject.name} expects a {typeof(T)}");
            }
        }
        public static T[]          BindChildren<T>      (GameObject o) where T : Component
        {
            List<T> result = new();
            for (int i = 0; i < o.transform.childCount; i++)
            {
                Transform t = o.transform.GetChild(i);
                T c = t.GetComponent<T>();

                if (c != null)
                {
                    result.Add(c);
                }
            }
            return result.ToArray();
        }
        public static void         TryBind<T>           (this MonoBehaviour mb, out T result, bool includeChildren = true) where T : Component
        {
            mb.Bind(out result, includeChildren, canBeNull: true);
        }
        public static void         BindHere<T>          (this MonoBehaviour mb, out T result) where T : Component
        {
            mb.Bind(out result, includeChildren: false);
        }
        public static void         BindAll<T>           (this Component mb, out T[] results, bool includeChildren = true, bool canBeNull = false) where T : Component
        {
            if (includeChildren)
            {
                results = mb.GetComponentsInChildren<T>();
            }
            else
            {
                results = mb.GetComponents<T>();
            }

            if (!canBeNull)
            {
                Debug.Assert(results != null, $"{mb.gameObject.name} expects an array of {typeof(T)}");
            }
        }
        public static GameObject   FindNameAll          (string name)
        {
            foreach(GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.name == name) return go;
            }
            return null;
        }
        public static bool         Matches              (this LayerMask layerMask, GameObject gameObject)
        {
            return (layerMask.value & (1 << gameObject.layer)) != 0;

        }
        public static void         CmToImperial         (this float cm, out int feet, out int inches)
        {
            const float feetToCm = 30.48f;
            const float inchToCm = 2.54f;

            feet = Mathf.FloorToInt(cm / feetToCm);
            inches = Mathf.RoundToInt((cm - feet * feetToCm) / inchToCm);
        }
        public static bool         Implies              (this bool A, bool B)
        {
            return !A || B;
        }

        public static string       Serialize<T>         (T obj)
        {
            string result = string.Empty;
            foreach (var field in typeof(T).GetFields())
            {
                result += field.GetValue(obj).ToString() + ",\t";
            }
            return result;
        }
        public static T            Deserialize<T>       (string s) where T : new() // must it be a class to work?
        {
            s = RemoveSpaceAndTabs(s);
            string[] values = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            T result = new();
            //T result = default(T);
            int v = 0;
            foreach (var fi in typeof(T).GetFields())
            {
                var val = Convert.ChangeType(values[v++], fi.FieldType); // convert it
                fi.SetValue(result, val);
            }
            return result;
        }
        public static void         SaveTextureAsPNG     (Texture2D texture, string fullpath)
        {
            byte[] _bytes = texture.EncodeToPNG();
            if (!fullpath.EndsWith(".png")) fullpath += ".png";
            System.IO.File.WriteAllBytes(fullpath, _bytes);
            Debug.Log($"{_bytes.Length / 1024} Kb was saved as: {fullpath}");
        }
        public static void         SetAlpha             (this Image image, float alpha)
        {
            if (image == null) return;
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
        public static void         SetAlpha             (this TextMeshProUGUI text, float alpha)
        {
            if (text == null) return;
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }
        public static void         SetAlpha             (this SpriteRenderer sr, float alpha)
        {
            if (sr == null) return;
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
        public static Color32      GetPixel32           (this Texture2D texture, int x, int y)
        {
            return texture.GetPixels32()[x + y * texture.width];
        }
        public static void         SetDirty             (this UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

        // PURPOSE: Returns the gameobject with name, looking also through disabled objects
        //public static void BindSerie(this Component mb, out object[] result)
        //{
        //
        //}
        //public static void AssertNotNull<T>(this MonoBehaviour mb, params T[] components) where T : Component
        //{
        //    foreach(T component in components)
        //    {
        //        Debug.Assert(component != null, $"{nameof(component)} is null on {mb.gameObject.name}");
        //    }
        //}

        // OTHER


        ////////////////////////// SERIALIZATION ////////////////////////////////


        ////////////////////////// IMAGES ////////////////////////////////

        ////////////////////////// MESH ////////////////////////////////
        public static Mesh         MakeMesh             (IEnumerable<Vector3> vertices, IEnumerable<int> indices, int submesh = 0)
        {
            // simple methods: SetColors, SetIndices, SetNormals, SetTangents, SetTriangles, SetUVs, SetVertices, SetBoneWeights

            //16-bit index by default, also when using int[] as vertices => 65k verts
            Mesh mesh = new();
            mesh.SetVertices(vertices.ToList());
            //mesh.SetIndices(indices.ToList(), MeshTopology.Triangles, submesh);
            mesh.SetTriangles(indices.ToList(), submesh);
            //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16; // 65k verts

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.Optimize();
            mesh.OptimizeIndexBuffers();
            mesh.OptimizeReorderVertexBuffer();

            return mesh;
        }

        ////////////////////////// COLOR ////////////////////////////////
        public static string       ToHex                (this Color color)
        {
            Color32 c = color;
            uint value = (uint)(c.r << 16) | (uint)(c.g << 8) | (uint)(c.b << 0);
            return $"#{value:X6}";
        }

        public static Color        Hex2Color            (uint hex)
        {
            byte r = (byte)((hex >> 16) & 0xff);
            byte g = (byte)((hex >>  8) & 0xff);
            byte b = (byte)((hex      ) & 0xff);
            return new Color32(r, g, b, 255);
        }
        public static Color        SmoothDampColor      (Color current, Color target, ref Color velocity, float smoothTime)
        {
            return new Color(
                Mathf.SmoothDamp(current.r, target.r, ref velocity.r, smoothTime),
                Mathf.SmoothDamp(current.g, target.g, ref velocity.g, smoothTime),
                Mathf.SmoothDamp(current.b, target.b, ref velocity.b, smoothTime),
                Mathf.SmoothDamp(current.a, target.a, ref velocity.a, smoothTime));
        }

        public static Color        Uint2Color           (uint value)
        {
            byte r = (byte)((value >> 24) & 0xff);
            byte g = (byte)((value >> 16) & 0xff);
            byte b = (byte)((value >>  8) & 0xff);
            byte a = (byte)((value      ) & 0xff);
            return new Color32(r, g, b, a);
        }
        public static uint         Color2Uint           (Color color)
        {
            Color32 c = color;
            uint value = (uint)(c.r << 24) | (uint)(c.g << 16) | (uint)(c.b << 8) | (uint)(c.a);
            return value;
        }
        public static Color        SetAlpha             (this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        ////////////////////////// TIME ////////////////////////////////
        public static void         LogWithTime          (string message)
        {
            Debug.Log($"{message} (gametime = {Time.time}, realtime  = {Time.realtimeSinceStartup})");
        }
        public static int          ExecuteAndTimeMs     (Action action)
        {
            // PURPOSE: Runs and returns the number of milliseconds it took
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            return (int)watch.ElapsedMilliseconds;
        }
        public static int          ExecuteAndTimeUs     (Action action)
        {
            // PURPOSE: Runs and returns the number of microseconds it took
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            int ticks = (int)watch.ElapsedTicks;
            int ticksOverSeconds = (int)System.Diagnostics.Stopwatch.Frequency;
            int microseconds = (int)((float)ticks * 1000000 / ticksOverSeconds);
            return microseconds;
        }


        ////////////////////////// NOISE ////////////////////////////////
        public static float        PerlinNoise3         (float time)
        {
            return Mathf.PerlinNoise(time, 0f) + 0.5f * Mathf.PerlinNoise(2f * time, 1f) + 0.25f * Mathf.PerlinNoise(4f * time, 2f);
        }
        public static Vector2      GetWiggle2D          (float t)
        {
            return new Vector2(
                Mathf.PerlinNoise(t + 38.38f, 21.66f),
                Mathf.PerlinNoise(t + 59.17f, 44.66f)) * 2f - Vector2.one;
        }
        public static Vector3      GetWiggle            (float t)
        {
            return new Vector3(
                Mathf.PerlinNoise(t + 38.38f, 21.66f),
                Mathf.PerlinNoise(t + 59.17f, 44.66f),
                Mathf.PerlinNoise(t + 95.41f, 19.96f)) * 2f - Vector3.one;
        }


        ////////////////////////// POLYGONCOLLIDER ////////////////////////////////
        public static void         Symmetrize           (this PolygonCollider2D collider)
        {
            Vector2[] points = collider.points;
            if (points.Length < 3) return;

            // find the 2 points closest to the x=0
            int[] smallerIndex = new int[2] { -1, -1 };
            float[] smallerDelta = new float[2] { float.MaxValue, float.MaxValue };
            for (int i = 0; i < points.Length; i++)
            {
                float delta = Mathf.Abs(points[i].x);
                if (delta < smallerDelta[0])
                {
                    smallerIndex[1] = smallerIndex[0];
                    smallerDelta[1] = smallerDelta[0];
                    smallerIndex[0] = i;
                    smallerDelta[0] = delta;
                }
                else if (delta < smallerDelta[1])
                {
                    smallerIndex[1] = i;
                    smallerDelta[1] = delta;
                }
            }

            Debug.Assert(smallerIndex[0] != smallerIndex[1], "Smaller index should be different");

            points[smallerIndex[0]].x = 0f;
            points[smallerIndex[1]].x = 0f;
            int[] cut = new int[2];
            cut[0] = Mathf.Min(smallerIndex[0], smallerIndex[1]);
            cut[1] = Mathf.Max(smallerIndex[0], smallerIndex[1]);

            // rotate the array, remove the smaller part
            List<Vector2> newpoints = new();
            if (cut[1] - cut[0] - 1 > points.Length / 2 - 1)
            {
                for (int i = cut[0]; i <= cut[1]; ++i) newpoints.Add(points[i]);
            }
            else
            {
                for (int i = cut[1]; i < points.Length; i++) newpoints.Add(points[i]);
                for (int i = 0; i <= cut[0]; ++i) newpoints.Add(points[i]);
            }

            // duplicate the points on the other side
            int n = newpoints.Count;
            for (int i = n - 2; i >= 1; --i)
            {
                Vector2 np = newpoints[i];
                np.x *= -1f;
                newpoints.Add(np);
            }

            // apply
            collider.points = newpoints.ToArray();
        }
        public static void         Flip                 (this PolygonCollider2D collider)
        {
            Vector2[] points = collider.points;
            for (int i = 0; i < points.Length; i++)
            {
                points[i].x *= -1f;
            }
            collider.points = points;
        }

    }
}
