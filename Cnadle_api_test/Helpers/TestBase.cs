// Helpers/TestBase.cs
using Microsoft.Extensions.Logging;
using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore;
using Candle_api_test.TestData;

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

    // Método helper para crear mock DbSet con datos
    public Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryableData = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(queryableData.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression)
            .Returns(queryableData.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType)
            .Returns(queryableData.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator())
            .Returns(queryableData.GetEnumerator());

        return mockSet;
    }


}