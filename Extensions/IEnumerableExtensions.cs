#region Using statements

using System;
using System.Collections.Generic;

using org.christchapelbc.Utility.Helpers;

#endregion

namespace org.christchapelbc.Utility.Extensions
{
    /// <summary>
    /// Additional functions for an <see cref="IEnumerable{T}"/> collection.
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Applies an <see cref="Action"/> to each object in an <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">The inferred <see cref="Type"/> of the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="ts">The enumerable collection of objects.</param>
        /// <param name="action">The <see cref="Action"/> to perform on each object in the collection.</param>
        public static void Each<T>( this IEnumerable<T> ts, Action<T> action )
        {
            DebugCheck.NotNull( ts );
            DebugCheck.NotNull( action );

            foreach( T t in ts )
            {
                action( t );
            }
        }
    }
}
