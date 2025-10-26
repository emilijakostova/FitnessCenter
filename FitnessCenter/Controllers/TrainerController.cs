using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    [Authorize(Roles ="Trainer")]
    public class TrainerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult AssignClients()
        {
            var trainerId = User.Identity.GetUserId();

            var availableClients = db.Users
                .Where(u=>u.Roles.Any(r=>r.RoleId==db.Roles.FirstOrDefault(role=>role.Name=="Member").Id)
                && u.TrainerId ==null)
                .ToList();

            ViewBag.Clients = new SelectList(availableClients, "Id", "Email");
            return View();
        }
        [HttpPost]
        public ActionResult AddClient(string clientId)
        {
            var trainerId = User.Identity.GetUserId();
            var client = db.Users.Find(clientId);
            if (client!=null&&client.TrainerId==null)
            {
                client.TrainerId = trainerId;
                db.SaveChanges();
            }
            return PartialView("_MyClientsPartial", GetMyClients(trainerId));
        }
        public PartialViewResult GetMyClientsPartial()
        {
            var trainerId = User.Identity.GetUserId();
            return PartialView("_MyClientsPartial",GetMyClients(trainerId));
        }
        private List<ApplicationUser> GetMyClients(string trainerId)
        {
            return db.Users
                .Where(u=>u.TrainerId == trainerId)
                .ToList();
        }
        [HttpPost]
        [Authorize(Roles ="Trainer")]
        public ActionResult RemoveClient(string clientId)
        {
            var client = db.Users.Find(clientId);
            var trainerId = User.Identity.GetUserId() ;
            if(client==null||client.TrainerId!=trainerId)
            {
                return HttpNotFound();
            }
            client.TrainerId = null;
            db.SaveChanges();
            return RedirectToAction("AssignClients");
        }
    }
}