using AutoFixture;

namespace WebApi.UnitTests;

public class BaseUnitTest
{
    protected static Fixture Fixture { get; }

    static BaseUnitTest()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}