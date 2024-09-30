using Microsoft.EntityFrameworkCore.Diagnostics;

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework.Extensions
{
    public class CommentCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            ReplaceCommandTextCommentN(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default(CancellationToken))
        {
            ReplaceCommandTextCommentN(command);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public static void ReplaceCommandTextCommentN(DbCommand command)
        {
            if (command.CommandText.Contains("is N'"))
            {
                command.CommandText = command.CommandText.Replace("is N'", "is '");
            }
        }
    }
}