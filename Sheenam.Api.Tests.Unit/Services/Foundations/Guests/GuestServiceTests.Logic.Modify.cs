﻿//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async Task ShouldModifyGuestAsync()
        {
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Guest randomGuest = CreateRandomModifyGuest(randomDate);
            Guest inputGuest = randomGuest;
            Guest storageGuest = inputGuest;
            Guest updatedGuest = inputGuest;
            Guest expectedGuest = updatedGuest.DeepClone();
            Guid guestId = inputGuest.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(guestId))
                    .ReturnsAsync(storageGuest);

            this.storageBrokerMock.Setup(broker =>
               broker.UpdateGuestAsync(inputGuest))
                   .ReturnsAsync(updatedGuest);

            //when
            Guest actualGuest =
                await this.guestService.ModifyGuestAsync(inputGuest);

            //then
            actualGuest.Should().BeEquivalentTo(expectedGuest);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(guestId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGuestAsync(inputGuest),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
