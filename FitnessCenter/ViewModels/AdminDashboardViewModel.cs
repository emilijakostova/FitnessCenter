using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FitnessCenter.Models;

namespace FitnessCenter.ViewModels
{
	public class AdminDashboardViewModel
	{
		public int TotalUsers { get; set; }
		public List<SupplementStatistics> TopSupplements { get; set; }
		public List<ProgramStatistics> TopPrograms { get; set; }
	}
	public class SupplementStatistics
	{
		public string SupplementName { get; set; }
		public int UsageCount { get; set; }
	}
	public class ProgramStatistics
	{
		public string ProgramName { get; set; }
		public int UsageCount { get; set; }
	}
}