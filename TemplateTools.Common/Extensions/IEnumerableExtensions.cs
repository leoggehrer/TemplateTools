//@BaseCode

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for the IEnumerable interface.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Searches for the first occurrence of an element in a sequence that satisfies a specified condition and returns the zero-based index of the element.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">An IEnumerable that contains the elements to search.</param>
        /// <param name="predicate">A Predicate that defines the condition to check against the elements.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element in the sequence that satisfies the specified condition, if found; otherwise, -1.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var idx = 0;
            var result = -1;
            var enumerator = source.GetEnumerator();
            
            while (result == -1 && enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    result = idx;
                }
                idx++;
            }
            return result;
        }
        /// <summary>
        /// Converts an IEnumerable sequence of objects of type T to an IEnumerable sequence of objects of type ST,
        /// using the specified expandSelector to select the target objects.
        /// </summary>
        /// <typeparam name="T">The type of objects in the source sequence.</typeparam>
        /// <typeparam name="ST">The type of objects in the target sequence.</typeparam>
        /// <param name="source">The IEnumerable sequence to convert.</param>
        /// <param name="expandSelector">A function that selects the target objects from the source objects.</param>
        /// <returns>An IEnumerable sequence of objects of type ST.</returns>
        public static IEnumerable<ST> ToEnumerable<T, ST>(this IEnumerable<T> source, Func<T, ST> expandSelector)
        {
            List<ST> expandResult = [];
            
            if (source != null && expandSelector != null)
            {
                foreach (var item in source)
                {
                    var subItem = expandSelector(item);
                    
                    if (subItem != null)
                    {
                        expandResult.Add(subItem);
                    }
                }
            }
            return expandResult;
        }
        /// <summary>
        /// Flattens a nested collection by expanding each element using the specified selector function.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <typeparam name="ST">The type of the elements in the expanded collection.</typeparam>
        /// <param name="source">The source collection to flatten.</param>
        /// <param name="expandSelector">A function to expand each element in the source collection.</param>
        /// <returns>An IEnumerable&lt;ST&gt; that contains the flattened elements.</returns>
        public static IEnumerable<ST> Flatten<T, ST>(this IEnumerable<T> source, Func<T, IEnumerable<ST>> expandSelector)
        {
            List<ST> expandResult = [];
            
            if (source != null && expandSelector != null)
            {
                foreach (var item in source)
                {
                    var subItems = expandSelector(item);
                    
                    if (subItems != null)
                    {
                        expandResult.AddRange(subItems);
                    }
                }
            }
            return expandResult;
        }
        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            int retVal = 0;
            
            foreach (var item in items)
            {
                if (predicate(item))
                return retVal;
                
                retVal++;
            }
            return -1;
        }
        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            var result = -1;
            
            if (items != null)
            {
                result = items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
            }
            return result;
        }
        
        /// <summary>
        /// Returns elements from a sequence until the specified condition is met.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="source">The sequence to take elements from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An IEnumerable that contains elements from the input sequence that occur before the element that matches the specified condition.</returns>
        public static IEnumerable<T> TakeTo<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var result = default(IEnumerable<T>);
            
            if (source != null)
            {
                var end = false;
                
                result = source.Where(e =>
                {
                    if (end == false && predicate != null)
                    {
                        end = predicate(e);
                    }
                    return end == false;
                });
            }
            return result ?? [];
        }
        /// <summary>
        /// Performs the specified action on each element of the IEnumerable.
        /// </summary>
        /// <typeparam name="T">The generic type T.</typeparam>
        /// <param name="source">The collection from type T.</param>
        /// <param name="action">The Action delegate to perform on each element of the IEnumerable.</param>
        /// <returns>The same collection as source.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null && action != null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }
            return source ?? [];
        }
        /// <summary>
        /// Copy the collection from source and return it.
        /// </summary>
        /// <typeparam name="T">The generic type T.</typeparam>
        /// <param name="source">The collection from type T.</param>
        /// <returns>A copy of source collection.</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> source)
        {
            return [.. source];
        }
    }
}

