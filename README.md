# FluentRockMigration
A shim to enable Entity Framework fluent-style API for migrations in Rock RMS

<a href="https://github.com/SparkDevNetwork/Rock">Rock RMS</a>
currently (version 1.7.3) does not include a way to use the Entity Framework fluent-style API for
`Rock.Plugin.Migration` in the same way it is used in `Rock.Migrations.Migrations.RockMigration`.
This shim circumvents the way plugin migrations are normally applied in order to allow the
fluent-style API that Entity Framework supplies.

## NOTICE ABOUT COPYRIGHT
This project re-implements content in
<a href="https://github.com/aspnet/EntityFramework6">Entity Framework 6</a>,
created by Microsoft and licensed under the Apache 2.0 license.
Re-implemented classes are marked with a remark about the class in Entity Framework
that they reference. Changes to these classes include but are not limited to
additional comments and variable name changes with the intent of adding clarity to the code.

## Installation
1. Use an existing Rock plugin project, or create a new one as the Rock documentation describes.

2. Add this project to the solution containing your plugin project(s).

3. In your plugin project(s) (e.g. `org.rocksolidchurch.SampleProject`),
add a reference to this project (`org.christchapelbc.Utility`).

4. Change your migrations to inherit from `org.christchapelbc.Utility.Migrations.FluentMigration`
instead of `Rock.Plugin.Migration`. The `Rock.Plugin.MigrationNumberAttribute` must still be used.

5. Call `base.Up()` and `base.Down()` at the <b>end</b> of the migrations' `Up()` and `Down()`
methods, respectively. This is what actually executes the migration operations.

## Example
Model
```C#
#region Using statements

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Rock.Data;
using Rock.Model;

#endregion

namespace org.christchapelbc.Goals.Models
{
    /// <summary>
    /// Represents a Goal that a <see cref="Rock.Model.Person"/> would have.
    /// </summary>
    public class Goal : Model<Goal>, IRockEntity
    {
        #region Entity properties

        /// <summary>
        /// The primary key of this <see cref="Goal"/>.
        /// </summary>
        [Key]
        [Required]
        public new int Id { get; set; }

        /// <summary>
        /// A foreign key pointing to the <see cref="Rock.Model.Person"/> who has this <see cref="Goal"/>.
        /// </summary>
        [ForeignKey( "Person" )]
        [Required]
        public int PersonId { get; set; }   // e.g. Goal.PersonId returns 1

        /// <summary>
        /// A short title for this <see cref="Goal"/>.
        /// </summary>
        [Required]
        [StringLength( 50 )]
        public string GoalTitle { get; set; }

        /// <summary>
        /// A breif, but more detailed, description of this <see cref="Goal"/>.
        /// </summary>
        [StringLength( 255 )]
        public string GoalDescription { get; set; }

        #endregion

        #region Virtual entity properties

        /// <summary>
        /// The <see cref="Rock.Model.Person"/> object with a primary key of <see cref="PersonId"/>.
        /// </summary>
        public virtual Person Person { get; set; }  // e.g. Goal.Person.FullName returns "John Doe"

        #endregion
    }
}

```

Migration
```C#
#region Using statements

using Rock.Plugin;

using org.christchapelbc.Utility.Migrations;

#endregion

namespace org.christchapelbc.Goals.Migrations
{
    [MigrationNumber( 1, "1.7.3" )]
    public partial class CreateGoalTable : FluentMigration
    {
        #region Member fields

        private static readonly string tableNameGoal    = "dbo._org_christchapelbc_Goals_Goal";
        private static readonly string tableNamePerson  = "dbo.Person";

        private readonly string pk_goal_id = string.Format( "PK_{0}", tableNameGoal );

        private readonly string fk_goal_personId_to_person_id = string.Format( "FK_{0}_{1}_Id", tableNameGoal, tableNamePerson );

        private readonly string ix_goal_personId = "IX_PersonId";

        #endregion

        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            CreateTable
            (
                tableNameGoal, c => new
                {
                    Id              = c.Int( nullable: false, identity: true ),
                    PersonId        = c.Int( nullable: false ),
                    GoalTitle       = c.String( maxLength: 50, nullable: false ),
                    GoalDescription = c.String( maxLength: 255 )
                }
            )
                .PrimaryKey
                (
                    keyExpression:  t => t.Id,
                    name:           pk_goal_id
                )
                .Index
                (
                    indexExpression:    t => t.PersonId,
                    name:               ix_goal_personId
                )
                .ForeignKey
                (
                    principalTable:         tableNamePerson,
                    dependentKeyExpression: t => t.PersonId,
                    name:                   fk_goal_personId_to_person_id,
                    cascadeDelete:          true
                )
                
            ;

            // Apply the migration operations
            base.Up();
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropIndex( tableNameGoal, ix_goal_personId );
            DropForeignKey( tableNameGoal, fk_goal_personId_to_person_id );
            DropTable( tableNameGoal );

            // Apply the migration operations
            base.Down();
        }
    }
}
```
