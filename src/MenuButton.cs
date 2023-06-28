using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Core
{
    public class MenuButton : Touchable
    {
        public vec2 Pos { get;  set; }
        public Rectangle tex_up_rect { get; private set; }
        Rectangle tex_down_rect;
        Screen Screen = null;
        public Action<SpriteBatch, MenuButton> AddlDraw { get; set; } = null;
        //   bool _bUp = false;

        //public GameObject Tag { get; set; } = null;
        public int ScrollOffset { get; set; } = 0;

        float fCustomWidth = 1, fCustomHeight = 1;
        Color Color = Color.White;
        public MenuButton(Screen w, vec2 xy1_tiles, vec2 wh1_tiles, vec2 xy2_tiles, vec2 wh2_tiles, vec2 pos_pixels, 
            bool visible, float customWidth = -1, float customHeight = -1, Color? color = null)
        {
            if (color != null)
            {
                Color = color.Value ;
            }
            fCustomWidth = customWidth;
            fCustomHeight = customHeight;

            Visible = visible;
            tex_up_rect = new Rectangle((int)xy1_tiles.x, (int)xy1_tiles.y, (int)wh1_tiles.x, (int)wh1_tiles.y);
            tex_down_rect = new Rectangle((int)xy2_tiles.x, (int)xy2_tiles.y, (int)wh2_tiles.x, (int)wh2_tiles.y);
            Pos = pos_pixels;
            Screen = w;
            // Click = action;
        }
        public override Rectangle GetDest()
        {
            //Return a rectangle in DEVICE PIXELS of this button

            //Used for both rastering + Touch.
            float width = 0;
            float height = 0;
            Rectangle d = new Rectangle(0,0,0,0);
            if (TouchState == TouchState.Up || TouchState == TouchState.Release)
            {
                if (fCustomWidth > 0) { width = fCustomWidth; } else { width = tex_up_rect.Width * Screen.Game.Res.Tiles.TileWidthPixels; }
                if (fCustomHeight > 0) { height = fCustomHeight; } else { height = tex_up_rect.Height * Screen.Game.Res.Tiles.TileHeightPixels; }

            }
            else
            {
                if (fCustomWidth > 0) { width = fCustomWidth; } else { width = tex_down_rect.Width * Screen.Game.Res.Tiles.TileWidthPixels; }
                if (fCustomHeight > 0) { height = fCustomHeight; } else { height = tex_down_rect.Height * Screen.Game.Res.Tiles.TileHeightPixels; }

            }

            //translate to world coords by adding vp pos - vp calculates everything in world, and then converts to screen.
            //so really ,there's no screen cords
            return Screen.Viewport.WorldToDevice(Pos + Screen.Viewport.Pos, new vec2(width, height));
        }

        public void Draw(SpriteBatch batch)
        {
           // Rectangle dest = GetDest();
            Rectangle device = GetDest();
            Rectangle frame ;
            if (TouchState == TouchState.Up || TouchState == TouchState.Release)
            {
                frame = tex_up_rect;
            }
            else
            {
                frame = tex_down_rect;
            }

            //Screen automatically converts world stuf, so we need to translate it back to the world.
           // vec2 pos_pixels = Screen.Viewport.Pos + new vec2(Pos.x, Pos.y);
           // vec2 wh_pixels = new vec2((float)up_or_down.Width , (float)up_or_down.Height );


            Screen.DrawFrameRectDevice(
                batch, 
                Screen.Game.Res.Tiles.FrameRect(frame),
                device,
                Color);

            if (AddlDraw != null)
            {
                AddlDraw(batch, this);
            }
        }
    }
}
