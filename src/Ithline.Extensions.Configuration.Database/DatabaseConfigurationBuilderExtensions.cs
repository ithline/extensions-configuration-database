using Microsoft.Extensions.Configuration;
using Ithline.Extensions.Configuration.Database;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods to add database based configuration providers.
/// </summary>
public static class DatabaseConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds the database configuration provider.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="database">The configuration database abstraction.</param>
    /// <param name="reloadDelay">Number of milliseconds that reload will wait before calling Load. This helps avoid triggering reload before a file is completely written.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDatabase(
        this IConfigurationBuilder builder,
        IConfigurationDatabase database,
        int reloadDelay = 250)
    {
        return builder.Add(new DatabaseConfigurationSource(database, reloadDelay));
    }
}
