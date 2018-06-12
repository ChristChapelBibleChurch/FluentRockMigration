#region Using statements

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using org.christchapelbc.Utility.Helpers;

#endregion

namespace org.christchapelbc.Utility.Extensions
{
    /// <summary>
    /// Additional functions for an <see cref="XContainer"/>.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="System.Data.Entity.Utilities.XContainerExtensions"/>.
    /// </remarks>
    internal static class XContainerExtensions
    {
        /// <summary>
        /// Returns a collection of a nested XML elements in a specific XML element.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> containing the XML elements.</param>
        /// <param name="name">The name of the XML attribute whose descendants are desired.</param>
        /// <returns>A collection of nested elements from the <paramref name="name"/> attribute in <paramref name="container"/>.</returns>
        public static IEnumerable<XElement> Descendants( this XContainer container, IEnumerable<XName> name )
        {
            DebugCheck.NotNull( container );
            DebugCheck.NotNull( name );

            return name.SelectMany( container.Descendants );
        }
    }
}
