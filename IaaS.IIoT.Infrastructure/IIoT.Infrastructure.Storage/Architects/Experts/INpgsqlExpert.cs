namespace Infrastructure.Storage.Architects.Experts;
public interface INpgsqlExpert
{
    string MarkTable<T>(in TableInfo info) where T : struct;
    string MarkInsert<T>() where T : struct;
    string MarkUpdate<T>(in IEnumerable<string> names) where T : struct;
    string MarkDelete<T>(Guid primaryKey) where T : struct;
    StringBuilder MarkQuery<T>(in IEnumerable<string> names) where T : struct;
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class NpgsqlElement : INpgsqlExpert
{
    const string _primaryKey = " PRIMARY KEY";
    public string MarkTable<T>(in TableInfo info) where T : struct
    {
        List<string> results = new();
        for (int i = default; i < typeof(T).GetProperties().Length; i++)
        {
            var field = typeof(T).GetProperties()[i].Name.FieldInfo<T>();
            System.Runtime.CompilerServices.DefaultInterpolatedStringHandler content = new(default, field.PK ? 3 : 2);
            content.AppendFormatted(field.Name);
            switch (typeof(T).GetProperties()[i].PropertyType)
            {
                case var item when item.Equals(typeof(Guid)):
                    content.AppendFormatted(" UUID NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.IsEnum:
                    content.AppendFormatted(" SMALLINT NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(short)):
                    content.AppendFormatted(" SMALLINT NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(int)):
                    content.AppendFormatted(" INTEGER NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(long)):
                    content.AppendFormatted(" BIGINT NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(float)):
                    content.AppendFormatted(" REAL NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(double)):
                    content.AppendFormatted(" DOUBLE PRECISION NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(string)):
                    content.AppendFormatted(" VARCHAR NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;

                case var item when item.Equals(typeof(DateTime)):
                    content.AppendFormatted(" TIMESTAMP WITHOUT TIME ZONE NOT NULL");
                    if (field.PK) content.AppendFormatted(_primaryKey);
                    break;
            }
            results.Add(content.ToStringAndClear());
        }
        results.AddRange(info.Uniques.Select(item => $"UNIQUE ({item})"));
        results.AddRange(info.Combos.Select(item => $"CONSTRAINT {item.tag} UNIQUE ({item.fields.DelimitMark()})"));
        results.AddRange(info.Foreigns.Select(item => $"FOREIGN KEY ({item.field}) REFERENCES {item.table} ({item.key})"));
        return $"CREATE TABLE {TableName<T>()} ({results.DelimitMark()})";
    }
    public string MarkInsert<T>() where T : struct
    {
        List<string> uppers = new();
        List<string> lowers = new();
        for (int i = default; i < typeof(T).GetProperties().Length; i++)
        {
            uppers.Add(typeof(T).GetProperties()[i].Name.FieldInfo<T>().Name);
            lowers.Add($"@{typeof(T).GetProperties()[i].Name}");
        }
        return $"INSERT INTO {TableName<T>()} ({uppers.DelimitMark()}) VALUES ({lowers.DelimitMark()})";
    }
    public string MarkUpdate<T>(in IEnumerable<string> names) where T : struct
    {
        var condition = string.Empty;
        List<string> results = new();
        for (int i = default; i < typeof(T).GetProperties().Length; i++)
        {
            var field = typeof(T).GetProperties()[i].Name.FieldInfo<T>();
            if (field.PK) condition = $"WHERE {field.Name} = @{typeof(T).GetProperties()[i].Name}";
            if (names.Any(item => item == typeof(T).GetProperties()[i].Name)) results.Add($"{field.Name} = @{typeof(T).GetProperties()[i].Name}");
        }
        return $"UPDATE {TableName<T>()} SET {results.DelimitMark()} {condition}";
    }
    public string MarkDelete<T>(Guid key) where T : struct
    {
        for (int i = default; i < typeof(T).GetProperties().Length; i++)
        {
            var field = typeof(T).GetProperties()[i].Name.FieldInfo<T>();
            if (field.PK) return $"DELETE FROM {TableName<T>()} WHERE {field.Name} = '{key}'";
        }
        return string.Empty;
    }
    public StringBuilder MarkQuery<T>(in IEnumerable<string> names) where T : struct
    {
        List<string> results = new();
        for (int i = default; i < typeof(T).GetProperties().Length; i++)
        {
            if (names.Any(item => item == typeof(T).GetProperties()[i].Name)) results.Add(typeof(T).GetProperties()[i].Name.FieldInfo<T>().Name);
        }
        return new StringBuilder().AppendFormat($"SELECT {results.DelimitMark()} FROM {TableName<T>()} ");
    }
}