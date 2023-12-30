// (c) Simone Guggiari 2022-2023

using System.Collections.Generic;
using UnityEngine;
#if SNETCODE
using Unity.Netcode;
#endif

////////// PURPOSE:  //////////

namespace sxg
{

    ////////////////////////// PAIR ////////////////////////////////
    [System.Serializable]
    public class Pair<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Pair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }


    ////////////////////////// TRANSF ////////////////////////////////
    [System.Serializable]
    public struct Transf
#if SNETCODE
        : INetworkSerializable
#endif
    {
        public Vector3 position;
        public Quaternion rotation;

        public Transf(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        public Transf(Vector3 position, Vector3 euler)
            : this(position, Quaternion.Euler(euler))
        {
        }
        public Transf(Vector3 position)
            : this(position, Quaternion.identity)
        {
        }

        public Transf(Transform t)
        : this(t?.position ?? Vector3.zero, t?.rotation ?? Quaternion.identity)
        {
        }
        //public static Transf SmoothDamp(Transf current, Transf target, ref Transf currentVelocity, float smoothTime)
        //{
        //    Vector3 pos = Vector3.SmoothDamp(current.position, target.position, ref currentVelocity.position, smoothTime);
        //    Quaternion rot = Utility.SmoothDamp(current.rotation, target.rotation, ref currentVelocity.rotation, smoothTime);
        //    return new(pos, rot);
        //}
        public static Transf identity => new(Vector3.zero, Quaternion.identity);

        public static implicit operator Transf(Transform t)
        {
            return new(t);
        }
#if SNETCODE
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
        }
#endif

        public static Transf operator*(Transf a, Transf b)
        {
            return new Transf(a.position + (a.rotation * b.position), a.rotation * b.rotation);
        }
        public static Transf operator *(Transf a, Transform b)
        {
            return a * new Transf(b);
        }
        public static Transf operator *(Transform a, Transf b)
        {
            return new Transf(a) * b;
        }

        public Transf inverse
        {
            get
            {
                Quaternion rotInv = rotation.Inverse();
                return new Transf(rotInv * (-position), rotInv);
            }
        }

        public static Transf Lerp(Transf a, Transf b, float t)
        {
            return new Transf(
                Vector3.Lerp(a.position, b.position, t),
                Quaternion.Slerp(a.rotation, b.rotation, t));
        }
        public Vector3 eulerAngles
        {
            get => rotation.eulerAngles;
            set => rotation = Quaternion.Euler(value);
        }
        public Vector3 axisAngle
        {
            get => rotation.ToAxisTimesAngle();
            set => rotation = Utility.QuaternionFromAxisTimesAngle(value);
        }
    }


    ////////////////////////// AVERAGER ////////////////////////////////
    struct Averager
    {
        private float sum;
        private int count;
        public void Average(float value)
        {
            sum += value;
            ++count;
        }
        public float Value => sum / count;
    }

    struct AveragerV3
    {
        private Vector3 sum;
        private int count;
        public void Average(Vector3 value)
        {
            sum += value;
            ++count;
        }
        public Vector3 Value => sum / count;
    }


    ////////////////////////// PROPERTY ////////////////////////////////
    //[System.Serializable]
    public struct Property<T>
    {
        [SerializeField] [ReadOnly] private T val;
        public event System.Action<T> OnChange;
        public event System.Action<T, T> OnChangeBoth;

        public Property(T val)
        {
            this.val = val;
            OnChange = null;
            OnChangeBoth = null;
        }

        public void SetValueSilent(T value)
        {
            //Debug.Log($"set value silent {value}");
            val = value;
        }
        public void SetValueNotify(T value)
        {
            //Debug.Log($"set value notify {value}");
            T oldVal = val;
            val = value;
            OnChange?.Invoke(val);
            OnChangeBoth?.Invoke(oldVal, val);
        }

        public T Value
        {
            get
            {
                return val;
            }
            set
            {
                if (Equals(val, value))// val.Equals(value))
                    return;
                //Debug.Log($"set value {value}");
                T oldVal = val;
                val = value;
                OnChange?.Invoke(val);
                OnChangeBoth?.Invoke(oldVal, val);
            }
        }

        public static implicit operator T(Property<T> x)
        {
            return x.Value;
        }

        public void ForceTrigger()
        {
            OnChange?.Invoke(val);
            OnChangeBoth?.Invoke(val, val);
        }

        public void ClearEvents()
        {
            OnChange = null;
            OnChangeBoth = null;
        }
    }


    ////////////////////////// RANDOMIZED ////////////////////////////////
    [System.Serializable]
    public struct Randomized<T> 
    {
        [SerializeField] private T average;
        [SerializeField] private float variance;
        //[ReadOnly] [SerializeField] private T val;
        private T val;

        public Randomized(T average) : this(average, 0.1f) { }
        public Randomized(T average, float variance)
        {
            this.average = average;
            this.variance = variance;
            this.val = average;

            Randomize(); // this might not be called
        }

        public T Value => val;
        public T Average => average;

        public void Randomize()
        {
            var func = GenericHelper.Get<T>();
            if (func != null)
            {
                val = func.Randomize(average, variance);
            }
        }

        //public static implicit operator T(Randomized<T> x)
        //{
        //    return x.Value;
        //}
    }


    ////////////////////////// PID ////////////////////////////////
    [System.Serializable]
    public struct PID//<T>
    {
        [SerializeField] private float Kp; // proportional
        [SerializeField] private float Kd; // derivative
        [SerializeField] private float Ki; // integral
        //[SerializeField] float integralSaturation; // prevents wind-up
        //[SerializeField] Range outputRange; // clamps the output of the system
        [SerializeField] private bool isAngle;

        float errorLast, valueLast;
        bool derivativeInitialized;
        float I;

        enum DerivativeMeasurement { Velocity, ErrorRateOfChange } // Velocity removes the Derivative Kick
        static readonly DerivativeMeasurement derivativeMeasurement = DerivativeMeasurement.Velocity;

        public float Update(float currentValue, float targetValue, float dt)
        {
            if (dt <= 0f)
                return 0f;
            float error = Difference(targetValue, currentValue);

            // P (= spring)
            float P = error;

            // D (= dampener)
            float D = 0f;
            if (derivativeInitialized)
            {
                if (derivativeMeasurement == DerivativeMeasurement.Velocity)
                {
                    float valueRateOfChange = Difference(currentValue, valueLast) / dt;
                    D = -valueRateOfChange;
                }
                else
                {
                    float errorRateOfChange = Difference(error, errorLast) / dt;
                    D = errorRateOfChange;
                }
            }
            else
            {
                derivativeInitialized = true;
            }
            valueLast = currentValue;
            errorLast = error;


            // I
            I += error * dt;
            //I = Mathf.Clamp(I, -integralSaturation, integralSaturation);

            // result
            float result = Kp * P + Kd * D + Ki * I;
            //result = outputRange.Clamp(result);
            return result;
        }

        float Difference(float a, float b)
        {
            if (isAngle)
                return AngleDifference(a, b);
            else
                return a - b;
        }

        public void Reset()
        {
            derivativeInitialized = false;
        }

        float AngleDifference(float a, float b) // provides the shortest angle. input=[0,360], output=[-180,180]
        {
            if (a < 0) a += 360f;
            if (b < 0) b += 360f;
            Debug.Assert(0 <= a && a <= 360f);
            Debug.Assert(0 <= b && b <= 360f);
            return (a - b + 540f) % 360f - 180f;
        }
    }

    [System.Serializable]
    public struct PID3//<T>
    {
        [SerializeField] private float Kp; // proportional
        [SerializeField] private float Kd; // derivative
        [SerializeField] private float Ki; // integral
        //[SerializeField] float integralSaturation; // prevents wind-up
        //[SerializeField] Range outputRange; // clamps the output of the system
        [SerializeField] private bool isAngle;

        Vector3 errorLast, valueLast;
        bool derivativeInitialized;
        Vector3 I;

        enum DerivativeMeasurement { Velocity, ErrorRateOfChange } // Velocity removes the Derivative Kick
        static readonly DerivativeMeasurement derivativeMeasurement = DerivativeMeasurement.Velocity;

        public Vector3 Update(Vector3 currentValue, Vector3 targetValue, float dt)
        {
            if (dt <= 0f)
                return Vector3.zero;
            Vector3 error = Difference(targetValue, currentValue);

            // P (= spring)
            Vector3 P = error;

            // D (= dampener)
            Vector3 D = Vector3.zero;
            if (derivativeInitialized)
            {
                if (derivativeMeasurement == DerivativeMeasurement.Velocity)
                {
                    Vector3 valueRateOfChange = Difference(currentValue, valueLast) / dt;
                    D = -valueRateOfChange;
                }
                else
                {
                    Vector3 errorRateOfChange = Difference(error, errorLast) / dt;
                    D = errorRateOfChange;
                }
            }
            else
            {
                derivativeInitialized = true;
            }
            valueLast = currentValue;
            errorLast = error;


            // I
            I += error * dt;
            //I = Mathf.Clamp(I, -integralSaturation, integralSaturation);

            // result
            Vector3 result = Kp * P + Kd * D + Ki * I;
            //result = outputRange.Clamp(result);
            return result;
        }

        Vector3 Difference(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        public void Reset()
        {
            derivativeInitialized = false;
        }
    }


    ////////////////////////// RANGE ////////////////////////////////
    [System.Serializable]
    public struct Range
    {
        [SerializeField] float min, max;

        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float Random => Lerp(UnityEngine.Random.value);
        public float Lerp(float t) => Mathf.Lerp(min, max, t);
        public float Clamp(float value) => Mathf.Clamp(value, min, max);
        public bool Contains(float t)
        {
            Debug.Assert(min <= max);
            return t >= min && t <= max;
        }
        public float Min => min;
        public float Max => max;
    }


    ////////////////////////// TIMED ////////////////////////////////
    [System.Serializable]
    public struct Throttler
    {
        [SerializeField] float timeBetween;
        float nextTime;

        public Throttler(float timeBetween) : this()
        {
            this.timeBetween = timeBetween;
        }

        public bool CanGo()
        {
            if (Time.time >= nextTime)
            {
                nextTime = Time.time + timeBetween;
                return true;
            }
            return false;
        }
    }


    ////////////////////////// SMOOTH ////////////////////////////////
    [System.Serializable]
    public struct Smooth<T>
    {
        [ReadOnly] [SerializeField] private T target;
        [ReadOnly] [SerializeField] private T value; // { get; private set; }
        private T velocity;
        public float time;


        public T Value => value;

        public Smooth(T value, float time)
        {
            this.target = value;
            this.value = value;
            this.velocity = default(T);
            this.time = time;
        }
        public void SetTarget(T target)
        {
            this.target = target;
        }
        public void SetTargetForced(T target)
        {
            this.target = target;
            value = target;
            velocity = default(T);
        }
        public void SetTargetAndSmooth(T target)
        {
            this.target = target;
            DoSmooth();
        }
        public void DoSmooth()
        {
            var func = GenericHelper.Get<T>();
            if (func != null)
            {
                value = func.SmoothFunc(value, target, ref velocity, time);
            }
        }

        public static implicit operator T(Smooth<T> x)
        {
            return x.Value;
        }
    }

    public interface IGenericHelper<T>
    {
        public abstract T SmoothFunc(T value, T target, ref T velocity, float time);
        public abstract T Randomize(T average, float variability);
    }

    public static class GenericHelper
    {
        public static IGenericHelper<T> Get<T>()
        {
            if (typeof(T) == typeof(float))
                return new SmoothFunctionalityFloat() as IGenericHelper<T>;
            if (typeof(T) == typeof(Vector2))
                return new SmoothFunctionalityVector2() as IGenericHelper<T>;
            if (typeof(T) == typeof(Vector3))
                return new SmoothFunctionalityVector3() as IGenericHelper<T>;
            return null;
        }

        public static System.Random prng = new System.Random();
        public static float RandomValue => (float)prng.NextDouble();
        public static float RandomRange(float from, float to) => from + (to-from)*RandomValue;
    }

    public class SmoothFunctionalityFloat : IGenericHelper<float>
    {
        public float Randomize(float average, float variability)
        {
            return average * GenericHelper.RandomRange(1f - variability, 1f + variability);
        }
        public float SmoothFunc(float value, float target, ref float velocity, float time)
        {
            return Mathf.SmoothDamp(value, target, ref velocity, time);
        }
    }
    public class SmoothFunctionalityVector2 : IGenericHelper<Vector2>
    {
        public Vector2 Randomize(Vector2 average, float variability)
        {
            return average * GenericHelper.RandomRange(1f - variability, 1f + variability);
        }
        public Vector2 SmoothFunc(Vector2 value, Vector2 target, ref Vector2 velocity, float time)
        {
            return Vector2.SmoothDamp(value, target, ref velocity, time);
        }
    }
    public class SmoothFunctionalityVector3 : IGenericHelper<Vector3>
    {
        public Vector3 Randomize(Vector3 average, float variability)
        {
            return average * GenericHelper.RandomRange(1f - variability, 1f + variability);
        }
        public Vector3 SmoothFunc(Vector3 value, Vector3 target, ref Vector3 velocity, float time)
        {
            return Vector3.SmoothDamp(value, target, ref velocity, time);
        }
    }

}