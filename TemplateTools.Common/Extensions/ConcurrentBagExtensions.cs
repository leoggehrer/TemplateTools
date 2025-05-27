//@BaseCode

using System.Collections.Concurrent;

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="ConcurrentBag{T}"/>.
    /// </summary>
    public static partial class ConcurrentBagExtensions
    {
        /// <summary>
        /// Adds a specified element to the <see cref="ConcurrentBag{T}"/> safely.
        /// </summary>
        /// <typeparam name="T">The type of elements in the bag.</typeparam>
        /// <param name="source">The source <see cref="ConcurrentBag{T}"/>.</param>
        /// <param name="otherElement">The element to add.</param>
        /// <remarks>
        /// This method ensures thread-safe access to the <see cref="ConcurrentBag{T}"/>.
        /// The element must be of reference type.
        /// </remarks>
        public static void AddSafe<T>(this ConcurrentBag<T> source, T otherElement)
        where T : class
        {
            otherElement.CheckArgument(nameof(otherElement));
            
            source.Add(otherElement);
        }
        /// <summary>
        /// Adds all items from the specified collection to the current thread-safe bag.
        /// </summary>
        /// <typeparam name="T">The type of elements in the bag.</typeparam>
        /// <param name="source">The current thread-safe bag.</param>
        /// <param name="otherSource">The collection whose elements should be added to the bag.</param>
        /// <returns>The updated thread-safe bag after adding all items from the collection.</returns>
        public static IEnumerable<T> AddRangeSafe<T>(this ConcurrentBag<T> source, IEnumerable<T> otherSource)
        {
            foreach (var item in otherSource)
            {
                source.Add(item);
            }
            return source;
        }
    }
}

