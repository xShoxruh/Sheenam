//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Sheenam.Api.Models.Foundations.Guests.Exceptions;
using Sheenam.Api.Models.Foundations.Guests;
using Sheenam.Api.Services.Foundations.Guests;
using System.Linq;
using System.Net.Sockets;

namespace Sheenam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : RESTFulController
    {
        private readonly IGuestService guestService;
        public GuestsController(IGuestService guestService)
        {
            this.guestService = guestService;
        }

        [HttpPost]
        public async ValueTask<ActionResult<Guest>> PostGuestAsync(Guest guest)
        {
            try
            {
                Guest postedGuest = await this.guestService.AddGuestAsync(guest);

                return Created(postedGuest);
            }
            catch (GuestValidationException guestValidationException)
            {
                return BadRequest(guestValidationException.InnerException);
            }
            catch (GuestDependencyValidationException guestDependenceValidationException)
                when (guestDependenceValidationException.InnerException is AlreadyExistGuestException)
            {
                return Conflict(guestDependenceValidationException.InnerException);
            }
            catch (GuestDependencyValidationException guestDependenceValidationException)
            {
                return BadRequest(guestDependenceValidationException.InnerException);
            }
            catch (GuestDependencyException guestDependenceException)
            {
                return InternalServerError(guestDependenceException.InnerException);
            }
            catch (GuestServiceException guestServiceException)
            {
                return InternalServerError(guestServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Guest>> GetAllGuests()
        {
            try
            {
                IQueryable<Guest> allGuests = this.guestService.RetrieveAllGuests();

                return Ok(allGuests);
            }
            catch (GuestDependencyException guestDependencyException)
            {
                return InternalServerError(guestDependencyException.InnerException);
            }
            catch (GuestServiceException guestServiceException)
            {
                return InternalServerError(guestServiceException.InnerException);
            }
        }
    }
}
