using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class WorkoutProgramExercise
	{
		public int WorkoutProgramId { get; set; }
		public virtual WorkoutProgram WorkoutProgram { get; set; }

		public int ExerciseId { get; set; }
		public virtual Exercise Exercise { get; set; }
	}
}