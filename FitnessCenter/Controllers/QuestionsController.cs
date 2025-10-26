using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //Клиентот ја гледа формата за поставување на прашање
        [Authorize(Roles ="Member")]
        public ActionResult Ask()
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.Include(u=>u.Trainer).FirstOrDefault(u=>u.Id== userId);
            if (member == null || string.IsNullOrEmpty(member.TrainerId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            ViewBag.TrainerName = member.Trainer.FirstName+" "+member.Trainer.LastName;
            return View();
        }

        //Членот поставува прашање
        [HttpPost]
        [Authorize(Roles ="Member")]
        [ValidateAntiForgeryToken]
        public ActionResult Ask(string Text)
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.FirstOrDefault(u => u.Id == userId);
            if(member == null || string.IsNullOrEmpty (member.TrainerId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var question = new Question
            {
                Text = Text,
                UserId = userId,
                TrainerId = member.TrainerId,
                DateAsked = DateTime.Now,
            };
            db.Questions.Add(question);
            db.SaveChanges();

            return RedirectToAction("MyQuestions");
        }

        //Членот ги гледа сопствените прашања
        [Authorize(Roles ="Member")]
        public ActionResult MyQuestions()
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.Include(u => u.Trainer).FirstOrDefault(u => u.Id == userId);
            if(member==null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if(member.Trainer == null)
            {
                ViewBag.TrainerName = "Сè уште немате назначен тренер.";
            }
            else
            {
                ViewBag.TrainerName = member.Trainer.FirstName + " " + member.Trainer.LastName;
            }

            var questions = db.Questions
                .Where(q=>q.UserId == userId)
                .OrderByDescending(q=>q.DateAsked)
                .ToList();
            return View(questions);
        }

        //Тренерот ги гледа прашањата од сопствените клиенти
        [Authorize(Roles ="Trainer")]
        public ActionResult ClientQuestions()
        {
            var trainerId = User.Identity.GetUserId();
            var questions = db.Questions
                .Include(q=>q.User)
                .Where(q=>q.TrainerId == trainerId)
                .OrderByDescending (q=>q.DateAsked)
                .ToList();
            return View(questions);
        }

        //Тренерот одговара на прашањето
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Trainer")]
        public ActionResult Answer(int id,string Answer)
        {
            var trainerId = User.Identity.GetUserId();
            var question = db.Questions.FirstOrDefault(q=>q.Id == id && q.TrainerId == trainerId);
            if(question == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            question.Answer = Answer;
            question.DateAnswered = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("ClientQuestions");
        }
    }
}