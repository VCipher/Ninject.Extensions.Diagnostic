using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Linq;

namespace Ninject.Extensions.Diagnostic.Common
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Преобразование исключения с его внутренними исключениями в строку
        /// </summary>
        /// <param name="ex"></param>
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
        /// Полное клонирование объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Возвращает пусто ли перечисление
        /// </summary>
        /// <typeparam name="TSource">Тип элементов перечисления source.</typeparam>
        /// <param name="source">Объект System.Collections.Generic.IEnumerable`1, проверяемый на отсутсвие элементов.</param>
        /// <returns>
        /// true, если исходная последовательность не содержит какие-либо элементы, 
        /// в противном случае — false.
        /// </returns>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Возвращает отсутствует ли в перечислении значение, удовлетворяющее данному предикату
        /// </summary>
        /// <typeparam name="TSource">Тип элементов перечисления source.</typeparam>
        /// <param name="source">Объект System.Collections.Generic.IEnumerable`1, проверяемый на отсутсвие элементов.</param>
        /// <returns>
        /// true, если исходная последовательность не содержит никаких элементов, 
        /// проходящих проверку, определяемую указанным предикатом;
        /// в противном случае — false.
        /// </returns>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source == null || !source.Any(predicate);
        }

        /// <summary>
        /// Возвращает пустое перечисление если исходное перечисление равно null
        /// </summary>
        /// <typeparam name="TSource">Тип элементов перечисления source</typeparam>
        /// <param name="source">Объект System.Collections.Generic.IEnumerable`1, проверяемый на null</param>
        /// <returns></returns>
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source != null ? source : Enumerable.Empty<TSource>();
        }

        /// <summary>
        /// Пытается произвести отображение множества IEnumerable<TSource> во множество IEnumerable<TResult>
        /// при помощи функтора selector. Если на каком-либо этапе произойдет Exception, то этот этап просто пропускается
        /// </summary>
        /// <typeparam name="TSource">Тип данных исходного множества</typeparam>
        /// <typeparam name="TResult">Тип данных результирующего множества</typeparam>
        /// <param name="source">Исходное множество</param>
        /// <param name="selector">Результирующее множество</param>
        /// <returns></returns>
        public static IEnumerable<TResult> TrySelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) yield break;
            if (selector == null) yield break;

            var enumerator = source.GetEnumerator();

            while (true)
            {
                TResult ret = default(TResult);
                try
                {
                    // если исходное множество закончилось,
                    // заканчиваем перебор
                    if (!enumerator.MoveNext())
                        break;

                    ret = selector(enumerator.Current);
                }
                catch
                {
                    // продолжаем итерировать по исходному множеству,
                    // пропуская ошибочный этап
                    continue;
                }

                yield return ret;
            }
        }

        /// <summary>
        /// Проверяет, содержит ли перечисление единственный элемент, равный данному
        /// </summary>
        /// <typeparam name="TSource">Тип элементов перечисления source.</typeparam>
        /// <param name="source">Объект System.Collections.Generic.IEnumerable`1, проверяемый на единственность элемента.</param>
        /// <param name="element">Элемент последовательности ожидаемый в перечислении</param>
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
        /// Возвращает значение аттрибута xml-элемента
        /// </summary>
        /// <param name="element">xml-элемент с аттрибутами</param>
        /// <param name="name">имя атрибута</param>
        /// <returns>Числовое значение атрибута или null, если атрибута нет</returns>
        public static int? GetAttributeInt(this XElement element, string name)
        {
            var attr = element.Attribute(name);
            return attr != null ? int.Parse(attr.Value) : default(int?);
        }

        /// <summary>
        /// Возвращает значение аттрибута xml-элемента
        /// </summary>
        /// <param name="element">xml-элемент с аттрибутами</param>
        /// <param name="name">имя атрибута</param>
        /// <returns>Строковое значение атрибута или null, если атрибута нет</returns>
        public static string GetAttribute(this XElement element, string name)
        {
            var attr = element.Attribute(name);
            return attr != null ? attr.Value : null;
        }

        /// <summary>
        /// Возвращает обрезанную строку со временным промежутком
        /// </summary>
        /// <param name="span">Временной промежуток</param>
        /// <returns></returns>
        public static string ToTruncatedString(this TimeSpan span)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                  span.Hours, span.Minutes,
                  span.Seconds, span.Milliseconds);
        }


        /// <summary>
        /// Если исходный объект равен null возвращает default(TSource), иначе резельтат функции
        /// </summary>
        /// <typeparam name="TSource">тип исходного объекта</typeparam>
        /// <typeparam name="TResult">тип возвращаемого объекта</typeparam>
        /// <param name="source">исходный объект</param>
        /// <param name="func">функция, возвращающая объект типа TResult</param>
        /// <returns></returns>
        public static TResult With<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
        {
            if (source == null)
                return default(TResult);

            return func(source);
        }

        /// <summary>
        /// Если исходный объект равен null то ничего не делает, иначе исполняет тело метода
        /// </summary>
        /// <typeparam name="TSource">тип исходного объекта</typeparam>
        /// <param name="source">исходный объект</param>
        /// <param name="func">функция на исполнение</param>
        /// <returns></returns>
        public static TSource Do<TSource>(this TSource source, Action<TSource> func)
        {
            if (source == null)
                return default(TSource);

            func(source);
            return source;
        }

        /// <summary>
        /// Соединяет массив с элементом
        /// </summary>
        /// <typeparam name="TSource">Тип элементов для соединения</typeparam>
        /// <param name="source">Исходный массив</param>
        /// <param name="element">Элемент для содинения</param>
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
