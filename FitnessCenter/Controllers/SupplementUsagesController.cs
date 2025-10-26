using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    public class SupplementUsagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: SupplementUsages
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult AddUsage(int supplementId)
        {
            var usage = new SupplementUsage
            {
                SupplementId = supplementId,
                DateStarted = DateTime.Today,
            };
            return View(usage);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUsage(SupplementUsage usage)
        {
            if (ModelState.IsValid)
            {
                usage.UserId = User.Identity.GetUserId();
                db.SupplementUsages.Add(usage);
                db.SaveChanges();
                return RedirectToAction("MyUsages");
            }
            return View(usage);
        }

        [Authorize]
        public ActionResult MyUsages()
        {
            string userId = User.Identity.GetUserId();
            var usages = db.SupplementUsages
                .Where(u=>u.UserId == userId)
                .Include(u=>u.Supplement.Reviews)
                .Include(u=>u.Supplement)
                .OrderByDescending(u=>u.DateStarted)
                .ToList();
            return View(usages);
        }

        [Authorize]
        public ActionResult Edit (int id)
        {
            var usage = db.SupplementUsages.Find(id);
            if(usage == null||usage.UserId!=User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            ViewBag.SupplementName = usage.Supplement?.Name;
            return View(usage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(SupplementUsage model)
        {
            if (ModelState.IsValid)
            {
                var usage = db.SupplementUsages.Find(model.Id);
                if (usage == null || usage.UserId != User.Identity.GetUserId())
                    return HttpNotFound();
                usage.Notes = model.Notes;
                usage.Dosage = model.Dosage;
                usage.DateStarted = model.DateStarted;
                db.SaveChanges();
                return RedirectToAction("MyUsages");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            var usage = db.SupplementUsages.Find(id);
            if (usage == null || usage.UserId != User.Identity.GetUserId())
                return HttpNotFound();
            return View(usage);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var usage = db.SupplementUsages.Find(id);
            if(usage==null || usage.UserId != User.Identity.GetUserId())
                return HttpNotFound();
            db.SupplementUsages.Remove(usage);
            db.SaveChanges();
            return RedirectToAction("MyUsages");
        }
    }
}