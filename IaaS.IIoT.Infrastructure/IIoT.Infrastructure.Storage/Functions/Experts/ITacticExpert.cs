namespace Infrastructure.Storage.Functions.Experts;
public interface ITacticExpert
{
    Task<bool> ExistTableAsync(string name);
    Task<bool> ExistDatabaseAsync(string name);
    Task<int> CountAsync(string content);
    ValueTask ExecuteAsync(string content, object? @object);
    ValueTask TransactionAsync(IEnumerable<(string content, object? @object)> values);
    Task<T> SingleQueryAsync<T>(string content, object? @object) where T : struct;
    Task<IEnumerable<T>> QueryAsync<T>(string content, object? @object) where T : struct;
    readonly ref struct Deputy
    {
        public const string Root = "root";
        public const string Manage = "manage";
        public const string Mission = "mission";
        public const string Workshop = "workshop";
        public const string Equipment = "equipment";
        public const string Foundation = "foundation";
        public const string Process = "process";
        public const string Stack = "stack";
        public static string ComboLink => "combos";
    }
}
public abstract class TacticExpert : ITacticExpert
{
    public async Task<bool> ExistDatabaseAsync(string name)
    {
        await using NpgsqlConnection npgsql = new(ConnectionString);
        await npgsql.OpenAsync();
        var results = await npgsql.QueryAsync($"SELECT datname FROM pg_catalog.pg_database WHERE datname = '{name}'");
        return results.Count() is not 0;
    }
    public async Task<bool> ExistTableAsync(string name)
    {
        await using NpgsqlConnection npgsql = new(ConnectionString);
        await npgsql.OpenAsync();
        return await npgsql.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM pg_class WHERE relname = @name", new
        {
            name
        });
    }
    public async Task<int> CountAsync(string content)
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            return await npgsql.QueryFirstAsync<int>(content);
        }
        return default;
    }
    public async Task<IEnumerable<T>> QueryAsync<T>(string content, object? @object) where T : struct
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            return await npgsql.QueryAsync<T>(content, @object);
        }
        return await ValueTask.FromResult(Array.Empty<T>());
    }
    public async Task<T> SingleQueryAsync<T>(string content, object? @object) where T : struct
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            return await npgsql.QuerySingleOrDefaultAsync<T>(content, @object);
        }
        return default;
    }
    public async ValueTask ExecuteAsync(string content, object? @object)
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            await npgsql.ExecuteAsync(content, @object);
            await npgsql.CloseAsync();
        }
    }
    public async ValueTask TransactionAsync(IEnumerable<(string content, object? @object)> values)
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            await using var result = await npgsql.BeginTransactionAsync();
            try
            {
                foreach (var (content, @object) in values)
                {
                    await npgsql.ExecuteAsync(content, @object, transaction: result);
                }
                await result.CommitAsync();
            }
            catch (Exception)
            {
                await result.RollbackAsync();
                throw;
            }
            finally
            {
                await npgsql.CloseAsync();
            }
        }
    }
}