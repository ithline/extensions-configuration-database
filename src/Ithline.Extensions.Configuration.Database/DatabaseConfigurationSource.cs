using Microsoft.Extensions.Configuration;

namespace Ithline.Extensions.Configuration.Database;

internal sealed class DatabaseConfigurationSource : IConfigurationSource
{
    private readonly IConfigurationDatabase _database;
    private readonly int _reloadDelay;

    public DatabaseConfigurationSource(IConfigurationDatabase database, int reloadDelay)
    {
        _database = database;
        _reloadDelay = reloadDelay;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DatabaseConfigurationProvider(_database, _reloadDelay);
    }
}
