using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class GameBase : Game
    {
        string log = "";
        public ShowScreen ShowScreen { get; set; } = ShowScreen.None;
        public AdMan AdMan { get; set; }
        public GameSystem GameSystem { get; set; }

        public Res Res { get; protected set; }

        public Platform Platform { get; protected set; } = Platform.Android;
        public Action ExitAction { get; protected set; } = null;
        public Input Input { get; private set; }
        public GameBase() {
            Input = new Input();
        }
        public void Update(float dt) {
            Input.Update();
        }
        public bool IsDebug()
        {
            //https://stackoverflow.com/questions/654450/programmatically-detecting-release-debug-mode-net
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public bool IsDesktop()
        {
            return GameSystem.GetPlatform() == Platform.Desktop;
        }
        public void Log(string x)
        {
            log += DateTime.Now.ToString() + ": " + x + "\r\n";
            if (log.Length > 100000)
            {
                log = "";
            }
        }
    }
}
