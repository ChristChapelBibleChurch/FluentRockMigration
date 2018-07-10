#region Using statements

using System;

#endregion

namespace org.christchapelbc.Utility.Helpers
{
    /// <summary>
    /// A collection of static functions that check the value of a variable
    /// and throw an exception if the value is not expected. Used when you want
    /// to throw an exception when variable values are not correct.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="System.Data.Entity.Utilities.Check"/>.
    /// </remarks>
    public class Check
    {
        /// <summary>
        /// Checks the <paramref name="value"/> of <typeparamref name="T"/> to see if it is null.
        /// </summary>
        /// <typeparam name="T">A class.</typeparam>
        /// <param name="value">The value of the object <typeparamref name="T"/>.</param>
        /// <param name="parameterName">The name of the field with type <typeparamref name="T"/>.</param>
        /// <returns>The object <typeparamref name="T"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="parameterName"/> is null.</exception>
        public static T NotNull<T>( T value, string parameterName ) where T : class
        {
            if ( value == null )
            {
                throw new ArgumentNullException( parameterName );
            }

            return value;
        }

        /// <summary>
        /// Checks the <paramref name="value"/> of a <see cref="string"/> to see if it is null or contains only whitespace.
        /// </summary>
        /// <param name="value">The value of the <see cref="string"/> object.</param>
        /// <param name="parameterName">The name of the <see cref="string"/> field.</param>
        /// <returns>The <see cref="string"/> object.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="parameterName"/> is null or contains only whitespace.</exception>
        public static string NotNullOrWhiteSpace( string value, string parameterName )
        {
            if ( string.IsNullOrWhiteSpace( value ) )
            {
                throw new ArgumentNullException( parameterName );
            }

            return value;
        }
    }
}
