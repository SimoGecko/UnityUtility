// (c) Simone Guggiari 2022

using System.Collections.Generic;
using UnityEngine;

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
    public struct Transf
    {
        public Vector3 pos;
        public Quaternion rot;

        public Transf(Vector3 pos, Quaternion rot)
        {
            this.pos = pos;
            this.rot = rot;
        }
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
        [SerializeField] private float proportionalGain; // proportional
        [SerializeField] private float integralGain; // integral
        [SerializeField] private float derivativeGain; // derivative
        //[SerializeField] float integralSaturation;
        //[SerializeField] Range outputRange;

        float errorLast;
        float valueLast;
        bool derivativeInitialized;
        float integralStored;

        enum DerivativeMeasurement { ErrorRateOfChange, Velocity }
        DerivativeMeasurement derivativeMeasurement;

        public float Update(float currentValue, float targetValue, float dt)
        {
            // P (= spring)
            float error = targetValue - currentValue;
            float P = proportionalGain * error;

            // D (= dampener)
            float errorRateOfChange = (error - errorLast) / dt;
            errorLast = error;
            float valueRateOfChange = (currentValue - valueLast) / dt;
            valueLast = currentValue;
            float deriveMeasure = 0f;

            if (derivativeInitialized)
            {
                if (derivativeMeasurement == DerivativeMeasurement.Velocity)
                    deriveMeasure = -valueRateOfChange;
                else
                    deriveMeasure = errorRateOfChange;
            }
            else
            {
                derivativeInitialized = true;
            }

            float D = derivativeGain * deriveMeasure;

            // I
            integralStored += error * dt;
            //integralStored = Mathf.Clamp(integralStored, -integralSaturation, integralSaturation);
            float I = integralGain * integralStored;


            // result
            float result = P + I + D;
            //result = outputRange.Clamp(result);
            return result;
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