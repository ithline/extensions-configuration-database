namespace Ithline.Extensions.Configuration.Npgsql;

internal sealed class NpgsqlCommandTexts
{
    public NpgsqlCommandTexts(string tableName, string? schemaName = null)
    {
        var table = string.IsNullOrWhiteSpace(schemaName)
            ? tableName
            : $"{schemaName}.{tableName}";

        CreateTable = $"""
            CREATE TABLE IF NOT EXISTS {table} (
                key      varchar(256) PRIMARY KEY,
                value   text
            );
            """;
        SelectKeys = $"SELECT key FROM {table};";
        SelectValues = $"SELECT key, value FROM {table};";
        DeleteItem = $"DELETE FROM {table} WHERE key = @id";
        UpdateItem = $"""
            INSERT INTO {table} (key, value) VALUES (@id, @value)
            ON CONFLICT (key) DO
            UPDATE SET value = @value
            """;
    }

    public string CreateTable { get; }
    public string SelectKeys { get; }
    public string SelectValues { get; }
    public string DeleteItem { get; }
    public string UpdateItem { get; }
}
