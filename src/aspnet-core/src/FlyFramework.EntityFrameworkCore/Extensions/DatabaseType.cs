using System.ComponentModel;

namespace FlyFramework.EntityFrameworkCore.Extensions
{
    public enum DatabaseType
    {
        [Description("SQLServer")]
        SqlServer,
        [Description("MySql")]
        MySql,
        [Description("Psotgre")]
        Psotgre,
        [Description("SQLite")]
        Sqlite
    }
}