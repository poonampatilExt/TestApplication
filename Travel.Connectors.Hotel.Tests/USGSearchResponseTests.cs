using System;
using System.Collections.Generic;
using System.Linq;
using Travel.Connectors.Hotel.GetARoom;
using Travel.Connectors.Hotel.Entities;
using Xunit;
using Travel.Connectors.Hotel.Configuration;
using Travel.Connectors.Hotel.Metadata.Models;
using Travel.Connectors.Hotel.Metadata;
using Travel.Connectors.Hotel.ErrorMapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Travel.Connectors.Hotel.Logger;
using Tavisca.Platform.Common.Configurations;
using Travel.Connectors.Hotel.Common;

namespace Travel.Connectors.Hotel.Tests
{
    public class USGSearchResponseTests
    {

        private readonly IConfigurationProvider appConfiguration = null;
        private readonly IConnectorError connectorError = null;
        private readonly FileLogger fileLogger = null;
        private readonly IConnectorMetadata connectorMetadata = null;

        public USGSearchResponseTests()
        {
            IHostingEnvironment hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.EnvironmentName = "Development";
            appConfiguration = new FileConfiguration(hostingEnvironment, null);
            connectorMetadata = new ServiceBasedMetadata(appConfiguration, null);
            connectorError = new ConnectorError();
            fileLogger = new FileLogger();
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
            arrHotelIds[0] = "d4b5bc60-1c9c-4e10-b8bd-cc6f6a1a08a8";
            arrHotelIds[1] = "480a6e89-4ba2-5677-b1b6-121fbdc5cd2b";
            usgSearchRequest.Criteria.HotelIds = arrHotelIds;

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[1];
            arrRoomOccupancy[0] = occupancyAge;
            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            usgSearchRequest.Supplier.Id = "123";
            usgSearchRequest.Supplier.Name = "GetARoom";
            usgSearchRequest.Supplier.Configurations.ApiKey = "0ef48cbe-e6c6-51cc-9f96-ca608f2868de";
            usgSearchRequest.Supplier.Configurations.AuthToken = "89a44a84-e042-585b-8dad-0c9c2c7a0ff3";
            usgSearchRequest.Supplier.Configurations.Istestbooking = true;

            #region optional fields
            //string[] arrRateCodes = new string[3];
            //arrRateCodes[0] = "THR";
            //arrRateCodes[1] = "THX";
            //arrRateCodes[2] = "BAR";
            //usgSearchRequest.Supplier.Options.RateCodes = arrRateCodes;
            //usgSearchRequest.Supplier.Options.TravellerNationality = "IN";
            //usgSearchRequest.Supplier.Options.TravellerCountryOfResidence = "IN";
            //usgSearchRequest.Supplier.Options.Currency = "USD";
            #endregion

            return usgSearchRequest;
        }

        [Fact]
        public void SearchHotels_ValidUSGSearchRequestObjectPassed_ReturnsUSGSearchResponse()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();

            // Act
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);

            if (usgSearchResponse.itineraries != null)
            {
                Assert.Equal(true, true);
            }
            else
            {
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound); 
            }
        }

        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_ShouldContainOnlyHolelIdsListedInTheRequest()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();
            string[] arrHotelIds = usgSearchRequest.Criteria.HotelIds;
            List<string> lstRequestedHotelIds = arrHotelIds.ToList();

            // Act
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);
            if (usgSearchResponse.itineraries != null)
            {
                List<string> lstInvalidHotelID = new List<string>();
                foreach (Itinerary item in usgSearchResponse.itineraries)
                {
                    if (!lstRequestedHotelIds.Contains(item.hotelInfo.id))
                    {
                        lstInvalidHotelID.Add(item.hotelInfo.id);
                    }
                }
                //Assert
                Assert.Equal(lstInvalidHotelID.Count(), 0);
            }
            else
            {
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }

        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_NoOfRoomsInResponseShouldBeAsPerTheRequest()
        {

            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchRespose = new USGSearchResponse();
            RoomOccupancy[] arrRoomOccupancy = usgSearchRequest.Criteria.Occupancies;
            bool isOccupancyLimitExceded = false;
            // Act
            usgSearchRespose = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);
            if (usgSearchRespose.itineraries != null)
            {
                foreach (Itinerary item in usgSearchRespose.itineraries)
                {
                    if (item.occupancies.Length > arrRoomOccupancy.Length)
                    {
                        isOccupancyLimitExceded = true;
                    }
                }
                //Assert
                Assert.Equal(isOccupancyLimitExceded, false);
            }
            else
            {
                Assert.Equal(usgSearchRespose.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }
        
        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_TotalNoOfGuestInResponseShouldBeAsPerTheRequestRoomOccupancy()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2];
            arrChildAges[0] = 5;
            arrChildAges[1] = 10;
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 2;
            int[] arrChildAges1 = new int[2];
            arrChildAges1[0] = 4;
            arrChildAges1[1] = 2;
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2];
            arrRoomOccupancy[0] = occupancyAge;
            arrRoomOccupancy[1] = occupancyAge1;

            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;

            bool isCheckFailed = false;
            List<RoomOccupancy> lstRequestRoomOccupancy = usgSearchRequest.Criteria.Occupancies.ToList();
            int totalGuestInTheRequest = 0;
            foreach (RoomOccupancy rc in lstRequestRoomOccupancy)
            {
                totalGuestInTheRequest = totalGuestInTheRequest + rc.NumOfAdults;
                if (rc.ChildAges != null)
                {
                    totalGuestInTheRequest = totalGuestInTheRequest + rc.ChildAges.Count();
                }
            }

            // Act
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);

            if (usgSearchResponse.itineraries != null)
            {
                for (int i = 0; i < usgSearchResponse.itineraries.Count; i++)
                {
                    int totalGuestInTheResponse = 0;
                    for (int j = 0; j < usgSearchResponse.itineraries[i].occupancies.Count(); j++)
                    {
                        totalGuestInTheResponse = totalGuestInTheResponse + usgSearchResponse.itineraries[i].occupancies[j].adults + usgSearchResponse.itineraries[i].occupancies[j].children;
                    }
                    if (totalGuestInTheRequest != totalGuestInTheResponse)
                    {
                        isCheckFailed = true;
                        break;
                    }
                }

                //Assert
                Assert.Equal(isCheckFailed, false);
            }
            else
            {
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }

        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_MultipleRoomsResponseShouldSpecifyRoomRatesPerBooking()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2];
            arrChildAges[0] = 5;
            arrChildAges[1] = 10;
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 2;
            int[] arrChildAges1 = new int[2];
            arrChildAges1[0] = 4;
            arrChildAges1[1] = 2;
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2];
            arrRoomOccupancy[0] = occupancyAge;
            arrRoomOccupancy[1] = occupancyAge1;

            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;
            // Act
            bool isCheckFailed = false;
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);
            if (usgSearchResponse.itineraries != null)
            {
                foreach (Itinerary item in usgSearchResponse.itineraries)
                {
                    if ((item.roomRate.perRoomRates != null) || (item.roomRate.perBookingRates == null || item.roomRate.perBookingRates.Count() <= 0))
                    {
                        isCheckFailed = true;
                    }
                }

                //Assert
                Assert.Equal(isCheckFailed, false);
            }
            else
            {
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }

        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_RoomsRatesShouldHaveSpecifiedCurrencyAsUSD()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();

            RoomOccupancy occupancyAge = new RoomOccupancy();
            occupancyAge.NumOfAdults = 1;
            int[] arrChildAges = new int[2];
            arrChildAges[0] = 5;
            arrChildAges[1] = 10;
            occupancyAge.ChildAges = arrChildAges;

            RoomOccupancy occupancyAge1 = new RoomOccupancy();
            occupancyAge1.NumOfAdults = 2;
            int[] arrChildAges1 = new int[2];
            arrChildAges1[0] = 4;
            arrChildAges1[1] = 2;
            occupancyAge1.ChildAges = arrChildAges1;

            RoomOccupancy[] arrRoomOccupancy = new RoomOccupancy[2];
            arrRoomOccupancy[0] = occupancyAge;
            arrRoomOccupancy[1] = occupancyAge1;

            usgSearchRequest.Criteria.Occupancies = arrRoomOccupancy;
            // Act
            bool isCheckFailed = false;
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);
            if (usgSearchResponse.itineraries != null)
            {
                foreach (Itinerary item in usgSearchResponse.itineraries)
                {
                    if (item.roomRate.perBookingRates != null)
                    {
                        foreach (PerBookingRates pbr in item.roomRate.perBookingRates)
                        {
                            if (pbr.currency != "USD")
                                isCheckFailed = true;
                        }
                    }
                }
                //Assert
                Assert.Equal(isCheckFailed, false);
            }
            else
            {
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }

        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_NoOfStayDaysShouldBeEqualToDailyRoomRatesBreakup()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();

            TimeSpan totalNoOfStayDays = usgSearchRequest.Criteria.CheckOut.Subtract(usgSearchRequest.Criteria.CheckIn);

            // Act
            bool isCheckFailed = false;
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);
            if (usgSearchResponse.itineraries != null)
            {
                foreach (Itinerary item in usgSearchResponse.itineraries)
                {
                    if ((item.roomRate.perBookingRates != null && item.roomRate.perBookingRates.Count() > 0))
                    {

                        for (int i = 0; i < item.roomRate.perBookingRates.Count(); i++)
                        {
                            int dailyRoomRatesBreakupLength = 0;
                            dailyRoomRatesBreakupLength = item.roomRate.perBookingRates[i].dailyRoomRates.breakup.Length;
                            if (totalNoOfStayDays.Days != dailyRoomRatesBreakupLength)
                            {
                                isCheckFailed = true;
                                break;
                            }
                        }
                    }
                }
                //Assert
                Assert.Equal(isCheckFailed, false);
            }
            else
            {
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }

        [Fact]
        public void SearchHotels_ReturnsUSGSearchResponse_PerBookingRatesAmountShouldBeEqualToSumOfDailyRoomRatesBreakupAmount()
        {
            // Arrange
            USGSearchRequest usgSearchRequest = new USGSearchRequest();
            usgSearchRequest = SetUSGSearchRequestObject();
            GetARoomProvider getARoomProvider = new GetARoomProvider(appConfiguration, fileLogger, connectorError, connectorMetadata);

            CommonLogParameters commonLogParameters = new CommonLogParameters();
            USGSearchResponse usgSearchResponse = new USGSearchResponse();

            // Act
            bool isCheckFailed = false;
            usgSearchResponse = getARoomProvider.SearchHotels(usgSearchRequest, commonLogParameters);
            if (usgSearchResponse.itineraries != null)
            {
                foreach (Itinerary item in usgSearchResponse.itineraries)
                {
                    if ((item.roomRate.perBookingRates != null && item.roomRate.perBookingRates.Count() > 0))
                    {
                        for (int i = 0; i < item.roomRate.perBookingRates.Count(); i++)
                        {
                            double amountToVerify = 0;
                            double sumofdailyRoomRatesBreakupAmount = 0;
                            List<Roombreakup> lstRoomBreakUp = new List<Roombreakup>();
                            lstRoomBreakUp = item.roomRate.perBookingRates[i].dailyRoomRates.breakup.ToList();

                            foreach (Roombreakup room in lstRoomBreakUp)
                            {
                                sumofdailyRoomRatesBreakupAmount = sumofdailyRoomRatesBreakupAmount + room.amount;
                            }

                            bool isTaxIncluded = item.roomRate.perBookingRates[i].dailyRoomRates.taxIncluded;
                            if (isTaxIncluded)
                            {
                                amountToVerify = item.roomRate.perBookingRates[i].total;
                            }
                            else
                            {
                                amountToVerify = item.roomRate.perBookingRates[i].breakup.baseRate;
                            }
                            if (Math.Round(sumofdailyRoomRatesBreakupAmount, 2) != amountToVerify)
                            {
                                isCheckFailed = true;
                                break;
                            }
                        }
                    }
                }
                //Assert
                Assert.Equal(isCheckFailed, false);
            }
            else
            {
                //Assert
                Assert.Equal(usgSearchResponse.errorInfo.code, ApplicationConstants.NoResultsFound);
            }
        }
    }
}
