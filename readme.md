    _________  ____   _ _   _ _       _   __           _                         _ _   _             _    
    |  ___|  \/  | | | | | (_) |     | | / /          | |                       | | | | |           | |   
    | |_  | .  . | | | | |_ _| |___  | |/ /  ___ _   _| |__   ___   __ _ _ __ __| | |_| | ___   ___ | | __
    |  _| | |\/| | | | | __| | / __| |    \ / _ \ | | | '_ \ / _ \ / _` | '__/ _` |  _  |/ _ \ / _ \| |/ /
    | |   | |  | | |_| | |_| | \__ \_| |\  \  __/ |_| | |_) | (_) | (_| | | | (_| | | | | (_) | (_) |   < 
    \_|   \_|  |_/\___/ \__|_|_|___(_)_| \_/\___|\__, |_.__/ \___/ \__,_|_|  \__,_\_| |_/\___/ \___/|_|\_\
                                                  __/ |                                                   
                                                 |___/                                                    


### A lightweight wrapper for SetWindowsHookEx
(but you can use it for whatever - see _license.txt_)


### State of the Code
- Mostly operational
- `Hook.cs` is where the action is, but it's in a project for simpler separation and distribution


### Usage

    var KeyboardHook = new Hook("Global Action Hook");
    KeyboardHook.KeyDownEvent += KeyDown;
    // Also: KeyboardHook.KeyUpEvent += KeyUp;
    
    private void KeyDown(KeyboardHookEventArgs e)
    {
        // handle keydown event here
        // Such as by checking if e (KeyboardHookEventArgs) matches the key you're interested in
        
        if (e.Key == Keys.F12 && e.isCtrlPressed)
        {
            // Do your magic...
        }
    }

`KeyboardHookEventArgs` is a helper class consisting of a `Key` core property, as well as breakout properties for left/right Ctrl/Alt/Win/Shift modifier keys.

See **[ProSnap](https://github.com/factormystic/ProSnap)** for an example of how I built an end-user application out of it.


