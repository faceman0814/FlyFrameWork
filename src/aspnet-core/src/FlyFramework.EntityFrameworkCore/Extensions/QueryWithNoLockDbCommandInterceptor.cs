using Microsoft.EntityFrameworkCore.Diagnostics;

using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


/// <summary>
/// EF Core 查询增加 With NoLock(仅限sqlserver)
/// </summary>
public class QueryWithNoLockDbCommandInterceptor : DbCommandInterceptor
{
    private static readonly Regex TableAliasRegex =
        new Regex(@"(?<tableAlias>(FROM|JOIN) \[[a-zA-Z]\w*\] AS \[[a-zA-Z]\w*\](?! WITH \(NOLOCK\)))",
            RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
            );
        if (command.CommandText.StartsWith("-- OPTION(RECOMPILE)", StringComparison.Ordinal))
        {
            command.CommandText += " OPTION(RECOMPILE)";
        }
        return base.ScalarExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
            );
        if (command.CommandText.StartsWith("-- OPTION(RECOMPILE)", StringComparison.Ordinal))
        {
            command.CommandText += " OPTION(RECOMPILE)";
        }
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
            );
        if (command.CommandText.StartsWith("-- OPTION(RECOMPILE)", StringComparison.Ordinal))
        {
            command.CommandText += " OPTION(RECOMPILE)";
        }
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        command.CommandText = TableAliasRegex.Replace(
            command.CommandText,
            "${tableAlias} WITH (NOLOCK)"
            );
        if (command.CommandText.StartsWith("-- OPTION(RECOMPILE)", StringComparison.Ordinal))
        {
            command.CommandText += " OPTION(RECOMPILE)";
        }
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
}

