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
    public class ProgressEntriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProgressEntries
        [Authorize(Roles ="Member")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var progressEntries = db.ProgressEntries
                .Include(p => p.User)
                .Where(p=>p.UserId==userId)
                .OrderByDescending(p=>p.Date)
                .ToList();
            return View(progressEntries);
        }

        // GET: ProgressEntries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgressEntry progressEntry = db.ProgressEntries.Include(p=>p.User).FirstOrDefault(p=>p.Id==id);
            if (progressEntry == null)
            {
                return HttpNotFound();
            }
            return View(progressEntry);
        }

        // GET: ProgressEntries/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: ProgressEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Date,Weight,Measurements")] ProgressEntry progressEntry)
        {
            if (ModelState.IsValid)
            {
                progressEntry.UserId = User.Identity.GetUserId();
                db.ProgressEntries.Add(progressEntry);
                db.SaveChanges();
                var user = db.Users.Find(progressEntry.UserId);
                if (user!=null && user.Weight!=progressEntry.Weight)
                {
                    user.Weight = progressEntry.Weight;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", progressEntry.UserId);
            return View(progressEntry);
        }

        // GET: ProgressEntries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgressEntry progressEntry = db.ProgressEntries.Find(id);
            if (progressEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", progressEntry.UserId);
            return View(progressEntry);
        }

        // POST: ProgressEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Date,Weight,Measurements")] ProgressEntry progressEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(progressEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", progressEntry.UserId);
            return View(progressEntry);
        }

        // GET: ProgressEntries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgressEntry progressEntry = db.ProgressEntries.Include(p => p.User).FirstOrDefault(p=>p.Id==id);
            if (progressEntry == null)
            {
                return HttpNotFound();
            }
            return View(progressEntry);
        }

        // POST: ProgressEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgressEntry progressEntry = db.ProgressEntries.Find(id);
            db.ProgressEntries.Remove(progressEntry);
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

        [Authorize(Roles ="Member")]
        public ActionResult Chart()
        {
            var userId = User.Identity.GetUserId();

            var progressEntries = db.ProgressEntries
                .Where(p => p.UserId == userId)
                .OrderBy(p=>p.Date)
                .ToList();
            return View(progressEntries);
        }
    }
}
