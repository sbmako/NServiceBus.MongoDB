namespace NServiceBus.MongoDB.Tests.TestingUtilities
{
    using Ploeh.AutoFixture;

    public class ConfigureCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ////var config = Configure.With(new[] { GetType().Assembly })
            ////                      .DefineEndpointName("UnitTests")
            ////                      .DefaultBuilder();

            ////fixture.Register(() => config);

            ////fixture.Customize<Address>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));

            ////fixture.Customize<TimeoutData>(c => c.With(t => t.OwningTimeoutManager, Configure.EndpointName));
        }
    }
}
