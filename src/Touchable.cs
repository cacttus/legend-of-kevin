using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Core
{
    public abstract class Touchable
    {
        public TouchState TouchState { get; private set; } = TouchState.Up;
        public bool Visible { get; set; } = true;
        public Action<object> Release { get; set; } = null;
        public Action<object> Press { get; set; } = null;
        public Action<object> Down { get; set; } = null;
        public bool Update(Input inp)
        {

            if (this.Visible && inp.Global.TouchState != TouchState.Up && Globals.Pick(GetDest(), inp.LastTouch))
            {
                //Visual state
                if (inp.Global.TouchState == TouchState.Release && (TouchState == TouchState.Down || TouchState == TouchState.Press) && Release!=null)
                {
                    Release(this);
                        
                }

                if (inp.Global.TouchState == TouchState.Press && Press != null)
                {
                    Press(this);
                }

                if (inp.Global.TouchState == TouchState.Down && Down != null)
                {
                    Down(this);
                }

                TouchState = inp.Global.TouchState;
            }
            else
            {
                TouchState = TouchState.Up;
            }

            return (TouchState == TouchState.Down || TouchState == TouchState.Press);
            

        }
        public abstract Rectangle GetDest();
    }


}
