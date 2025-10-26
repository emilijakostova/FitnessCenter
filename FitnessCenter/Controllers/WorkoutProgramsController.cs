using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;

namespace FitnessCenter.Controllers
{
    public class WorkoutProgramsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkoutPrograms
        public ActionResult Index()
        {
            var workoutPrograms = db.WorkoutPrograms.Include(w => w.Trainer);
            return View(workoutPrograms.ToList());
        }

        // GET: WorkoutPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var workoutProgram = db.WorkoutPrograms
                .Include(w => w.WorkoutProgramExercises
                .Select(e=>e.Exercise))
                .FirstOrDefault(w=>w.Id == id);
            ViewBag.Exercises = new SelectList(db.Exercises.ToList(), "Id", "Description");
            return View(workoutProgram);
        }

        // GET: WorkoutPrograms/Create
        public ActionResult Create()
        {
            var trainers = db.Users
                .Where(u=>u.Roles.Any(r=>r.RoleId==db.Roles.FirstOrDefault(role=>role.Name=="Trainer").Id))
                .ToList();
            ViewBag.TrainerId = new SelectList(trainers, "Id", "FirstName");
            return View();
        }

        // POST: WorkoutPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Level,DurationInDays,TargerGender,TrainerId")] WorkoutProgram workoutProgram)
        {
            if (ModelState.IsValid)
            {
                db.WorkoutPrograms.Add(workoutProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TrainerId = new SelectList(db.Users, "Id", "FirstName", workoutProgram.TrainerId);
            return View(workoutProgram);
        }

        // GET: WorkoutPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkoutProgram workoutProgram = db.WorkoutPrograms.Find(id);
            if (workoutProgram == null)
            {
                return HttpNotFound();
            }
            var trainerRole = db.Roles.FirstOrDefault(r => r.Name == "Trainer");
            var trainerIds = trainerRole.Users.Select(u => u.UserId).ToList();
            var trainers = db.Users.Where(u=>trainerIds.Contains(u.Id)).ToList();
            ViewBag.TrainerId = new SelectList(trainers,"Id","FirstName",workoutProgram.TrainerId);
            return View(workoutProgram);
        }

        // POST: WorkoutPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Level,DurationInDays,TargerGender,TrainerId")] WorkoutProgram workoutProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workoutProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TrainerId = new SelectList(db.Users, "Id", "FirstName", workoutProgram.TrainerId);
            return View(workoutProgram);
        }

        // GET: WorkoutPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkoutProgram workoutProgram = db.WorkoutPrograms.Find(id);
            if (workoutProgram == null)
            {
                return HttpNotFound();
            }
            return View(workoutProgram);
        }

        // POST: WorkoutPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkoutProgram workoutProgram = db.WorkoutPrograms.Find(id);
            db.WorkoutPrograms.Remove(workoutProgram);
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

        [HttpPost]
        public ActionResult AddExercise(int WorkoutProgramId, int ExerciseId)
        {
            var exists = db.WorkoutProgramExercises
                .Any(x=>x.WorkoutProgramId==WorkoutProgramId&&x.ExerciseId==ExerciseId);

            if(!exists)
            {
                db.WorkoutProgramExercises.Add(new WorkoutProgramExercise
                {
                    WorkoutProgramId=WorkoutProgramId,
                    ExerciseId=ExerciseId
                });
                db.SaveChanges();
            }
            return RedirectToAction("Details",new {id = WorkoutProgramId});
        }
        public ActionResult RemoveExercise(int workoutProgramId, int exerciseId)
        {
            var entry = db.WorkoutProgramExercises
                .FirstOrDefault(x => x.WorkoutProgramId == workoutProgramId
                                && x.ExerciseId == exerciseId);
            if(entry != null)
            {
                db.WorkoutProgramExercises.Remove(entry);
                db.SaveChanges();
            }
            return RedirectToAction("Details",new {id = workoutProgramId});
        }
    }
}
