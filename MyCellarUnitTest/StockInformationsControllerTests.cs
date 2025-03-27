using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCellarApi.Controllers;
using MyCellarApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MyCellarApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Http;

namespace MyCellarUnitTest.Controllers
{
    [TestClass]
    public class StockInformationsControllerTests
    {
        private readonly MyCellarDbContext _context;

        public StockInformationsControllerTests()
        {
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new MyCellarDbContext(options);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public void Constructor_InitializesController()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Act
            var controller = new StockInformationsController(context);

            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllStockInformations()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            context.StockInformations.Add(new StockInformation { Id = 1, Quantity = 10 });
            context.StockInformations.Add(new StockInformation { Id = 2, Quantity = 20 });
            context.SaveChanges();

            // Ensure proper initialization of BaseController dependencies
            var controller = new StockInformationsController(context)
            {
                Url = new Mock<IUrlHelper>().Object
            };

            // Ensure proper initialization of BaseController dependencies
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsNotNull(result.Value);
            var stockInformations = result.Value as IEnumerable<StockInformation>;
            Assert.IsNotNull(stockInformations);
            Assert.AreEqual(2, stockInformations.Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsStockInformation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            var stockInformation = new StockInformation { Id = 1, Quantity = 10 };
            context.StockInformations.Add(stockInformation);
            context.SaveChanges();

            var controller = new StockInformationsController(context);

            // Act
            var result = await controller.GetModel(1);

            // Assert
            Assert.IsNotNull(result.Value);
            var retrievedStockInformation = result.Value as StockInformation;
            Assert.IsNotNull(retrievedStockInformation);
            Assert.AreEqual(stockInformation.Id, retrievedStockInformation.Id);
        }

        [TestMethod]
        public async Task Create_AddsNewStockInformation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            var controller = new StockInformationsController(context);
            var newStockInformation = new StockInformation { WineBottleId = 1, CellarId = 1, Quantity = 10 };

            // Act
            var result = await controller.PostModel(newStockInformation);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdStockInformation = ((CreatedAtActionResult)result.Result).Value as StockInformation;
            Assert.IsNotNull(createdStockInformation);
            Assert.AreEqual(newStockInformation.Quantity, createdStockInformation.Quantity);
        }

        [TestMethod]
        public async Task Update_UpdatesExistingStockInformation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            
            // Seed data
            var stockInformation = new StockInformation { Id = 1, WineBottleId = 1, CellarId = 1, Quantity = 10 };
            context.StockInformations.Add(stockInformation);
            context.SaveChanges();
            
            var controller = new StockInformationsController(context);
            var updatedStockInformation = new StockInformation { Id = 1, WineBottleId = 1, CellarId = 1, Quantity = 20 };
            
            // Ensure no duplicate tracking
            context.Entry(stockInformation).State = EntityState.Detached;

            // Act
            var result = await controller.PutModel(updatedStockInformation.Id, updatedStockInformation);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_RemovesStockInformation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            
            // Seed data
            var stockInformation = new StockInformation { Id = 1, WineBottleId = 1, CellarId = 1, Quantity = 10 };
            context.StockInformations.Add(stockInformation);
            context.SaveChanges();
            
            var controller = new StockInformationsController(context);

            // Act
            var result = await controller.DeleteModel(stockInformation.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}