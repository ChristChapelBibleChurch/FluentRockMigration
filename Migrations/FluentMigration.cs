#region Using statements

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Builders;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using Rock;
using Rock.Data;
using Rock.Plugin;

using org.christchapelbc.Utility.Extensions;
using org.christchapelbc.Utility.Helpers;
using org.christchapelbc.Utility.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace org.christchapelbc.Utility.Migrations
{
    /// <summary>
    /// A shim over <see cref="Migration"/> that allows the use of the
    /// fluent-style API in Entity Framework.
    /// </summary>
    public abstract class FluentMigration : Migration
    {
        #region Member fields

        /// <summary>
        /// The <see cref="DbMigration"/> implementation that allows the use
        /// of fluent-style API from Entity Framework.
        /// </summary>
        private FluentDbMigration _migration = new FluentDbMigration();

        /// <summary>
        /// The default schema name of the database, in case one a
        /// fully-qualified database table name is not supplied
        /// in fluent-style API functions.
        /// </summary>
        /// <remarks>
        /// This uses properties instead of insane levels of reflection
        /// to use internal classes in Entity Framework.
        /// </remarks>
        protected string _defaultSchema = "dbo";

        #endregion

        #region Member properties

        /// <summary>
        /// The default schema name of the database, in case one a
        /// fully-qualified database table name is not supplied
        /// in fluent-style API functions.
        /// </summary>
        public string DefaultSchema {
            get
            {
                return _defaultSchema;
            }

            protected set
            {
                _migration.DefaultSchema    = value;
                _defaultSchema              = value;
            }
        }

        #endregion

        #region Override functions

        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            Sql( _migration.ToSql( SqlConnection ) );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            Sql( _migration.ToSql( SqlConnection ) );
        }

        #endregion

        #region CREATE functions

        /// <summary>
        /// Adds an operation to create a new table. Entity Framework Migrations APIs are
        /// not designed to accept input provided by untrusted sources (such as the end user
        /// of an application). If input is accepted from such sources it should be validated
        /// before being passed to these APIs to protect against SQL injection attacks etc.
        /// </summary>
        /// <typeparam name="TColumns">
        ///     The columns in this create table operation. You do not need to specify this type,
        ///     it will be inferred from the <paramref name="columnsAction"/> parameter you supply.
        /// </typeparam>
        /// <param name="name">The name of the table. Schema name is optional, if no schema is specified then dbo is assumed.</param>
        /// <param name="columnsAction">
        ///     An action that specifies the columns to be included in the table.
        ///     i.e. <code>t => new { Id = t.Int( identity: true ), Name = t.String() }</code>.
        /// </param>
        /// <param name="anonymousArguments">
        ///     Additional arguments that may be processed by providers.
        ///     Use anonymous type syntax to specify arguments e.g. <code>new { SampleArgument = "MyValue" }</code>.
        /// </param>
        /// <returns>An object that allows further configuration of the table creation operation.</returns>
        public new TableBuilder<TColumns> CreateTable<TColumns>(
            string name,
            Func<ColumnBuilder, TColumns> columnsAction,
            object anonymousArguments = null
        )
        {
            Check.NotNullOrWhiteSpace( name, nameof( name ) );
            Check.NotNull( columnsAction, nameof( columnsAction ) );

            return CreateTable( name, columnsAction, null, anonymousArguments );
        }

        /// <summary>
        /// Adds an operation to create a new table. Entity Framework Migrations APIs are
        /// not designed to accept input provided by untrusted sources (such as the end user
        /// of an application). If input is accepted from such sources it should be validated
        /// before being passed to these APIs to protect against SQL injection attacks etc.
        /// </summary>
        /// <typeparam name="TColumns">
        ///     The columns in this create table operation. You do not need to specify this type,
        ///     it will be inferred from the <paramref name="columnsAction"/> parameter you supply.
        /// </typeparam>
        /// <param name="name">The name of the table. Schema name is optional, if no schema is specified then dbo is assumed.</param>
        /// <param name="columnsAction">
        ///     An action that specifies the columns to be included in the table.
        ///     i.e. <code>t => new { Id = t.Int( identity: true ), Name = t.String() }</code>.
        /// </param>
        /// <param name="annotations">Custom annotations that exist on the table to be created. May be null or empty.</param>
        /// <param name="anonymousArguments">
        ///     Additional arguments that may be processed by providers.
        ///     Use anonymous type syntax to specify arguments e.g. <code>new { SampleArgument = "MyValue" }</code>.
        /// </param>
        /// <returns>An object that allows further configuration of the table creation operation.</returns>
        public TableBuilder<TColumns> CreateTable<TColumns>(
            string name,
            Func<ColumnBuilder, TColumns> columnsAction,
            IDictionary<string, object> annotations,
            object anonymousArguments = null
        )
        {
            Check.NotNullOrWhiteSpace( name, nameof( name ) );
            Check.NotNull( columnsAction, nameof( columnsAction ) );

            return _migration.CreateTable( name, columnsAction, annotations, anonymousArguments );
        }

        #endregion

        /// <summary>
        /// An extension of <see cref="DbMigration"/> that uses reflection
        /// to access the <see cref="MigrationOperation"/>s queued up
        /// by the fluent-style API.
        /// </summary>
        private class FluentDbMigration : DbMigration
        {
            #region Member properties

            /// <summary>
            /// The default schema name to use when one is not supplied
            /// with a table name.
            /// </summary>
            public string DefaultSchema { get; set; }

            #endregion

            #region Override functions

            /// <summary>
            /// Operations to be performed during the upgrade process.
            /// </summary>
            public override void Up()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region Public member functions

            /// <summary>
            /// Adds an operation to create a new table. Entity Framework Migrations APIs are
            /// not designed to accept input provided by untrusted sources (such as the end user
            /// of an application). If input is accepted from such sources it should be validated
            /// before being passed to these APIs to protect against SQL injection attacks etc.
            /// </summary>
            /// <typeparam name="TColumns">
            ///     The columns in this create table operation. You do not need to specify this type,
            ///     it will be inferred from the <paramref name="columnsAction"/> parameter you supply.
            /// </typeparam>
            /// <param name="name">The name of the table. Schema name is optional, if no schema is specified then dbo is assumed.</param>
            /// <param name="columnsAction">
            ///     An action that specifies the columns to be included in the table.
            ///     i.e. <code>t => new { Id = t.Int( identity: true ), Name = t.String() }</code>.
            /// </param>
            /// <param name="annotations">Custom annotations that exist on the table to be created. May be null or empty.</param>
            /// <param name="anonymousArguments">
            ///     Additional arguments that may be processed by providers.
            ///     Use anonymous type syntax to specify arguments e.g. <code>new { SampleArgument = "MyValue" }</code>.
            /// </param>
            /// <returns>An object that allows further configuration of the table creation operation.</returns>
            public new TableBuilder<TColumns> CreateTable<TColumns>(
                string name,
                Func<ColumnBuilder, TColumns> columnsAction,
                IDictionary<string, object> annotations,
                object anonymousArguments = null
            )
            {
                Check.NotNullOrWhiteSpace( name, nameof( name ) );
                Check.NotNull( columnsAction, nameof( columnsAction ) );

                return base.CreateTable( name, columnsAction, annotations, anonymousArguments );
            }

            /// <summary>
            /// Converts all the <see cref="MigrationOperation"/>s applied to
            /// this <see cref="FluentDbMigration"/> to the SQL equivalent.
            /// </summary>
            /// <returns>A <see cref="string"/> containing all the SQL for the <see cref="MigrationOperation"/>s.</returns>
            /// <exception cref="System.Exception">The table a foreign key references does not exist.</exception>
            public string ToSql( SqlConnection sqlConnection )
            {
                Check.NotNull( sqlConnection, nameof( sqlConnection ) );

                StringBuilder sql = new StringBuilder();

                // Get the "Operations" property of DbMigrtion through reflection, since the property is marked "internal"
                PropertyInfo operationsProperty = this.GetType().GetProperty( "Operations", BindingFlags.NonPublic | BindingFlags.Instance );
                if ( operationsProperty != null )
                {
                    // Enumerate PrincipalColumns in AddForeignKeyOperation operations
                    IEnumerable<MigrationOperation> operations = operationsProperty.GetValue( this ) as IEnumerable<MigrationOperation>;

                    CompleteForeignKeyOperations( operations, sqlConnection );

                    // Translate each MigrationOperation to valid SQL using whatever version of SQL we are connecting to.
                    SqlServerMigrationSqlGenerator generator    = new SqlServerMigrationSqlGenerator();
                    IEnumerable<MigrationStatement> statements  = generator.Generate( operations, ( ( sqlConnection.ServerVersion.AsInteger() > 10 ) ? "2008" : "2005" ) );

                    // Concatenate all SQL statements to generate a SQL script for this migration
                    foreach ( MigrationStatement statement in statements )
                    {
                        sql.Append( statement.Sql );
                    }
                }

                return sql.ToString();
            }

            #endregion

            #region Private member functions

            /// <summary>
            /// Searches for <see cref="AddForeignKeyOperation"/>s in <paramref name="operations"/> and
            /// completes the operation by adding the principal column of the referenced table.
            /// </summary>
            /// <param name="operations">The <see cref="MigrationOperation"/>s that possibly contain an <see cref="AddForeignKeyOperation"/>.</param>
            /// <param name="sqlConnection">A connection to a SQL server where the database used in the migration is stored.</param>
            private void CompleteForeignKeyOperations( IEnumerable<MigrationOperation> operations, SqlConnection sqlConnection )
            {
                DebugCheck.NotNull( operations );
                Check.NotNull( sqlConnection, nameof( sqlConnection ) );

                foreach
                (
                    AddForeignKeyOperation operation in operations.OfType<AddForeignKeyOperation>()
                        .Where
                        (
                            addForeignKeyOperation =>
                            (
                                ( addForeignKeyOperation.PrincipalTable != null ) &&
                                ( !addForeignKeyOperation.PrincipalColumns.Any() )
                            )
                        )
                )
                {
                    // Go to the principal table this operation references
                    RockContext context     = new RockContext( sqlConnection.ConnectionString );
                    XDocument targetModel   = context.GetModel();

                    string principalTableName   = GetStandardizedTableName( operation.PrincipalTable );
                    string databaseTableName    =
                    (
                        from databases in targetModel.Descendants( EdmXNames.Ssdl.EntitySetNames )
                        where new DatabaseName( (string)databases.Attribute( "Table" ), (string)databases.Attribute( "Schema" ) ).ToString()
                            .EqualsIgnoreCase( principalTableName )
                        select (string)databases.Attribute( "Name" )
                    ).SingleOrDefault();

                    if ( databaseTableName != null )
                    {
                        // The table already exists in the database.
                        // Enumerate all of the PrincipalColumns and add them to the operation.
                        var databaseTable = targetModel.Descendants( EdmXNames.Ssdl.EntityTypeNames )
                            .Single( database =>
                            (
                                (string)database.Attribute( "Name" ) )
                                    .EqualsIgnoreCase( databaseTableName )
                            )
                        ;

                        databaseTable.Descendants( EdmXNames.Ssdl.PropertyRefNames ).Each( column => operation.PrincipalColumns.Add( (string)column.Attribute( "Name" ) ) );
                    }
                    else
                    {
                        // The table does not exist in the database.
                        // Try to find the table in the migration operations.
                        CreateTableOperation table = operations.OfType<CreateTableOperation>()
                            .SingleOrDefault( createTableOperation => createTableOperation.Name.EqualsIgnoreCase( principalTableName ) )
                        ;

                        if ( ( table != null ) && ( table.PrimaryKey != null ) )
                        {
                            // Enumerate all of the PrincipalColumns and add them to the operation
                            table.PrimaryKey.Columns.Each( column => operation.PrincipalColumns.Add( column ) );
                        }
                        else
                        {
                            // The table does not exist in the database or in the migrations
                            throw new Exception( string.Format( "Table {0} does not exist in database {1} or in the current migration operation.", principalTableName, sqlConnection.Database ) );
                        }
                    }
                }
            }

            /// <summary>
            /// Formats a database table name to a <code>Schema.TableName</code> pattern.
            /// Schema name is optional; if no schema is specified then the
            /// default schema from the <see cref="SqlConnection"/> is assumed.
            /// </summary>
            /// <param name="tableName">The name of the table, standardized or not.</param>
            /// <returns>A standardized database table name.</returns>
            private string GetStandardizedTableName( string tableName )
            {
                DebugCheck.NotNullOrWhiteSpace( tableName );

                var databaseName = DatabaseName.Parse( tableName );

                if ( !string.IsNullOrWhiteSpace( databaseName.Schema ) )
                {
                    return tableName;
                }

                return new DatabaseName( tableName, DefaultSchema ).ToString();
            }

            #endregion

            /// <summary>
            /// A representation of a (sanitized) database table name
            /// and optionally a (sanitized) database schema name.
            /// </summary>
            /// <remarks>
            /// Adapted from <see cref="System.Data.Entity.Utilities.DatabaseName"/>.
            /// </remarks>
            private class DatabaseName
            {
                #region Member fields

                /// <summary>
                /// Immutable field representing the name of the database.
                /// </summary>
                private readonly string _name;

                /// <summary>
                /// Immutable field representing the name of the schema of the database.
                /// </summary>
                private readonly string _schema;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="DatabaseName"/> class.
                /// </summary>
                /// <param name="name">The name of the database.</param>
                public DatabaseName( string name ) : this( name, null ) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="DatabaseName"/> class.
                /// </summary>
                /// <param name="name">The name of the database.</param>
                /// <param name="schema">The name of the schema of the database.</param>
                public DatabaseName( string name, string schema )
                {
                    _name   = name;
                    _schema = !string.IsNullOrEmpty( schema ) ? schema : null;
                }

                #endregion

                #region Member properties

                /// <summary>
                /// The name of the database.
                /// </summary>
                public string Name
                {
                    get
                    {
                        return _name;
                    }
                }

                /// <summary>
                /// The name of the schema of the database.
                /// </summary>
                public string Schema
                {
                    get
                    {
                        return _schema;
                    }
                }

                #endregion

                #region Public member functions

                /// <summary>
                /// Extracts possibly the table name and schema name
                /// from a <see cref="string"/>.
                /// </summary>
                /// <param name="name">A <see cref="string"/> that possibly contains a table name and schema name.</param>
                /// <returns>A <see cref="DatabaseName"/> object with properties extracted from the input.</returns>
                public static DatabaseName Parse( string name )
                {
                    DebugCheck.NotNullOrWhiteSpace( name );

                    string namePartRegex    = @"(?:(?:\[(?<part{0}>(?:(?:\]\])|[^\]])+)\])|(?<part{0}>[^\.\[\]]+))";
                    Regex namePartExtractor = new Regex
                    (
                        string.Format
                        (
                            CultureInfo.InvariantCulture,
                            @"^{0}(?:\.{1})?$",
                            string.Format
                            (
                                CultureInfo.InvariantCulture,
                                namePartRegex,
                                1
                            ),
                            string.Format
                            (
                                CultureInfo.InvariantCulture,
                                namePartRegex,
                                2
                            )
                        ),
                        RegexOptions.Compiled
                    );
                    Match match             = namePartExtractor.Match( name.Trim() );

                    if ( !match.Success )
                    {
                        throw new ArgumentException( string.Format( "{0} is not a valid database table name.", name ) );
                    }

                    string match1   = match.Groups["part1"].Value.Replace( "]]", "]" );
                    string match2   = match.Groups["part2"].Value.Replace( "]]", "]" );

                    return ( !string.IsNullOrWhiteSpace( match2 ) ? new DatabaseName( match2, match1 ) : new DatabaseName( match1 ) );
                }

                #endregion

                #region Private member functions

                /// <summary>
                /// Escapes a SQL string by placing square brackets around it.
                /// </summary>
                /// <param name="str">The <see cref="string"/> to escape.</param>
                /// <returns>A <see cref="string"/> with square brackets around it.</returns>
                private static string Escape( string str )
                {
                    return
                    (
                        ( str.IndexOfAny( new[] { ']', '[', '.' } ) != -1 )
                            ? '[' + str.Replace( "]", "]]" ) + ']'
                            : str
                    )
                    ;
                }

                #endregion

                #region Object functions

                /// <summary>
                /// Overrides the <see cref="object.ToString"/> function to return
                /// the fully-qualified table name (with schema).
                /// </summary>
                /// <returns>A fully-qualified table name of the form <code>schema.tableName</code>.</returns>
                public override string ToString()
                {
                    var str = Escape( _name );

                    if ( _schema != null )
                    {
                        str = Escape( _schema ) + '.' + str;
                    }

                    return str;
                }

                #endregion
            }
        }
    }
}
