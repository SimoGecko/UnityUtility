// (c) Simone Guggiari 2022

using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE:  //////////

namespace sxg
{

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


    //[System.Serializable]
    public struct Property<T>
    {
        T val;
        public event System.Action<T, T> OnChange; // TODO: add 

        public Property(T val)
        {
            this.val = val;
            OnChange = null;
        }

        public void SetValueSilent(T value)
        {
            val = value;
        }

        public T Value
        {
            get
            {
                return val;
            }
            set
            {
                if (val.Equals(value))
                    return;
                T oldVal = val;
                val = value;
                OnChange?.Invoke(oldVal, val);
            }
        }

        public void ForceTrigger() { OnChange?.Invoke(val, val); }
    }

    //[System.Serializable]
    //public struct VariableValue<T> 
    //{
    //    T average;
    //    T val;
    //
    //    public VariableValue(T average, float variability)
    //    {
    //        this.average = average;
    //        this.val = average;
    //        this.val = Add<T>(val, val);
    //        //this.val = average * Random.Range(1f - variability, 1f + variability);
    //    }
    //
    //    public T Value { get { return val; } }
    //}


    [System.Serializable]
    public struct Smooth<T>
    {
        [ReadOnly] [SerializeField] private T target;
        [ReadOnly] public T value; // { get; private set; }
        private T velocity;
        public float time;

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
            var func = GetFunc();
            if (func != null)
            {
                value = func.SmoothFunc(value, target, ref velocity, time);
            }
        }
        private ISmoothFunctionality<T> GetFunc()
        {
            if (typeof(T) == typeof(float))
                return new SmoothFunctionalityFloat() as ISmoothFunctionality<T>;
            if (typeof(T) == typeof(Vector2))
                return new SmoothFunctionalityVector2() as ISmoothFunctionality<T>;
            if (typeof(T) == typeof(Vector3))
                return new SmoothFunctionalityVector3() as ISmoothFunctionality<T>;
            return null;
        }
    }

    public interface ISmoothFunctionality<T>
    {
        public abstract T SmoothFunc(T value, T target, ref T velocity, float time);
    }
    public class SmoothFunctionalityFloat : ISmoothFunctionality<float>
    {
        public float SmoothFunc(float value, float target, ref float velocity, float time)
        {
            return Mathf.SmoothDamp(value, target, ref velocity, time);
        }
    }
    public class SmoothFunctionalityVector2 : ISmoothFunctionality<Vector2>
    {
        public Vector2 SmoothFunc(Vector2 value, Vector2 target, ref Vector2 velocity, float time)
        {
            return Vector2.SmoothDamp(value, target, ref velocity, time);
        }
    }
    public class SmoothFunctionalityVector3 : ISmoothFunctionality<Vector3>
    {
        public Vector3 SmoothFunc(Vector3 value, Vector3 target, ref Vector3 velocity, float time)
        {
            return Vector3.SmoothDamp(value, target, ref velocity, time);
        }
    }

    /*
    [System.Serializable]
    public struct floatS
    {
        [ReadOnly] [SerializeField] private float target;
        [ReadOnly] public float value;// { get; private set; }
        private float velocity;
        public float time;

        public floatS(float f, float t) { target = f; value = f; velocity = 0f; time = t; }
        public void SetTarget(float f) { target = f; }
        public void SetTargetForced(float f) { target = f; value = f; velocity = 0f; }
        public void SetTargetAndSmooth(float f) { target = f; Smooth(); }
        public void Smooth() { value = Mathf.SmoothDamp(value, target, ref velocity, time); }
    }

    [System.Serializable]
    public struct Vector2S
    {
        [ReadOnly] [SerializeField] private Vector2 target;
        [ReadOnly] public Vector2 value;
        private Vector2 velocity;
        public float time;

        public Vector2S(Vector2 v, float t) { target = v; value = v; velocity = Vector2.zero; time = t; }
        public void SetTarget(Vector2 v) { target = v; }
        public void SetTargetForced(Vector2 v) { target = v; value = v; velocity = Vector2.zero; }
        public void SetTargetAndSmooth(Vector2 v) { target = v; Smooth(); }
        public void Smooth() { value = Vector2.SmoothDamp(value, target, ref velocity, time); }
    }

    [System.Serializable]
    public struct Vector3S
    {
        [ReadOnly] [SerializeField] private Vector3 target;
        [ReadOnly] public Vector3 value;
        private Vector3 velocity;
        public float time;

        public Vector3S(Vector3 v, float t) { target = v; value = v; velocity = Vector3.zero; time = t; }
        public void SetTarget(Vector3 v) { target = v; }
        public void SetTargetForced(Vector3 v) { target = v; value = v; velocity = Vector3.zero; }
        public void SetTargetAndSmooth(Vector3 v) { target = v; Smooth(); }
        public void Smooth() { value = Vector3.SmoothDamp(value, target, ref velocity, time); }
    }

    [System.Serializable]
    public struct Vector3SI
    {
        [ReadOnly] [SerializeField] private Vector3 target;
        [ReadOnly] public Vector3 value;
        private Vector3 velocity;
        public Vector3 time;

        public Vector3SI(Vector3 v, Vector3 t) { target = v; value = v; velocity = Vector3.zero; time = t; }
        public void SetTarget(Vector3 v) { target = v; }
        public void SetTargetForced(Vector3 v) { target = v; value = v; velocity = Vector3.zero; }
        public void SetTargetAndSmooth(Vector3 v) { target = v; Smooth(); }
        public void Smooth()
        {
            value = new Vector3(
                Mathf.SmoothDamp(value.x, target.x, ref velocity.x, time.x),
                Mathf.SmoothDamp(value.y, target.y, ref velocity.y, time.y),
                Mathf.SmoothDamp(value.z, target.z, ref velocity.z, time.z));
        }
    }
    */

}