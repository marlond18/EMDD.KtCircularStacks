using KtExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CircularStack
{
    /// <summary>
    /// Main Object of the Circular Stack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularStack<T> : IEquatable<CircularStack<T>>, IEnumerable<CircularStackElement<T>>, ICloneable
    {
        private readonly List<CircularStackElement<T>> _data;

        /// <summary>
        /// get the numbers of Elements
        /// </summary>
        public int Count => _data.Count;

        /// <summary>
        /// Create empty Stack
        /// </summary>
        public CircularStack()
        {
            _data = new List<CircularStackElement<T>>();
        }

        /// <summary>
        /// Create Stack from enumerable
        /// </summary>
        /// <param name="inenumerable"></param>
        public CircularStack(IEnumerable<T> inenumerable)
        {
            _data = new List<CircularStackElement<T>>();
            foreach (var item in inenumerable) Add(item);
        }

        /// <summary>
        /// Get the item by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CircularStackElement<T> this[int index] => _data[index > (Count - 1) ? Count - 1 - index : index];

        /// <summary>
        /// Add to the stack
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            if (value.IsNull()) throw new ArgumentNullException($"Can't add a null {typeof(T)} to a Circular stack");
            var newData = new CircularStackElement<T>(value) { Parent = this };
            if (_data.Count > 0)
            {
                AssignPreviousAndNext(newData, _data[0]);
                AssignPreviousAndNext(_data[^1], newData);
            }
            else
            {
                AssignPreviousAndNext(newData, newData);
            }
            newData.Index = _data.Count;
            _data.Add(newData);
        }

        /// <summary>
        /// Find all items matching the fucntion
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<CircularStackElement<T>> FindAll(Func<CircularStackElement<T>, bool> condition)
        {
            if (condition == null) yield break;
            foreach (var datum in _data) if (condition(datum)) yield return datum;
        }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CircularStack<T> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (this is null || other is null) return false;
            if (Count != other.Count) return false;
            return _data.All(other._data.Contains);
        }

        /// <summary>
        /// Equality to object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (this is null || obj is null) return false;
            if (GetType() != obj.GetType()) return false;
            return Equals((CircularStack<T>)obj);
        }

        /// <summary>
        /// get hash by returning the enumerable hash of the data
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return _data.GetHashCodeOfEnumerable();
            }
        }

        /// <summary>
        /// return enumerator implicit from Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CircularStackElement<T>> GetEnumerator() => _data.GetEnumerator();

        /// <summary>
        /// Reverse stack
        /// </summary>
        /// <returns></returns>
        public CircularStack<T> Reverse()
        {
            var newstack = new CircularStack<T>();
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                newstack.Add(_data[i].Value);
            }
            return newstack;
        }

        /// <summary>
        /// Remove the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default)) return false;
            if (_data.Count < 1) return false;
            var index = _data.FindIndex(d => Equals(d._value, value));
            return RemoveAt(index);
        }

        internal void InsertAt(T value, int index)
        {
            if (value.IsNull()) return;
            if (index > _data.Count - 2)
            {
                Add(value);
                return;
            }
            var newData = new CircularStackElement<T>(value);
            var current = _data[index];
            AssignPreviousAndNext(current.Previous, newData);
            AssignPreviousAndNext(newData, current);
            ModifyIndicesUntilEnd(index + 1, 1);
            _data.Insert(index, newData);
        }

        /// <summary>
        /// Make Stack empty
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Remove Item at the index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveAt(int index)
        {
            if (index > _data.Count - 1 || index < 0) return false;
            var datum = _data[index];
            if (datum == null) return false;
            if (_data.Count > 1) AssignPreviousAndNext(datum.Previous, datum.Next);
            datum.Next = null;
            datum.Previous = null;
            ModifyIndicesUntilEnd(index + 1, -1);
            _data.Remove(datum);
            return true;
        }

        internal static void AssignPreviousAndNext(CircularStackElement<T> previous, CircularStackElement<T> next)
        {
            previous.Next = next;
            next.Previous = previous;
        }

        internal void ModifyIndicesUntilEnd(int startIndex, int Modifier)
        {
            if (startIndex > _data.Count - 1) return;
            for (int i = startIndex; i < _data.Count; i++)
            {
                _data[i].Index += Modifier;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Implicit Cast to List
        /// </summary>
        /// <param name="ienumerable"></param>
        public static implicit operator CircularStack<T>(List<T> ienumerable) => new(ienumerable);

        /// <summary>
        /// Implicit Cast to Array
        /// </summary>
        /// <param name="ienumerable"></param>
        public static implicit operator CircularStack<T>(T[] ienumerable) => new(ienumerable);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="ienumerable"></param>
        /// <returns></returns>
        public static CircularStack<T> Create(IEnumerable<T> ienumerable) => new(ienumerable);

        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _data.Select(datum => datum.Value.ToString()).BuildString(", ");

        /// <summary>
        /// element cloning
        /// </summary>
        /// <returns></returns>
        public object Clone() => new CircularStack<T>(_data.Select(datum => datum.Value));
    }
}