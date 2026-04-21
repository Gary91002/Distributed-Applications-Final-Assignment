using G2VehicleInventory.Application.Interfaces;
using G2VehicleInventory.Application.Services;
using G2VehicleInventory.Domain.Enums;
using G2VehicleInventory.Domain.Exceptions;
using G2VehicleInventory.Domain.ValueObjects;
using G2VehicleInventory.Domain.VehicleAggregate;
using Moq;

namespace G2VehicleInventory.Domain.Tests.Services
{
	public class G2UpdateVehicleStatusTests
	{
		[Fact]
		public async Task UpdateVehicleStatus_Should_MarkVehicleAsReserved_And_SaveChanges()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("ABC123"),
				1,
				VehicleType.Car,
				VehicleStatus.Available);

			var repoMock = new Mock<G2IVehicleRepository>();
			repoMock.Setup(r => r.GetById(1)).ReturnsAsync(vehicle);

			var service = new G2UpdateVehicleStatus(repoMock.Object);

			var result = await service.UpdateVehicleStatus(1, VehicleStatus.Reserved);

			Assert.Equal(VehicleStatus.Reserved, result.VehicleStatus);
			repoMock.Verify(r => r.SaveChanges(), Times.Once);
		}

		[Fact]
		public async Task UpdateVehicleStatus_Should_ThrowException_WhenVehicleDoesNotExist()
		{
			var repoMock = new Mock<G2IVehicleRepository>();
			repoMock.Setup(r => r.GetById(99)).ReturnsAsync((G2Vehicle?)null);

			var service = new G2UpdateVehicleStatus(repoMock.Object);

			var ex = await Assert.ThrowsAsync<Exception>(() =>
				service.UpdateVehicleStatus(99, VehicleStatus.Reserved));

			Assert.Equal("Vehicle with id 99 not found.", ex.Message);
			repoMock.Verify(r => r.SaveChanges(), Times.Never);
		}

		[Fact]
		public async Task UpdateVehicleStatus_Should_Throw_WhenTryingToReserveRentedVehicle()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("RENT01"),
				1,
				VehicleType.Car,
				VehicleStatus.Rented);

			var repoMock = new Mock<G2IVehicleRepository>();
			repoMock.Setup(r => r.GetById(1)).ReturnsAsync(vehicle);

			var service = new G2UpdateVehicleStatus(repoMock.Object);

			await Assert.ThrowsAsync<G2InvalidVehicleStatusChangeException>(() =>
				service.UpdateVehicleStatus(1, VehicleStatus.Reserved));

			repoMock.Verify(r => r.SaveChanges(), Times.Never);
		}

		[Fact]
		public async Task UpdateVehicleStatus_Should_Throw_WhenTryingToMakeReservedVehicleAvailable()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("RES001"),
				1,
				VehicleType.Car,
				VehicleStatus.Reserved);

			var repoMock = new Mock<G2IVehicleRepository>();
			repoMock.Setup(r => r.GetById(1)).ReturnsAsync(vehicle);

			var service = new G2UpdateVehicleStatus(repoMock.Object);

			await Assert.ThrowsAsync<G2InvalidVehicleStatusChangeException>(() =>
				service.UpdateVehicleStatus(1, VehicleStatus.Available));

			repoMock.Verify(r => r.SaveChanges(), Times.Never);
		}

		[Fact]
		public async Task UpdateVehicleStatus_Should_MarkVehicleAsMaintenance_WhenVehicleIsAvailable()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("MAIN01"),
				1,
				VehicleType.Car,
				VehicleStatus.Available);

			var repoMock = new Mock<G2IVehicleRepository>();
			repoMock.Setup(r => r.GetById(1)).ReturnsAsync(vehicle);

			var service = new G2UpdateVehicleStatus(repoMock.Object);

			var result = await service.UpdateVehicleStatus(1, VehicleStatus.Maintenance);

			Assert.Equal(VehicleStatus.Maintenance, result.VehicleStatus);
			repoMock.Verify(r => r.SaveChanges(), Times.Once);
		}
	}
}