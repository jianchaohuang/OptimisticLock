namespace EFCodeFirstOptimistic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        SocialSecurityNumber = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RowVersion = c.Binary(),
                    })
                .PrimaryKey(t => t.PersonId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StuId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Age = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StuId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Students");
            DropTable("dbo.People");
        }
    }
}
