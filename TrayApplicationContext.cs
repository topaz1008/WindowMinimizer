using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowMinimizer
{
    public class TrayApplicationContext : ApplicationContext
    {
        // Must be static to be accessed by the unmanaged static HookCallback
        private static Keys _triggerKey;

        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenuStrip _contextMenu;

        // Pinned references to prevent GC collection during unmanaged callbacks
        private static NativeMethods.LowLevelKeyboardProc _proc = null!;
        private static IntPtr _hookId = IntPtr.Zero;

        public TrayApplicationContext()
        {
            _triggerKey = SettingsManager.GetTriggerKey();

            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Settings", null, Settings_Click);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("Exit", null, Exit_Click);

            _trayIcon = new NotifyIcon
            {
                Icon = CreateTrayIcon(),
                ContextMenuStrip = _contextMenu,
                Visible = true,
                Text = $"Window Minimizer (Listening for {_triggerKey})"
            };

            _trayIcon.DoubleClick += Settings_Click;

            _proc = HookCallback;
            _hookId = SetHook(_proc);

            if (SettingsManager.IsFirstRun())
                _trayIcon.ShowBalloonTip(4000, "Window Minimizer",
                    $"Running in the background. Press {_triggerKey} to minimize the active window.", ToolTipIcon.Info);
        }

        // Called by OptionsForm after the user saves — updates the hook without a restart.
        public void ApplyNewKeybind(Keys newKey)
        {
            _triggerKey = newKey;
            _trayIcon.Text = $"Window Minimizer (Listening for {_triggerKey})";
        }

        private void Settings_Click(object? sender, EventArgs e)
        {
            // Prevent multiple settings windows from opening simultaneously
            if (Application.OpenForms["OptionsForm"] == null)
            {
                new OptionsForm(this).Show();
            }
            else
            {
                Application.OpenForms["OptionsForm"]!.Activate();
            }
        }

        private static Icon CreateTrayIcon()
        {
            using Bitmap bmp = new Bitmap(16, 16);
            using Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Transparent);
            g.FillRectangle(Brushes.DodgerBlue, 2, 2, 12, 12);
            g.DrawLine(new Pen(Color.White, 2), 4, 8, 12, 8);

            return Icon.FromHandle(bmp.GetHicon());
        }

        private static IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;

            return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc,
                NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    int message = wParam.ToInt32();

                    if (message == (int)NativeMethods.WM_KEYDOWN)
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        Keys pressedKey = (Keys)vkCode;

                        if (pressedKey == _triggerKey)
                        {
                            IntPtr handle = NativeMethods.GetForegroundWindow();
                            if (handle != IntPtr.Zero)
                            {
                                NativeMethods.ShowWindow(handle, (int)NativeMethods.SW_MINIMIZE);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore exceptions to prevent crashing the hook chain
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            NativeMethods.UnhookWindowsHookEx(_hookId);
            Application.Exit();
        }
    }
}
