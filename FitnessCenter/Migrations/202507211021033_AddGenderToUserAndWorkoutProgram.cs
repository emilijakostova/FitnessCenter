namespace FitnessCenter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGenderToUserAndWorkoutProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkoutPrograms", "TargerGender", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Gender", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Gender");
            DropColumn("dbo.WorkoutPrograms", "TargerGender");
        }
    }
}
