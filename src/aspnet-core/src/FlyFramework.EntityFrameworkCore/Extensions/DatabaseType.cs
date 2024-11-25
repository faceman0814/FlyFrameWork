using System.ComponentModel;

namespace FlyFramework.Extensions
{
    public enum DatabaseType
    {
        [Description("SQLServer")]
        SqlServer,
        [Description("MySql")]
        MySql,
        [Description("Postgre")]
        Postgre,
        [Description("Oracle")]
        Oracle,
        [Description("SQLite")]
        Sqlite
    }
}