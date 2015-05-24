using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace FMUtils.KeyboardHook
{
    public sealed class Hook : IDisposable
    {
        public string Name { get; private set; }

        /// <summary>
        /// When true, suspends firing of the hook notification events
        /// </summary>
        public bool isPaused
        {
            get
            {
                return _ispaused;
            }
            set
            {
                _ispaused = value;
            }
        }
        bool _ispaused = false;

        public event EventHandler<KeyboardHookEventArgs> KeyDownEvent = delegate { };
        public event EventHandler<KeyboardHookEventArgs> KeyUpEvent = delegate { };

        IntPtr _hhook = IntPtr.Zero;


        public Hook(string name)
        {
            Name = name;
            StartHook();
        }

        private void StartHook()
        {
            Trace.WriteLine(string.Format("Starting hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            _hhook = Win32.SetWindowsHookEx(Win32.HookType.WH_KEYBOARD_LL, new Win32.HookProc(HookCallback), Win32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            if (_hhook == IntPtr.Zero)
            {
                Win32Exception LastError = new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void StopHook()
        {
            Trace.WriteLine(string.Format("Stopping hook '{0}'...", Name), string.Format("Hook.StopHook [{0}]", Thread.CurrentThread.Name));

            if (_hhook == IntPtr.Zero)
                return;

            if (Win32.UnhookWindowsHookEx(_hhook) == 0)
            {
                Win32Exception LastError = new Win32Exception(Marshal.GetLastWin32Error());
            }

            _hhook = IntPtr.Zero;
        }

        private int HookCallback(int code, IntPtr wParam, ref Win32.KBDLLHOOKSTRUCT lParam)
        {
            int result = 0;

            try
            {
                if (!isPaused && code >= 0)
                {
                    if (wParam.ToInt32() == Win32.WM_SYSKEYDOWN || wParam.ToInt32() == Win32.WM_KEYDOWN)
                        KeyDownEvent(this, new KeyboardHookEventArgs(lParam));

                    if (wParam.ToInt32() == Win32.WM_SYSKEYUP || wParam.ToInt32() == Win32.WM_KEYUP)
                        KeyUpEvent(this, new KeyboardHookEventArgs(lParam));
                }
            }
            finally
            {
                result = Win32.CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
            }

            return result;
        }

        ~Hook()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool isDisposing)
        {
            StopHook();
        }
    }
}
