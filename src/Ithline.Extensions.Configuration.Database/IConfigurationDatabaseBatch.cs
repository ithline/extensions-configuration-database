namespace Ithline.Extensions.Configuration.Database;

/// <summary>
/// Represents a batch of <see cref="IConfigurationDatabase"/> operations.
/// </summary>
public interface IConfigurationDatabaseBatch
{
    /// <summary>
    /// Sets the value of the specified key.
    /// </summary>
    /// <param name="id">Id of the setting to change.</param>
    /// <param name="value">Value to set the setting to.</param>
    /// <returns><see cref="IConfigurationDatabaseBatch"/>.</returns>
    IConfigurationDatabaseBatch Set(string id, string? value);

    /// <summary>
    /// Removes the specified key from the configuration database.
    /// </summary>
    /// <param name="id">Id of the setting to delete.</param>
    /// <returns><see cref="IConfigurationDatabaseBatch"/>.</returns>
    IConfigurationDatabaseBatch Delete(string id);

    /// <summary>
    /// Executes the batch.
    /// </summary>
    void Run();
}
