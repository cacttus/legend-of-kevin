using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace Core
{
    public abstract class AdMan
    {
        public abstract bool IsVisible(string adName);

        public abstract void ShowAd(string adName);

        public abstract void HideAd(string adName);
        public abstract void Add(string name, string unitId, Vector2 xy, Vector2 wh);
    }
}
