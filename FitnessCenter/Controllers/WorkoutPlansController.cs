using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    public class WorkoutPlansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkoutPlans
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            IQueryable<WorkoutPlan> workoutPlans = db.WorkoutPlans
                .Include(w=>w.User).Include(w=>w.WorkoutProgram);
            if(User.IsInRole("Member"))
            {
                workoutPlans = workoutPlans.Where(w=>w.UserId == currentUserId);
            }
            else if(User.IsInRole("Trainer"))
            {
                workoutPlans=workoutPlans.Where(w=>w.User.TrainerId== currentUserId);
            }
            return View(workoutPlans.ToList());
        }

        // GET: WorkoutPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkoutPlan workoutPlan = db.WorkoutPlans.Find(id);
            if (workoutPlan == null)
            {
                return HttpNotFound();
            }
            return View(workoutPlan);
        }

        // GET: WorkoutPlans/Create
        [Authorize(Roles ="Member")]
        public ActionResult Create()
        {
            var currentUserId = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserId);

            if(user.TrainerId == null)
            {
                TempData["Error"] = "Немате назначен тренер.";
                return RedirectToAction("Index");
            }

            var programs = db.WorkoutPrograms
                .Where(wp => wp.TrainerId == user.TrainerId && wp.TargerGender == user.Gender)
                .ToList();

            ViewBag.WorkoutProgramId = new SelectList(db.WorkoutPrograms, "Id", "Name");
            ViewBag.UserFullName = user.FirstName + " " + user.LastName;

            return View(new WorkoutPlan { UserId = currentUserId});
        }

        // POST: WorkoutPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Member")]
        public ActionResult Create([Bind(Include = "Id,WorkoutProgramId,UserId,StrartDate,Notes")] WorkoutPlan workoutPlan)
        {
            if (ModelState.IsValid)
            {
                db.WorkoutPlans.Add(workoutPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var currentUserId = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserId);

            var programs = db.WorkoutPrograms
                .Where(wp => wp.TrainerId==user.TrainerId && wp.TargerGender==user.Gender)
                .ToList();

            ViewBag.WorkoutProgramId = new SelectList(db.WorkoutPrograms, "Id", "Name", workoutPlan.WorkoutProgramId);
            return View(workoutPlan);
        }

        // GET: WorkoutPlans/Edit/5
        [Authorize(Roles ="Trainer,Member")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var workoutPlan = db.WorkoutPlans.Include(w => w.WorkoutProgram).FirstOrDefault(w => w.Id == id);
            if (workoutPlan == null)
            {
                return HttpNotFound();
            }
            var currentUserId = User.Identity.GetUserId();

            if(User.IsInRole("Member"))
            {
                if(workoutPlan.UserId != currentUserId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            else if(User.IsInRole("Trainer"))
            {
                var member = db.Users.Find(workoutPlan.UserId);
                if(member.TrainerId!=currentUserId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                ViewBag.WorkoutProgramId = new SelectList(
                    db.WorkoutPrograms.Where(p => p.TrainerId == currentUserId),
                    "Id",
                    "Name",
                    workoutPlan.WorkoutProgramId
                    );
            }

            if(User.IsInRole("Member"))
            {
                ViewBag.WorkoutProgramId = new SelectList(
                    new[] { workoutPlan.WorkoutProgram },
                    "Id",
                    "Name",
                    workoutPlan.WorkoutProgramId
                );
            }

            var user = db.Users.Find(workoutPlan.UserId);
            ViewBag.UserFullName = user != null ? user.FirstName + " " + user.LastName : "";

            return View(workoutPlan);
        }

        // POST: WorkoutPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Trainer,Member")]
        public ActionResult Edit([Bind(Include = "Id,WorkoutProgramId,UserId,StrartDate,Notes")] WorkoutPlan workoutPlan)
        {
            var currentUserId = User.Identity.GetUserId();

            var existingPlan = db.WorkoutPlans.Include(w => w.WorkoutProgram).FirstOrDefault(w => w.Id == workoutPlan.Id);
            if (existingPlan == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Member"))
            {
                if (existingPlan.UserId != currentUserId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                workoutPlan.WorkoutProgramId = existingPlan.WorkoutProgramId;
                workoutPlan.UserId = existingPlan.UserId;
            }
            else if(User.IsInRole("Trainer"))
            {
                var member = db.Users.Find(workoutPlan.UserId);
                if (member.TrainerId != currentUserId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(existingPlan).CurrentValues.SetValues(workoutPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if(User.IsInRole("Trainer"))
            {
                ViewBag.WorkoutProgramId = new SelectList(
                    db.WorkoutPrograms.Where(p => p.TrainerId == currentUserId),
                    "Id",
                    "Name",
                    workoutPlan.WorkoutProgramId
                    );
            }
            else if(User.IsInRole("Member"))
            {
                ViewBag.WorkoutProgramId = new SelectList(
                    new[] {existingPlan.WorkoutProgram},
                    "Id",
                    "Name",
                    workoutPlan.WorkoutProgramId
                    );
            }

            var user = db.Users.Find(workoutPlan.UserId);
            ViewBag.UserFullName = user != null ? user.FirstName + " " + user.LastName : "";

            return View(workoutPlan);
        }

        // GET: WorkoutPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkoutPlan workoutPlan = db.WorkoutPlans.Find(id);
            if (workoutPlan == null)
            {
                return HttpNotFound();
            }
            return View(workoutPlan);
        }

        // POST: WorkoutPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkoutPlan workoutPlan = db.WorkoutPlans.Find(id);
            db.WorkoutPlans.Remove(workoutPlan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
