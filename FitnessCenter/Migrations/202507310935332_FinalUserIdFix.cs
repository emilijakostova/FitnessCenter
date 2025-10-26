namespace FitnessCenter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalUserIdFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SupplementReviews", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.SupplementReviews", new[] { "User_Id" });
            DropColumn("dbo.SupplementReviews", "UserId");
            RenameColumn(table: "dbo.SupplementReviews", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.SupplementReviews", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.SupplementReviews", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.SupplementReviews", "UserId");
            AddForeignKey("dbo.SupplementReviews", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplementReviews", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SupplementReviews", new[] { "UserId" });
            AlterColumn("dbo.SupplementReviews", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.SupplementReviews", "UserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.SupplementReviews", name: "UserId", newName: "User_Id");
            AddColumn("dbo.SupplementReviews", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.SupplementReviews", "User_Id");
            AddForeignKey("dbo.SupplementReviews", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
