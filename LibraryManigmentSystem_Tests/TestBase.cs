using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibraryManagementSystem_Tests
{
    public abstract class TestBase
    {
        protected ApplicationDbContext _context;

        [SetUp]
        public void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestLibraryDb_{GetType().Name}")
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Clear the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _context.Dispose();
        }
    }
} 