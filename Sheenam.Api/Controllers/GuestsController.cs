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
using System;
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

        [HttpGet("{id}")]
        public async ValueTask<ActionResult<Guest>> GetGuestByIdAsync(Guid id)
        {
            try
            {
                Guest currentGuest = await this.guestService.RetrieveGuestByIdAsync(id);
                return Ok(currentGuest);
            }
            catch (GuestDependencyException guestDependencyException)
            {
                return InternalServerError(guestDependencyException.InnerException);
            }
            catch (GuestValidationException guestValidationException)
                when (guestValidationException.InnerException is InvalidGuestException)
            {
                return BadRequest(guestValidationException.InnerException);
            }
            catch (GuestValidationException guestValidationException)
                when (guestValidationException.InnerException is NotFoundGuestException)
            {
                return NotFound(guestValidationException.InnerException);
            }
            catch (GuestServiceException guestServiceException)
            {
                return InternalServerError(guestServiceException.InnerException);
            }
        }

        [HttpPut("update")]
        public async ValueTask<ActionResult<Guest>> PutGuestAsync([FromBody] Guest guest)
        {
            try
            {
                Guest updatedGuest =
                    await this.guestService.ModifyGuestAsync(guest);

                return Ok(updatedGuest);
            }
            catch (GuestValidationException guestValidationException)
            {
                return BadRequest(guestValidationException.InnerException);
            }
            catch (GuestDependencyValidationException guestDependencyValidationException)
                when (guestDependencyValidationException.InnerException is AlreadyExistGuestException)

            {
                return Conflict(guestDependencyValidationException.InnerException);
            }
            catch (GuestDependencyValidationException guestDependencyValidationException)
            {
                return BadRequest(guestDependencyValidationException.InnerException);
            }
            catch (GuestDependencyException guestDependencyException)
            {
                return InternalServerError(guestDependencyException.InnerException);
            }
        }

        [HttpDelete("{guestId}")]
        public async ValueTask<ActionResult<Guest>> DeleteGuestByIdAsync(Guid guestId)
        {
            try
            {
                Guest deletedGuest =
                    await this.guestService.RemoveGuestByIdAsync(guestId);

                return Ok(deletedGuest);
            }
            catch (GuestValidationException GuestValidationException)
                when (GuestValidationException.InnerException is NotFoundGuestException)
            {
                return NotFound(GuestValidationException.InnerException);
            }
            catch (GuestValidationException GuestValidationException)
            {
                return BadRequest(GuestValidationException.InnerException);
            }
            catch (GuestDependencyValidationException GuestDependencyValidationException)
            {
                return BadRequest(GuestDependencyValidationException.InnerException);
            }
            catch (GuestDependencyException GuestDependencyException)
            {
                return InternalServerError(GuestDependencyException);
            }
            catch (GuestServiceException GuestServiceException)
            {
                return InternalServerError(GuestServiceException);
            }
        }
    }
}
