namespace FitnessCenter.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FitnessCenter.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<FitnessCenter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FitnessCenter.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var passwordHasher = new PasswordHasher();

            string[] roles = { "Admin", "Trainer", "Member" };
            foreach(var role in roles)
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }
            if (userManager.FindByEmail("scott.admin@fitness.com") == null)
            {
                var admin1 = new ApplicationUser
                {
                    UserName = "scott.admin@fitness.com",
                    Email = "scott.admin@fitness.com",
                    FirstName = "Scott",
                    LastName = "Admin",
                    Height = 180,
                    Weight = 90,
                    Goal = Goal.WeightLoss,
                    Gender = Gender.Male
                };
                admin1.PasswordHash = passwordHasher.HashPassword("Scott!1");
                userManager.Create(admin1);
                userManager.AddToRole(admin1.Id, "Admin");
            }
            if (userManager.FindByEmail("emma.admin@fitness.com") == null)
            {
                var admin2 = new ApplicationUser
                {
                    UserName = "emma.admin@fitness.com",
                    Email = "emma.admin@fitness.com",
                    FirstName = "Emma",
                    LastName = "Admin",
                    Height = 175,
                    Weight = 70,
                    Goal = Goal.Endurance,
                    Gender = Gender.Female
                };
                admin2.PasswordHash = passwordHasher.HashPassword("Emma!2");
                userManager.Create(admin2);
                userManager.AddToRole(admin2.Id, "Admin");
            }
            if (userManager.FindByEmail("sebastian.trainer@fitness.com") == null)
            {
                var trainer1 = new ApplicationUser
                {
                    UserName = "sebastian.trainer@fitness.com",
                    Email = "sebastian.trainer@fitness.com",
                    FirstName = "Sebastian",
                    LastName = "Trainer",
                    Height = 179,
                    Weight = 97,
                    Goal = Goal.MuscleGain,
                    Gender = Gender.Male
                };
                trainer1.PasswordHash = passwordHasher.HashPassword("Sebastian!1");
                userManager.Create(trainer1);
                userManager.AddToRole(trainer1.Id, "Trainer");
            }
            if (userManager.FindByEmail("louise.trainer@fitness.com") == null)
            {
                var trainer2 = new ApplicationUser
                {
                    UserName = "louise.trainer@fitness.com",
                    Email = "louise.trainer@fitness.com",
                    FirstName = "Louise",
                    LastName = "Trainer",
                    Height = 170,
                    Weight = 67,
                    Goal = Goal.WeightLoss,
                    Gender = Gender.Female
                };
                trainer2.PasswordHash = passwordHasher.HashPassword("Louise!2");
                userManager.Create(trainer2);
                userManager.AddToRole(trainer2.Id, "Trainer");
            }
            context.SaveChanges();
        }
    }
}
