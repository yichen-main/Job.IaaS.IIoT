namespace Infrastructure.Garner.Architects.Develops;
public abstract class TacticDevelop
{
    public readonly ref struct Deputy
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
    public static async Task<bool> ExistDatabaseAsync(string name)
    {
        await using NpgsqlConnection npgsql = new(ConnectionString);
        await npgsql.OpenAsync();
        var results = await npgsql.QueryAsync($"SELECT datname FROM pg_catalog.pg_database WHERE datname = '{name}'");
        return results.Count() is not 0;
    }
    public static async Task<bool> ExistTableAsync(string name)
    {
        await using NpgsqlConnection npgsql = new(ConnectionString);
        await npgsql.OpenAsync();
        return await npgsql.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM pg_class WHERE relname = @name", new
        {
            name
        });
    }
    public static async Task<int> CountAsync(string content)
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            return await npgsql.QueryFirstAsync<int>(content);
        }
        return default;
    }
    public static async Task<IEnumerable<T>> QueryAsync<T>(string content, object? @object) where T : struct
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            return await npgsql.QueryAsync<T>(content, @object);
        }
        return await ValueTask.FromResult(Array.Empty<T>());
    }
    public static async Task<T> SingleQueryAsync<T>(string content, object? @object) where T : struct
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            return await npgsql.QuerySingleOrDefaultAsync<T>(content, @object);
        }
        return default;
    }
    public static async ValueTask ExecuteAsync(string content, object? @object)
    {
        if (ConnectionString is not null)
        {
            await using NpgsqlConnection npgsql = new(ConnectionString);
            await npgsql.OpenAsync();
            await npgsql.ExecuteAsync(content, @object);
            await npgsql.CloseAsync();
        }
    }
    public static async ValueTask TransactionAsync(IEnumerable<(string content, object? @object)> values)
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