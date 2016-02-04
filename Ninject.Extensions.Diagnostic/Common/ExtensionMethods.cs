using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Extensions.Diagnostic.Common
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Convert exception with its inner exceptions into a string
        /// </summary>
        /// <param name="ex">exception to convertation</param>
        /// <returns></returns>
        public static string ToStringWithInnerExceptions(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.ToString());

            var inner = ex.InnerException;
            while (inner != null)
            {
                sb.Append("\n===INNER EXCEPTION===\n");
                sb.Append(inner.ToString());
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns whether the enumerable is empty
        /// </summary>
        /// <typeparam name="TSource">Type of enumerable elements</typeparam>
        /// <param name="source">Object of type System.Collections.Generic.IEnumerable`1, is checked for the absence of the elements</param>
        /// <returns>true, if source enumerable is empty or null, otherwise — false</returns>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Returns whether there is no value in the enumerable that satisfies a predicate
        /// </summary>
        /// <typeparam name="TSource">Type of enumerable elements</typeparam>
        /// <param name="source">Object of type System.Collections.Generic.IEnumerable`1, is checked for the absence of the elements</param>
        /// <returns>true, if source enumerable is empty or null, or does not contains any elements that satisfies a predicate, otherwise — false</returns>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source == null || !source.Any(predicate);
        }

        /// <summary>
        /// Returns empty enumerable if source enumerable is null
        /// </summary>
        /// <typeparam name="TSource">Type of enumerable elements</typeparam>
        /// <param name="source">Object of type System.Collections.Generic.IEnumerable`1, is checked for null</param>
        /// <returns></returns>
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source != null ? source : Enumerable.Empty<TSource>();
        }

        /// <summary>
        /// Returns whether there is value the only element in the enumerable that equals passed element
        /// </summary>
        /// <typeparam name="TSource">Type of enumerable elements</typeparam>
        /// <param name="source">Object of type System.Collections.Generic.IEnumerable`1, is checked for the uniqueness of the element</param>
        /// <param name="element">Element expected in the enumerable</param>
        /// <returns></returns>
        public static bool HasOnly<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
                return false;

            var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
                return false;

            if (enumerator.Current == null || !enumerator.Current.Equals(element))
                return false;

            return !enumerator.MoveNext();
        }

        /// <summary>
        /// Returns default(TSourse) if passed object is null, otherwise returns result of passed function execution 
        /// </summary>
        /// <typeparam name="TSource">Passed object type</typeparam>
        /// <typeparam name="TResult">Return object type</typeparam>
        /// <param name="source">Passed object</param>
        /// <param name="func">Function that returns object of type TResult</param>
        /// <returns></returns>
        public static TResult With<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
        {
            if (source == null)
                return default(TResult);

            return func(source);
        }

        /// <summary>
        /// Do nothing if passed object is null, otherwise execute passed function
        /// </summary>
        /// <typeparam name="TSource">Passed object type</typeparam>
        /// <typeparam name="TResult">Return object type</typeparam>
        /// <param name="func">Function for execution</param>
        /// <returns></returns>
        public static TSource Do<TSource>(this TSource source, Action<TSource> func)
        {
            if (source == null)
                return default(TSource);

            func(source);
            return source;
        }

        /// <summary>
        /// Concatenate array with element
        /// </summary>
        /// <typeparam name="TSource">Array's elements type</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="element">Element for concatenation</param>
        /// <returns></returns>
        public static TSource[] Concat<TSource>(this TSource[] source, TSource element)
        {
            var list = new List<TSource>(source.Length + 1);
            list.AddRange(source);
            list.Add(element);

            return list.ToArray();
        }
    }
}
