using System;
using System.Windows.Forms;

namespace FMUtils.KeyboardHook
{
    public class KeyboardHookEventArgs : EventArgs
    {
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
            this.isLAltPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_LALT) & Win32.KEY_PRESSED) || this.Key == Keys.LMenu;
            this.isRAltPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_RALT) & Win32.KEY_PRESSED) || this.Key == Keys.RMenu;

            this.isLCtrlPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_LCONTROL) & Win32.KEY_PRESSED) || this.Key == Keys.LControlKey;
            this.isRCtrlPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_RCONTROL) & Win32.KEY_PRESSED) || this.Key == Keys.RControlKey;

            this.isLShiftPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_LSHIFT) & Win32.KEY_PRESSED) || this.Key == Keys.LShiftKey;
            this.isRShiftPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_RSHIFT) & Win32.KEY_PRESSED) || this.Key == Keys.RShiftKey;

            this.isLWinPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_LWIN) & Win32.KEY_PRESSED) || this.Key == Keys.LWin;
            this.isRWinPressed = Convert.ToBoolean(Win32.GetKeyState(Win32.VirtualKeyStates.VK_RWIN) & Win32.KEY_PRESSED) || this.Key == Keys.RWin;

            switch (this.Key)
            {
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.LWin:
                case Keys.RWin:
                    this.Key = Keys.None;
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("Key={0}; Win={1}; Alt={2}; Ctrl={3}; Shift={4}", new object[] { Key, isWinPressed, isAltPressed, isCtrlPressed, isShiftPressed });
        }
    }
}
