#region Using statements

using System;

#endregion

namespace org.christchapelbc.Utility.Extensions
{
    /// <summary>
    /// Additional functions for a <see cref="string"/> object.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="System.Data.Entity.Utilities.StringExtensions"/>.
    /// </remarks>
    internal static class StringExtensions
    {
        /// <summary>
        /// Checks whether strings <paramref name="a"/> and <paramref name="b"/> are equal
        /// if letter casing is not considered.
        /// </summary>
        /// <param name="a">The <see cref="string"/> being checked.</param>
        /// <param name="b">The <see cref="string"/> that <paramref name="a"/> will be checked against.</param>
        /// <returns>
        ///     <code>true</code> if strings <paramref name="a"/> and <paramref name="b"/>
        ///     are the same if letter casing is not considered, and <code>false</code>
        ///     otherwise.
        /// </returns>
        public static bool EqualsIgnoreCase( this string a, string b )
        {
            return string.Equals( a, b, StringComparison.OrdinalIgnoreCase );
        }
    }
}
