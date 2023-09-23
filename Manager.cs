// (c) Simone Guggiari 2023

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

////////// PURPOSE: Provides the Singleton pattern //////////

namespace sxg
{
    public class Manager<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<T>();
                return instance;
            }
        }

        public virtual void OnDestroy()
        {
            instance = null;
        }
    }

    public class NetworkManager<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<T>();
                return instance;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }
    }

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