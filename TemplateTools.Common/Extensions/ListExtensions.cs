//@BaseCode

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for list operations.
    /// </summary>
    public static partial class ListExtensions
    {
        /// <summary>
        /// Removes all items from the list and returns them as a new array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="source">The list to eject items from.</param>
        /// <returns>An array containing all items that were removed from the list.</returns>
        public static IEnumerable<T> Eject<T>(this List<T> source)
        {
            var result = source.ToArray();
            
            source.Clear();
            return result;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the list and returns the count of added elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="source">The list to add the elements to.</param>
        /// <param name="items">The collection whose elements should be added to the list.</param>
        /// <returns>The count of elements added to the list.</returns>
        public static int AddRangeAndCount<T>(this List<T> source, IEnumerable<T> items)
        {
            var result = 0;
            
            foreach (var item in items)
            {
                source.Add(item);
                result++;
            }
            return result;
        }
    }
}

