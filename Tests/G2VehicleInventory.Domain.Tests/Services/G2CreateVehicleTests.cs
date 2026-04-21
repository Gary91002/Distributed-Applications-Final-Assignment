using G2VehicleInventory.Application.DTOs;
using G2VehicleInventory.Application.Interfaces;
using G2VehicleInventory.Application.Services;
using G2VehicleInventory.Domain.Enums;
using G2VehicleInventory.Domain.VehicleAggregate;
using Moq;

namespace G2VehicleInventory.Domain.Tests.Services
{
	public class G2CreateVehicleTests
	{
		[Fact]
		public async Task CreateVehicle_Should_AddVehicle_And_SaveChanges()
		{
			var repoMock = new Mock<G2IVehicleRepository>();
			var service = new G2CreateVehicle(repoMock.Object);

			var dto = new G2CreateVehicleDto
			{
				VehicleCode = "car123",
				LocationId = 5,
				VehicleType = VehicleType.SUV,
				VehicleStatus = VehicleStatus.Rented
			};

			await service.CreateVehicle(dto);

			repoMock.Verify(r => r.Add(It.Is<G2Vehicle>(v =>
				v.VehicleCode.Value == "CAR123" &&
				v.LocationId == 5 &&
				v.VehicleType == VehicleType.SUV &&
				v.VehicleStatus == VehicleStatus.Available
			)), Times.Once);

			repoMock.Verify(r => r.SaveChanges(), Times.Once);
		}

		[Fact]
		public async Task CreateVehicle_Should_IgnoreDtoVehicleStatus_And_Always_SetAvailable()
		{
			var repoMock = new Mock<G2IVehicleRepository>();
			var service = new G2CreateVehicle(repoMock.Object);

			var dto = new G2CreateVehicleDto
			{
				VehicleCode = "truck1",
				LocationId = 2,
				VehicleType = VehicleType.Truck,
				VehicleStatus = VehicleStatus.Maintenance
			};

			await service.CreateVehicle(dto);

			repoMock.Verify(r => r.Add(It.Is<G2Vehicle>(v =>
				v.VehicleStatus == VehicleStatus.Available
			)), Times.Once);
		}

		[Fact]
		public async Task CreateVehicle_With_EmptyVehicleCode_Should_ThrowArgumentException()
		{
			var repoMock = new Mock<G2IVehicleRepository>();
			var service = new G2CreateVehicle(repoMock.Object);

			var dto = new G2CreateVehicleDto
			{
				VehicleCode = "",
				LocationId = 1,
				VehicleType = VehicleType.Car,
				VehicleStatus = VehicleStatus.Available
			};

			await Assert.ThrowsAsync<ArgumentException>(() => service.CreateVehicle(dto));

			repoMock.Verify(r => r.Add(It.IsAny<G2Vehicle>()), Times.Never);
			repoMock.Verify(r => r.SaveChanges(), Times.Never);
		}
	}
}