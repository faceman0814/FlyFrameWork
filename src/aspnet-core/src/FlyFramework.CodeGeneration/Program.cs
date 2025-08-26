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
            // 初始化应用程序设置
            ApplicationConfiguration.Initialize();
            // 运行主窗体
            Application.Run(new Main());
        }
    }
}