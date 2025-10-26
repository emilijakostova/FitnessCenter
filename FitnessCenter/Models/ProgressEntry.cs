using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class ProgressEntry
	{
		public int Id { get; set; }
		[Required]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }
		[Required]
		[DataType(DataType.Date)]
		public DateTime Date { get; set; }
		[Required]
		[Range(30,250,ErrorMessage = "Внеси валидна телесна маса (во килограми)")]
		public double Weight { get; set; }
		[Display (Prompt = "Гради,Струк,Рака -> e.g. 95,80,32")]
		public string Measurements { get; set; }
	}
}