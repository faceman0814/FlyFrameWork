using System.ComponentModel;

namespace FlyFramework.Extensions
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