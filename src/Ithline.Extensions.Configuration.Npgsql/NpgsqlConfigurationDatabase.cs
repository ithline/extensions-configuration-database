using Ithline.Extensions.Configuration.Database;
using Ithline.Extensions.Configuration.Npgsql.Internal;
using Microsoft.Extensions.Primitives;
using Npgsql;

namespace Ithline.Extensions.Configuration.Npgsql;

public sealed class NpgsqlConfigurationDatabase : IConfigurationDatabase
{
    private readonly ChangeTokenSource _tokenSource = new();
    private readonly NpgsqlCommandTexts _commandTexts;
    private readonly NpgsqlDataSource _dataSource;

    private bool _initialized;

    public NpgsqlConfigurationDatabase(NpgsqlDataSource dataSource, string tableName, string? schemaName = null)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _commandTexts = new NpgsqlCommandTexts(tableName, schemaName);
    }

    public string[] GetKeys()
    {
        this.Initialize();

        using var command = _dataSource.CreateCommand(_commandTexts.SelectKeys);
        using var reader = command.ExecuteReader();

        var builder = new ArrayBuilder<string>();
        while (reader.Read())
        {
            builder.Add(reader.GetString(0));
        }

        return builder.ToArray();
    }

    public KeyValuePair<string, string?>[] GetValues()
    {
        this.Initialize();

        using var command = _dataSource.CreateCommand(_commandTexts.SelectValues);
        using var reader = command.ExecuteReader();

        var builder = new ArrayBuilder<KeyValuePair<string, string?>>();
        while (reader.Read())
        {
            var key = reader.GetString(0);
            var value = reader.IsDBNull(1) ? null : reader.GetString(1);

            builder.Add(KeyValuePair.Create(key, value));
        }

        return builder.ToArray();
    }

    public IConfigurationDatabaseBatch CreateBatch()
    {
        return new BatchBuilder(_dataSource, _commandTexts, _tokenSource, this.Initialize);
    }

    public IDisposable OnChange(Action handler)
    {
        return ChangeToken.OnChange(() => _tokenSource.Token, handler);
    }

    public IDisposable OnChange<TState>(Action<TState> handler, TState state)
    {
        return ChangeToken.OnChange(() => _tokenSource.Token, handler, state);
    }

    private void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        lock (_tokenSource)
        {
            if (_initialized)
            {
                return;
            }

            using var command = _dataSource.CreateCommand(_commandTexts.CreateTable);
            command.ExecuteNonQuery();
            _initialized = true;
        }
    }

    private sealed class BatchBuilder : IConfigurationDatabaseBatch
    {
        private readonly NpgsqlCommandTexts _commandTexts;
        private readonly ChangeTokenSource _tokenSource;
        private readonly NpgsqlDataSource _dataSource;
        private readonly Action _initialize;

        private readonly Dictionary<string, string?> _setters = [];
        private readonly List<string> _deletes = [];

        public BatchBuilder(NpgsqlDataSource dataSource, NpgsqlCommandTexts commandTexts, ChangeTokenSource tokenSource, Action initialize)
        {
            _dataSource = dataSource;
            _commandTexts = commandTexts;
            _tokenSource = tokenSource;
            _initialize = initialize;
        }

        public IConfigurationDatabaseBatch Delete(string id)
        {
            _deletes.Add(id);
            return this;
        }

        public IConfigurationDatabaseBatch Set(string id, string? value)
        {
            _setters[id] = value;
            return this;
        }

        public void Run()
        {
            if (_deletes.Count is 0 && _setters.Count is 0)
            {
                return;
            }

            _initialize();
            using var batch = _dataSource.CreateBatch();

            foreach (var id in _deletes)
            {
                var command = batch.AddBatchCommand(_commandTexts.DeleteItem);
                command.Parameters.AddWithValue("@id", id);
            }

            foreach (var (id, value) in _setters)
            {
                var command = batch.AddBatchCommand(_commandTexts.UpdateItem);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@value", value ?? string.Empty);
            }

            batch.ExecuteNonQuery();
            _tokenSource.Raise();
        }
    }
}
