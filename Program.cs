namespace WindowMinimizer
{
    internal static class Program
    {
        // The main entry point for the application.
        [STAThread]
        private static void Main()
        {
            // Ensure only one instance is running
            using var mutex = new Mutex(true, "Global\\WindowMinimizer", out bool createdNew);
            if (!createdNew)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Launch the application without a visible form by passing our custom ApplicationContext
            Application.Run(new TrayApplicationContext());
        }
    }
}
