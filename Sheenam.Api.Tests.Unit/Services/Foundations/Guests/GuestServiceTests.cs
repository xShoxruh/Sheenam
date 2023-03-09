﻿//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Brokers.Storages;
using Sheenam.Api.Brokers.Storages.Loggings;
using Sheenam.Api.Models.Foundations.Guests;
using Sheenam.Api.Services.Foundations.Guests;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IGuestService guestService;

        public GuestServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.guestService = new GuestService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Guest CreateRandomGuest() =>
             CreateGuestFiller(dates: GetRandomDateTimeOffset()).Create();

        private static IQueryable<Guest> CreateRandomGuests() =>
            CreateGuestFiller(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber()).AsQueryable();

        private static Guest CreateRandomGuest(DateTimeOffset dates) =>
            CreateGuestFiller(dates).Create();
        private static int GetRandomNegativeNumber() =>
           -1 * new IntRange(min: 2, max: 10).GetValue();

        private static Guest CreateRandomModifyGuest(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Guest randomGuest = CreateRandomGuest(dates);

            randomGuest.CreatedDate = 
                randomGuest.CreatedDate.AddDays(randomDaysInPast);

            return randomGuest;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomMessage() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static SqlException GetSqlError() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static T GetInvalidEnum<T>()
        {
            int randomNumber = GetRandomNumber();

            while (Enum.IsDefined(typeof(T), randomNumber) is true)
            {
                randomNumber = GetRandomNumber();
            }

            return (T)(object)randomNumber;
        }

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
           (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static Filler<Guest> CreateGuestFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Guest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}