using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessCenter.Models
{
	public class Supplement
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public decimal? Price { get; set; }
		[Required]
		[Url]
		public string ProductUrl {  get; set; }
		public string Description { get; set; }
		public bool Availability { get; set; }

		public virtual ICollection<SupplementUsage> Usages { get; set; }
		public virtual ICollection<SupplementReview> Reviews { get; set; }
    }
}