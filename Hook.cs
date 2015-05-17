using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FMUtils.KeyboardHook
{
    public class Hook
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
                if (value != _ispaused && value == true)
                    StopHook();

                if (value != _ispaused && value == false)
                    StartHook();

                _ispaused = value;
            }
        }
        bool _ispaused = false;

        public delegate void KeyDownEventDelegate(KeyboardHookEventArgs e);
        public KeyDownEventDelegate KeyDownEvent = delegate { };

        public delegate void KeyUpEventDelegate(KeyboardHookEventArgs e);
        public KeyUpEventDelegate KeyUpEvent = delegate { };

        Win32.HookProc _hookproc;
        IntPtr _hhook;


        public Hook(string name)
        {
            Name = name;
            StartHook();
        }

        private void StartHook()
        {
            Trace.WriteLine(string.Format("Starting hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            _hookproc = new Win32.HookProc(HookCallback);
            _hhook = Win32.SetWindowsHookEx(Win32.HookType.WH_KEYBOARD_LL, _hookproc, Win32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            if (_hhook == null || _hhook == IntPtr.Zero)
            {
                Win32Exception LastError = new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void StopHook()
        {
            Trace.WriteLine(string.Format("Stopping hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            Win32.UnhookWindowsHookEx(_hhook);
        }

        private int HookCallback(int code, IntPtr wParam, ref Win32.KBDLLHOOKSTRUCT lParam)
        {
            int result = 0;

            try
            {
                if (!isPaused && code >= 0)
                {
                    if (wParam.ToInt32() == Win32.WM_SYSKEYDOWN || wParam.ToInt32() == Win32.WM_KEYDOWN)
                        KeyDownEvent(new KeyboardHookEventArgs(lParam));

                    if (wParam.ToInt32() == Win32.WM_SYSKEYUP || wParam.ToInt32() == Win32.WM_KEYUP)
                        KeyUpEvent(new KeyboardHookEventArgs(lParam));
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
            StopHook();
        }
    }

    public class KeyboardHookEventArgs
    {
        #region PInvoke
        [DllImport("user32.dll")]
        static extern short GetKeyState(VirtualKeyStates nVirtKey);

        private enum VirtualKeyStates : int
        {
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LALT = 0xA4, //aka VK_LMENU
            VK_RALT = 0xA5, //aka VK_RMENU
        }

        private const int KEY_PRESSED = 0x8000;
        #endregion

        public Keys Key { get; private set; }

        public bool isAltPressed { get { return isLAltPressed || isRAltPressed; } }
        public bool isLAltPressed { get; private set; }
        public bool isRAltPressed { get; private set; }

        public bool isCtrlPressed { get { return isLCtrlPressed || isRCtrlPressed; } }
        public bool isLCtrlPressed { get; private set; }
        public bool isRCtrlPressed { get; private set; }

        public bool isShiftPressed { get { return isLShiftPressed || isRShiftPressed; } }
        public bool isLShiftPressed { get; private set; }
        public bool isRShiftPressed { get; private set; }

        public bool isWinPressed { get { return isLWinPressed || isRWinPressed; } }
        public bool isLWinPressed { get; private set; }
        public bool isRWinPressed { get; private set; }

        internal KeyboardHookEventArgs(Win32.KBDLLHOOKSTRUCT lParam)
        {
            this.Key = (Keys)lParam.vkCode;

            //Control.ModifierKeys doesn't capture alt/win, and doesn't have r/l granularity
            this.isLAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LALT) & KEY_PRESSED) || this.Key == Keys.LMenu;
            this.isRAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RALT) & KEY_PRESSED) || this.Key == Keys.RMenu;

            this.isLCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LCONTROL) & KEY_PRESSED) || this.Key == Keys.LControlKey;
            this.isRCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RCONTROL) & KEY_PRESSED) || this.Key == Keys.RControlKey;

            this.isLShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LSHIFT) & KEY_PRESSED) || this.Key == Keys.LShiftKey;
            this.isRShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RSHIFT) & KEY_PRESSED) || this.Key == Keys.RShiftKey;

            this.isLWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LWIN) & KEY_PRESSED) || this.Key == Keys.LWin;
            this.isRWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RWIN) & KEY_PRESSED) || this.Key == Keys.RWin;

            if (new[] { Keys.LMenu, Keys.RMenu, Keys.LControlKey, Keys.RControlKey, Keys.LShiftKey, Keys.RShiftKey, Keys.LWin, Keys.RWin }.Contains(this.Key))
                this.Key = Keys.None;
        }

        public override string ToString()
        {
            return string.Format("Key={0}; Win={1}; Alt={2}; Ctrl={3}; Shift={4}", new object[] { Key, isWinPressed, isAltPressed, isCtrlPressed, isShiftPressed });
        }
    }
}
