#region Using statements

using System.Diagnostics;

#endregion

namespace org.christchapelbc.Utility.Helpers
{
    /// <summary>
    /// A collection of static functions that check the value of a variable
    /// and trigger a debug assertion if the value is not expected. Used
    /// when a warned should be issued in development that a value is not
    /// correct, but should continue anyway in production.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="System.Data.Entity.Utilities.DebugCheck"/>.
    /// </remarks>
    public class DebugCheck
    {
        /// <summary>
        /// Checks the <paramref name="value"/> of <typeparamref name="T"/> to see if it is null.
        /// </summary>
        /// <typeparam name="T">A class.</typeparam>
        /// <param name="value">The value of the object <typeparamref name="T"/>.</param>
        [Conditional( "DEBUG" )]
        public static void NotNull<T>( T value ) where T : class
        {
            Debug.Assert( value != null );
        }

        /// <summary>
        /// Checks the <paramref name="value"/> of a <see cref="string"/> to see if it is null or contains only whitespace.
        /// </summary>
        /// <param name="value">The value of the <see cref="string"/> object.</param>
        [Conditional( "DEBUG" )]
        public static void NotNullOrWhiteSpace( string value )
        {
            Debug.Assert( !string.IsNullOrWhiteSpace( value ) );
        }
    }
}
