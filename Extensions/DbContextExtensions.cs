#region Using statements

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using org.christchapelbc.Utility.Helpers;

#endregion

namespace org.christchapelbc.Utility.Extensions
{
    /// <summary>
    /// Additional functions for a <see cref="DbContext"/>.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="System.Data.Entity.Utilities.DbContextExtensions"/>.
    /// </remarks>
    internal static class DbContextExtensions
    {
        /// <summary>
        /// Returns the database schema in the form XML.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> to access the database with.</param>
        /// <returns>The database schema as XML.</returns>
        public static XDocument GetModel( this DbContext context )
        {
            DebugCheck.NotNull( context );

            return GetModel( writer => EdmxWriter.WriteEdmx( context, writer ) );
        }

        /// <summary>
        /// Returns the database schema in the form XML.
        /// </summary>
        /// <param name="writeXml">The action to perform on an <see cref="XmlWriter"/>.</param>
        /// <returns>The database schema as XML.</returns>
        public static XDocument GetModel( Action<XmlWriter> writeXml )
        {
            DebugCheck.NotNull( writeXml );

            using ( MemoryStream memoryStream = new MemoryStream() )
            {
                using
                (
                    XmlWriter writer = XmlWriter.Create
                    (
                        memoryStream,
                        new XmlWriterSettings
                        {
                            Indent = true
                        }
                    )
                )
                {
                    writeXml( writer );
                }

                memoryStream.Position = 0;

                return XDocument.Load( memoryStream );
            }
        }
    }
}
