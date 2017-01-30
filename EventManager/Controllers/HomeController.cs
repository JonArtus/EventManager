﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventManager.Areas.Admin.Models;
using EventManager.DbContexts;
using EventManager.Filters;
using EventManager.Models;

namespace EventManager.Controllers
{
    
    public class HomeController : Controller
    {
        private EventManagerDbContext db = new EventManagerDbContext();

        public ActionResult Index()
        {
            if (Request.Cookies["UserEmail"] != null)
            {
                string email = Request.Cookies["UserEmail"]?.Value;
                Booking booking = db.Bookings.First(b => b.Guest1 == email || b.Guest2 == email || b.Guest3 == email);
                return RedirectToAction("Details", "Bookings", new { id = booking.EventId });
            }
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string userEmail, string invitationCode)
        {
            var invitation = db.Invitations.Where(i => i.Email == userEmail);
            if (invitation.Any() && invitation.First().InvitationCode == invitationCode)
            {
                Response.Cookies.Add(new HttpCookie("UserEmail", userEmail));
                if (db.Bookings.Any(b => b.Guest1 == userEmail || b.Guest2 == userEmail || b.Guest3 == userEmail))
                {
                    Booking booking = db.Bookings.First(b => b.Guest1 == userEmail || b.Guest2 == userEmail || b.Guest3 == userEmail);
                    return RedirectToAction("Details", "Bookings", new {id = booking.EventId});
                }
                return RedirectToAction("Upcoming");
            }
            return RedirectToAction("Index");
        }

        [InvitedUserOnlyFilter]
        public ActionResult Upcoming()
        {
            ViewBag.UnbookedEvents = db.Events.Where(e => e.Booking == null).ToList();
            return View(db.Events.Where(e => e.Date > DateTime.Now ).OrderBy(e => e.Date));
        }

       
    }
}