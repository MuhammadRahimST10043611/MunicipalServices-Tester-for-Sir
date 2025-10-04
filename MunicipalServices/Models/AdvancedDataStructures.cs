#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;

namespace MunicipalServices.Models
{
    // Priority Queue Implementation for Event Management
    public class CustomPriorityQueue<T> where T : class
    {
        private CustomLinkedList<PriorityItem<T>> _items;

        public CustomPriorityQueue()
        {
            _items = new CustomLinkedList<PriorityItem<T>>();
        }

        public int Count => _items.Count;

        public void Enqueue(T item, int priority)
        {
            var newItem = new PriorityItem<T>(item, priority);

            if (_items.Count == 0)
            {
                _items.Add(newItem);
                return;
            }

            // Insert based on priority (higher priority first)
            var tempList = new CustomLinkedList<PriorityItem<T>>();
            bool inserted = false;

            foreach (var existingItem in _items)
            {
                if (!inserted && priority > existingItem.Priority)
                {
                    tempList.Add(newItem);
                    inserted = true;
                }
                tempList.Add(existingItem);
            }

            if (!inserted)
            {
                tempList.Add(newItem);
            }

            _items = tempList;
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            var first = _items[0];
            var newList = new CustomLinkedList<PriorityItem<T>>();

            for (int i = 1; i < _items.Count; i++)
            {
                newList.Add(_items[i]);
            }

            _items = newList;
            return first.Item;
        }

        public T Peek()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            return _items[0].Item;
        }

        public bool Any()
        {
            return _items.Count > 0;
        }
    }

    public class PriorityItem<T>
    {
        public T Item { get; set; }
        public int Priority { get; set; }

        public PriorityItem(T item, int priority)
        {
            Item = item;
            Priority = priority;
        }
    }

    // Custom Stack Implementation
    public class CustomStack<T>
    {
        private CustomLinkedList<T> _items;

        public CustomStack()
        {
            _items = new CustomLinkedList<T>();
        }

        public int Count => _items.Count;

        public void Push(T item)
        {
            var newList = new CustomLinkedList<T>();
            newList.Add(item);

            foreach (var existingItem in _items)
            {
                newList.Add(existingItem);
            }

            _items = newList;
        }

        public T Pop()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Stack is empty");

            var top = _items[0];
            var newList = new CustomLinkedList<T>();

            for (int i = 1; i < _items.Count; i++)
            {
                newList.Add(_items[i]);
            }

            _items = newList;
            return top;
        }

        public T Peek()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Stack is empty");

            return _items[0];
        }

        public bool Any()
        {
            return _items.Count > 0;
        }

        public CustomLinkedList<T> ToList()
        {
            return _items;
        }
    }

    // Custom Queue Implementation
    public class CustomQueue<T>
    {
        private CustomLinkedList<T> _items;

        public CustomQueue()
        {
            _items = new CustomLinkedList<T>();
        }

        public int Count => _items.Count;

        public void Enqueue(T item)
        {
            _items.Add(item);
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            var first = _items[0];
            var newList = new CustomLinkedList<T>();

            for (int i = 1; i < _items.Count; i++)
            {
                newList.Add(_items[i]);
            }

            _items = newList;
            return first;
        }

        public T Peek()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            return _items[0];
        }

        public bool Any()
        {
            return _items.Count > 0;
        }
    }

    // Custom Set Implementation
    public class CustomSet<T> : IEnumerable<T>
    {
        private CustomLinkedList<T> _items;

        public CustomSet()
        {
            _items = new CustomLinkedList<T>();
        }

        public int Count => _items.Count;

        public bool Add(T item)
        {
            if (Contains(item))
                return false;

            _items.Add(item);
            return true;
        }

        public bool Contains(T item)
        {
            foreach (var existingItem in _items)
            {
                if (existingItem != null && existingItem.Equals(item))
                    return true;
            }
            return false;
        }

        public bool Remove(T item)
        {
            var newList = new CustomLinkedList<T>();
            bool removed = false;

            foreach (var existingItem in _items)
            {
                if (!removed && existingItem != null && existingItem.Equals(item))
                {
                    removed = true;
                    continue;
                }
                newList.Add(existingItem);
            }

            _items = newList;
            return removed;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public CustomLinkedList<T> ToCustomList()
        {
            return _items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // Custom Hash Table Implementation
    public class CustomHashTable<TKey, TValue> where TKey : notnull
    {
        private const int InitialSize = 16;
        private CustomLinkedList<KeyValuePair<TKey, TValue>>[] _buckets;
        private int _size;
        private int _count;

        public CustomHashTable()
        {
            _size = InitialSize;
            _buckets = new CustomLinkedList<KeyValuePair<TKey, TValue>>[_size];
            _count = 0;

            for (int i = 0; i < _size; i++)
            {
                _buckets[i] = new CustomLinkedList<KeyValuePair<TKey, TValue>>();
            }
        }

        public int Count => _count;

        private int GetBucketIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % _size;
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int bucketIndex = GetBucketIndex(key);
            var bucket = _buckets[bucketIndex];

            // Check if key already exists
            foreach (var pair in bucket)
            {
                if (pair.Key.Equals(key))
                {
                    throw new ArgumentException("Key already exists");
                }
            }

            bucket.Add(new KeyValuePair<TKey, TValue>(key, value));
            _count++;

            // Resize if load factor is too high
            if (_count > _size * 0.75)
            {
                Resize();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                value = default(TValue);
                return false;
            }

            int bucketIndex = GetBucketIndex(key);
            var bucket = _buckets[bucketIndex];

            foreach (var pair in bucket)
            {
                if (pair.Key.Equals(key))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                return false;

            int bucketIndex = GetBucketIndex(key);
            var bucket = _buckets[bucketIndex];
            var newBucket = new CustomLinkedList<KeyValuePair<TKey, TValue>>();
            bool removed = false;

            foreach (var pair in bucket)
            {
                if (!removed && pair.Key.Equals(key))
                {
                    removed = true;
                    _count--;
                    continue;
                }
                newBucket.Add(pair);
            }

            _buckets[bucketIndex] = newBucket;
            return removed;
        }

        private void Resize()
        {
            var oldBuckets = _buckets;
            _size *= 2;
            _buckets = new CustomLinkedList<KeyValuePair<TKey, TValue>>[_size];
            _count = 0;

            for (int i = 0; i < _size; i++)
            {
                _buckets[i] = new CustomLinkedList<KeyValuePair<TKey, TValue>>();
            }

            // Rehash all items
            foreach (var bucket in oldBuckets)
            {
                foreach (var pair in bucket)
                {
                    Add(pair.Key, pair.Value);
                }
            }
        }

        public CustomLinkedList<TKey> GetKeys()
        {
            var keys = new CustomLinkedList<TKey>();
            foreach (var bucket in _buckets)
            {
                foreach (var pair in bucket)
                {
                    keys.Add(pair.Key);
                }
            }
            return keys;
        }

        public CustomLinkedList<TValue> GetValues()
        {
            var values = new CustomLinkedList<TValue>();
            foreach (var bucket in _buckets)
            {
                foreach (var pair in bucket)
                {
                    values.Add(pair.Value);
                }
            }
            return values;
        }
    }
}