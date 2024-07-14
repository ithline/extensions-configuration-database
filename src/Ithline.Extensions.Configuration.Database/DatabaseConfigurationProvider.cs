using Microsoft.Extensions.Configuration;

namespace Ithline.Extensions.Configuration.Database;

internal sealed class DatabaseConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly IDisposable _tokenRegistration;
    private readonly IConfigurationDatabase _database;
    private readonly int _reloadDelay;

    public DatabaseConfigurationProvider(IConfigurationDatabase database, int reloadDelay)
    {
        _database = database;
        _reloadDelay = reloadDelay;

        _tokenRegistration = database.OnChange(obj =>
        {
            Thread.Sleep(obj._reloadDelay);
            obj.Load();
        }, this);
    }

    public override void Load()
    {
        Data = _database.GetValues().ToDictionary(StringComparer.OrdinalIgnoreCase);

        // notify that values have changed
        this.OnReload();
    }

    public void Dispose()
    {
        _tokenRegistration.Dispose();
    }
}
