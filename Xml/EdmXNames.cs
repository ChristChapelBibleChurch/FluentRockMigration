#region Using statements

using System.Collections.Generic;
using System.Xml.Linq;

using org.christchapelbc.Utility.Helpers;

#endregion

namespace org.christchapelbc.Utility.Xml
{
    /// <summary>
    /// Interfaces with an Entity Data Model expressed as XML.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="System.Data.Entity.Migrations.Edm.EdmXNames"/>.
    /// </remarks>
    internal static class EdmXNames
    {
        #region Member fields

        private static readonly XNamespace _ssdlNamespaceV2 = XNamespace.Get( "http://schemas.microsoft.com/ado/2009/02/edm/ssdl" );
        private static readonly XNamespace _ssdlNamespaceV3 = XNamespace.Get( "http://schemas.microsoft.com/ado/2009/11/edm/ssdl" );

        #endregion

        /// <summary>
        /// Interfaces with a Store Schema Definition Language (SSDL).
        /// </summary>
        public static class Ssdl
        {
            #region Member fields

            public static readonly IEnumerable<XName> EntitySetNames        = Names( "EntitySet" );
            public static readonly IEnumerable<XName> EntityTypeNames       = Names( "EntityType" );
            public static readonly IEnumerable<XName> PropertyRefNames      = Names( "PropertyRef" );

            #endregion

            #region Private member functions

            /// <summary>
            /// Constructs the proper SSDL identifier for a given XML element name.
            /// </summary>
            /// <param name="elementName">The <code>name</code> attribute on the XML element.</param>
            /// <returns>The proper SSDL identifier based on <paramref name="elementName"/>.</returns>
            private static IEnumerable<XName> Names( string elementName )
            {
                DebugCheck.NotNullOrWhiteSpace( elementName );

                return new List<XName>
                    {
                        _ssdlNamespaceV3 + elementName,
                        _ssdlNamespaceV2 + elementName
                    };
            }

            #endregion
        }
    }
}
