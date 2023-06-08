using System;
using UnityEngine;

namespace Helpers
{
    [System.Serializable]
    public class SerializableKeyValPair<K, V>
    {
        public SerializableKeyValPair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }
    
        public K key;
        public V value;
    }
}