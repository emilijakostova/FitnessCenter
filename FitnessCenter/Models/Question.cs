using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class Question
	{
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }

		public string TrainerId { get; set; }
		public virtual ApplicationUser Trainer { get; set; }

		[Required]
		public string Text {  get; set; }
		public string Answer {  get; set; }

		public DateTime DateAsked { get; set; }
		public DateTime? DateAnswered { get; set; }
	}
}