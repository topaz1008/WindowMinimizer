using Microsoft.Win32;
using System.Diagnostics;

namespace WindowMinimizer
{
    /// <summary>
    /// Handles saving and loading user preferences via the CurrentUser Registry.
    /// Requires no Administrator privileges.
    /// </summary>
    public static class SettingsManager
    {
        private const string AppRegistryKey = @"Software\WindowMinimizer";
        private const string RunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string FirstRun = "FirstRun";
        private const string AppName = "WindowMinimizer";

        public static Keys GetTriggerKey()
        {
            using var key = Registry.CurrentUser.CreateSubKey(AppRegistryKey);
            var value = key.GetValue("TriggerKey");

            if (value != null && Enum.TryParse(typeof(Keys), value.ToString(), out var result))
            {
                return (Keys)result;
            }

            return Keys.F24; // Default fallback
        }

        public static void SetTriggerKey(Keys keyBind)
        {
            using var key = Registry.CurrentUser.CreateSubKey(AppRegistryKey);
            key.SetValue("TriggerKey", keyBind.ToString());
        }

        public static bool GetRunAtStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey);

            return key?.GetValue(AppName) != null;
        }

        public static bool IsFirstRun()
        {
            using var key = Registry.CurrentUser.CreateSubKey(AppRegistryKey);
            if (key.GetValue(FirstRun) != null)
                return false;

            key.SetValue(FirstRun, 1);

            return true;
        }

        public static void SetRunAtStartup(bool run)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true);
            if (run)
            {
                // Encapsulate the executable path in quotes to prevent path-space issues
                string exePath = Process.GetCurrentProcess().MainModule!.FileName;
                key?.SetValue(AppName, $"\"{exePath}\"");
            }
            else
            {
                key?.DeleteValue(AppName, false);
            }
        }
    }
}
