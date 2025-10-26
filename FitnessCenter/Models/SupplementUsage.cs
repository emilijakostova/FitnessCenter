using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class SupplementUsage
	{
		public int Id { get; set; }

		public int SupplementId { get; set; }
		public virtual Supplement Supplement { get; set; }

		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }

		public string Notes { get; set; }
		public string Dosage { get; set; }
		public DateTime DateStarted { get; set; }
	}
}