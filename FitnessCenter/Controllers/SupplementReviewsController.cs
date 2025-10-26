using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    public class SupplementReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: SupplementReviews
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Member")]
        public ActionResult SubmitReview(SupplementReview review)
        {
            review.UserId = User.Identity.GetUserId();
            db.SupplementReviews.Add(review);
            db.SaveChanges();
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        [Authorize(Roles="Member")]
        public ActionResult DeleteReview(int supplementId)
        {
            string userId = User.Identity.GetUserId();
            var review = db.SupplementReviews
                .FirstOrDefault(r=>r.SupplementId== supplementId && r.UserId==userId);
            if(review==null)
            {
                return HttpNotFound();
            }
            db.SupplementReviews .Remove(review);
            db.SaveChanges();
            return new HttpStatusCodeResult(200);
        }
    }
}