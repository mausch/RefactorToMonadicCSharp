using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefactorToMonadicCSharp
{
    /// <summary>
    /// Represents a possibly null value.
    /// Copyright Sandro Magi (http://higherlogics.blogspot.com/)
    /// From the Sasa project (http://sourceforge.net/projects/sasa/), licensed under the LGPL
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <remarks>
    /// When it comes to high assurance code, you should utilize Option and NonNull types for
    /// method arguments, to declare which arguments may be null and which must necessarily be
    /// non-null. The type checker will ensure that values are handled properly within the method,
    /// and client code will receive the errors when passing in null references for NonNull values.
    /// </remarks>
    [Serializable]
    public struct Option<T> : IEquatable<T>, IEquatable<Option<T>>
    {
        /// <summary>
        /// The wrapped value.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Returns true if there is a value.
        /// </summary>
        /// <returns>True if not null.</returns>
        public bool HasValue { get; private set; }

        /// <summary>
        /// An empty option value.
        /// </summary>
        public static Option<T> None
        {
            get { return new Option<T>(); }
        }

        /// <summary>
        /// Construct an optional value.
        /// </summary>
        /// <param name="value">The wrapped value.</param>
        public Option(T value)
            : this()
        {
            this.Value = value;
            // Check value is a struct AND T is a struct type, since this could be an
            // an int being assigned to an Option<object>, in which case we should perform
            // the null test, not just check that it's a value type.
            this.HasValue = value is ValueType //&& typeof(T).Subtypes<ValueType>()
                            || value != null;
        }

        public static Option<T> Some(T value)
        {
            var o = new Option<T>(value);
            o.HasValue = true;
            return o;
        }

        /// <summary>
        /// Compares Option&lt;T&gt; and a T for equality.
        /// </summary>
        /// <param name="other">The other object to compare to.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other);
        }

        /// <summary>
        /// Compares two Option&lt;T&gt; instances for equality.
        /// </summary>
        /// <param name="other">The other object to compare to.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Option<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Compares two objects for equality.
        /// </summary>
        /// <param name="obj">The other object to compare to.</param>
        /// <returns>True if the instances are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is T && Equals((T)obj)
                || obj is Option<T> && Equals((Option<T>)obj);
        }

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return HasValue ? Value.GetHashCode() : base.GetHashCode();
        }

        /// <summary>
        /// Attempts to extract the value.
        /// </summary>
        /// <param name="value">The value extracted.</param>
        /// <returns>Returns true if a value was available, false otherwise.</returns>
        public bool TryGetValue(out T value)
        {
            value = Value;
            return HasValue;
        }

        /// <summary>
        /// An implicit conversion from any value to an optional value.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>Returns a wrapped optional reference.</returns>
        public static implicit operator Option<T>(T value)
        {
            return new Option<T>(value);
        }
        ///// <summary>
        ///// An implicit conversion from any value to an optional value.
        ///// </summary>
        ///// <param name="value">The value to be converted.</param>
        ///// <returns>Returns a wrapped optional reference.</returns>
        //public static implicit operator Option<T>(T? value)
        //    where T : struct
        //{
        //    return value.HasValue ? new Option<T>(value.Value) : Option.None<T>();
        //}

        /// <summary>
        /// Return a string representation.
        /// </summary>
        /// <returns>A string representation of the optional value.</returns>
        public override string ToString()
        {
            return "Option<" + (HasValue ? Value.ToString() : "null") + ">";
        }

        /// <summary>
        /// Compares two objects for equality.
        /// </summary>
        /// <param name="left">The left comparand.</param>
        /// <param name="right">The right comparand.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }
        /// <summary>
        /// Compares two objects for inequality.
        /// </summary>
        /// <param name="left">The left comparand.</param>
        /// <param name="right">The right comparand.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }
        /// <summary>
        /// Compares two objects for equality.
        /// </summary>
        /// <param name="left">The left comparand.</param>
        /// <param name="right">The right comparand.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Option<T> left, T right)
        {
            return left.Equals(right);
        }
        /// <summary>
        /// Compares two objects for inequality.
        /// </summary>
        /// <param name="left">The left comparand.</param>
        /// <param name="right">The right comparand.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Option<T> left, T right)
        {
            return !left.Equals(right);
        }
        /// <summary>
        /// Compares two objects for equality.
        /// </summary>
        /// <param name="left">The left comparand.</param>
        /// <param name="right">The right comparand.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(T left, Option<T> right)
        {
            return right == left;
        }
        /// <summary>
        /// Compares two objects for inequality.
        /// </summary>
        /// <param name="left">The left comparand.</param>
        /// <param name="right">The right comparand.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(T left, Option<T> right)
        {
            return right != left;
        }
    }

    /// <summary>
    /// Option operations.
    /// </summary>
    /// <remarks>
    /// This class provides all the LINQ overloads needed to transparently work
    /// with System.Nullable&lt;T&gt; and Option&lt;&gt; in LINQ computations.
    /// </remarks>
    public static class Option
    {
        /// <summary>
        /// Construct a new optional value.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="value">The value to track.</param>
        /// <returns></returns>
        public static Option<T> ToOption<T>(this T value)
        {
            return new Option<T>(value);
        }
        /// <summary>
        /// Construct a new optional value.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="value">A nullable value.</param>
        /// <returns></returns>
        public static Option<T> ToOption<T>(this T? value)
            where T : struct
        {
            return value.HasValue ? new Option<T>() : new Option<T>(value.Value);
        }
        /// <summary>
        /// Construct a Nullable value type given an option type.
        /// </summary>
        /// <typeparam name="T">The nullable type.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <returns>A new nullable value.</returns>
        public static T? ToNullable<T>(this Option<T> option)
            where T : struct
        {
            return option.HasValue ? new Nullable<T>(option.Value) : new T?();
        }
        /// <summary>
        /// Transforms the embedded value to a new value if it exists, otherwise
        /// returns None.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="R">The type of the returned optional value.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="some">The function to apply if <paramref name="option"/> has a value.</param>
        /// <returns>
        /// Returns <paramref name="some"/>(<paramref name="option"/>) if <code>o.HasValue</code>
        /// is true, or <code>new Option(default(R))</code> otherwise.
        /// </returns>
        public static Option<R> Select<T, R>(this Option<T> option, Func<T, R> some)
        {
            return option.HasValue ? some(option.Value) : Option<R>.None;
        }
        /// <summary>
        /// Transforms the embedded value to a new value if it exists, otherwise
        /// returns None.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="R">The type of the returned optional value.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="some">The function to apply if <paramref name="option"/> has a value.</param>
        /// <returns>
        /// Returns <paramref name="some"/>(<paramref name="option"/>) if <code>o.HasValue</code>
        /// is true, or <code>new Option(default(R))</code> otherwise.
        /// </returns>
        public static Option<R> Select<T, R>(this T? option, Func<T, R> some)
            where T : struct
        {
            return option.HasValue ? some(option.Value) : Option<R>.None;
        }

        /// <summary>
        /// Performs a total match on the optional value and returns a new value.
        /// </summary>
        /// <typeparam name="T">The type of the encapsulated value.</typeparam>
        /// <typeparam name="R">The type of the returned value.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="some">The function to call with the encapsulated value.</param>
        /// <param name="none">The return value if optional value is None.</param>
        /// <returns>A value computed from the given functions.</returns>
        public static R Select<T, R>(this Option<T> option, Func<T, R> some, R none)
        {
            return option.HasValue ? some(option.Value) : none;
        }

        /// <summary>
        /// Performs a total match on the optional value and returns a new value.
        /// </summary>
        /// <typeparam name="T">The type of the encapsulated value.</typeparam>
        /// <typeparam name="R">The type of the returned value.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="some">The function to call with the encapsulated value.</param>
        /// <param name="none">The function to call if no value available.</param>
        /// <returns>A value computed from the given functions.</returns>
        public static R Select<T, R>(this Option<T> option, Func<T, R> some, Func<R> none)
        {
            return option.HasValue ? some(option.Value) : none();
        }

        /// <summary>
        /// Returns the encapsulated value if Some, returns 'none' otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="none">The value to return if o.IsNone.</param>
        /// <returns>The value encapsulated in the option if <code>o.HasValue</code> is true,
        /// <paramref name="none"/> otherwise.</returns>
        public static T Select<T>(this Option<T> option, T none)
        {
            return option.HasValue ? option.Value : none;
        }

        /// <summary>
        /// Project an optional value to another optional value.
        /// </summary>
        /// <typeparam name="T">The type of the original value.</typeparam>
        /// <typeparam name="U">The type of the projected value.</typeparam>
        /// <param name="option">The original optional instance.</param>
        /// <param name="selector">The projection function.</param>
        /// <returns>A new optional value computed from the original.</returns>
        public static Option<U> SelectMany<T, U>(this Option<T> option, Func<T, Option<U>> selector)
        {
            return option.HasValue ? selector(option.Value) : Option<U>.None;
        }
        /// <summary>
        /// Project an optional value to another optional value.
        /// </summary>
        /// <typeparam name="T">The type of the original value.</typeparam>
        /// <typeparam name="U">The type of the projected value.</typeparam>
        /// <param name="option">The original optional instance.</param>
        /// <param name="selector">The projection function.</param>
        /// <returns>A new optional value computed from the original.</returns>
        public static U? SelectMany<T, U>(this Option<T> option, Func<T, U?> selector)
            where U : struct
        {
            return option.HasValue ? selector(option.Value) : new U?();
        }
        /// <summary>
        /// Project an optional value to another optional value.
        /// </summary>
        /// <typeparam name="T">The type of the original value.</typeparam>
        /// <typeparam name="U">The type of the projected value.</typeparam>
        /// <param name="option">The original optional instance.</param>
        /// <param name="selector">The projection function.</param>
        /// <returns>A new optional value computed from the original.</returns>
        public static Option<U> SelectMany<T, U>(this T? option, Func<T, Option<U>> selector)
            where T : struct
        {
            return option.HasValue ? selector(option.Value) : Option<U>.None;
        }
        /// <summary>
        /// Project an optional value to another optional value.
        /// </summary>
        /// <typeparam name="T">The type of the original value.</typeparam>
        /// <typeparam name="U">The type of the projected value.</typeparam>
        /// <param name="option">The original optional instance.</param>
        /// <param name="selector">The projection function.</param>
        /// <returns>A new optional value computed from the original.</returns>
        public static U? SelectMany<T, U>(this T? option, Func<T, U?> selector)
            where T : struct
            where U : struct
        {
            return option.HasValue ? selector(option.Value) : new U?();
        }
        /// <summary>
        /// Projects two optional values to a third value.
        /// </summary>
        /// <typeparam name="T">The type of the first value.</typeparam>
        /// <typeparam name="U">The type of the second value.</typeparam>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="option">The optional type.</param>
        /// <param name="selector">The intermediate projection function.</param>
        /// <param name="result">The final projection function.</param>
        /// <returns>The returned optional value.</returns>
        public static Option<R> SelectMany<T, U, R>(this Option<T> option, Func<T, Option<U>> selector, Func<T, U, R> result)
        {
            var u = option.SelectMany(selector);
            return u.HasValue ? result(option.Value, u.Value) : Option<R>.None;
        }
        /// <summary>
        /// Projects two optional values to a third value.
        /// </summary>
        /// <typeparam name="T">The type of the first value.</typeparam>
        /// <typeparam name="U">The type of the second value.</typeparam>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="option">The optional type.</param>
        /// <param name="selector">The intermediate projection function.</param>
        /// <param name="result">The final projection function.</param>
        /// <returns>The returned optional value.</returns>
        public static Option<R> SelectMany<T, U, R>(this T? option, Func<T, Option<U>> selector, Func<T, U, R> result)
            where T : struct
        {
            var u = option.SelectMany(selector);
            return u.HasValue ? result(option.Value, u.Value) : Option<R>.None;
        }
        /// <summary>
        /// Projects two optional values to a third value.
        /// </summary>
        /// <typeparam name="T">The type of the first value.</typeparam>
        /// <typeparam name="U">The type of the second value.</typeparam>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="option">The optional type.</param>
        /// <param name="selector">The intermediate projection function.</param>
        /// <param name="result">The final projection function.</param>
        /// <returns>The returned optional value.</returns>
        public static Option<R> SelectMany<T, U, R>(this Option<T> option, Func<T, U?> selector, Func<T, U, R> result)
            where U : struct
        {
            var u = option.SelectMany(selector);
            return u.HasValue ? result(option.Value, u.Value) : Option<R>.None;
        }
        /// <summary>
        /// Projects two optional values to a third value.
        /// </summary>
        /// <typeparam name="T">The type of the first value.</typeparam>
        /// <typeparam name="U">The type of the second value.</typeparam>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="option">The optional type.</param>
        /// <param name="selector">The intermediate projection function.</param>
        /// <param name="result">The final projection function.</param>
        /// <returns>The returned optional value.</returns>
        public static Option<R> SelectMany<T, U, R>(this T? option, Func<T, U?> selector, Func<T, U, R> result)
            where T : struct
            where U : struct
        {
            var u = option.SelectMany(selector);
            return u.HasValue ? result(option.Value, u.Value) : Option<R>.None;
        }
        /// <summary>
        /// Performs the given action on the embedded value if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="action">The function to apply.</param>
        public static void Do<T>(this Option<T> option, Action<T> action)
        {
            if (option.HasValue) action(option.Value);
        }

        // code below added by Mauricio Scheffer

        public static readonly Option<Unit> SomeUnit = Option<Unit>.Some(null);

        public static Func<Option<T>> OrElse<T>(this Option<T> option, Func<Option<T>> alt)
        {
            return () => {
                if (option.HasValue)
                    return option;
                return alt();
            };
        }

        public static Func<Option<T>> OrElse<T>(this Func<Option<T>> option, Func<Option<T>> alt)
        {
            return () => {
                var v = option();
                if (v.HasValue)
                    return v;
                return alt();
            };
        }
    }
}
