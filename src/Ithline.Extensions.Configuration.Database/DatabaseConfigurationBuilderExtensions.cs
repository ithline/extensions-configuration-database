using Microsoft.Extensions.Configuration;
using Ithline.Extensions.Configuration.Database;

namespace Microsoft.Extensions.DependencyInjection;

public static class DatabaseConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddDatabase(
        this IConfigurationBuilder builder,
        IConfigurationDatabase database,
        int reloadDelay = 250)
    {
        return builder.Add(new DatabaseConfigurationSource(database, reloadDelay));
    }
}
