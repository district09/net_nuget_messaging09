namespace Messaging09.Amqp.IntegrationTests.Setup;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollectionFixture : ICollectionFixture<BobFixture>, ICollectionFixture<AliceFixture>
{
}
