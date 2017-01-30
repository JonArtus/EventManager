﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventManager.Areas.Admin.Models;
using EventManager.DbContexts;
using EventManager.Filters;
using EventManager.Models;
using EventManager.Services;
using WebGrease.Css.Extensions;

namespace EventManager.Controllers
{
    [InvitedUserOnlyFilter]
    public class BookingsController : Controller
    {
        private EventManagerDbContext db = new EventManagerDbContext();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Event);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult New()
        {
            var events = db.Events.Where(m => m.Booking.EventId != m.EventId && m.Date >= DateTime.Now);
            ViewBag.EventId = new SelectList(events, "EventId", "Name");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "EventId,Guest1,Guest2,Guest3")] Booking booking)
        {
            var bookings = db.Bookings.Where(m => m.EventId == booking.EventId);
            var events = db.Events.Where(m => m.Booking.EventId != m.EventId && m.Date >= DateTime.Now);

            if (bookings.Any())
            {
                ViewBag.EventId = new SelectList(events, "EventId", "Name");
                return View(booking);
            }

            var guest1 =
                db.Bookings.Where(
                    b => b.Guest1 == booking.Guest1 || b.Guest1 == booking.Guest2 || b.Guest1 == booking.Guest3);
            var guest2 =
                db.Bookings.Where(
                    b => b.Guest2 == booking.Guest1 || b.Guest2 == booking.Guest2 || b.Guest2 == booking.Guest3);
            var guest3 =
                db.Bookings.Where(
                    b => b.Guest3 == booking.Guest1 || b.Guest3 == booking.Guest2 || b.Guest3 == booking.Guest3);

            if (guest1.Any() || guest2.Any() || guest3.Any())
            {
                ViewBag.EventId = new SelectList(events, "EventId", "Name");
                return View(booking);
            }


            if (ModelState.IsValid)
            {
                BookingService.DisableInvitation(db, booking);
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventId,Guest1,Guest2,Guest3")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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