using Microsoft.AspNetCore.Hosting.Internal;
using System;
using Microsoft.AspNetCore.Hosting;
using Travel.Connectors.Hotel.Configuration;
using Travel.Connectors.Hotel.ErrorMapping;
using Travel.Connectors.Hotel.ErrorMapping.Models;
using Travel.Connectors.Hotel.Metadata;
using Travel.Connectors.Hotel.Metadata.Models;
using Travel.Connectors.Hotel.Entities;
using Xunit;
using Tavisca.Platform.Common.Configurations;
using Travel.Connectors.Hotel.Common;

namespace Travel.Connectors.Hotel.Tests
{

    public class USGSearchRequestTests
    {
        private readonly IConfigurationProvider appConfiguration = null;
        private readonly MetadataResponse metadata = null;
        private readonly IConnectorError connectorError = null;

        public USGSearchRequestTests()
        {
            IHostingEnvironment hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.EnvironmentName = "local";
            appConfiguration = new FileConfiguration(hostingEnvironment, null);

            IConnectorMetadata connectorMetadata = new ServiceBasedMetadata(appConfiguration, null);
            metadata = connectorMetadata.ReadMetaData();
            connectorError = new ConnectorError();
        }

        [Fact]
        public void IsUSGSearchRequestValid_CheckInDateNotProvided_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.CheckIn = DateTime.MinValue;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyCheckin);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CheckInDatePastDate_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.CheckIn = DateTime.Today.AddDays(-1);

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.PastCheckinDate); //ApplicationConstants.CheckInDatePastDate

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CheckOutDateNotProvided_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.CheckOut = DateTime.MinValue;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyCheckin);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CheckOutDateGreaterThan29daysFromCheckinDate_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.CheckOut = usgSearchRequest.Criteria.CheckIn.AddDays(30);

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MaxStayExceed); // ApplicationConstants.EmptyCheckin

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CheckinDateGreaterThanCheckOutDate_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.CheckIn = usgSearchRequest.Criteria.CheckOut.AddDays(1);

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.CheckoutBeforeCheckin);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CheckinAndCheckoutDateAreOnSameDay_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.CheckOut = usgSearchRequest.Criteria.CheckIn;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MinOneDayDifference);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_HotelIdNull_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.HotelIds = null;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptySearchPattern);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_HotelIdLenghtLessThanEqualToZero_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            string[] arrHotelIds = new string[] { };
            usgSearchRequest.Criteria.HotelIds = arrHotelIds;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptySearchPattern);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListIsNull_ReturnsErrorMessage()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Criteria.Occupancies = null;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyOccupancy);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListIsEmpty_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            RoomOccupancy[] arroccupancies = new RoomOccupancy[] { };
            usgSearchRequest.Criteria.Occupancies = arroccupancies;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyOccupancy);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListHasZeroAdultSpecified_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2];
            arrChildAges[0] = 5;
            arrChildAges[1] = 10;
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 0;
            int[] arrChildAges1 = new int[2];
            arrChildAges1[0] = 4;
            arrChildAges1[1] = 2;
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2];
            arrRoomOccupancy[0] = occupancyAge;
            arrRoomOccupancy[1] = occupancyAge1;
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MinOneAdult);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListHasEmptyListForARoom_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2] { 5, 10 };
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2] { occupancyAge, occupancyAge1 };
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MinOneAdult);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListHasChildAgeMoreThanEqualTo18_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2] { 5, 18 };
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 10;
            int[] arrChildAges1 = new int[2] { 21, 6 };
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2] { occupancyAge, occupancyAge1 };
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MaxChildAge);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListHasMoreThan8RoomBookingInSingleRequest_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 2;
            int[] arrChildAges = new int[2] { 5, 10 };
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 2;
            int[] arrChildAges1 = new int[2] { 4, 8 };
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy occupancyAge2 = new RoomOccupancy();
            occupancyAge2.NumOfAdults = 2;
            int[] arrChildAges2 = new int[2] { 2, 8 };
            occupancyAge2.ChildAges = arrChildAges2;

            RoomOccupancy occupancyAge3 = new RoomOccupancy();
            occupancyAge3.NumOfAdults = 3;
            int[] arrChildAges3 = new int[2] { 3, 7 };
            occupancyAge3.ChildAges = arrChildAges3;

            RoomOccupancy occupancyAge4 = new RoomOccupancy();
            occupancyAge4.NumOfAdults = 3;
            int[] arrChildAges4 = new int[2] { 3, 7 };
            occupancyAge4.ChildAges = arrChildAges4;

            RoomOccupancy occupancyAge5 = new RoomOccupancy();
            occupancyAge5.NumOfAdults = 3;
            int[] arrChildAges5 = new int[2] { 3, 7 };
            occupancyAge5.ChildAges = arrChildAges5;

            RoomOccupancy occupancyAge6 = new RoomOccupancy();
            occupancyAge6.NumOfAdults = 3;
            int[] arrChildAges6 = new int[2] { 3, 7 };
            occupancyAge6.ChildAges = arrChildAges6;

            RoomOccupancy occupancyAge7 = new RoomOccupancy();
            occupancyAge7.NumOfAdults = 3;
            int[] arrChildAges7 = new int[2] { 3, 7 };
            occupancyAge7.ChildAges = arrChildAges7;

            RoomOccupancy occupancyAge8 = new RoomOccupancy();
            occupancyAge8.NumOfAdults = 3;
            int[] arrChildAges8 = new int[2] { 3, 7 };
            occupancyAge8.ChildAges = arrChildAges8;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[9];
            arrRoomOccupancy[0] = occupancyAge;
            arrRoomOccupancy[1] = occupancyAge1;
            arrRoomOccupancy[2] = occupancyAge2;
            arrRoomOccupancy[3] = occupancyAge3;
            arrRoomOccupancy[4] = occupancyAge4;
            arrRoomOccupancy[5] = occupancyAge5;
            arrRoomOccupancy[6] = occupancyAge6;
            arrRoomOccupancy[7] = occupancyAge7;
            arrRoomOccupancy[8] = occupancyAge8;
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MaxOccupancy);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListHasMoreThan15GuestSpecifiedInaRoom_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2] { 5, 10 };
            occupancyAge.ChildAges = arrChildAges;

            //Total guest in this room = 16
            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 10;
            int[] arrChildAges1 = new int[6] { 3, 4, 6, 1, 7, 10 };
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2] { occupancyAge, occupancyAge1 };
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MaxGuestsPerRoom);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_RoomOccupancyListHasMoreThan120Guest_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 13;
            int[] arrChildAges = new int[2];
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 13;
            int[] arrChildAges1 = new int[2];
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy occupancyAge2 = new RoomOccupancy();
            occupancyAge2.NumOfAdults = 13;
            int[] arrChildAges2 = new int[2];
            occupancyAge2.ChildAges = arrChildAges2;

            RoomOccupancy occupancyAge3 = new RoomOccupancy();
            occupancyAge3.NumOfAdults = 13;
            int[] arrChildAges3 = new int[2];
            occupancyAge3.ChildAges = arrChildAges3;

            RoomOccupancy occupancyAge4 = new RoomOccupancy();
            occupancyAge4.NumOfAdults = 13;
            int[] arrChildAges4 = new int[2];
            occupancyAge4.ChildAges = arrChildAges4;

            RoomOccupancy occupancyAge5 = new RoomOccupancy();
            occupancyAge5.NumOfAdults = 13;
            int[] arrChildAges5 = new int[2];
            occupancyAge5.ChildAges = arrChildAges5;

            RoomOccupancy occupancyAge6 = new RoomOccupancy();
            occupancyAge6.NumOfAdults = 13;
            int[] arrChildAges6 = new int[2];
            occupancyAge6.ChildAges = arrChildAges6;

            //Total guest in this room = 15
            RoomOccupancy occupancyAge7 = new RoomOccupancy();
            occupancyAge7.NumOfAdults = 10;
            int[] arrChildAges7 = new int[5] { 3, 5, 6, 7, 8};
            occupancyAge7.ChildAges = arrChildAges7;

            RoomOccupancy occupancyAge8 = new RoomOccupancy();
            occupancyAge8.NumOfAdults = 1;

            //Total Guest in all rooms = 121
            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[9] { occupancyAge, occupancyAge1, occupancyAge2, occupancyAge3, occupancyAge4, occupancyAge5, occupancyAge6, occupancyAge7, occupancyAge8 };
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.MaxGuestsPerBooking);

            //Assert
            Assert.Equal(isTestPassed, true);
        }


        [Fact]
        public void IsUSGSearchRequestValid_SupplierIdIsNull_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Id = null;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptySupplierId);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierIdIsEmpty_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Id = string.Empty;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptySupplierId);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierNameIsNull_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Name = null;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptySupplierName);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierNameIsEmpty_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Name = string.Empty;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptySupplierName);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierConfigurationUserIdIsNull_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Configurations.ApiKey = null;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyUserId);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierConfigurationUserIdIsEmpty_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Configurations.ApiKey = string.Empty;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyUserId);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierConfigurationUserIdHasMoreThan300Chars_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Configurations.ApiKey = "123dsrtrdsghsdfhtfshfthdfghhfshrthfhtrfhtrsyghwor578yt9s0g8h9ps9ghpas98e5ptypgs$&^#%@^*@@*@*@@*@*@*^@*@#%@%#@^#%#@%#@^%#@^@%#^%@$^@$&$&@gfzxsdfyufgyukavuyergflugakuysdebcrvyfaugyuewrgftw4e7t467584rwqeegfyugyswgfoaewrofiautewuiritfpqiauergfrefaiote7rtetf$^%$%^%$^YFTFTYftyrtyrsydrtyfrtsyryfrrefdrer";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);

            //Assert
            Assert.Equal(errorinfo, null);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierConfigurationUserPasswordIsNull_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Configurations.AuthToken = null;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyPassword);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_SupplierConfigurationUserPasswordIsEmpty_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Configurations.ApiKey = string.Empty;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.EmptyPassword);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_TravellerNationalitySpecifiedWithInvalidCountryCode_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.TravellerNationality = "fggh";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.InvalidCountryCode);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_TravellerNationalityNotSpecified_ShouldNotReturnAnyError()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.TravellerNationality = string.Empty;

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);

            //Assert
            Assert.Equal(errorinfo, null);
        }

        [Fact]
        public void IsUSGSearchRequestValid_TravellerCountryOfResidenceSpecifiedWithInvalidCountryCode_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.TravellerCountryOfResidence = "fhgh";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.InvalidCountryCode);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_TravellerCountryOfResidenceNotSpecified_ShouldNotReturnAnyError()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.TravellerCountryOfResidence = "";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);

            //Assert
            Assert.Equal(errorinfo, null);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CurrencySpecifiedOtherThanUSD_ReturnsErrorMessage()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.Currency = "EUR";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);
            bool isTestPassed = checkIsTestPassed(errorinfo, ApplicationConstants.UnSupportedCurrency);

            //Assert
            Assert.Equal(isTestPassed, true);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CurrencyNotSpecified_ShouldNotReturnAnyError()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.Currency = "";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);

            //Assert
            Assert.Equal(errorinfo, null);
        }

        [Fact]
        public void IsUSGSearchRequestValid_CurrencySpecifiedAsUSD_ShouldNotReturnAnyError()
        {
            //Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            usgSearchRequest.Supplier.Options.Currency = "USD";

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);

            //Assert
            Assert.Equal(errorinfo, null);
        }

        [Fact]
        public void IsUSGSearchRequestValid_ValidUSGSearchRequest_ShouldNotReturnAnyError()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();

            //Act
            ErrorInfo errorinfo = null;
            errorinfo = usgSearchRequest.IsUSGSearchRequestValid(metadata, connectorError);

            //Assert
            Assert.Equal(errorinfo, null);
        }

        public USGSearchRequest SetUSGSearchRequestObject()
        {
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            Criteria criteria = new Criteria();
            Supplier supplier = new Supplier();
            Configurations configurations = new Configurations();
            Options options = new Options();

            usgSearchRequest.Criteria = criteria;
            usgSearchRequest.Supplier = supplier;
            usgSearchRequest.Supplier.Configurations = configurations;
            usgSearchRequest.Supplier.Options = options;
            DateTime checkinDate = new DateTime(2017, 05, 07);
            DateTime checkOutDate = new DateTime(2017, 05, 12);

            usgSearchRequest.Criteria.CheckIn = checkinDate;
            usgSearchRequest.Criteria.CheckOut = checkOutDate;
            usgSearchRequest.Criteria.City = null;
            usgSearchRequest.Criteria.RadialRegion = null;

            string[] arrHotelIds = new string[2];
            arrHotelIds[0] = "d4b5bc60-1c9c-4e10 - b8bd - cc6f6a1a08a8";
            arrHotelIds[1] = "480a6e89-4ba2-5677-b1b6-121fbdc5cd2b";
            usgSearchRequest.Criteria.HotelIds = arrHotelIds;

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2];
            arrChildAges[0] = 5;
            arrChildAges[1] = 10;
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[1];
            arrRoomOccupancy[0] = occupancyAge;
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            usgSearchRequest.Supplier.Id = "123";
            usgSearchRequest.Supplier.Name = "GetARoom";
            usgSearchRequest.Supplier.Configurations.ApiKey = "0ef48cbe-e6c6-51cc-9f96-ca608f2868de";
            usgSearchRequest.Supplier.Configurations.AuthToken = "89a44a84-e042-585b-8dad-0c9c2c7a0ff3";

            string[] arrRateCodes = new string[3];
            arrRateCodes[0] = "THR";
            arrRateCodes[1] = "THX";
            arrRateCodes[2] = "BAR";

            usgSearchRequest.Supplier.Options.RateCodes = arrRateCodes;
            usgSearchRequest.Supplier.Options.TravellerNationality = "IN";
            usgSearchRequest.Supplier.Options.TravellerCountryOfResidence = "IN";
            usgSearchRequest.Supplier.Options.Currency = "USD";

            return usgSearchRequest;
        }

        private bool checkIsTestPassed(ErrorInfo errorInfo, string errorCode)
        {
            bool isTestPassed = false;
            if (errorInfo?.info != null)
            {
                foreach (Info info in errorInfo.info)
                {
                    if (info.code.Equals(errorCode))
                    {
                        isTestPassed = true;
                    }
                }
            }
            return isTestPassed;
        }

        private string getExpectedUsgErrorMessage(string errorCode)
        {
            UsgError usgError = connectorError.GetUsgError(errorCode);
            if (usgError != null)
            {
                return usgError.ErrorMessage;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
