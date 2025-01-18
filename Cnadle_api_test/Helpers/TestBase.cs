// Helpers/TestBase.cs
using Microsoft.Extensions.Logging;
using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace Candle_API_test.Helpers.TestBase;
public abstract class TestBase
{
    protected Mock<IMapper> MockMapper;
    protected Mock<CandleDbContext> MockContext;

    protected TestBase()
    {
        MockMapper = new Mock<IMapper>();

        var options = new DbContextOptionsBuilder<CandleDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        MockContext = new Mock<CandleDbContext>(options);
    }

    protected Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    protected Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}