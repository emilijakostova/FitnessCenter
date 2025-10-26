using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class WorkoutProgram
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		public string Level { get; set; }
		[Range(1,7)]
		public int DurationInDays { get; set; }
		[Required]
		public Gender TargerGender { get; set; }

		public string TrainerId { get; set; }
		public virtual ApplicationUser Trainer { get; set; }

		public virtual ICollection<WorkoutProgramExercise> WorkoutProgramExercises { get; set; }
	}
}