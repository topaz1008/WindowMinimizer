using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowMinimizer
{
    // Manages the system tray icon, lifecycle, and the global keyboard hook.
    public class TrayApplicationContext : ApplicationContext
    {
        // Key assigned in G-hub
        private const Keys TriggerKey = Keys.F24;

        private readonly NotifyIcon _trayIcon;
        private readonly IntPtr _hookId;

        public TrayApplicationContext()
        {
            // 1. Setup Context Menu
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, Exit_Click);

            // 2. Setup System Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = CreateTrayIcon(),
                ContextMenuStrip = contextMenu,
                Visible = true,
                Text = $"Window Minimizer (Listening for {TriggerKey})"
            };

            // 3. Register Global Keyboard Hook
            // We maintain a reference to the delegate (_proc) to prevent garbage collection
            NativeMethods.LowLevelKeyboardProc proc = HookCallback;
            _hookId = SetHook(proc);
        }

        // Generates a simple 16x16 icon (Blue background with a white minus sign)
        // dynamically in memory for the system tray.
        private Icon CreateTrayIcon()
        {
            using Bitmap bmp = new Bitmap(16, 16);
            using Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Transparent);
            g.FillRectangle(Brushes.DodgerBlue, 2, 2, 12, 12);
            g.DrawLine(new Pen(Color.White, 2), 4, 8, 12, 8);

            return Icon.FromHandle(bmp.GetHicon());
        }

        // Initializes the low-level keyboard hook into the Windows API.
        private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc,
                NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }

        // The callback function that Windows invokes when a key is pressed.
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Process WM_KEYDOWN events
            if (nCode >= 0 && wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
            {
                // Cast the memory address directly to the .NET Keys enum
                Keys pressedKey = (Keys)Marshal.ReadInt32(lParam);

                if (pressedKey == TriggerKey)
                {
                    // Find the currently active window and minimize it
                    IntPtr handle = NativeMethods.GetForegroundWindow();
                    if (handle != IntPtr.Zero)
                    {
                        NativeMethods.ShowWindow(handle, NativeMethods.SW_MINIMIZE);
                    }
                }
            }

            // Always pass the event to the next hook in the chain to not break other software
            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        // Cleans up unmanaged resources and exits the application.
        private void Exit_Click(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            NativeMethods.UnhookWindowsHookEx(_hookId);
            Application.Exit();
        }
    }
}
