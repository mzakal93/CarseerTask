using CarseerTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;

namespace CarseerTask.Tests
{
    [TestClass]
    public class CarseerTaskControllerTests
    {
        [Fact]
        [TestMethod]
        public async Task GetModelsByMakeNameAndYearAsyncValidCarNameReturnsOkResult()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["ApiSetting:GetModelsForMakeIdYearAPI"]).Returns("https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMakeIdYear");

            var carMakeCsvHelperMock = new Mock<ICarMakeCsvHelper>();
            carMakeCsvHelperMock.Setup(helper => helper.GetMakeIdByMakeName(It.IsAny<string>())).Returns(123);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClientMock = new Mock<HttpClient>();
            httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);

            var loggerMock = new Mock<ILogger<CarseerTaskController>>();

            var controller = new CarseerTaskController(loggerMock.Object, configurationMock.Object, httpClientFactoryMock.Object, carMakeCsvHelperMock.Object);

            // Act
            var result = await controller.GetModelsByMakeNameAndYearAsync(2022, "Toyota");

            // Assert
            // Check if the result is an ObjectResult or a derived type
            Xunit.Assert.IsType<OkObjectResult>(result);

            // Check if the actual value is a ModelResult
            var modelResult = Xunit.Assert.IsType<ModelResult>((result as OkObjectResult).Value);

            //// Ensure that Models property is not null
            Xunit.Assert.NotNull(modelResult);
        }

        [Fact]
        [TestMethod]
        public async Task GetModelsByMakeNameAndYearAsyncInvalidCarNameReturnsBadRequestResult()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["ApiSetting:GetModelsForMakeIdYearAPI"]).Returns("https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMakeIdYear");

            var carMakeCsvHelperMock = new Mock<ICarMakeCsvHelper>();
            carMakeCsvHelperMock.Setup(helper => helper.GetMakeIdByMakeName(It.IsAny<string>())).Returns((int?)null);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>(); 
            var httpClientMock = new Mock<HttpClient>();
            httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);


            var loggerMock = new Mock<ILogger<CarseerTaskController>>();

            var controller = new CarseerTaskController(loggerMock.Object, configurationMock.Object, httpClientFactoryMock.Object, carMakeCsvHelperMock.Object);

            // Act
            var result = await controller.GetModelsByMakeNameAndYearAsync(2022, "InvalidCar");

            // Assert
            Xunit.Assert.IsType<BadRequestObjectResult>(result);

            var badRequestResult = (BadRequestObjectResult)result;
            Xunit.Assert.Equal("Car name 'InvalidCar' is not valid.", badRequestResult.Value);
        }
    }
}
