namespace WindowMinimizer
{
    internal static class Program
    {
        // The main entry point for the application.
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Launch the application without a visible form by passing our custom ApplicationContext
            Application.Run(new TrayApplicationContext());
        }
    }
}
