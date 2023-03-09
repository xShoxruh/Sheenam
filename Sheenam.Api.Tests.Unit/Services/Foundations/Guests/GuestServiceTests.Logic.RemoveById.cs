//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async Task ShouldRemoveGuestByIdAsync()
        {
            //given
            Guest randomGuest = CreateRandomGuest();
            Guid guestId = randomGuest.Id;
            Guest storageGuest = randomGuest;
            Guest expectedGuest = storageGuest;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(guestId))
                .ReturnsAsync(storageGuest);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteGuestAsync(storageGuest))
                .ReturnsAsync(expectedGuest);

            //when
            Guest actualGuest = 
                await this.guestService.RemoveGuestByIdAsync(guestId);

            //then
            actualGuest.Should().BeEquivalentTo(expectedGuest);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(guestId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGuestAsync(storageGuest),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
