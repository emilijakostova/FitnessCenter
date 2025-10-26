using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class WorkoutPlan
	{
		public int Id { get; set; }

		[Required]
		public int WorkoutProgramId { get; set; }
		public virtual WorkoutProgram WorkoutProgram { get; set; }

		[Required]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }

		[Required]
		public DateTime StrartDate { get; set; }
		public string Notes { get; set; }
	}
}