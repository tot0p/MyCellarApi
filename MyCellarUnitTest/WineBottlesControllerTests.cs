using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCellarApi.Controllers;
using MyCellarApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MyCellarApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace MyCellarUnitTest.Controllers
{
    [TestClass]
    public class WineBottlesControllerTests
    {
        private MyCellarDbContext _context;

        public WineBottlesControllerTests()
        {
            InitializeContext();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            InitializeContext();
        }

        private void InitializeContext()
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
            var controller = new WineBottlesController(context);

            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllWineBottles()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            context.WineBottles.Add(new WineBottle
            {
                Id = 1,
                BrandName = "Wine 1",
                BottleName = "Bottle 1",
                Year = 2020,
                VarietalType = new List<string> { "Cabernet" },
                Region = "Region 1",
                WineType = "Red",
                Producer = "Producer 1",
                NetContent = 750,
                AlcoholContent = 13.5f,
                SulfiteWarning = true,
                Vegan = false,
                Labels = new List<string> { "Organic" },
                Description = "Default Description",
                Designation = "Default Designation"
            });
            context.WineBottles.Add(new WineBottle
            {
                Id = 2,
                BrandName = "Wine 2",
                BottleName = "Bottle 2",
                Year = 2021,
                VarietalType = new List<string> { "Merlot" },
                Region = "Region 2",
                WineType = "White",
                Producer = "Producer 2",
                NetContent = 750,
                AlcoholContent = 12.5f,
                SulfiteWarning = true,
                Vegan = true,
                Labels = new List<string> { "Organic" },
                Description = "Default Description",
                Designation = "Default Designation"
            });
            context.SaveChanges();

            var controller = new WineBottlesController(context);

            // Ensure proper initialization of BaseController dependencies
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsNotNull(result.Value);
            var wineBottles = result.Value as IEnumerable<WineBottle>;
            Assert.IsNotNull(wineBottles);
            Assert.AreEqual(2, wineBottles.Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsWineBottle()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            var controller = new WineBottlesController(context);

            var testWineBottle = new WineBottle
            {
                Id = 1,
                BrandName = "Test Brand",
                BottleName = "Test Bottle",
                Year = 2020,
                VarietalType = new List<string> { "Cabernet" },
                Region = "Test Region",
                WineType = "Red",
                Producer = "Test Producer",
                NetContent = 750,
                AlcoholContent = 13.5f,
                Description = "Test Description",
                Designation = "Test Designation",
                Labels = new List<string> { "Organic" },
                SulfiteWarning = true,
                Vegan = false
            };
            context.WineBottles.Add(testWineBottle);
            context.SaveChanges();

            // Act
            var result = await controller.GetModel(1);

            // Assert
            Assert.IsNotNull(result.Value);
            var wineBottle = result.Value as WineBottle;
            Assert.IsNotNull(wineBottle);
            Assert.AreEqual(testWineBottle.Id, wineBottle.Id);
        }

        [TestMethod]
        public async Task Create_AddsNewWineBottle()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            var controller = new WineBottlesController(context);
            var newWineBottle = new WineBottle
            {
                BrandName = "Test Brand",
                BottleName = "Test Bottle",
                Year = 2020,
                VarietalType = new List<string> { "Cabernet" },
                Region = "Test Region",
                WineType = "Red",
                Producer = "Test Producer",
                NetContent = 750,
                AlcoholContent = 13.5f,
                Description = "Test Description",
                Designation = "Test Designation",
                Labels = new List<string> { "Organic" },
                SulfiteWarning = true,
                Vegan = false
            };

            // Act
            var result = await controller.PostModel(newWineBottle);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdWineBottle = ((CreatedAtActionResult)result.Result).Value as WineBottle;
            Assert.IsNotNull(createdWineBottle);
            Assert.AreEqual(newWineBottle.BrandName, createdWineBottle.BrandName);
        }

        [TestMethod]
        public async Task Update_UpdatesExistingWineBottle()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            
            // Seed data
            var wineBottle = new WineBottle 
            { 
                Id = 1, 
                BrandName = "Original Brand", 
                BottleName = "Original Bottle", 
                Year = 2020,
                VarietalType = new List<string> { "TestType" },
                Region = "Test Region",
                WineType = "Red",
                Producer = "Test Producer",
                NetContent = 750,
                AlcoholContent = 13.5f,
                SulfiteWarning = true,
                Vegan = false,
                Labels = new List<string> { "Test" },
                Description = "Test Description",
                Designation = "Test Designation"
            };
            context.WineBottles.Add(wineBottle);
            context.SaveChanges();
            
            var controller = new WineBottlesController(context);
            var updatedWineBottle = new WineBottle 
            { 
                Id = 1, 
                BrandName = "Updated Brand", 
                BottleName = "Updated Bottle", 
                Year = 2021,
                VarietalType = new List<string> { "UpdatedType" },
                Region = "Updated Region",
                WineType = "White",
                Producer = "Updated Producer",
                NetContent = 750,
                AlcoholContent = 14.5f,
                SulfiteWarning = false,
                Vegan = true,
                Labels = new List<string> { "Updated" },
                Description = "Updated Description",
                Designation = "Updated Designation"
            };
            
            // Ensure no duplicate tracking
            context.Entry(wineBottle).State = EntityState.Detached;

            // Act
            var result = await controller.PutModel(updatedWineBottle.Id, updatedWineBottle);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_RemovesWineBottle()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            
            // Seed data
            var wineBottle = new WineBottle 
            { 
                Id = 1, 
                BrandName = "Test Brand", 
                BottleName = "Test Bottle", 
                Year = 2020,
                VarietalType = new List<string> { "TestType" },
                Region = "Test Region",
                WineType = "Red",
                Producer = "Test Producer",
                NetContent = 750,
                AlcoholContent = 13.5f,
                SulfiteWarning = true,
                Vegan = false,
                Labels = new List<string> { "Test" },
                Description = "Test Description",
                Designation = "Test Designation"
            };
            context.WineBottles.Add(wineBottle);
            context.SaveChanges();
            
            var controller = new WineBottlesController(context);

            // Act
            var result = await controller.DeleteModel(wineBottle.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}