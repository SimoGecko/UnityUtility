// (c) Simone Guggiari 2024

using System;
using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE: A Dictionary<K,V> that is serializable. You need to specialize it with the actual types. //////////

namespace sxg.serializabledictionary
{
    // from https://discussions.unity.com/t/solved-how-to-serialize-dictionary-with-unity-serialization-system/71474

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new();

        [SerializeField]
        private List<TValue> values = new();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            keys ??= new();
            values ??= new();

            if (keys.Count != values.Count)
                throw new Exception($"Size mismatch in SerializableDictionary after deserialization: #keys={keys.Count} #values={values.Count}. " +
                    $"Make sure that both key and value types are serializable.");

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }

    [Serializable] public class Dictionary_string_int : SerializableDictionary<string, int> { }
    [Serializable] public class Dictionary_string_object : SerializableDictionary<string, object> { }
}