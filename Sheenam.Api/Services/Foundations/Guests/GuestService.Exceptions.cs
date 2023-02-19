//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Sheenam.Api.Models.Foundations.Guests;
using Sheenam.Api.Models.Foundations.Guests.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.Guests
{
    public partial class GuestService
    {
        private delegate ValueTask<Guest> ReturningGuestFunction();
        private delegate IQueryable<Guest> ReturningGuestsFunctions();

        private async ValueTask<Guest> TryCatch(ReturningGuestFunction returningGuestFunction)
        {
            try
            {
                return await returningGuestFunction();
            }
            catch (NullGuestException nullGuestException)
            {
                throw CreateAndLogValidationException(nullGuestException);
            }
            catch (InvalidGuestException invalidGuestException)
            {
                throw CreateAndLogValidationException(invalidGuestException);
            }
            catch (SqlException sqlException)
            {
                var failedGuestStorageException = new FailedGuestStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedGuestStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistGuestException =
                    new AlreadyExistGuestException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistGuestException);
            }
            catch (Exception exception)
            {
                var failedGuestServiceException =
                    new FailedGuestServiceException(exception);

                throw CreateAndLogServiceException(failedGuestServiceException);
            }
        }

        private IQueryable<Guest> TryCatch(ReturningGuestsFunctions returningGuestsFunctions)
        {
            try
            {
                return returningGuestsFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedGuestServiceException = 
                    new FailedGuestServiceException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedGuestServiceException);
            }
            catch (Exception serviException)
            {
                var failedServiceGuestException = 
                    new FailedGuestServiceException(serviException);

                throw CreateAndLogServiceException(failedServiceGuestException);
            }
        }

        private GuestValidationException CreateAndLogValidationException(Xeption exception)
        {
            var guestValidationException =
                   new GuestValidationException(exception);

            this.loggingBroker.LogError(guestValidationException);
            return guestValidationException;
        }

        private GuestDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var guestDependencyException = 
                new GuestDependencyException(exception);

            this.loggingBroker.LogCritical(guestDependencyException);
            return guestDependencyException;
        }

        private GuestDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var guestDependencyValidationException =
                new GuestDependencyValidationException(exception);

            this.loggingBroker.LogError(guestDependencyValidationException);
            return guestDependencyValidationException;
        }

        private GuestServiceException CreateAndLogServiceException(Xeption exception)
        {
            var guestServiceException = 
                new GuestServiceException(exception);

            this.loggingBroker.LogError(guestServiceException);
            return guestServiceException;
        }
    }
}
