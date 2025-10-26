using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class ProfileViewModel
	{
		public string Id { get; set; }

		[Required]
		[Display(Name ="Име")]
		public string FirstName { get; set; }

        [Required]
        [Display(Name = "Презиме")]
        public string LastName { get; set; }

		[Display(Name ="Висина (cm)")]
		[Range(100,250)]
		public double Height { get; set; }

        [Display(Name = "Телесна маса (kg)")]
        [Range(30,250)]
        public double Weight { get; set; }

		[Display(Name ="Цел")]
		public Goal Goal { get; set; }

		[Display(Name ="Пол")]
		public Gender Gender { get; set; }
    }
}