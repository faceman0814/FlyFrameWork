using Dapper;

using System.Data;

using static Dapper.SqlMapper;

namespace FlyFramework.Common.Utilities.Dappers
{
    public class DapperManager<T> : IDapperManager<T>
    {
        private readonly IDbConnection _dbConnection;
        public DapperManager(IDbConnection dbConnection)

        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // 假设表名与类名相同，首字母大写
            string tableName = typeof(T).Name;
            return await _dbConnection.QueryAsync<T>($"SELECT * FROM [{tableName}]");
        }

        public async Task<T> GetByIdAsync(string id)
        {
            // 假设表名与类名相同，首字母大写
            string tableName = typeof(T).Name;
            return await _dbConnection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM [{tableName}] WHERE Id = @id", new { id });
        }

        public async Task AddAsync(T entity)
        {
            // 假设表名与类名相同，首字母大写
            string tableName = typeof(T).Name;

            // 使用反射获取所有公共属性
            var properties = typeof(T).GetProperties();

            // 构建SQL语句的列部分和参数部分
            var columnNames = new List<string>();
            var parameterNames = new List<string>();
            foreach (var property in properties)
            {
                columnNames.Add(property.Name);
                parameterNames.Add("@" + property.Name);
            }

            string sql = $"INSERT INTO [{tableName}] ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", parameterNames)})";

            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                // 获取属性值
                var value = property.GetValue(entity);
                // 添加参数
                parameters.Add("@" + property.Name, value);
            }

            await _dbConnection.ExecuteAsync(sql, parameters);
        }

        public async Task UpdateAsync(T entity)
        {
            // 假设表名与类名相同，首字母大写
            string tableName = typeof(T).Name;

            // 使用反射获取所有公共属性
            var properties = typeof(T).GetProperties();

            // 构建SQL语句的SET部分
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                if (property.Name != "Id" &&
                    property.Name != "DeleterUserName" &&
                    property.Name != "DeletionTime" &&
                    property.Name != "DeleterUserId" &&
                    property.Name != "IsDelete" &&
                    property.Name != "CreatorUserId" &&
                    property.Name != "CreatorUserName" &&
                    property.Name != "CreationTime")  // 假设"Id"是主键，不包括在更新中
                {
                    setClauses.Add($"{property.Name} = @{property.Name}");
                    parameters.Add("@" + property.Name, property.GetValue(entity));
                }
            }

            // 添加主键到参数
            var keyProperty = properties.FirstOrDefault(p => p.Name == "Id");
            if (keyProperty == null)
            {
                throw new Exception("Key property 'Id' not found in " + typeof(T).Name);
            }
            parameters.Add("@Id", keyProperty.GetValue(entity));

            string sql = $"UPDATE [{tableName}] SET {string.Join(", ", setClauses)} WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(sql, parameters);

        }

        public async Task DeleteAsync(string id)
        {
            // 假设表名与类名相同，首字母大写
            string tableName = typeof(T).Name;
            await _dbConnection.ExecuteAsync($"DELETE FROM [{tableName}] WHERE Id = @id", new { id });
        }

        public async Task ExecuteAsync(string sql,
                               object param = null,
                               CommandType commandType = CommandType.Text,
                               int? commandTimeout = null,
                               IDbTransaction transaction = null,
                               bool buffered = true,
                               IEnumerable<string> columns = null)
        {
            // 判断是否只需要选取特定的列
            if (columns != null && columns.Any())
            {
                string columnString = string.Join(", ", columns);
                sql = $"SELECT {columnString} FROM ({sql}) AS SubQuery";
            }

            // 执行 SQL 命令
            await _dbConnection.ExecuteAsync(new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: transaction,
                commandTimeout: commandTimeout,
                commandType: commandType,
                flags: buffered ? CommandFlags.Buffered : CommandFlags.None
            ));
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql,
                                                object param = null,
                                                bool buffered = true,
                                                int? commandTimeout = null,
                                                CommandType? commandType = null)
        {
            // 执行 SQL 查询并返回结果集
            var result = await _dbConnection.QueryAsync<T>(
                sql: sql,
                param: param,
                commandTimeout: commandTimeout,
                commandType: commandType ?? CommandType.Text // 如果未指定，则默认为 Text
            );

            return result;
        }
    }
}