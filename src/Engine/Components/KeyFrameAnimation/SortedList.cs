using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace Fusse.KeyFrameAnimation
{
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        private static TKey[] emptyKeys = new TKey[0];
        private static TValue[] emptyValues = new TValue[0];
        private TKey[] keys;
        private TValue[] values;
        private int _size;
        private int version;
        private IComparer<TKey> comparer;
        private SortedList<TKey, TValue>.KeyList keyList;
        private SortedList<TKey, TValue>.ValueList valueList;
        [NonSerialized]
        private object _syncRoot;
        private const int _defaultCapacity = 4;
        private const int MaxArrayLength = 2146435071;

        public int Capacity
        {
            get
            {
                return this.keys.Length;
            }
            set
            {
                if (value == this.keys.Length)
                    return;
                if (value < this._size)
                    throw new ArgumentOutOfRangeException("value");
                if (value > 0)
                {
                    TKey[] keyArray = new TKey[value];
                    TValue[] objArray = new TValue[value];
                    if (this._size > 0)
                    {
                        Array.Copy((Array)this.keys, 0, (Array)keyArray, 0, this._size);
                        Array.Copy((Array)this.values, 0, (Array)objArray, 0, this._size);
                    }
                    this.keys = keyArray;
                    this.values = objArray;
                }
                else
                {
                    this.keys = SortedList<TKey, TValue>.emptyKeys;
                    this.values = SortedList<TKey, TValue>.emptyValues;
                }
            }
        }

        public IComparer<TKey> Comparer
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.comparer;
            }
        }

        public int Count
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._size;
            }
        }

        public IList<TKey> Keys
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return (IList<TKey>)this.GetKeyListHelper();
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return (ICollection<TKey>)this.GetKeyListHelper();
            }
        }

        ICollection IDictionary.Keys
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return (ICollection)this.GetKeyListHelper();
            }
        }

        public IList<TValue> Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return (IList<TValue>)this.GetValueListHelper();
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return (ICollection<TValue>)this.GetValueListHelper();
            }
        }

        ICollection IDictionary.Values
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return (ICollection)this.GetValueListHelper();
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), (object)null);
                return this._syncRoot;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int index = this.IndexOfKey(key);
                if (index >= 0)
                    return this.values[index];
                throw new KeyNotFoundException();
                return default(TValue);
            }
            set
            {
                if ((object)key == null)
                    throw new ArgumentNullException("key");
                int index = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
                if (index >= 0)
                {
                    this.values[index] = value;
                    ++this.version;
                }
                else
                    this.Insert(~index, key, value);
            }
        }

        static SortedList()
        {
        }

        public SortedList()
        {
            this.keys = SortedList<TKey, TValue>.emptyKeys;
            this.values = SortedList<TKey, TValue>.emptyValues;
            this._size = 0;
            this.comparer = (IComparer<TKey>)Comparer<TKey>.Default;
        }

        public SortedList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            this.comparer = (IComparer<TKey>)Comparer<TKey>.Default;
        }

        public SortedList(IComparer<TKey> comparer)
            : this()
        {
            if (comparer == null)
                return;
            this.comparer = comparer;
        }

        public SortedList(int capacity, IComparer<TKey> comparer)
            : this(comparer)
        {
            this.Capacity = capacity;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public SortedList(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, (IComparer<TKey>)null)
        {
        }

        public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
            : this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            dictionary.Keys.CopyTo(this.keys, 0);
            dictionary.Values.CopyTo(this.values, 0);
            Array.Sort<TKey, TValue>(this.keys, this.values, comparer);
            this._size = dictionary.Count;
        }

        public void Add(TKey key, TValue value)
        {
            if ((object)key == null)
                throw new ArgumentNullException("key");
            int num = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
            if (num >= 0)
                throw new ArgumentException("ExceptionResource.Argument_AddingDuplicate");
            this.Insert(~num, key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = this.IndexOfKey(keyValuePair.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(this.values[index], keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = this.IndexOfKey(keyValuePair.Key);
            if (index < 0 || !EqualityComparer<TValue>.Default.Equals(this.values[index], keyValuePair.Value))
                return false;
            this.RemoveAt(index);
            return true;
        }

        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");
            try
            {
                TKey key1 = (TKey)key;
                try
                {
                    this.Add(key1, (TValue)value);
                }
                catch (InvalidCastException ex)
                {
                    throw new Exception("illegal value");
                }
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("wrong type");
            }
        }

        private SortedList<TKey, TValue>.KeyList GetKeyListHelper()
        {
            if (this.keyList == null)
                this.keyList = new SortedList<TKey, TValue>.KeyList(this);
            return this.keyList;
        }

        private SortedList<TKey, TValue>.ValueList GetValueListHelper()
        {
            if (this.valueList == null)
                this.valueList = new SortedList<TKey, TValue>.ValueList(this);
            return this.valueList;
        }

        public void Clear()
        {
            ++this.version;
            Array.Clear((Array)this.keys, 0, this._size);
            Array.Clear((Array)this.values, 0, this._size);
            this._size = 0;
        }

        bool IDictionary.Contains(object key)
        {
            if (SortedList<TKey, TValue>.IsCompatibleKey(key))
                return this.ContainsKey((TKey)key);
            else
                return false;
        }

        public bool ContainsKey(TKey key)
        {
            return this.IndexOfKey(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            return this.IndexOfValue(value) >= 0;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("ExceptionResource.Arg_ArrayPlusOffTooSmall");
            for (int index = 0; index < this.Count; ++index)
            {
                KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(this.keys[index], this.values[index]);
                array[arrayIndex + index] = keyValuePair;
            }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank != 1)
                throw new ArgumentException("ExceptionResource.Arg_RankMultiDimNotSupported");
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("ExceptionResource.Arg_NonZeroLowerBound");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("ExceptionResource.Arg_ArrayPlusOffTooSmall");
            KeyValuePair<TKey, TValue>[] keyValuePairArray = array as KeyValuePair<TKey, TValue>[];
            if (keyValuePairArray != null)
            {
                for (int index = 0; index < this.Count; ++index)
                    keyValuePairArray[index + arrayIndex] = new KeyValuePair<TKey, TValue>(this.keys[index], this.values[index]);
            }
            else
            {
                object[] objArray = array as object[];
                if (objArray == null)
                    throw new ArgumentException("InvalidArrayType");
                try
                {
                    for (int index = 0; index < this.Count; ++index)
                        objArray[index + arrayIndex] = (object)new KeyValuePair<TKey, TValue>(this.keys[index], this.values[index]);
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException("InvalidArrayType");
                }
            }
        }

        private void EnsureCapacity(int min)
        {
            int num = this.keys.Length == 0 ? 4 : this.keys.Length * 2;
            if ((uint)num > 2146435071U)
                num = 2146435071;
            if (num < min)
                num = min;
            this.Capacity = num;
        }

        private TValue GetByIndex(int index)
        {
            if (index < 0 || index >= this._size)
                throw new ArgumentOutOfRangeException("index");
            return this.values[index];
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<TKey, TValue>>)new SortedList<TKey, TValue>.Enumerator(this, 1);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<TKey, TValue>>)new SortedList<TKey, TValue>.Enumerator(this, 1);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)new SortedList<TKey, TValue>.Enumerator(this, 2);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new SortedList<TKey, TValue>.Enumerator(this, 1);
        }

        private TKey GetKey(int index)
        {
            if (index < 0 || index >= this._size)
                throw new ArgumentOutOfRangeException("index");
            return this.keys[index];
        }

        object IDictionary.this[object key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public object get_Item(object key)
        {
            if (SortedList<TKey, TValue>.IsCompatibleKey(key))
            {
                int index = this.IndexOfKey((TKey)key);
                if (index >= 0)
                    return (object)this.values[index];
            }
            return (object)null;
        }

        public void set_Item(object key, object value)
        {
            if (!SortedList<TKey, TValue>.IsCompatibleKey(key))
                throw new ArgumentNullException("ExceptionArgument.key");
            if (value == null)
                throw new ArgumentException("value");
            try
            {
                TKey index = (TKey)key;
                try
                {
                    this[index] = (TValue)value;
                }
                catch (InvalidCastException ex)
                {
                    throw new ArgumentException("value, typeof (TValue)");
                }
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("key, typeof (TKey)");
            }
        }

        public int IndexOfKey(TKey key)
        {
            if ((object)key == null)
                throw new ArgumentNullException("ExceptionArgument.key");
            int num = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
            if (num < 0)
                return -1;
            else
                return num;
        }

        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf<TValue>(this.values, value, 0, this._size);
        }

        private void Insert(int index, TKey key, TValue value)
        {
            if (this._size == this.keys.Length)
                this.EnsureCapacity(this._size + 1);
            if (index < this._size)
            {
                Array.Copy((Array)this.keys, index, (Array)this.keys, index + 1, this._size - index);
                Array.Copy((Array)this.values, index, (Array)this.values, index + 1, this._size - index);
            }
            this.keys[index] = key;
            this.values[index] = value;
            ++this._size;
            ++this.version;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = this.IndexOfKey(key);
            if (index >= 0)
            {
                value = this.values[index];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this._size)
                throw new ArgumentOutOfRangeException("ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
            --this._size;
            if (index < this._size)
            {
                Array.Copy((Array)this.keys, index + 1, (Array)this.keys, index, this._size - index);
                Array.Copy((Array)this.values, index + 1, (Array)this.values, index, this._size - index);
            }
            this.keys[this._size] = default(TKey);
            this.values[this._size] = default(TValue);
            ++this.version;
        }

        public bool Remove(TKey key)
        {
            int index = this.IndexOfKey(key);
            if (index >= 0)
                this.RemoveAt(index);
            return index >= 0;
        }

        void IDictionary.Remove(object key)
        {
            if (!SortedList<TKey, TValue>.IsCompatibleKey(key))
                return;
            this.Remove((TKey)key);
        }

        public void TrimExcess()
        {
            if (this._size >= (int)((double)this.keys.Length * 0.9))
                return;
            this.Capacity = this._size;
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
                throw new ArgumentNullException("ExceptionArgument.key");
            return key is TKey;
        }

        [Serializable]
        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private TKey key;
            private TValue value;
            private int index;
            private int version;
            private int getEnumeratorRetType;
            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    return (object)this.key;
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    return new DictionaryEntry((object)this.key, (object)this.value);
                }
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(this.key, this.value);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    if (this.getEnumeratorRetType == 2)
                        return (object)new DictionaryEntry((object)this.key, (object)this.value);
                    else
                        return (object)new KeyValuePair<TKey, TValue>(this.key, this.value);
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    return (object)this.value;
                }
            }

            internal Enumerator(SortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                this._sortedList = sortedList;
                this.index = 0;
                this.version = this._sortedList.version;
                this.getEnumeratorRetType = getEnumeratorRetType;
                this.key = default(TKey);
                this.value = default(TValue);
            }

            public void Dispose()
            {
                this.index = 0;
                this.key = default(TKey);
                this.value = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                if ((uint)this.index < (uint)this._sortedList.Count)
                {
                    this.key = this._sortedList.keys[this.index];
                    this.value = this._sortedList.values[this.index];
                    ++this.index;
                    return true;
                }
                else
                {
                    this.index = this._sortedList.Count + 1;
                    this.key = default(TKey);
                    this.value = default(TValue);
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                this.index = 0;
                this.key = default(TKey);
                this.value = default(TValue);
            }
        }

        [Serializable]
        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private int index;
            private int version;
            private TKey currentKey;

            public TKey Current
            {
                [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
                get
                {
                    return this.currentKey;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    return (object)this.currentKey;
                }
            }

            internal SortedListKeyEnumerator(SortedList<TKey, TValue> sortedList)
            {
                this._sortedList = sortedList;
                this.version = sortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentKey = default(TKey);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                if ((uint)this.index < (uint)this._sortedList.Count)
                {
                    this.currentKey = this._sortedList.keys[this.index];
                    ++this.index;
                    return true;
                }
                else
                {
                    this.index = this._sortedList.Count + 1;
                    this.currentKey = default(TKey);
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                this.index = 0;
                this.currentKey = default(TKey);
            }
        }

        [Serializable]
        private sealed class SortedListValueEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private int index;
            private int version;
            private TValue currentValue;

            public TValue Current
            {
                [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
                get
                {
                    return this.currentValue;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    return (object)this.currentValue;
                }
            }

            internal SortedListValueEnumerator(SortedList<TKey, TValue> sortedList)
            {
                this._sortedList = sortedList;
                this.version = sortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                if ((uint)this.index < (uint)this._sortedList.Count)
                {
                    this.currentValue = this._sortedList.values[this.index];
                    ++this.index;
                    return true;
                }
                else
                {
                    this.index = this._sortedList.Count + 1;
                    this.currentValue = default(TValue);
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                this.index = 0;
                this.currentValue = default(TValue);
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        private sealed class KeyList : IList<TKey>, ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
        {
            private SortedList<TKey, TValue> _dict;

            public int Count
            {
                get
                {
                    return this._dict._size;
                }
            }

            public object SyncRoot { get; private set; }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }


            public TKey this[int index]
            {
                get
                {
                    return this._dict.GetKey(index);
                }
                set
                {
                    throw new NotSupportedException("ExceptionResource.NotSupported_KeyCollectionSet");
                }
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            internal KeyList(SortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(TKey key)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }

            public void Clear()
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }

            public bool Contains(TKey key)
            {
                return this._dict.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Array.Copy((Array)this._dict.keys, 0, (Array)array, arrayIndex, this._dict.Count);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array != null)
                {
                    if (array.Rank != 1)
                        throw new ArgumentException("ExceptionResource.Arg_RankMultiDimNotSupported");
                }
                try
                {
                    Array.Copy((Array)this._dict.keys, 0, array, arrayIndex, this._dict.Count);
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException("ExceptionResource.Argument_InvalidArrayType");
                }
            }

            public void Insert(int index, TKey value)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return (IEnumerator<TKey>)new SortedList<TKey, TValue>.SortedListKeyEnumerator(this._dict);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)new SortedList<TKey, TValue>.SortedListKeyEnumerator(this._dict);
            }

            public int IndexOf(TKey key)
            {
                if ((object)key == null)
                    throw new ArgumentNullException("ExceptionArgument.key");
                int num = Array.BinarySearch<TKey>(this._dict.keys, 0, this._dict.Count, key, this._dict.comparer);
                if (num >= 0)
                    return num;
                else
                    return -1;
            }

            public bool Remove(TKey key)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
                return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        private sealed class ValueList : IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
        {
            private SortedList<TKey, TValue> _dict;

            public int Count
            {
                get
                {
                    return this._dict._size;
                }
            }

            public object SyncRoot { get; private set; }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }


            public TValue this[int index]
            {
                get
                {
                    return this._dict.GetByIndex(index);
                }
                set
                {
                    throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
                }
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            internal ValueList(SortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(TValue key)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }

            public void Clear()
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }

            public bool Contains(TValue value)
            {
                return this._dict.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy((Array)this._dict.values, 0, (Array)array, arrayIndex, this._dict.Count);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array != null)
                {
                    if (array.Rank != 1)
                        throw new ArgumentException("ExceptionResource.Arg_RankMultiDimNotSupported");
                }
                try
                {
                    Array.Copy((Array)this._dict.values, 0, array, arrayIndex, this._dict.Count);
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException("ExceptionResource.Argument_InvalidArrayType");
                }
            }

            public void Insert(int index, TValue value)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return (IEnumerator<TValue>)new SortedList<TKey, TValue>.SortedListValueEnumerator(this._dict);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)new SortedList<TKey, TValue>.SortedListValueEnumerator(this._dict);
            }

            public int IndexOf(TValue value)
            {
                return Array.IndexOf<TValue>(this._dict.values, value, 0, this._dict.Count);
            }

            public bool Remove(TValue value)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
                return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("ExceptionResource.NotSupported_SortedListNestedWrite");
            }
        }
    }
}
