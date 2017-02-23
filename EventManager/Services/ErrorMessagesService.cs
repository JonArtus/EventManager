﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventManager.DbContexts;
using EventManager.Models;

namespace EventManager.Services
{
    public static class ErrorMessagesService
    {
        public static void AddingErrorMessages(ModelStateDictionary modelState, EventManagerDbContext db, Booking booking)
        {
            if (BookingService.CheckForPreviousBookings(db, booking.Guest1))
            {
                modelState.AddModelError("Guest1", "This email is already booking into another event.");
            }
            if (BookingService.CheckForPreviousBookings(db, booking.Guest2))
            {
                modelState.AddModelError("Guest2", "This email is already booking into another event.");
            }
            if (BookingService.CheckForPreviousBookings(db, booking.Guest3))
            {
                modelState.AddModelError("Guest3", "This email is already booking into another event.");
            }
        }
    }
}