namespace FitnessCenter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exercises",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MuscleGroup = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        MediaUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkoutProgramExercises",
                c => new
                    {
                        WorkoutProgramId = c.Int(nullable: false),
                        ExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.WorkoutProgramId, t.ExerciseId })
                .ForeignKey("dbo.Exercises", t => t.ExerciseId, cascadeDelete: true)
                .ForeignKey("dbo.WorkoutPrograms", t => t.WorkoutProgramId, cascadeDelete: true)
                .Index(t => t.WorkoutProgramId)
                .Index(t => t.ExerciseId);
            
            CreateTable(
                "dbo.WorkoutPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Level = c.String(),
                        DurationInDays = c.Int(nullable: false),
                        TrainerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TrainerId)
                .Index(t => t.TrainerId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Height = c.Double(nullable: false),
                        Weight = c.Double(nullable: false),
                        Goal = c.Int(nullable: false),
                        TrainerId = c.String(maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TrainerId)
                .Index(t => t.TrainerId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProgressEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Weight = c.Double(nullable: false),
                        Measurements = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        TrainerId = c.String(maxLength: 128),
                        Text = c.String(nullable: false),
                        Answer = c.String(),
                        DateAsked = c.DateTime(nullable: false),
                        DateAnswered = c.DateTime(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TrainerId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.UserId)
                .Index(t => t.TrainerId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.SupplementReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SupplementId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Supplements", t => t.SupplementId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.SupplementId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Supplements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ImageUrl = c.String(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        ProductUrl = c.String(nullable: false),
                        Description = c.String(),
                        Availability = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupplementUsages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SupplementId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Notes = c.String(),
                        Dosage = c.String(),
                        DateStarted = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Supplements", t => t.SupplementId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.SupplementId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.WorkoutPlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkoutProgramId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        StrartDate = c.DateTime(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.WorkoutPrograms", t => t.WorkoutProgramId, cascadeDelete: true)
                .Index(t => t.WorkoutProgramId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.WorkoutProgramExercises", "WorkoutProgramId", "dbo.WorkoutPrograms");
            DropForeignKey("dbo.WorkoutPrograms", "TrainerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkoutPlans", "WorkoutProgramId", "dbo.WorkoutPrograms");
            DropForeignKey("dbo.WorkoutPlans", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SupplementReviews", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SupplementUsages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SupplementUsages", "SupplementId", "dbo.Supplements");
            DropForeignKey("dbo.SupplementReviews", "SupplementId", "dbo.Supplements");
            DropForeignKey("dbo.Questions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Questions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Questions", "TrainerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProgressEntries", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "TrainerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkoutProgramExercises", "ExerciseId", "dbo.Exercises");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.WorkoutPlans", new[] { "UserId" });
            DropIndex("dbo.WorkoutPlans", new[] { "WorkoutProgramId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.SupplementUsages", new[] { "UserId" });
            DropIndex("dbo.SupplementUsages", new[] { "SupplementId" });
            DropIndex("dbo.SupplementReviews", new[] { "User_Id" });
            DropIndex("dbo.SupplementReviews", new[] { "SupplementId" });
            DropIndex("dbo.Questions", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Questions", new[] { "TrainerId" });
            DropIndex("dbo.Questions", new[] { "UserId" });
            DropIndex("dbo.ProgressEntries", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "TrainerId" });
            DropIndex("dbo.WorkoutPrograms", new[] { "TrainerId" });
            DropIndex("dbo.WorkoutProgramExercises", new[] { "ExerciseId" });
            DropIndex("dbo.WorkoutProgramExercises", new[] { "WorkoutProgramId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.WorkoutPlans");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.SupplementUsages");
            DropTable("dbo.Supplements");
            DropTable("dbo.SupplementReviews");
            DropTable("dbo.Questions");
            DropTable("dbo.ProgressEntries");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.WorkoutPrograms");
            DropTable("dbo.WorkoutProgramExercises");
            DropTable("dbo.Exercises");
        }
    }
}
