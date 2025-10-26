using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        // GET: Profile
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string id = null)
        {
            string currentUserId = User.Identity.GetUserId();
            string targetUserId = id ?? currentUserId;

            var user = db.Users.Include("ProgressEntries").FirstOrDefault(u=>u.Id==targetUserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            var lastEntry = user.ProgressEntries
                .OrderByDescending(pe=>pe.Date)
                .FirstOrDefault();
            if(lastEntry != null&&user.Weight!=lastEntry.Weight)
            {
                user.Weight = lastEntry.Weight;
                db.SaveChanges();
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                Gender = user.Gender,
            };
            return View(model);
        }
        [Authorize(Roles = "Member")]
        public ActionResult Edit()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var model = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                Gender = user.Gender,
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]        
        [Authorize(Roles ="Member")]
        public ActionResult Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var user = db.Users.Find(model.Id);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName; 
            if (User.IsInRole("Member"))
            {
                user.Height = model.Height;
                user.Weight = model.Weight;
                user.Goal = model.Goal;
                user.Gender = model.Gender;
            }
            

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}