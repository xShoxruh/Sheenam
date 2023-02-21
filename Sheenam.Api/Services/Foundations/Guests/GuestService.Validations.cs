﻿//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using System;
using System.Data;
using System.Reflection.Metadata;
using Sheenam.Api.Models.Foundations.Guests;
using Sheenam.Api.Models.Foundations.Guests.Exceptions;

namespace Sheenam.Api.Services.Foundations.Guests
{
    public partial class GuestService
    {
        private void ValidateGuestOnAdd(Guest guest)
        {
            ValidationGuestNotNull(guest);

            Validate(
                (Rule: IsInvalid(guest.Id), Parameter: nameof(Guest.Id)),
                (Rule: IsInvalid(guest.FirstName), Parameter: nameof(Guest.FirstName)),
                (Rule: IsInvalid(guest.LastName), Parameter: nameof(Guest.LastName)),
                (Rule: IsInvalid(guest.DateOfBirth), Parameter: nameof(Guest.DateOfBirth)),
                (Rule: IsInvalid(guest.Email), Parameter: nameof(Guest.Email)),
                (Rule: IsInvalid(guest.Address), Parameter: nameof(Guest.Address)),
                (Rule: IsInvalid(guest.Gender), Parameter: nameof(Guest.Gender)));
        }

        private void ValidateGuestOnModify(Guest guest)
        {
            ValidationGuestNotNull(guest);

            Validate(
              (Rule: IsInvalid(guest.Id), Parameter: nameof(guest.Id)),
              (Rule: IsInvalid(guest.FirstName), Parameter: nameof(guest.FirstName)),
              (Rule: IsInvalid(guest.LastName), Parameter: nameof(guest.LastName)),
              (Rule: IsInvalid(guest.DateOfBirth), Parameter: nameof(guest.DateOfBirth)),
              (Rule: IsInvalid(guest.Email), Parameter: nameof(guest.Email)),
              (Rule: IsInvalid(guest.Address), Parameter: nameof(guest.Address)),
              (Rule: IsInvalid(guest.Gender), Parameter: nameof(guest.Gender)),
              (Rule: IsSame(
                    firstDate: guest.UpdatedDate,
                    secondDate: guest.CreatedDate,
                    secondDateName: nameof(Guest.CreatedDate)),
                Parameter: nameof(Guest.UpdatedDate)));
        }

        private void ValidateGuestId(Guid guestId) =>
            Validate((Rule: IsInvalid(guestId), Parameter: nameof(Guest.Id)));

        private void ValidateStorageGuest(Guest maybeGuest, Guid guestId)
        {
            if(maybeGuest is null)
            {
                throw new NotFoundGuestException(guestId);
            }
        }

        private void ValidationGuestNotNull(Guest guest)
        {
            if (guest is null)
            {
                throw new NullGuestException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsInvalid(GenderType gender) => new
        {
            Condition = Enum.IsDefined(gender) is false,
            Message = "Value is invalid"
        };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
		{
			DateTimeOffset currentDateTime =
			    DateTimeOffset.Now;

			TimeSpan timeDifference = currentDateTime.Subtract(date);
			TimeSpan oneMinute = TimeSpan.FromMinutes(1);

			return timeDifference.Duration() > oneMinute;
		}

        private static dynamic IsSame(
           DateTimeOffset firstDate,
           DateTimeOffset secondDate,
           string secondDateName) => new
           {
               Condition = firstDate == secondDate,
               Message = $"Date is the same as {secondDateName}"
           };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidGuestException = new InvalidGuestException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidGuestException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidGuestException.ThrowIfContainsErrors();
        }
    }
}
