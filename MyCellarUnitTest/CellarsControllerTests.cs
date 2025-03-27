using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCellarApi.Controllers;
using MyCellarApi.Models;
using MyCellarApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyCellarUnitTest.Controllers
{
    [TestClass]
    public class CellarsControllerTests
    {
        private MyCellarDbContext _context;

        public CellarsControllerTests()
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
            var controller = new CellarsController(context);

            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllCellars()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            context.Cellars.Add(new Cellar { Id = 1, Name = "Cellar 1", Location = "Location 1" });
            context.Cellars.Add(new Cellar { Id = 2, Name = "Cellar 2", Location = "Location 2" });
            context.SaveChanges();

            // Ensure proper initialization of BaseController dependencies
            var controller = new CellarsController(context)
            {
                Url = new Mock<IUrlHelper>().Object
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.GetAll();

            // Assert
            // The GetAll method returns List<Cellar> directly, not wrapped in OkObjectResult
            Assert.IsNotNull(result.Value);
            var cellars = result.Value as IEnumerable<Cellar>;
            Assert.IsNotNull(cellars);
            Assert.AreEqual(2, cellars.Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsCellar()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            context.Cellars.Add(new Cellar { Id = 1, Name = "Cellar 1", Location = "Location 1" });
            context.SaveChanges();

            var controller = new CellarsController(context);
            int testId = 1;

            // Act
            var result = await controller.GetModel(testId);

            // Assert
            Assert.IsNotNull(result.Value);
            var cellar = result.Value as Cellar;
            Assert.IsNotNull(cellar);
            Assert.AreEqual(testId, cellar.Id);
        }

        [TestMethod]
        public async Task Create_AddsNewCellar()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);
            var controller = new CellarsController(context);
            var newCellar = new Cellar { Name = "Test Cellar", Location = "Test Location" };

            // Act
            var result = await controller.PostModel(newCellar);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdCellar = ((CreatedAtActionResult)result.Result).Value as Cellar;
            Assert.IsNotNull(createdCellar);
            Assert.AreEqual(newCellar.Name, createdCellar.Name);
        }

        [TestMethod]
        public async Task Update_UpdatesExistingCellar()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            context.Cellars.Add(new Cellar { Id = 1, Name = "Cellar 1", Location = "Location 1" });
            context.SaveChanges();

            var controller = new CellarsController(context);
            var updatedCellar = new Cellar { Id = 1, Name = "Updated Cellar", Location = "Updated Location" };

            // Ensure no duplicate tracking
            var existingCellar = context.Cellars.Find(updatedCellar.Id);
            if (existingCellar != null)
            {
                context.Entry(existingCellar).State = EntityState.Detached;
            }

            // Act
            var result = await controller.PutModel(updatedCellar.Id, updatedCellar);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_RemovesCellar()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyCellarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new MyCellarDbContext(options);

            // Seed data
            context.Cellars.Add(new Cellar { Id = 1, Name = "Cellar 1", Location = "Location 1" });
            context.SaveChanges();

            var controller = new CellarsController(context);
            int testId = 1;

            // Act
            var result = await controller.DeleteModel(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}