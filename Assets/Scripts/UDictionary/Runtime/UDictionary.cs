using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace NKStudio
{
    [Serializable]
    public class UDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<KvP> list = new List<KvP>();
        
        public UDictionary() { }
        public UDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public UDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        
        #if UNITY_2021_1_OR_NEWER
        public UDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }
        public UDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : base(collection, comparer) { }
        #endif
        public UDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public UDictionary(int capacity) : base(capacity) { }
        public UDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        protected UDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            var duplicates = list
                .Select((kvp, i) => (index: i, kvp))
                .Where(t => t.kvp.duplicatedKey).ToArray();
#endif
            var newPairs = this.Select(kvp => new KvP(kvp.Key, kvp.Value));
            list.Clear();
            list.AddRange(newPairs);
#if UNITY_EDITOR
            foreach (var (index, kvp) in duplicates)
                list.Insert(index, kvp);
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            foreach (var kvp in list)
            {
                var key = kvp.key;
                var canAddKey = key != null && !ContainsKey(key);
                if (canAddKey)
                    Add(key, kvp.value);

                kvp.duplicatedKey = !canAddKey;
            }
#if !UNITY_EDITOR
            list.Clear();
#endif
        }

        [Serializable]
        internal class KvP
        {
            public TKey key;
            public TValue value;

            [SerializeField, HideInInspector]
            internal bool duplicatedKey;

            public KvP(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }
}