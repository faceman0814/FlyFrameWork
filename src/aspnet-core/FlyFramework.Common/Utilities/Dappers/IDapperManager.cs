using Dapper;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Dapper.SqlMapper;

namespace FlyFramework.Common.Utilities.Dappers
{
    public interface IDapperManager<T>
    {
        /// <summary>
        /// 获取指定表所有数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// 获取指定表指定条件的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// 指定表添加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// 指定表更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// 指定表删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.Text, int? commandTimeout = null, IDbTransaction transaction = null, bool buffered = true, IEnumerable<string> columns = null);

        /// <summary>
        /// 执行查询SQL语句并返回结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    }
}
