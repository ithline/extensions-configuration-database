namespace Ithline.Extensions.Configuration.Database;

public interface IConfigurationDatabase
{
    string[] GetKeys();
    KeyValuePair<string, string?>[] GetValues();
    IConfigurationDatabaseBatch CreateBatch();

    IDisposable OnChange(Action handler);
    IDisposable OnChange<TState>(Action<TState> handler, TState state);
}
