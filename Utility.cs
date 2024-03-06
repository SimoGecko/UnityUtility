// (c) Simone Guggiari 2020-2024

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.Events;
using System.IO;

////////// PURPOSE: Various Utility functions //////////

namespace sxg
{
    // Naming: Get / Find / Compute / 
    public static partial class Utility
    {
        static float eps = 1e-4f;
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
            return value > eps ? 1f : value < -eps ? -1f : 0f;
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
        public static float        Remap                (this float value, Range from, Range to)
        {
            return to.Min + (to.Max - to.Min) * ((value - from.Min) / (from.Max - from.Min));
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
            if (Mathf.Abs(value) > max)
                return Mathf.Sign(value) * max;
            return value;
        }
        public static float        Truncate             (this float value, float min, float max)
        {
            return Mathf.Sign(value) * Mathf.Clamp(value, min, max);
        }
        public static bool         IsClose              (this float value, float value2)
        {
            return Mathf.Abs(value-value2) <= eps;
        }
        public static float        SumProduct           (float[] a, float[] b)
        {
            Debug.Assert(a.Length == b.Length);
            float ans = 0f;
            for (int i = 0; i < a.Length; i++)
            {
                ans += a[i] * b[i];
            }
            return ans;
        }
        public static bool         Between              (this int value, int minInclusive, int maxInclusive)
        {
            return minInclusive <= value && value <= maxInclusive;
        }
        public static float        InverseLerpUnclamped (float a, float b, float value)
        {
            if (a == b)
                return 0f;
            return (value - a) / (b - a);
        }
        public static double       SumProduct           (double[] a, double[] b)
        {
            Debug.Assert(a.Length == b.Length);
            double ans = 0;
            for (int i = 0; i < a.Length; i++)
            {
                ans += a[i] * b[i];
            }
            return ans;
        }
        

        ////////////////////////// ANGLE ////////////////////////////////
        public static float        Canonicalize         (float angleDeg)
        {
            // returns the angle in the canonical range [-180, 180[
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
        public static Vector3      Canonicalize         (this Vector3 anglesDeg)
        {
            // returns the angle in the canonical range [-180, 180[
            return new Vector3(Canonicalize(anglesDeg.x), Canonicalize(anglesDeg.y), Canonicalize(anglesDeg.z));
        }
        public static float        ClampAngle           (float angle, float min, float max)
        {
            // from https://stackoverflow.com/questions/42246870/clamp-angle-to-arbitrary-range
            float n_min = Canonicalize(min-angle);
            float n_max = Canonicalize(max-angle);

            if (n_min <= 0f && n_max >= 0f)
            {
                return angle;
            }
            if (Mathf.Abs(n_min) < Mathf.Abs(n_max))
                return min;
            return max;
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
        public static int          Round                (int value, int amount)
        {
            return Mathf.RoundToInt(value / amount) * amount;
        }
        public static int          RoundDown            (int value, int amount)
        {
            return Mathf.FloorToInt(value / amount) * amount;
        }
        public static int          RoundUp              (int value, int amount)
        {
            return Mathf.CeilToInt(value / amount) * amount;
        }
        public static float        RoundDecimals        (this float value, int decimals)
        {
            return System.MathF.Round(value, decimals);
        }

        public static float        Clean                (float value, float amount, float eps)
        {
            float closest = Mathf.Round(value/amount) * amount;
            if (Mathf.Abs(closest - value) <= eps) return closest;
            return value;
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

        public static Vector3      Clean                (this Vector3 vector, float amount, float eps)
        {
            return new Vector3(Clean(vector.x, amount, eps), Clean(vector.y, amount, eps), Clean(vector.z, amount, eps));
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
        public static float        Volume               (this Bounds b)
        {
            return b.size.x * b.size.y * b.size.z;
        }


        ////////////////////////// VECTOR2 ////////////////////////////////
        public static Vector2      Truncate             (this Vector2 vector, float max)
        {
            Debug.Assert(max >= 0f);
            if (vector.sqrMagnitude > max * max)
                return vector.normalized * max;
            return vector;
        }
        public static Vector2      ClampMagnitude       (this Vector2 vector, float min, float max)
        {
            return vector.normalized * Mathf.Clamp(vector.magnitude, min, max);
        }
        public static Vector2      ClampComponents      (this Vector2 vector, float min, float max)
        {
            return new Vector2(
                Mathf.Clamp(vector.x, min, max),
                Mathf.Clamp(vector.y, min, max));
        }
        public static Vector2 ClampComponents(this Vector2 vector, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y));
        }
        public static Vector2      Quantize             (this Vector2 vector, int angleQuantization, int magnitudeQuantization)
        {
            // quantizes a vector inside the unit circle
            float angle = Mathf.Atan2(vector.x, vector.y);
            float magnitude = vector.magnitude;
            {
                angle /= (2f * Mathf.PI); // + 0.5f / angleQuantization;
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
            return Rotate(vector, sin, cos);
        }
        public static Vector2      Rotate               (this Vector2 vector, float sin, float cos)
        {
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
        public static bool         IsNan                (this Vector2 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.y);
        }
        public static bool         IsWithin             (this Vector2 vector, Vector2 other, float dist)
        {
            return (vector - other).sqrMagnitude <= dist * dist;
        }
        public static bool         IsZero               (this Vector2 vector)
        {
            return vector.sqrMagnitude <= eps * eps;
        }
        public static Vector2      Perpendicular        (this Vector2 vector)
        {
            return Vector2.Perpendicular(vector);
        }
        public static bool         IsParallel           (this Vector2 v1, Vector2 v2)
        {
            //return (v1.x / v2.x).FuzzyEq(v1.y / v2.y);
            return Mathf.Abs(Vector2.Dot(v1.normalized, v2.normalized)) > 0.9999f;
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
        public static bool         Collinear            (this Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b).IsZero();
        }
        public static Vector3      GetTangent           (this Vector3 vector)
        {
            Vector3 normal = vector.normalized;

            // Choose another vector not parallel
            Vector3 tangent = (Mathf.Abs(normal.x) < 0.9f) ? xAxis : yAxis;

            // Remove the part that is parallel to vector
            tangent -= normal * Vector3.Dot(normal, tangent);
            return tangent.normalized;
        }
        public static void         GetTangents          (this Vector3 vector, out Vector3 tangent, out Vector3 bitangent)
        {
            tangent = GetTangent(vector);
            bitangent = Vector3.Cross(vector, tangent).normalized;
        }
        public static Vector3      Y                    (this Vector3 vector)
        {
            return new Vector3(0f, vector.y, 0f);
        }
        public static Vector3      XZ                   (this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }
        public static bool         IsZero               (this Vector3 vector)
        {
            return vector.sqrMagnitude <= eps * eps;
        }
        public static bool         IsClose              (this Vector3 vector, Vector3 other)
        {
            return (vector - other).IsZero();
        }
        public static Vector3      Mult                 (this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3      Div                  (this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static bool         IsNan                (this Vector3 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
        }
        public static bool         IsWithin             (this Vector3 vector, Vector3 other, float dist)
        {
            return (vector - other).sqrMagnitude <= dist * dist;
        }
        public static bool         IsUnit               (this Vector3 vector)
        {
            return Mathf.Abs(vector.sqrMagnitude - 1f) <= eps;
        }
        public static void         Deconstruct          (this Vector3 vector, out float x, out float y, out float z)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public static Vector3      ClampComponents      (this Vector3 vector, Vector3 min, Vector3 max)
        {
            return new(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z));
        }
        public static Vector3      Truncate             (this Vector3 vector, float max)
        {
            Debug.Assert(max >= 0f);
            if (vector.sqrMagnitude > max * max)
                return vector.normalized * max;
            return vector;
        }
        public static Vector3      Clamp                (this Vector3 vector, float min, float max)
        {
            return vector.normalized * Mathf.Clamp(vector.magnitude, min, max);
        }

        public static Vector3      xAxis                => Vector3.right;
        public static Vector3      yAxis                => Vector3.up;
        public static Vector3      zAxis                => Vector3.forward;


        ////////////////////////// QUATERNION ////////////////////////////////
        public static Vector3      GetSwing             (this Quaternion q)
        {
            return q * Vector3.forward;
        }
        public static float        GetTwist             (this Quaternion q)
        {
            //return Vector3.SignedAngle(Vector3.up, q * Vector3.up, GetSwing(q));

            Vector3 swing = q * Vector3.forward;
            Quaternion swingRot = Quaternion.FromToRotation(Vector3.forward, swing);
            //Quaternion twistRot = q * Quaternion.Inverse(swingRot);
            float twist = Vector3.SignedAngle(swingRot*Vector3.up, q*Vector3.up, swing);
            return twist;
        }
        public static Quaternion   FlipX                (this Quaternion q)
        {
            // This works but is less efficient as it needs to convert
            //q.ToAngleAxis(out float angle, out Vector3 axis);
            //axis.x *= -1f; // flip along x
            //return Quaternion.AngleAxis(-angle, axis);

            //return new Quaternion(-q.x, q.y, q.z, q.w); // this doesn't work, only flips axis.x *= -1
            return new Quaternion(q.x, -q.y, -q.z, q.w); // this works, and flips both axis and angle
        }
        public static Quaternion   Inverse              (this Quaternion q)
        {
            return Quaternion.Inverse(q);
        }
        public static Vector3      GetImag              (this Quaternion q)
        {
            return new Vector3(q.x, q.y, q.z);
        }
        public static void         SetImag              (this Quaternion q, Vector3 vector)
        {
            q.x = vector.x;
            q.y = vector.y;
            q.z = vector.z;
            q.w = 0f;
        }
        public static Quaternion   SmoothDamp           (Quaternion current, Quaternion target, ref Quaternion currentVelocity, float smoothTime)
        {
            Quaternion changeQuat = current * target.Inverse();
            changeQuat.ToAngleAxis(out float angle, out Vector3 axis);
            Vector3 change = axis * angle;
            Vector3 imag = currentVelocity.GetImag();
            Vector3 angularStep = Vector3.SmoothDamp(change, Vector3.zero, ref imag, smoothTime);
            currentVelocity.SetImag(imag);
            if (angularStep.IsZero())
                return target;
            angle = angularStep.magnitude;
            axis = angularStep / angle;
            Quaternion step = Quaternion.AngleAxis(angle, axis);
            Quaternion output = step * target;
            return output.normalized;
        }
        public static Quaternion   SmoothDampOld        (Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
        {
            if (Time.deltaTime < Mathf.Epsilon) return rot;
            // account for double-cover
            float dot = Quaternion.Dot(rot, target);
            float multi = dot >= 0f ? 1f : -1f;
            target.x *= multi;
            target.y *= multi;
            target.z *= multi;
            target.w *= multi;
            // smooth damp (nlerp approx)
            var result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
            ).normalized;

            // ensure deriv is tangent
            var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), result);
            deriv.x -= derivError.x;
            deriv.y -= derivError.y;
            deriv.z -= derivError.z;
            deriv.w -= derivError.w;

            return new Quaternion(result.x, result.y, result.z, result.w);
        }
        public static Quaternion   RandomQuaternion     ()
        {
            //return Quaternion.Euler(Random.value * 360f, Random.value * 360f, Random.value * 360f);
            float u = UnityEngine.Random.value;
            float v = UnityEngine.Random.value;
            float w = UnityEngine.Random.value;
            float twoPi = Mathf.PI * 2f;
            return new Quaternion(Mathf.Sqrt(1f - u) * Mathf.Sin(twoPi * v), Mathf.Sqrt(1f - u) * Mathf.Cos(twoPi * v), Mathf.Sqrt(u) * Mathf.Sin(twoPi * w), Mathf.Sqrt(u) * Mathf.Cos(twoPi * w));
        }
        public static Quaternion   FromDirection2D      (this Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x);
            return Quaternion.Euler(-angle*Mathf.Rad2Deg, 90f, 0f);
        }
        public static Quaternion   FromAngle2D          (this float angle)
        {
            return Quaternion.Euler(-angle, 90f, 0f);
        }
        public static Vector3      ToAxisTimesAngle     (this Quaternion q)
        {
            q.ToAngleAxis(out float angle, out Vector3 axis);
            return axis * angle;
        }
        public static Quaternion   QuaternionFromAxisTimesAngle(this Vector3 v)
        {
            if (v.IsZero())
                return Quaternion.identity;
            float angle = v.magnitude;
            Vector3 axis = v / angle;
            return Quaternion.AngleAxis(angle, axis);
        }


        ////////////////////////// FUZZY MATH ////////////////////////////////
        public static bool         FuzzyEq               (this float a, float b)
        {
            //return Mathf.Approximately(a, b);
            return Mathf.Abs(a - b) <= eps;
        }
        public static bool         FuzzyEq               (this Vector2 v, Vector2 o)
        {
            return FuzzyEq(v.x, o.x) && FuzzyEq(v.y, o.y);
        }
        public static bool         FuzzyEq               (this Vector3 v, Vector3 o)
        {
            return FuzzyEq(v.x, o.x) && FuzzyEq(v.y, o.y) && FuzzyEq(v.z, o.z);
        }
        public static bool         FuzzyEq               (this Matrix4x4 m1, Matrix4x4 m2)
        {
            for (int i = 0; i < 16; ++i)
                if (!FuzzyEq(m1[i], m2[i]))
                    return false;
            return true;
        }

        public static bool         FuzzyLt               (this float a, float b)
        {
            return a < b + eps;
        }
        public static bool         FuzzyGt               (this float a, float b)
        {
            return a > b - eps;
        }
        public static bool         FuzzyLe               (this float a, float b)
        {
            return a <= b + eps;
        }
        public static bool         FuzzyGe               (this float a, float b)
        {
            return a >= b - eps;
        }
        public static bool         FuzzyNe               (this float a, float b)
        {
            return Mathf.Abs(a - b) > eps;
        }


        ////////////////////////// LERP INTERPOLATION ////////////////////////////////
        private static float       GlerpAlpha           (float time, float dt)
        {
            // original: 1-e^(-speed * dt)
            // using time = 1/speed. The 5x is because it takes 5x the time to reach 99.3%
            // speed = 5/time =>  time = 5/speed
            return time == 0f ? 1f : 1f - Mathf.Exp(-5f / time * dt);
        }
        public static float        Glerp                (float a, float b, float time, float dt)
            => Mathf.Lerp(a, b, GlerpAlpha(time, dt));
        public static Vector2      Glerp                (Vector2 a, Vector2 b, float time, float dt)
            => Vector2.Lerp(a, b, GlerpAlpha(time, dt));
        public static Vector3      Glerp                (Vector3 a, Vector3 b, float time, float dt)
            => Vector3.Lerp(a, b, GlerpAlpha(time, dt));
        public static Quaternion   Glerp                (Quaternion a, Quaternion b, float time, float dt)
            => Quaternion.Slerp(a, b, GlerpAlpha(time, dt));
        public static CFrame       Glerp                (CFrame a, CFrame b, float time, float dt)
        {
            float t = GlerpAlpha(time, dt);
            return new(Vector3.Lerp(a.position, b.position, t), Quaternion.Slerp(a.rotation, b.rotation, t));
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
            Debug.Assert(!a.IsNullOrEmpty());
            float sum = a.Sum();

            if (sum == 0f)
            { // equiprobable
                for (int j = 0; j < a.Length; j++) a[j] = 1;
                sum = a.Length;
            }

            float x = 0f;
            float r;
            if (randValue == -1f)
                r = UnityEngine.Random.Range(0f, sum);
            else
                r = randValue * sum;

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
        public static int WheelOfFortune2(float[] a, float randValue)
        {
            Debug.Assert(!a.IsNullOrEmpty() && a.All(x => x >= 0f));
            Debug.Assert(randValue >= 0f && randValue <= 1f);
            float sum = a.Sum();
            if (sum == 0f)
                return Mathf.Min(Mathf.FloorToInt(randValue * a.Length), a.Length-1);
            float r = randValue * sum;
            for (int i = 0; i < a.Length; i++)
            {
                if (r <= a[i])
                    return i;
                r -= a[i];
            }
            Debug.Assert(false);
            return Mathf.Min(Mathf.FloorToInt(randValue * sum), a.Length - 1);
        }

        public static float        Normal               ()
        {
            // PURPOSE: Normal distribution with avg=0 and std=1
            float result = 0f;
            for (int i = 0; i < 12; i++) result += UnityEngine.Random.value;
            return result - 6f;
        }
        public static float        SampleNormal()
        {
            //Avoid getting u == 0
            float u1 = 0f, u2 = 0f;
            while (u1 < Mathf.Epsilon || u2 < Mathf.Epsilon)
            {
                u1 = UnityEngine.Random.value; //random.random()
                u2 = UnityEngine.Random.value;
            }
            float n1 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2 * Mathf.PI * u2);
            //float n2 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2 * Mathf.PI * u2);
            return n1;//, n2
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
            if (n01 < 0.5f) return Mathf.Lerp(min, avg, n01 * 2f);
            else return Mathf.Lerp(avg, max, (n01 - 0.5f) * 2f);
        }
        public static float        SampleRandomDistribution(float average, float deviation)
        {
            return Mathf.Max(0f, UnityEngine.Random.Range(average - deviation, average + deviation));
        }
        public static float        SampleWaitTimePoisson(float avgEventWaitTime)
        {
            // NOTES: this was previously taking eventRate, which is 1/avgEventWaitTime
            // exponential distribution CDF: L*exp(-L*x) => inverse: -ln(1-y)/L, y~[0,1], L=lambda
            // https://www.desmos.com/calculator/wo5u0iid05
            float y;
            do
            {
                y = UnityEngine.Random.value;
            } while (y == 0f); // ln(0)==-inf
            return -Mathf.Log(y) * avgEventWaitTime;
        }
        public static float        SampleVariability    (this float avg, float var)
        {
            Debug.Assert(var >= 0f && var <= 1f);
            return avg * UnityEngine.Random.Range(1f - var, 1f + var);
        }
        public static int          GetRandom            (this MonoBehaviour mb, int number)
        {
            return System.HashCode.Combine(mb.GetHashCode(), number);
        }
        public static float        GetRandomValue       (this MonoBehaviour mb, int number)
        {
            return System.HashCode.Combine(mb.GetHashCode(), number);
        }
        public static uint hash(uint state)
        {
            // https://www.cs.ubc.ca/~rebridson/docs/schechter-sca08-turbulence.pdf
            state ^= 2747636419u;
            // 3 times the same
            state *= 2654435769u;
            state ^= state >> 16;
            state *= 2654435769u;
            state ^= state >> 16;
            state *= 2654435769u;
            return state;
        }


        ////////////////////////// SEQUENCES / COLLECTIONS ////////////////////////////////
        public static T            Last<T>              (this IEnumerable<T> enumerable, T defaultT = default)
        {
            if (enumerable == null || enumerable.Count() == 0) return defaultT;
            return enumerable.ElementAt(enumerable.Count() - 1);
        }
        public static T            SafeGet<T>           (this IEnumerable<T> enumerable, int index, T defaultT = default)
        {
            if (enumerable == null || index < 0 || index >= enumerable.Count())
                return defaultT;
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
            return enumerable.ElementAt(Mathf.Clamp(index, 0, enumerable.Count() - 1));
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
            foreach (T elem in enumerable)
            {
                if (predicate(elem)) return elem;
            }
            return defaultT;
        }
        public static bool         Contains<T>          (this IEnumerable<T> enumerable, T element)
        {
            return enumerable.FindIndex(element) != -1;
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
            int bestIndex = FindBestIndex<T>(enumerable, costFunction, filter);
            if (bestIndex != -1)
                return enumerable.ElementAt(bestIndex);
            return default;
        }
        public static int          FindBestIndex<T>     (this IEnumerable<T> enumerable, Func<T, float> costFunction, Predicate<T> filter = null)
        {
            if (enumerable == null || enumerable.Count() == 0)
                return -1;
            float minCost = float.MaxValue;
            int bestIndex = -1;
            for (int i = 0, end = enumerable.Count(); i < end; i++)
            {
                T element = enumerable.ElementAt(i);
                if (filter == null || filter(element)) // pass filter
                {
                    float elementCost = costFunction(element);
                    if (elementCost < minCost)
                    {
                        minCost = elementCost;
                        bestIndex = i;
                    }
                }
            }
            return bestIndex;
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
            foreach (float f in array)
            {
                if (f != maxValue)
                {
                    oneDifferent = true;
                    break;
                }
            }
            if (oneDifferent == false)
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
        public static int[]        Sequence             (int from, int to)
        {
            // NOTES: to is exclusive
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
        public static void         AddSorted<T>         (this List<T> list, T item, IComparer<T> comparer)
        {
            if (list.Count == 0)
            {
                list.Add(item);
            }
            else if (comparer.Compare(list[list.Count - 1], item) <= 0)
            {
                list.Add(item);
            }
            else if (comparer.Compare(list[0], item) >= 0)
            {
                list.Insert(0, item);
            }
            else
            {
                int index = list.BinarySearch(item, comparer);
                if (index < 0)
                    index = ~index;
                list.Insert(index, item);
            }
        }
        public static IEnumerable<(T,U)> Zip<T, U>      (this IEnumerable<T> list1, IEnumerable<U> list2)
        {
            if (list1 != null && list2 != null)
            {
                using (var enumerator1 = list1.GetEnumerator())
                using (var enumerator2 = list2.GetEnumerator())
                {
                    while (enumerator1.MoveNext() && enumerator2.MoveNext())
                    {

                        yield return (
                            enumerator1.Current,
                            enumerator2.Current);
                    }
                }
            }
        }
        // TODO: Deprecate the following two methods
        public static void         ForeachPair<T>       (this List<T> list, System.Action<T, T> action)
        {
            if (list == null || action == null)
                return;
            for (int i = 0; i < list.Count; ++i)
            {
                for (int j = i+1; j < list.Count; ++j)
                {
                    action(list[i], list[j]);
                }
            }
        }
        public static void         ForeachPair<T>       (this List<T> list1, List<T> list2, System.Action<T, T> action)
        {
            if (list1 == null || list2 == null || action == null)
                return;
            for (int i = 0; i < list1.Count; ++i)
            {
                for (int j = 0; j < list2.Count; ++j)
                {
                    action(list1[i], list2[j]);
                }
            }
        }
        public static void         Shuffle<T>           (this IList<T> list, System.Random prng = null)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = prng != null ? prng.Next(n + 1) : UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static T            Pop<T>               (this IList<T> list)
        {
            if (list.Count == 0)
                return default(T);
            T ans = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return ans;
        }
        public static void         InsertionSort<T>     (this IList<T> arr, IComparer<T> comparer)
        {
            for (int i = 1; i < arr.Count; ++i)
            {
                T key = arr[i];
                int j = i - 1;
                while (j >= 0 && comparer.Compare(key, arr[j]) < 0)
                {
                    arr[j + 1] = arr[j];
                    --j;
                }
                arr[j + 1] = key;
            }
        }
        public static V            GetOrInsert<K,V>     (this Dictionary<K,V> dictionary, K key) where V : new()
        {
            if (!dictionary.TryGetValue(key, out V value))
            {
                value = new();
                dictionary[key] = value;
            }
            return value;
        }
        public static IEnumerable<T> EmptyEnumerable<T> ()
        {
            return Enumerable.Empty<T>();
        }
        public static T            GetMod<T>            (this IEnumerable<T> enumerable, int index)
        {
            return enumerable.ElementAt(index.Mod(enumerable.Count()));
        }


        ////////////////////////// GEOMETRY ////////////////////////////////
        public static bool         IsPointInsideTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            // taken from https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
            static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
            {
                return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
            }

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
            if (FindSegmentIntersection(pab, c, pac, b, out Vector2 ans))
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
        public static bool         Intersect            (Ray2D r0, Ray2D r1, out Vector2 output)
        {
            return FindSegmentIntersection(r0.origin, r0.GetPoint(10000f), r1.origin, r1.GetPoint(10000f), out output);
        }
        public static Vector2      FindIntersectionPointWithOffset(Vector2 p0, Vector2 p1, Vector2 p2, float offset)
        {
            Vector2 d0 = (p1 - p0).normalized;
            Vector2 d1 = (p2 - p1).normalized;
            float dot = Vector2.Dot(d0, d1);
            if (dot >= 1f - eps) // collinear
            {
                return p1 + d0.Rotate90() * offset;
            }
            else if (dot <= -1f + eps) // opposite
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
            if (dot > 1f - eps) // collinear
            {
                return d.Rotate90();
            }
            else if (dot < -1f + eps) // opposite
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

            if (e == 0f)
            {
                return false;
            }

            float u = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / e;
            float v = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / e;

            if (u < 0f || u > 1f || v < 0f || v > 1f)
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
            if (Mathf.Abs(l2) < eps) return (p - a).sqrMagnitude;
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
        public static bool         LineIntersection                   (Vector2 point1, Vector2 dir1, Vector2 point2, Vector2 dir2, out Vector2 ans)
        {
            dir1.Normalize();
            dir2.Normalize();
            float L = 1000f;
            return Utility.FindSegmentIntersection(point1 - dir1 * L, point1 + dir1 * L, point2 - dir2 * L, point2 + dir2 * L, out ans);
        }


        ////////////////////////// ENUM ////////////////////////////////
        public static int          EnumLength<Enum>     ()
        {
            return System.Enum.GetValues(typeof(Enum)).Length;
        }
        public static string[]     EnumNames<Enum>      ()
        {
            return System.Enum.GetNames(typeof(Enum));
        }
        public static T            RandomEnumValue<T>   ()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(UnityEngine.Random.Range(0, v.Length));
        }
        public static T[]          EnumValues<T>()
        {
            var v = Enum.GetValues(typeof(T));
            T[] ans = new T[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                ans[i] = (T)v.GetValue(i);
            }
            return ans;
        }
        public static T            ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        public static T            Next<T>                            (this T enumValue) where T : Enum
        {
            int enumIntValue = Convert.ToInt32(enumValue);
            int enumIntCount = Enum.GetValues(typeof(T)).Length;
            enumIntValue = (enumIntValue + 1) % enumIntCount;
            return (T)Enum.ToObject(typeof(T), enumIntValue);
        }


        ////////////////////////// TRANSFORM ////////////////////////////////
        public static float        Heading              (this Transform transform)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < eps)
                return 0f;
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
        public static void         DestroyAllChildrenImmediate(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        public static void         SortChildren         (this Transform transform, bool recursive = false, Comparison<Transform> comparison = null)
        {
            // PURPOSE: Sorts the children of the given gameobject by name
            if (comparison == null)
                comparison = (t1, t2) => t1.name.CompareTo(t2.name);
            Transform[] ts = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                ts[i] = transform.GetChild(i);
                if (recursive)
                {
                    SortChildren(ts[i], true, comparison);
                }
            }
            System.Array.Sort(ts, comparison);

            for (int i = 0; i < ts.Length; i++)
            {
                ts[i].SetSiblingIndex(i);
            }
        }
        public static void         SortChildrenByVolume (this Transform transform)
        {
            float Volume(Transform t)
            {
                return t.GetComponentInChildren<MeshRenderer>()?.localBounds.Volume() ?? 0f;
            }
            SortChildren(transform, false, (t1, t2) => Volume(t1).CompareTo(Volume(t2)));
        }
        public static void         PrintChildren        (this Transform transform, StringBuilder sb, bool recursive = true, int depth = 0)
        {
            // PURPOSE: prints the children of the given gameobject by name
            if (depth == 0)
            {
                sb.Append($"{new string('\t', depth)}{transform.gameObject.name}\n");
            }
            ++depth;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                sb.Append($"{new string('\t', depth)}{child.gameObject.name}\n");
                if (recursive)
                {
                    PrintChildren(child, sb, recursive, depth);
                }
            }
        }

        public static IEnumerable<Transform> GetChildren(this Transform transform, bool includeSelf = false)
        {
            if (includeSelf)
                yield return transform;
            for (int i = 0; i < transform.childCount; ++i)
            {
                yield return transform.GetChild(i);
            }
        }
        public static IEnumerable<T> GetChildrenOfType<T>(this Transform transform, bool includeSelf = false)
        {
            return transform.GetChildren(includeSelf).Select(t => t.GetComponent<T>()).Where(c => c != null);
        }
        public static IEnumerable<Transform> GetDescendants(this Transform transform, bool includeSelf = false, bool useDfsOrder = false)
        {
            // By default we run BFS. DFS is also available
            if (includeSelf)
                yield return transform;

            if (!useDfsOrder)
            {
                // BFS
                Queue<Transform> queue = new();
                queue.Enqueue(transform);

                while (queue.Count > 0)
                {
                    Transform t = queue.Dequeue();
                    // already returned
                    for (int i = 0; i < t.childCount; i++)
                    {
                        Transform child = t.GetChild(i);
                        yield return child;
                        if (child.childCount > 0)
                            queue.Enqueue(child);
                    }
                }
            }
            else
            {
                // DFS
                Stack<Transform> stack = new();
                if (includeSelf)
                {
                    stack.Push(transform);
                }
                else
                {
                    for (int i = transform.childCount - 1; i >= 0; --i)
                        stack.Push(transform.GetChild(i));
                }

                while (stack.Count > 0)
                {
                    Transform t = stack.Pop();
                    yield return t;
                    for (int i = t.childCount - 1; i >= 0; --i)
                    {
                        Transform child = t.GetChild(i);
                        stack.Push(child);
                    }
                }
            }
        }
        public static IEnumerable<T> GetDescendantsOfType<T>(this Transform transform, bool includeSelf = false, bool useDfsOrder = false) where T : Component
        {
            // TODO: ensure that no gameobject has multiple components of the same type
            return transform.GetDescendants(includeSelf, useDfsOrder).Select(t => t.GetComponent<T>()).Where(c => c != null);
        }
        public static Transform    FindDescendant       (this Transform transform, string name)
        {
            // NOTES: Transform.Find only looks in the children, not in the descendants
            // NOTES: this is a behavior change, previously we were also looking is self
            return transform.GetDescendants(false).Where(t => t.name == name).FirstOrDefault();
        }
        public static Transform    FindChildOrCreate    (this Transform transform, string name)
        {
            Transform child = transform.Find(name);
            if (child == null)
            {
                child = new GameObject(name).transform;
                child.parent = transform;
                child.Reset();
            }
            return child;
        }
        public static int          DescendantCount      (this Transform transform)
        {
            return transform.GetDescendants().Count();
        }
        // NOTES: these deprecated methods will eventually be removed.
        [Obsolete("ForeachChild is obsolete. Use foreach with .GetChildren() instead.", true)]
        public static void ForeachChild(this Transform transform, System.Action<Transform> action, bool includeSelf = false)
        {
            if (transform == null || action == null)
                return;
            foreach (var child in transform.GetChildren(includeSelf))
                action(child);
        }
        [Obsolete("ForeachDescendant is obsolete. Use foreach with .GetDescendants() instead.", true)]
        public static void ForeachDescendant(this Transform transform, System.Action<Transform> action, bool includeSelf = false)
        {
            if (transform == null || action == null)
                return;
            foreach (var descendant in transform.GetDescendants(includeSelf))
                action(descendant);
        }
        [Obsolete("ForeachDescendantOfType is obsolete. Use foreach with .GetDescendantsOfType() instead.", true)]
        public static void ForeachDescendantOfType<T>(this Transform transform, System.Action<T> action, bool includeSelf = false) where T : Component
        {
            if (transform == null || action == null)
                return;
            foreach (var descendant in transform.GetDescendantsOfType<T>(includeSelf))
                action(descendant);
        }
        [Obsolete("ForeachDescendantDFS is obsolete. Use foreach with .GetDescendants(useDfsOrder=true) instead.", true)]
        public static void ForeachDescendantDFS(this Transform transform, System.Action<Transform> action, bool includeSelf = true)
        {
            if (transform == null || action == null)
                return;
            foreach (var descendant in transform.GetDescendants(includeSelf, true))
                action(descendant);
        }

        public static void         CopyPositionRotation (this Transform transform, Transform other)
        {
            if (transform == null || other == null) return;
            transform.position = other.position;
            transform.rotation = other.rotation;
        }
        public static void         Reset                (this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        public static void         Clean                (this Transform transform, float eps = 1e-4f)
        {
            transform.localPosition = Clean(transform.localPosition, 1f, eps);
            transform.localEulerAngles = Clean(transform.localEulerAngles, 90f, eps);
            transform.localScale = Clean(transform.localScale, 1f, eps);
        }
        public static Vector3      ToLocalP             (this Transform transform, Vector3 point)
        {
            if (!transform)
                return point;
            return transform.InverseTransformPoint(point);
        }
        public static Vector3      ToWorldP             (this Transform transform, Vector3 point)
        {
            if (!transform)
                return point;
            return transform.TransformPoint(point);
        }
        public static Vector3      ToLocalV             (this Transform transform, Vector3 vector)
        {
            if (!transform)
                return vector;
            return transform.InverseTransformVector(vector);
        }
        public static Vector3      ToWorldV             (this Transform transform, Vector3 vector)
        {
            if (!transform)
                return vector;
            return transform.TransformVector(vector);
        }
        public static Quaternion   ToLocal              (this Transform transform, Quaternion q)
        {
            if (!transform)
                return q;
            return Quaternion.Inverse(transform.rotation) * q;
        }
        public static Quaternion   ToWorld              (this Transform transform, Quaternion q)
        {
            if (!transform)
                return q;
            return transform.rotation * q;
        }
        public static bool         IsDescendantOf       (this Transform transform, Transform query)
        {
            if (transform == null || query == null || transform == query)
                return false;
            do
            {
                transform = transform.parent;
                if (transform == null)
                    return false;
                if (transform == query)
                    return true;
            } while (true);
        }
        public static bool         IsAncestorOf         (this Transform transform, Transform query)
        {
            return IsDescendantOf(query, transform);
        }
        public static int          Depth                (this Transform transform)
        {
            int ans = 0;
            while (transform != null)
            {
                transform = transform.parent;
                ++ans;
            }
            return ans-1;
        }
        public static void         SplayChildren        (this Transform transform, float splayAmount, int splayRow = -1)
        {
            int splayCount = splayRow > 0 ? splayRow : Mathf.RoundToInt(Mathf.Sqrt(transform.childCount));
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                t.localRotation = Quaternion.identity;
                t.localPosition = new Vector3(i % splayCount, 0, i / splayCount) * splayAmount;
            }
        }
        public static int          Depth                (this Transform transform, Transform ancestor)
        {
            int depth = 0;
            while (transform != null && transform != ancestor)
            {
                ++depth;
                transform = transform.parent;
            }
            return transform == ancestor ? depth : -1;
        }
        public static Bounds       CombinedBounds       (this Transform transform)
        {
            MeshRenderer[] mrs = transform.GetComponentsInChildren<MeshRenderer>();
            if (mrs.IsNullOrEmpty())
            {
                return new Bounds();
            }
            else
            {
                Matrix4x4 m = transform.localToWorldMatrix;
                Bounds b = new Bounds();
                for (int i = 0; i < mrs.Length; i++)
                {
                    Matrix4x4 m2 = mrs[i].transform.localToWorldMatrix;
                    Matrix4x4 m3 = m.inverse * m2;
                    Bounds localBounds = mrs[i].localBounds;
                    for (int j = 0; j < 8; j++)
                    {
                        Vector3 localcorner = localBounds.GetCorner(j);
                        Vector3 worldcorner = m3.MultiplyPoint3x4(localcorner);
                        if (i == 0 && j == 0)
                        {
                            b = new Bounds(worldcorner, Vector3.zero);
                        }
                        else
                        {
                            b.Encapsulate(worldcorner);
                        }
                    }
                }
                return b;
            }
        }
        public static Vector3      GetCorner            (this Bounds b, int index)
        {
            Vector3 x = new Vector3((index & 1), (index >> 1) & 1, (index >> 2) & 1);
            return b.min + b.size.Mult(x);
        }
        public static T            Find<T>              (this Transform transform, string nameOrPath)
        {
            Transform t = transform.Find(nameOrPath);
            if (t != null)
                return t.GetComponent<T>();
            return default(T);
        }
        public static void         Set                  (this Transform transform, CFrame transf)
        {
            transform.position = transf.position;
            transform.rotation = transf.rotation;
        }
        public static void         SetLocal             (this Transform transform, CFrame transf)
        {
            transform.localPosition = transf.position;
            transform.localRotation = transf.rotation;
        }
        public static CFrame       GetTransf            (this Transform transform)
        {
            return new(transform.position, transform.rotation);
        }
        public static CFrame       GetTransfLocal       (this Transform transform)
        {
            return new(transform.localPosition, transform.localRotation);
        }
        public static CFrame       Inverse              (this Transform transform)
        {
            return new CFrame(transform).inverse;
        }
        public static void         PreRotate            (this Transform transform, Quaternion rot)
        {
            transform.rotation = rot * transform.rotation;
        }
        public static bool         IsIdentity           (this Transform transform)
        {
            return transform.localPosition == Vector3.zero
                && transform.localRotation == Quaternion.identity
                && transform.localScale == Vector3.one;
        }


        ////////////////////////// RECTTRANSFORM ////////////////////////////////
        public static Vector3      GetOrigin(this Plane plane)
        {
            return -plane.normal * plane.distance;
        }


        ////////////////////////// RECTTRANSFORM ////////////////////////////////
        public static Rect         GetWorldRect         (this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector2 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
            float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
            float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
            Vector2 size = new Vector2(maxX - minX, maxY - minY);
            return new Rect(center - size / 2f, size);
        }


        ////////////////////////// RIGIDBODY ////////////////////////////////
        public static void         AddVelocity          (this Rigidbody rb, Vector3 velocity)
        {
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
        public static void         SetVelocity          (this Rigidbody rb, Vector3 velocity)
        {
            rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
        }
        public static void         AddAcceleration      (this Rigidbody rb, Vector3 acceleration)
        {
            rb.AddForce(acceleration, ForceMode.Acceleration);
        }

        public static void         AddAccelerationMaxVelocity_DEPRECATED(this Rigidbody rb, Vector3 acceleration, float maxVelocity)
        {
            float speedDelta = maxVelocity - rb.velocity.magnitude;
            if (speedDelta > 0f)
            {
                float maxAccForVel = speedDelta / Time.deltaTime;
                if (acceleration.sqrMagnitude > maxAccForVel*maxAccForVel)
                {
                    acceleration = acceleration.normalized * maxAccForVel;
                }
                rb.AddForce(acceleration, ForceMode.Acceleration);
            }
        }
        public static void         Brake_DEPRECATED     (this Rigidbody rb, float breakAcc)
        {
            if (rb.velocity.magnitude > eps)
            {
                float maxAccNeeded = rb.velocity.magnitude / Time.fixedDeltaTime;
                //acceleration += -velocity.normalized * maxAcceleration;//Mathf.Min(maxAccNeeded, breakAcc);
                rb.AddAcceleration(-rb.velocity.normalized * Mathf.Min(maxAccNeeded, breakAcc));
                //rb.AddAcceleration(-rb.velocity.normalized * breakAcc);
            }
        }
        public static void         AddRelativeForceAtPosition(this Rigidbody rb, Vector3 force, Vector3 position, ForceMode mode)
        {
            Vector3 forceWorld = rb.transform.TransformVector(force);
            Vector3 posWorld   = rb.transform.TransformPoint(position);
            rb.AddForceAtPosition(forceWorld, posWorld, mode);
        }

        public static Vector3      ConvertToForce       (Vector3 value, ForceMode mode, float mass, float dt)
        {
            switch (mode)
            {
            case ForceMode.Force: // N=kg*m/s^2 -> rb.velocity += value * dt / mass
                return value;
            case ForceMode.Acceleration: // m/s^2 -> rb.velocity += value * dt
                return value * mass;
            case ForceMode.Impulse: // momentum=kg*m/s -> rb.velocity += value / mass
                return value / dt;
            case ForceMode.VelocityChange: // m/s -> rb.velocity += value
                return value * mass / dt;
            }
            Debug.Assert(false);
            return Vector3.zero;
        }
        public static float        ConvertToForce       (float value, ForceMode mode, float mass, float dt)
            => ConvertToForce(new Vector3(value, 0f, 0f), mode, mass, dt).x;


        // from https://discussions.unity.com/t/force-to-velocity-scaling/120739/3
        public static float GetFinalVelocity(float aVelocityChange, float aDrag)
        {
            return aVelocityChange * (1 / Mathf.Clamp01(aDrag * Time.fixedDeltaTime) - 1);
        }
        public static float GetFinalVelocityFromAcceleration(float aAcceleration, float aDrag)
        {
            return GetFinalVelocity(aAcceleration * Time.fixedDeltaTime, aDrag);
        }
        public static float GetDrag(float aVelocityChange, float aFinalVelocity)
        {
            return aVelocityChange / ((aFinalVelocity + aVelocityChange) * Time.fixedDeltaTime);
        }
        public static float GetDragFromAcceleration(float aAcceleration, float aFinalVelocity)
        {
            return GetDrag(aAcceleration * Time.fixedDeltaTime, aFinalVelocity);
        }

        ////////////////////////// RIGIDBODY 2D ////////////////////////////////
        public static void         AddForce             (this Rigidbody2D rb, Vector2 force, ForceMode mode = ForceMode.Force)
        {
            rb.AddForce(ConvertToForce(force, mode, rb.mass, Time.fixedDeltaTime));
        }
        public static void         AddRelativeForce     (this Rigidbody2D rb, Vector2 force, ForceMode mode = ForceMode.Force)
        {
            rb.AddRelativeForce(ConvertToForce(force, mode, rb.mass, Time.fixedDeltaTime));
        }
        public static void         AddTorque            (this Rigidbody2D rb, float torque, ForceMode mode = ForceMode.Force)
        {
            rb.AddTorque(ConvertToForce(torque, mode, rb.inertia, Time.fixedDeltaTime));
        }
        public static void         AddVelocity          (this Rigidbody2D rb, Vector2 velocity)
        {
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
        public static void         SetVelocity          (this Rigidbody2D rb, Vector2 velocity)
        {
            rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
        }
        public static void         AddAcceleration      (this Rigidbody2D rb, Vector2 acceleration)
        {
            rb.AddForce(acceleration, ForceMode.Acceleration);
        }
        public static void         AddRelativeAcceleration(this Rigidbody2D rb, Vector2 acceleration)
        {
            rb.AddRelativeForce(acceleration, ForceMode.Acceleration);
        }
        public static void         CopyVelocities       (this Rigidbody2D rb, Rigidbody2D other)
        {
            rb.velocity = other.velocity;
            rb.angularVelocity = other.angularVelocity;
        }

        public static void         AddTorqueToReachRotation_DEPRECATED(this Rigidbody2D rb, float targetRotation, float maxTorque)
        {
            float deltaAngle = Mathf.DeltaAngle(rb.rotation, targetRotation); // todo: ensure in [-180, 180
            float torque = Math.Clamp(deltaAngle, -maxTorque, maxTorque);
            rb.AddTorque(torque, ForceMode2D.Force);
        }
        public static (float, float) ComputeAccelerationAndDragToObtainMaxVelocityInTime_DEPRECATED(float velocityMax, float timeToVelocityMax)
        {
            float v = velocityMax;
            float T = timeToVelocityMax;
            float t = Time.fixedDeltaTime;
            float p = 0.99f; // the percentage of maxv to consider it reached, e.g. 99%

            float d = (1f - Mathf.Exp(Mathf.Log(1f - p) * t / T)) / t;
            float a = v * d;
            return (a, d);
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
            foreach (char c in characters)
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
        public static string       FormatTimestamp      (this DateTime value, string format = null)
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
        public static string       GetTimestampWeb      (System.DateTime value)
        {
            return value.ToString("dd/MM/yy_HH:mm:ss");
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
        public static string[][]   SplitCsv             (string content, string filename = "", bool trim = false)
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
            if (trim)
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    for (int j = 0; j < cells[i].Length; j++)
                    {
                        cells[i][j] = cells[i][j].Trim();
                    }
                }
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
        public static IEnumerable<T> ParseCsv<T>        (string csvData, char separator = ',') where T : struct
        {
            List<T> ans = new();
            StringReader reader = new(csvData);
            int numStructFields = typeof(T).GetFields().Length;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.IsNullOrEmpty() || line[0] == '#')
                    continue;

                string[] fields = line.Split(separator);
                if (fields.Length != numStructFields)
                {
                    Debug.Log($"Skipping line: {line} - Field count mismatch");
                    continue;
                }

                T parsedStruct = ParseStruct<T>(fields);
                ans.Add(parsedStruct);
            }
            return ans;
        }
        private static T           ParseStruct<T>       (string[] fields) where T : struct
        {
            T result = default;
            int index = 0;
            foreach (var fieldInfo in typeof(T).GetFields())
            {
                object parsedValue = Convert.ChangeType(fields[index], fieldInfo.FieldType);
                fieldInfo.SetValueDirect(__makeref(result), parsedValue);
                index++;
            }
            return result;
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
        }
        public static bool         IsRegexMatch         (this string input, string pattern, out string[] tokens)
        {
            Regex regex = new(pattern);
            if (regex.IsMatch(input))
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
        public static string       TrimStart            (this string str, string prefix)
        {
            if (str.StartsWith(prefix))
                return str.Substring(prefix.Length);
            return str;
        }
        public static string       TrimEnd              (this string str, string suffix)
        {
            if (str.EndsWith(suffix))
                return str.Substring(0, str.Length - suffix.Length);
            return str;
        }


        ////////////////////////// ROUTINE / INVOKE ////////////////////////////////
        public static IEnumerator  SimpleRoutine        (Action<float> function, float duration, float delay = 0f, Action endFunction = null)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            float speed = 1f / duration;
            float percent = 0f;
            function(0f);
            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                function(Mathf.Min(1f, percent));
                yield return null;
            }
            //function(1f);
            if (endFunction != null)
                endFunction();
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
            // TODO: remove
            Debug.LogWarning("Invoke without mb is disabled. Please call the Invoke on a MonoBehavior");
            //Invoke(CoroutineManager.Instance, action, delay);
        }
        private static IEnumerator PrivateInvokeRoutine (Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
        public static IEnumerator  SimpleRoutineRealTime(Action<float> function, float duration, float delay = 0f, Action endFunction = null)
        {
            // copy of SimpleRoutine, uses real time
            if (delay > 0f)
                yield return new WaitForSecondsRealtime(delay);
            float speed = 1f / duration;
            float percent = 0f;
            function(0f);
            while (percent < 1f)
            {
                percent += Time.unscaledDeltaTime * speed;
                function(Mathf.Min(1f, percent));
                yield return null;
            }
            //function(1f);
            if (endFunction != null)
                endFunction();
        }


        ////////////////////////// GAMEOBJECT / COMPONENT ////////////////////////////////
        public static void         Bind<T>              (this Component mb, out T result, bool includeChildren = true, bool canBeNull = false, bool includeParent = false) where T : Component
        {
            if (includeChildren)
            {
                result = mb.GetComponentInChildren<T>();
            }
            else if (includeParent)
            {
                result = mb.GetComponentInParent<T>();
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
        public static T[]          BindChildren<T>      (this GameObject o) where T : Component
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
        public static void         BindParent<T>        (this MonoBehaviour mb, out T result) where T : Component
        {
            mb.Bind(out result, includeChildren: false, includeParent: true);
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
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
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
            int v = 0;
            foreach (var fi in typeof(T).GetFields())
            {
                var val = Convert.ChangeType(values[v++], fi.FieldType); // convert it
                fi.SetValue(result, val);
            }
            return result;
        }
        public static void         SaveTextureAsPNG     (this Texture2D texture, string fullpath, bool log = true)
        {
            byte[] _bytes = texture.EncodeToPNG();
            if (!fullpath.EndsWith(".png")) fullpath += ".png";
            System.IO.File.WriteAllBytes(fullpath, _bytes);
            if (log)
                Debug.Log($"{_bytes.Length / 1024} Kb was saved as: {fullpath}");
        }
        public static void ExportMeshAsObj(this Mesh mesh, string fullpath, bool log = true)
        {
            // NOTICE the x coordinate is flipped because OBJ is right-handed and Unity Left-handed
            using (StreamWriter sw = new StreamWriter(fullpath))
            {
                sw.WriteLine($"# Exported Mesh {mesh.name}");// at {GetTimestampNow()}");
                if (mesh.colors.IsNullOrEmpty())
                {
                    foreach (Vector3 vertex in mesh.vertices)
                        sw.WriteLine("v " + -vertex.x + " " + vertex.y + " " + vertex.z);
                }
                else
                {
                    for (int i = 0; i < mesh.vertexCount; ++i)
                    {
                        Vector3 vertex = mesh.vertices[i];
                        Color color = mesh.colors[i];
                        sw.WriteLine("v " + -vertex.x + " " + vertex.y + " " + vertex.z + " " + color.r + " " + color.g + " " + color.b);
                    }
                }

                foreach (Vector3 normal in mesh.normals)
                    sw.WriteLine("vn " + -normal.x + " " + normal.y + " " + normal.z);

                foreach (Vector2 uv in mesh.uv)
                    sw.WriteLine("vt " + uv.x + " " + uv.y);

                for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                {
                    int[] triangles = mesh.GetTriangles(submesh);
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        // OBJ indices start from 1, so add 1 to each index
                        sw.WriteLine(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
                            triangles[i] + 1, triangles[i + 2] + 1, triangles[i + 1] + 1));
                    }
                }
                if (log)
                    Debug.Log($"Mesh '{mesh.name}' with {mesh.vertexCount} vertices was saved as: {fullpath}");
            }
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
#if SEDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif // SEDITOR
        }
        public static string       GetPath              (this GameObject go)
        {
            string path = "/" + go.name;
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                path = "/" + go.name + path;
            }
            return path;
        }
        
        public static void SyncChildrenInstantiate<T, U>(this Transform parent, IEnumerable<T> list, U prefab, Action<T, U> action, Action<U> init = null) where U : Component
        {
            SyncChildrenInstantiate(list, parent, prefab, action, init);
        }
        public static void SyncChildrenInstantiate<T, U>(IEnumerable<T> list, Transform parent, U prefab, Action<T, U> action, Action<U> init = null) where U : Component
        {
            int i = 0;
            foreach (T element in list)
            {
                if (i >= parent.childCount)
                {
                    U newPrefab = GameObject.Instantiate(prefab, parent) as U;
                    if (init != null)
                        init(newPrefab);
                }
                Transform t = parent.GetChild(i) as Transform;
                t.gameObject.SetActive(true);
                action(element, t.GetComponent<U>());
                ++i;
            }
            for (; i < parent.childCount; ++i)
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        public static void SyncChildrenInstantiate<T, U>(this Transform parent, IEnumerable<T> list, Action<T, U> action, Action<U> init = null) where U : Component
        {
            SyncChildrenInstantiate(list, parent, action, init);
        }
        public static void SyncChildrenInstantiate<T, U>(IEnumerable<T> list, Transform parent, Action<T, U> action, Action<U> init = null) where U : Component
        {
            int i = 1;
            Debug.Assert(parent.childCount >= 1);
            parent.GetChild(0).gameObject.SetActive(false);
            if (list != null)
            {
                foreach (T element in list)
                {
                    if (i >= parent.childCount)
                    {
                        U prefab = parent.GetChild(0).GetComponentInChildren<U>();
                        U newPrefab = GameObject.Instantiate(prefab, parent) as U;
                        if (init != null)
                            init(newPrefab);
                    }
                    Transform t = parent.GetChild(i) as Transform;
                    t.gameObject.SetActive(true);
                    action(element, t.GetComponent<U>());
                    ++i;
                }
            }
            for (; i < parent.childCount; ++i)
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static GameObject   Find                 (string name, bool includeInactive = false)
        {
            if (!includeInactive)
                return GameObject.Find(name);
            return GameObject.FindObjectsOfType<GameObject>(true).Where(go => go.name == name).FirstOrDefault();
        }

        ////////////////////////// SERIALIZATION ////////////////////////////////
        private static string F(float value) => $"{value:0.00000}f";
        public static string       Prettify(this Vector3 v)
        {
            return $"Vector3({F(v.x)}, {F(v.y)}, {F(v.z)})";
        }
        public static string       Prettify(this Quaternion q)
        {
            return $"Quat({F(q.x)}, {F(q.y)}, {F(q.z)}, {F(q.w)})";
        }
        public static void SetListener<T>(this UnityEvent<T> unityEvent, UnityAction<T> call)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(call);
        }
        public static void SetListener(this UnityEvent unityEvent, UnityAction call)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(call);
        }


        ////////////////////////// IMAGES ////////////////////////////////
        public static Vector3      TrilinearSample      (Vector3[] points, Vector3 t)
        {
            Vector3 a = Vector3.Lerp(points[0], points[1], t.x);
            Vector3 b = Vector3.Lerp(points[2], points[3], t.x);
            Vector3 c = Vector3.Lerp(points[4], points[5], t.x);
            Vector3 d = Vector3.Lerp(points[6], points[7], t.x);

            Vector3 e = Vector3.Lerp(a, b, t.y);
            Vector3 f = Vector3.Lerp(c, d, t.y);

            return Vector3.Lerp(e, f, t.z);
        }


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
        public static void         Translate            (this Mesh mesh, Vector3 offset)
        {
            mesh.SetVertices(mesh.vertices.Select(v => v += offset).ToArray());
            mesh.RecalculateBounds();
        }


        ////////////////////////// COLOR ////////////////////////////////
        public static string       ToHex                (this Color color)
        {
            Color32 c = color;
            uint value = (uint)(c.r << 16) | (uint)(c.g << 8) | (uint)(c.b << 0);
            return $"#{value:X6}";
        }

        public static Color        Hex2Color            (string hex)
        {
            if (hex.IsNullOrEmpty())
                return Color.white;
            if (hex[0] == '#')
                hex = hex.Substring(1);
            uint ans = Convert.ToUInt32(hex, 16);
            return Hex2Color(ans);
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
        public static int          ExecuteAndTimeMs     (Action action, string message = "")
        {
            // PURPOSE: Runs and returns the number of milliseconds it took
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            int ms = (int)watch.ElapsedMilliseconds;
            if (!message.IsNullOrEmpty())
            {
                Debug.Log($"{message} took {ms} ms");
            }
            return ms;
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
        public static float        Noise01              (Vector2 point, int seed = 0, float scale = 1f, int octaves = 1)
        {
            return Mathf.Clamp01(Noise11(point, seed, scale, octaves) / 2f + 0.5f);
        }
        public static float        Noise11              (Vector2 point, int seed = 0, float scale = 1f, int octaves = 1)
        {
            //Vector2 offset = Vector2.zero;// new Vector2(seed ^ 0xDD71C, seed ^ 0x359F9);
            float ans = 0f;
            float m = 1f;
            Vector2 offset = Vector2.zero;
            //UnityEngine.Random.InitState(seed);
            //offset = new Vector2(seed^0x44279, seed^ 0xCB81);
            //Vector2 offset = new Vector2(UnityEngine.Random.value * 1e10f, UnityEngine.Random.value * 1e10f);
            for (int i = 0; i < octaves; ++i)
            {
                //Vector2 offset = new Vector2(seed, seed ^ 0xC9EFB81);
                //seed = nextRand(seed);
                Vector2 p = (point) / scale * m + offset;
                ans += (Mathf.PerlinNoise(p.x, p.y)*2f-1f) / m;
                m *= 2;
            }
            return ans;
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


        ////////////////////////// CAMERA ////////////////////////////////
        public static Vector3      EditorCameraPos()
        {
#if SEDITOR
            return UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
#else
            return Vector3.zero;
#endif // SEDITOR
        }
        public static float        HorizontalFieldOfView(this Camera camera)
        {
            Debug.Assert(camera != null);
            float angleRad = camera.fieldOfView * Mathf.Deg2Rad;
            float fovHorizRad = 2f * Mathf.Atan(Mathf.Tan(angleRad / 2f) * camera.aspect);
            return Mathf.Rad2Deg * fovHorizRad;
        }
        public static float VerticalFieldOfView(this Camera camera)
        {
            Debug.Assert(camera != null);
            return camera.fieldOfView;
        }

        public static void         Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }

}
