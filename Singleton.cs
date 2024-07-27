// (c) Simone Guggiari 2020-2024

using System;
using UnityEngine;
#if SNETCODE
using Unity.Netcode;
#endif

////////// PURPOSE: Provides the Singleton pattern //////////

namespace sxg
{
    [Obsolete("Manager<T> is obsolete. Use Singleton<T> instead.", false)]
    public class Manager<T> : Singleton<T> where T : MonoBehaviour { }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
#if UNITY_6000_0_OR_NEWER
                    instance = FindAnyObjectByType<T>();
                //Debug.Assert(FindObjectsByType<T>(FindObjectsSortMode.None).Length <= 1, $"Singleton: multiple instances of {nameof(T)}");
#else
                    instance = FindObjectOfType<T>();
                //Debug.Assert(FindObjectsOfType<T>().Length <= 1, $"Singleton: multiple instances of {nameof(T)}");
#endif
                return instance;
            }
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }

    public class SingletonTest
    {
        void CheckSingletonInstance<T>() where T : MonoBehaviour
        {
#if UNITY_6000_0_OR_NEWER
            bool atMostOne = GameObject.FindObjectsByType<Singleton<T>>(FindObjectsSortMode.None).Length <= 1;
#else
            bool atMostOne = GameObject.FindObjectsOfType<Singleton<T>>().Length <= 1;
#endif
            if (!atMostOne)
                Debug.LogWarning($"Invalid number of Managers: {typeof(T).FullName}");
        }
    }

#if SNETCODE
    [Obsolete("NetworkedManager<T> is obsolete. Use NetworkedSingleton<T> instead.", false)]
    public class NetworkedManager<T> : NetworkedSingleton<T> where T : NetworkBehaviour { }

    // TODO: Merge with Singleton<T>?
    public class NetworkedSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<T>();
                return instance;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }
    }
#endif

    /*
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
}