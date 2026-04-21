using G2VehicleInventory.Domain.Enums;
using G2VehicleInventory.Domain.Exceptions;
using G2VehicleInventory.Domain.ValueObjects;
using G2VehicleInventory.Domain.VehicleAggregate;

namespace G2VehicleInventory.Domain.Tests.Domain
{
	public class G2VehicleTests
	{
		[Fact]
		public void MarkReserved_Should_SetStatusToReserved_WhenVehicleIsAvailable()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("CAR001"),
				1,
				VehicleType.Car,
				VehicleStatus.Available);

			vehicle.MarkReserved();

			Assert.Equal(VehicleStatus.Reserved, vehicle.VehicleStatus);
		}

		[Fact]
		public void MarkReserved_Should_Throw_WhenVehicleIsAlreadyReserved()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("CAR002"),
				1,
				VehicleType.Car,
				VehicleStatus.Reserved);

			var ex = Assert.Throws<G2InvalidVehicleStatusChangeException>(() => vehicle.MarkReserved());

			Assert.Equal("Vehicle is already reserved.", ex.Message);
		}

		[Fact]
		public void MarkRented_Should_Throw_WhenVehicleIsReserved()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("CAR003"),
				1,
				VehicleType.Car,
				VehicleStatus.Reserved);

			var ex = Assert.Throws<G2InvalidVehicleStatusChangeException>(() => vehicle.MarkRented());

			Assert.Equal("Cannot mark a reserved vehicle as rented.", ex.Message);
		}

		[Fact]
		public void MarkServiced_Should_Throw_WhenVehicleIsReserved()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("CAR004"),
				1,
				VehicleType.Car,
				VehicleStatus.Reserved);

			var ex = Assert.Throws<G2InvalidVehicleStatusChangeException>(() => vehicle.MarkServiced());

			Assert.Equal("Cannot mark a reserved vehicle as under maintenance.", ex.Message);
		}

		[Fact]
		public void MarkAvailable_Should_Throw_WhenVehicleIsReserved()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("CAR005"),
				1,
				VehicleType.Car,
				VehicleStatus.Reserved);

			var ex = Assert.Throws<G2InvalidVehicleStatusChangeException>(() => vehicle.MarkAvailable());

			Assert.Equal("Cannot mark a reserved vehicle as available.", ex.Message);
		}

		[Fact]
		public void MarkRented_Should_SetStatusToRented_WhenVehicleIsAvailable()
		{
			var vehicle = new G2Vehicle(
				new G2VehicleCode("CAR006"),
				1,
				VehicleType.Car,
				VehicleStatus.Available);

			vehicle.MarkRented();

			Assert.Equal(VehicleStatus.Rented, vehicle.VehicleStatus);
		}
	}
}