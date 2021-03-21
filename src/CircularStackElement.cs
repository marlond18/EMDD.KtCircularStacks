using KtExtensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CircularStack
{
    /// <summary>
    /// Stack Element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularStackElement<T> : IEquatable<CircularStackElement<T>>, ICloneable, IEnumerable<T>
    {
        internal readonly T _value;
        /// <summary>
        /// Stack host
        /// </summary>
        public CircularStack<T> Parent { get; internal set; }

        /// <summary>
        /// Create the element
        /// </summary>
        /// <param name="value"></param>
        public CircularStackElement(T value)
        {
            if (value.IsNull()) throw new ArgumentNullException(nameof(value), "Cannot create a null circular element");
            _value = value is ICloneable clonable ? (T)(clonable.Clone()) : value;
        }

        /// <summary>
        /// Inner value
        /// </summary>
        public T Value => _value;

        /// <summary>
        /// Equality to an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;
            return GetType() == obj.GetType() && Equals((CircularStackElement<T>)obj);
        }

        /// <summary>
        /// Return the Tostring() of the Element
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _value.ToString();

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CircularStackElement<T> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null || this is null) return false;
            return EqualityComparer<T>.Default.Equals(_value, other._value)
                && EqualityComparer<T>.Default.Equals(Next._value, other.Next._value)
                && EqualityComparer<T>.Default.Equals(Previous._value, other.Previous._value);
        }

        /// <summary>
        /// return the hash of the next current, and previous
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => _value.GetHashCode().ChainHashCode(Next).ChainHashCode(Previous);

        /// <summary>
        /// CLone object
        /// </summary>
        /// <returns></returns>
        public object Clone() => new CircularStackElement<T>(_value);

        /// <summary>
        /// Next element
        /// </summary>
        public CircularStackElement<T> Next { get; internal set; }

        /// <summary>
        /// Previous Element
        /// </summary>
        public CircularStackElement<T> Previous { get; internal set; }

        /// <summary>
        /// Index on the Host Stack
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Go to next element
        /// </summary>
        /// <param name="todo"></param>
        public void LoopForward(Action<CircularStackElement<T>> todo)
        {
            if (Parent == null || todo == null) return;
            var current = GetCurrent(todo, this);
            while (!ReferenceEquals(this, current))
            {
                current = GetCurrent(todo, current);
            }
        }

        private static CircularStackElement<T> GetCurrent(Action<CircularStackElement<T>> todo, CircularStackElement<T> current)
        {
            todo.Invoke(current);
            return current.Next;
        }

        /// <summary>
        /// Attach a next element
        /// </summary>
        /// <param name="value"></param>
        public void Append(T value)
        {
            if (Parent == null) return;
            Parent.InsertAt(value, Index + 1);
        }

        /// <summary>
        /// Prepend an element
        /// </summary>
        /// <param name="value"></param>
        public void Prepend(T value)
        {
            if (Parent == null) return;
            Parent.InsertAt(value, Index);
        }

        /// <summary>
        /// Base Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = this;
            yield return current;
            current = current.Next;
            while (!ReferenceEquals(this, current))
            {
                yield return current;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Cast to Stack Element
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator T(CircularStackElement<T> element) => element.Value;
    }
}