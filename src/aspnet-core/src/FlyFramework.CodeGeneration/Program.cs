namespace FlyFramework.CodeGeneration
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主要入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 加载应用程序配置
            ApplicationConfiguration.Initialize();
            // 创建主窗体
            Application.Run(new Main());
        }
    }
}