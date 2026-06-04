namespace WindowMinimizer
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Ensure only one instance is running
            using var mutex = new Mutex(true, "Global\\WindowMinimizer", out bool createdNew);
            if (!createdNew)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // No visible form — the tray icon is the entire UI
            Application.Run(new TrayApplicationContext());
        }
    }
}
