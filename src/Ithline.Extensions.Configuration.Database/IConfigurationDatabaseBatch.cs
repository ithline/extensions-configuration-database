namespace Ithline.Extensions.Configuration.Database;

public interface IConfigurationDatabaseBatch
{
    IConfigurationDatabaseBatch Set(string id, string? value);
    IConfigurationDatabaseBatch Delete(string id);
    void Run();
}
