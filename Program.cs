using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using StayAwake.Properties;

namespace StayAwake
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
            using var singleInstanceMutex = new Mutex(false, "StayAwakeMutex");
            var isAlreadyRunning = !singleInstanceMutex.WaitOne(TimeSpan.Zero);

            if (isAlreadyRunning)
                return;

            MainSingleInstance();
            singleInstanceMutex.ReleaseMutex();
        }

        private static void MainSingleInstance()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var _notifyIcon = new NotifyIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.Text = @"StayAwake";
            _notifyIcon.Icon = Resources.StayAwake;
            _notifyIcon.ContextMenu = new ContextMenu(new[]
            {
                new MenuItem("Exit", (sender, args) => Application.Exit())
            });

            winbase.SetThreadExecutionState(winbase.ExecutionState.EsContinuous |
                                            winbase.ExecutionState.EsDisplayRequired);
            Application.Run();

            winbase.SetThreadExecutionState(winbase.ExecutionState.EsContinuous);

            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }

    static class winbase
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [FlagsAttribute]
        public enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous       = 0x80000000,
            EsDisplayRequired  = 0x00000002,
            EsSystemRequired   = 0x00000001
        }
    }
}
