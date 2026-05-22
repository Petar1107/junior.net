namespace AbySalto.Junior.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<AbySaltoApiWebApplicationFactory>;
