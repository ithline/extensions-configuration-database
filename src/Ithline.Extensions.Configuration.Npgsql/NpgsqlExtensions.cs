namespace Npgsql;

internal static class NpgsqlExtensions
{
    public static NpgsqlBatchCommand AddBatchCommand(this NpgsqlBatch batch, string commandText)
    {
        var command = batch.CreateBatchCommand();
        command.CommandText = commandText;
        batch.BatchCommands.Add(command);

        return command;
    }
}
