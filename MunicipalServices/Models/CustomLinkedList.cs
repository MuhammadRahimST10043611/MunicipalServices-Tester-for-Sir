#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;

namespace MunicipalServices.Models
{
    // Interface that is being used for identifiable objects
    public interface IIdentifiable
    {
        int Id { get; set; }
    }

    // Custom Linked List Node for datastructure
    public class LinkedListNode<T>
    {
        public T Data { get; set; }
        public LinkedListNode<T> Next { get; set; }

        public LinkedListNode(T data)
        {
            Data = data;
            Next = null;
        }
    }

    // Single Generic Custom Linked List Implementation
    public class CustomLinkedList<T> : IEnumerable<T>
    {
        private LinkedListNode<T> head;
        private LinkedListNode<T> tail;
        private int count;

        public int Count => count;

        public void Add(T item)
        {
            var newNode = new LinkedListNode<T>(item);

            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                if (tail != null)
                {
                    tail.Next = newNode;
                    tail = newNode;
                }
            }
            count++;
        }

        public bool Contains(T item)
        {
            var current = head;
            while (current != null)
            {
                if (current.Data != null && current.Data.Equals(item))
                    return true;
                current = current.Next;
            }
            return false;
        }

        public bool Any()
        {
            return count > 0;
        }

        // Custom method was used to get items without using built-in data collections
        public void CopyTo(T[] destinationArray, int startIndex = 0)
        {
            if (destinationArray == null)
                throw new ArgumentNullException(nameof(destinationArray));

            if (startIndex < 0 || startIndex + count > destinationArray.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count == 0) return; // Handle empty collection

            var current = head;
            int index = startIndex;

            while (current != null && index < destinationArray.Length)
            {
                destinationArray[index] = current.Data;
                current = current.Next;
                index++;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException();

                var current = head;
                for (int i = 0; i < index; i++)
                {
                    current = current.Next;
                }

                return current.Data;
            }
        }

        public T GetById(int id)
        {
            var current = head;
            while (current != null)
            {
                if (current.Data is IIdentifiable identifiable && identifiable.Id == id)
                    return current.Data;
                current = current.Next;
            }
            return default(T);
        }

        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public T FirstOrDefault()
        {
            return head != null ? head.Data : default(T);
        }

        public T LastOrDefault()
        {
            return tail != null ? tail.Data : default(T);
        }

        public CustomLinkedList<T> TakeLast(int count)
        {
            var result = new CustomLinkedList<T>();

            if (count <= 0 || this.count == 0)
                return result;

            int startIndex = Math.Max(0, this.count - count);
            var current = head;

            for (int i = 0; i < startIndex; i++)
            {
                if (current != null)
                    current = current.Next;
            }

            while (current != null)
            {
                result.Add(current.Data);
                current = current.Next;
            }

            return result;
        }

        public CustomLinkedList<T> Skip(int count)
        {
            var result = new CustomLinkedList<T>();
            var current = head;
            int skipped = 0;

            while (current != null)
            {
                if (skipped >= count)
                {
                    result.Add(current.Data);
                }
                current = current.Next;
                skipped++;
            }

            return result;
        }

        public CustomLinkedList<T> Take(int count)
        {
            var result = new CustomLinkedList<T>();
            var current = head;
            int taken = 0;

            while (current != null && taken < count)
            {
                result.Add(current.Data);
                current = current.Next;
                taken++;
            }

            return result;
        }

        // Helper method to convert to List
        public List<T> ToList()
        {
            var list = new List<T>();
            var current = head;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            return list;
        }
    }

    // NEW: Custom Array-like structure to replace built-in arrays
    public class CustomArray<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _size;

        public CustomArray(int capacity = 10)
        {
            _items = new T[capacity];
            _size = 0;
        }

        public int Count => _size;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException();
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException();
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_size >= _items.Length)
            {
                Resize();
            }
            _items[_size++] = item;
        }

        private void Resize()
        {
            var newArray = new T[_items.Length * 2];
            for (int i = 0; i < _size; i++)
            {
                newArray[i] = _items[i];
            }
            _items = newArray;
        }

        public void CopyFrom(CustomLinkedList<T> source)
        {
            _size = 0;
            foreach (var item in source)
            {
                Add(item);
            }
        }

        public CustomLinkedList<T> ToCustomLinkedList()
        {
            var list = new CustomLinkedList<T>();
            for (int i = 0; i < _size; i++)
            {
                list.Add(_items[i]);
            }
            return list;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _size; i++)
            {
                if (_items[i] != null && _items[i].Equals(item))
                    return true;
            }
            return false;
        }

        // ADD: IEnumerable implementation to support foreach
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // NEW: Custom Collection interface to replace ICollection<T>
    public interface ICustomCollection<T>
    {
        int Count { get; }
        void Add(T item);
        bool Contains(T item);
        void Clear();
        CustomLinkedList<T> ToCustomLinkedList();
    }

    // NEW: Custom Collection implementation
    public class CustomCollection<T> : ICustomCollection<T>, IEnumerable<T>
    {
        private readonly CustomLinkedList<T> _items;

        public CustomCollection()
        {
            _items = new CustomLinkedList<T>();
        }

        public int Count => _items.Count;

        public void Add(T item)
        {
            _items.Add(item);
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public CustomLinkedList<T> ToCustomLinkedList()
        {
            return _items;
        }

        public T this[int index] => _items[index];

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Any()
        {
            return _items.Any();
        }
    }

    public class SimpleKeyValuePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public SimpleKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class CustomDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly CustomLinkedList<SimpleKeyValuePair<TKey, TValue>> _items;

        public CustomDictionary()
        {
            _items = new CustomLinkedList<SimpleKeyValuePair<TKey, TValue>>();
        }

        public int Count => _items.Count;

        public void Add(TKey key, TValue value)
        {
            foreach (var item in _items)
            {
                if (item.Key.Equals(key))
                {
                    item.Value = value;
                    return;
                }
            }

            _items.Add(new SimpleKeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var item in _items)
            {
                if (item.Key.Equals(key))
                    return true;
            }
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                foreach (var item in _items)
                {
                    if (item.Key.Equals(key))
                        return item.Value;
                }
                throw new KeyNotFoundException($"Key '{key}' not found");
            }
            set
            {
                Add(key, value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var item in _items)
            {
                if (item.Key.Equals(key))
                {
                    value = item.Value;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        public CustomLinkedList<TKey> GetKeys()
        {
            var keys = new CustomLinkedList<TKey>();
            foreach (var item in _items)
            {
                keys.Add(item.Key);
            }
            return keys;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}