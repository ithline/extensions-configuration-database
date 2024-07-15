using Microsoft.Extensions.Primitives;

namespace Ithline.Extensions.Configuration.Database;

/// <summary>
/// Represents a configuration database.
/// </summary>
public interface IConfigurationDatabase
{
    /// <summary>
    /// Returns a collection of configuration keys.
    /// </summary>
    /// <returns>A collection of configuration keys.</returns>
    string[] GetKeys();

    /// <summary>
    /// Returns a collection of key-value pairs.
    /// </summary>
    /// <returns>A collection of key-value pairs.</returns>
    KeyValuePair<string, string?>[] GetValues();

    /// <summary>
    /// Creates a batch operation used to modify the database.
    /// </summary>
    /// <returns>A batch operation builder.</returns>
    IConfigurationDatabaseBatch CreateBatch();

    /// <summary>
    /// Returns a <see cref="IChangeToken"/> that can be used to observe when this configuration is reloaded.
    /// </summary>
    /// <returns>A <see cref="IChangeToken"/>.</returns>
    IChangeToken GetReloadToken();
}
