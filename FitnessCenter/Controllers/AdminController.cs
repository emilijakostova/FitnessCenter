using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using FitnessCenter.ViewModels;

namespace FitnessCenter.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult AddTrainer()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainer(RegisterViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Height = model.Height,
                Weight = model.Weight,
                Goal = Goal.WeightLoss,
                Gender = model.Gender
            };
            var result = userManager.Create(user, model.Password);
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, "Trainer");
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(model);
        }

        public ActionResult TrainersList()
        {
            var trainerRole = db.Roles.FirstOrDefault(r => r.Name == "Trainer");
            var trainers = db.Users
                .Where(u => u.Roles.Any(r => r.RoleId == trainerRole.Id))
                .OrderBy(u => u.UserName)
                .ToList();
            return PartialView("_TrainerList", trainers);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Dashboard()
        {
            var viewModel = new AdminDashboardViewModel();

            viewModel.TotalUsers = db.Users.Count();

            viewModel.TopSupplements = db.SupplementUsages
                .Include(s=>s.Supplement)
                .GroupBy(p => p.Supplement.Name)
                .Select(g => new SupplementStatistics
                {
                    SupplementName = g.Key,
                    UsageCount = g.Count()
                })
                .OrderByDescending(x => x.UsageCount)
                .Take(5)
                .ToList();

            viewModel.TopPrograms = db.WorkoutPlans
                .Include(p=>p.WorkoutProgram)
                .GroupBy(p=>p.WorkoutProgram.Name)
                .Select(g=>new ProgramStatistics
                { 
                    ProgramName = g.Key,
                    UsageCount = g.Count()
                })
                .OrderByDescending (x => x.UsageCount)
                .Take (5)
                .ToList();

            return View(viewModel);
        }
    }
}