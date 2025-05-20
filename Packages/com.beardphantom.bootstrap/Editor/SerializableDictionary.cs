using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        private List<SerializedPair> SerializedPairs { get; set; } = new();

        public void RemoveAll(Predicate<KeyValuePair<TKey, TValue>> predicate)
        {
            CopyToSerializedList();
            SerializedPairs.RemoveAll(WrappedPredicate);
            CopyFromSerializedList();
            return;

            bool WrappedPredicate(SerializedPair pair)
            {
                return predicate(pair);
            }
        }

        private void CopyToSerializedList()
        {
            SerializedPairs.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                SerializedPairs.Add(pair);
            }
        }

        private void CopyFromSerializedList()
        {
            Clear();
            foreach (SerializedPair serializedPair in SerializedPairs)
            {
                this[serializedPair.Key] = serializedPair.Value;
            }

            SerializedPairs.Clear();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CopyFromSerializedList();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            CopyToSerializedList();
        }

        [Serializable]
        private struct SerializedPair
        {
            [field: SerializeField]
            public TKey Key { get; private set; }

            [field: SerializeField]
            public TValue Value { get; private set; }

            public SerializedPair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public static implicit operator SerializedPair(KeyValuePair<TKey, TValue> pair)
            {
                return new SerializedPair(pair.Key, pair.Value);
            }

            public static implicit operator KeyValuePair<TKey, TValue>(SerializedPair pair)
            {
                return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
            }
        }
    }
}