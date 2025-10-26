using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class SupplementReview
	{
		public int Id { get; set; }

		[Required]
		public int SupplementId { get; set; }
		public virtual Supplement Supplement { get; set; }

		[Required]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }

		[Required]
		[Range (1, 5, ErrorMessage = "Оцената мора да биде помеѓу 1 и 5.")]
		public int Rating { get; set; }
		public string Comment { get; set; }
	}
}