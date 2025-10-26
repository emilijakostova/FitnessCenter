using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FitnessCenter.Models
{
    public enum Goal
    {
        [Display(Name = "Намалување тежина")]
        WeightLoss,
        [Display(Name ="Зголемување мускулна маса")]
        MuscleGain,
        [Display(Name ="Подобрување на издржливост")]
        Endurance
    }
    public enum Gender
    {
        [Display(Name ="Машки")]
        Male,
        [Display(Name = "Женски")]
        Female
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName {  get; set; }
        [Range (100,250,ErrorMessage = "Внеси валидна висина (во сантиметри).")]
        public double Height { get; set; }
        [Range(30,250,ErrorMessage = "Внеси валидна телесна маса (во килограми).")]
        public double Weight { get; set; }
        [Required]
        public Goal Goal { get; set; }
        public Gender Gender { get; set; }

        public string TrainerId { get; set; }
        public virtual ApplicationUser Trainer { get; set; }
        public virtual ICollection<ApplicationUser> Clients { get; set; }


        public virtual ICollection<ProgressEntry> ProgressEntries { get; set; }
        public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; }
        public virtual ICollection<SupplementUsage> SupplementUsages { get; set; }
        public virtual ICollection<SupplementReview> Reviews { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ProgressEntry> ProgressEntries { get; set; }
        public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutProgramExercise> WorkoutProgramExercises { get; set; }
        public DbSet<Supplement> Supplements { get; set; }
        public DbSet<SupplementUsage> SupplementUsages { get; set; }
        public DbSet<SupplementReview> SupplementReviews { get; set; }
        public DbSet<Question> Questions { get; set;}
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkoutProgramExercise>().HasKey(wpe => new { wpe.WorkoutProgramId, wpe.ExerciseId });

            modelBuilder.Entity<WorkoutProgramExercise>()
                .HasRequired(wpe => wpe.WorkoutProgram)
                .WithMany(wp => wp.WorkoutProgramExercises)
                .HasForeignKey(wpe => wpe.WorkoutProgramId);

            modelBuilder.Entity<WorkoutProgramExercise>()
                .HasRequired(wpe => wpe.Exercise)
                .WithMany(e => e.WorkoutProgramExercises)
                .HasForeignKey(wpe => wpe.ExerciseId);
        }

        public System.Data.Entity.DbSet<FitnessCenter.Models.ProfileViewModel> ProfileViewModels { get; set; }
    }
}