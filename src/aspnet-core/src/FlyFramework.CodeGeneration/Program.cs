namespace FlyFramework.CodeGeneration
{
    internal static class Program
    {
        /// <summary>
        /// Ӧ�ó������Ҫ��ڵ㡣
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ����Ӧ�ó�������
            ApplicationConfiguration.Initialize();
            // ����������
            Application.Run(new Main());
        }
    }
}