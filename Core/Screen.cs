using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Core
{
    public class Viewport
    {
        //public float DeviceToWorldRatioW { get; private set; }
        //public float DeviceToWorldRatioH { get; private set; }
        public Box2f Box;

        public void CalcBoundBox()
        {
            Box = new Box2f();
            Box.Min = Pos;
            Box.Max = Box.Min + new vec2(WidthPixels, HeightPixels);
        }
        public vec2 Pos;//Position in Game Pixels (not device pixels)

        public float WidthPixels {
            get {
                return TilesWidth * Res.Tiles.TileWidthPixels;
            }
        }
        public float HeightPixels {
            get {
                return TilesHeight * Res.Tiles.TileHeightPixels;
            }
        }
        //public vec2 WH { get; set; }
        //Not XNA viewprot, this is how many "tiles" width 
        public float TilesWidth
        {
            get
            {
                return (float)Math.Ceiling(TilesHeight * (float)((float)Screen.Game.GraphicsDevice.Viewport.Width / (float)Screen.Game.GraphicsDevice.Viewport.Height));
            }
        } // This is calculated automatically based on idsplay size.
        //Height is always 6 tiles.
        public float TilesHeight { get; set; } = 10;
        public Screen Screen;

        public Viewport(Screen s)
        {
            Screen = s;
        }
        public void FollowObject(GameObject ob)
        {
            //Follow a game object
            //Call with LimitScrollBounds to limit scrolling in a region
            Screen.Viewport.Pos = new vec2(
            ob.Pos.x - Res.Tiles.TileWidthPixels * Screen.Viewport.TilesWidth * 0.5f + Res.Tiles.TileWidthPixels * 0.5f
            ,
            ob.Pos.y - Res.Tiles.TileHeightPixels * Screen.Viewport.TilesHeight * 0.5f + Res.Tiles.TileHeightPixels * 0.5f
            ) + Screen.ScreenShake.ScreenShakeOffset;
        }
        public void LimitScrollBounds(Box2f box)
        {
            //Limit the viewport to scrolling box
            if(box.Width() > this.WidthPixels )
            {
                if(Pos.x < box.Min.x)
                {
                    Pos.x = box.Min.x;
                }
                if(Pos.x + this.WidthPixels >= box.Max.x)
                {
                    Pos.x = box.Max.x - this.WidthPixels;
                }
            }
            if(box.Height() > this.HeightPixels)
            {
                if (Pos.y < box.Min.y)
                {
                    Pos.y = box.Min.y;
                }
                if (Pos.y + this.HeightPixels >= box.Max.y)
                {
                    Pos.y = box.Max.y - this.HeightPixels;
                }
            }
        }
        public bool ObjectIsOutsideViewport(GameObject ob)
        {
            //Return true if gameobjet is outside viewport
            if(ob.Pos.x > Pos.x + WidthPixels) { return true; }
            if(ob.Pos.y > Pos.y + HeightPixels) { return true; }
            if(ob.Pos.x + ob.Size.x < Pos.x) { return true; }
            if(ob.Pos.y + ob.Size.y < Pos.y) { return true; }
            return false;
        }

        public vec2 MeasureString(SpriteFont font, string str)
        {
            Vector2 v = font.MeasureString(str);
            float w_ratio_inv = 1.0f / (Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels);
            float h_ratio_inv = 1.0f / (Screen.Game.GraphicsDevice.Viewport.Height / HeightPixels);

            return new vec2(v.X * w_ratio_inv, v.Y * h_ratio_inv);


        }
        public vec2 ScreenPixelsToScreenRaster(vec2 xy)
        {
            float w_ratio = Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels;
            float h_ratio = Screen.Game.GraphicsDevice.Viewport.Height / HeightPixels;

            return new vec2(xy.x * w_ratio, xy.y * h_ratio);
        }
        public vec2 ScreenRasterToScreenPixels(vec2 xy)
        {
            float w_ratio = Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels;
            float h_ratio = Screen.Game.GraphicsDevice.Viewport.Height / HeightPixels;

            return new vec2(xy.x * (1/w_ratio), xy.y * (1/h_ratio));
        }
        public vec2 WorldToDevice(vec2 world)
        {
            //World => Screen => Device
            vec2 dp = world - Pos;
            return ScreenToDevice(dp);
        }
        public vec2 ScreenToDevice(vec2 screen)
        {
            //World => Screen => Device

            float w_ratio = Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels;
            float h_ratio = Screen.Game.GraphicsDevice.Viewport.Height / HeightPixels;

            return new vec2(
                (float)Math.Round(screen.x * w_ratio),
                (float)Math.Round(screen.y * h_ratio)
            );

        }
        public Rectangle WorldToDevice(vec2 pos_pixels, vec2 wh_pixels)
        {
            //Converts Pos + wh from WORLD to SCREEN coordinates
            vec2 dp = pos_pixels - Pos;

            float w_ratio = Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels;
            float h_ratio = Screen.Game.GraphicsDevice.Viewport.Height/ HeightPixels;

            Rectangle ret = new Rectangle(
                (int)(Math.Ceiling(dp.x * w_ratio)), 
                (int)(Math.Ceiling(dp.y * h_ratio)),
                (int)(Math.Ceiling((wh_pixels.x) * w_ratio)), 
                (int)(Math.Ceiling((wh_pixels.y) * h_ratio))
                );

            return ret;
        }
    }

    public abstract class Screen
    {
        public List<DelayedAction> Actions { get; private set; } = new List<DelayedAction>();
        public Viewport Viewport { get; private set; }
        public ScreenShake ScreenShake { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public List<MenuButton> MenuButtons = new List<MenuButton>();
        public GameBase Game { get; private set; } = null;
       // public Joystick Joystick { get; private set; }

        public bool DisableMenuInput { get; set; } = false;

        public Texture2D PixelTexture { get; private set; }


        public virtual void Init(GameBase game)
        {
            Game = game;
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
 
            Viewport = new Viewport(this);

            ScreenShake = new ScreenShake(Viewport);
           // Joystick = new Joystick(this);

            // Somewhere in your LoadContent() method:
            PixelTexture = new Texture2D(game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            PixelTexture.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it

        }

        public void DrawBoxOutlineWorld(SpriteBatch spriteBatch, Box2f box, Color color)
        {
            //Draw a box that is specified in world coordinates
            Box2f gridBox = new Box2f(
                Viewport.WorldToDevice(box.Min),
                Viewport.WorldToDevice(box.Max)
                );
            DrawBoxOutlineDevice(spriteBatch, gridBox, color);
        }
        public void DrawBoxOutlineScreen(SpriteBatch spriteBatch, Box2f box, Color color)
        {
            //Draw a box that is specified in world coordinates
            Box2f gridBox = new Box2f(
                Viewport.ScreenToDevice(box.Min),
                Viewport.ScreenToDevice(box.Max)
                );
            DrawBoxOutlineDevice(spriteBatch, gridBox, color);
        }
        public void DrawPointWorld(SpriteBatch sb, vec2 point, float size_pixels, Color color)
        {
            Box2f b = new Box2f(point - new vec2(size_pixels, size_pixels), point + new vec2(size_pixels, size_pixels));

            DrawBoxSolidWorld(sb, b, color);
        }
        public void DrawBoxSolidWorld(SpriteBatch sb, Box2f b, Color color)
        {
            Box2f gridBox = new Box2f( Viewport.WorldToDevice(b.Min), Viewport.WorldToDevice(b.Max));

            DrawBoxSolidDevice(sb, gridBox.ToXNARect(), color);
        }
        public void DrawBoxSolidDevice(SpriteBatch sb, Rectangle rect, Color color)
        {
            sb.Draw(PixelTexture, rect, color);
        }
        public void DrawBoxOutlineDevice(SpriteBatch spriteBatch, Box2f box, Color color)
        {
            //Draws a box that is specified in device pixels
            DrawBoxOutlineDevice(spriteBatch, box.ToXNARect(), 1, color);
        }
        public void DrawBoxOutlineDevice(SpriteBatch spriteBatch, Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            spriteBatch.Draw(PixelTexture, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(PixelTexture, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(PixelTexture, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(PixelTexture, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
        public void DrawLineWorld(SpriteBatch spriteBatch, vec2 a, vec2 b, int thicknessOfBorder, Color borderColor)
        {
            DrawLineDevice(spriteBatch, Viewport.WorldToDevice(a), Viewport.WorldToDevice(b), thicknessOfBorder, borderColor);
        }

        public void DrawLineDevice(SpriteBatch spriteBatch, vec2 a, vec2 b, int thicknessOfBorder, Color borderColor)
        {
            float angle = MathUtils.GetRotationFromLine(a.x, a.y, b.x, b.y);

            float off = (float)thicknessOfBorder * 0.5f;

            // Draw top line
            spriteBatch.Draw(PixelTexture, 
                new Rectangle((int)(a.x ), (int)(a.y), (int)(b-a).Len(), thicknessOfBorder), 
                null, 
                borderColor, angle, new Vector2(0, 0.5f), SpriteEffects.None, 0.0f);
        }

        public void UpdateActions(float dt)
        {
            //These are actions for the room.  Generally
            //List<DelayedAction> dead = new List<DelayedAction>();
            //foreach (DelayedAction a in Actions)
            //{
            //    a.Update(dt);
            //    if (a.TimeRemaining <= 0.0 && a.Repeat == false)
            //    {
            //        dead.Add(a);
            //    }
            //}
            //foreach (DelayedAction d in dead)
            //{
            //    Actions.Remove(d);
            //}

        }
        public virtual void Update(float dt)
        {
            UpdateActions(dt);

            ScreenShake.Update(dt);

            if (DisableMenuInput == false)
            {
                //Update and also calcualte whether the user clicked a menu item.
                Game.Input.UIClicked = false;
                foreach (MenuButton mb in MenuButtons)
                {
                    if (mb.Visible)
                    {
                        bool click = mb.Update(Game.Input);
                        Game.Input.UIClicked = Game.Input.UIClicked || click;
                    }
                }
            }
        }
        public void BeginDraw()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, 
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
        public abstract void Draw();
        public virtual void DrawMenu()
        {
            foreach (MenuButton mb in MenuButtons)
            {
                if (mb.Visible)
                {
                    mb.Draw(SpriteBatch);
                }
            }
        }
        public void EndDraw()
        {
            SpriteBatch.End();
        }
        public void DrawStringFit(SpriteBatch sb, string str, SpriteFont f, vec2 pos, float width)
        {
            //Draws a string at Pos fitted to Width, which is relative to the viewport width in pixesl

            Vector2 siz = f.MeasureString(str);
            //+new vec2(Screen.Viewport.WidthPixels - siz.X, siz.Y)
            //Here we can add adndoird scaling.
            sb.DrawString(
                f,
                str,
                (pos).toXNA(),
                Color.Black
                );
        }
        public void DrawFrame(SpriteBatch sb, Frame f, vec2 pos, vec2 wh, Color color, 
            vec2 scale, float rotation = 0.0f, vec2 origin = default(vec2), SpriteEffects se = SpriteEffects.None)
        {

            DrawFrameRect(sb, f.R, pos, wh, color, scale, rotation, origin, se);
        }
        public void DrawFrameRect(SpriteBatch sb, Rectangle rect_tiles, vec2 pos_pixels, vec2 wh_pixels, Color color,
            vec2 scale, float rotation = 0.0f, vec2 origin = default(vec2), SpriteEffects se = SpriteEffects.None)
        {
            Rectangle r = Viewport.WorldToDevice(pos_pixels, wh_pixels * scale);
            sb.Draw(Res.Tiles.Texture,
                r,
                rect_tiles
                , color , rotation, origin.toXNA(), se, 0.0f);
        }
        public void DrawFrameRectDevice(SpriteBatch sb, Rectangle rect_tiles, Rectangle deviceRect, Color color,
    float a = 1.0f, float scale = 1.0f, float rotation = 0.0f, vec2 origin = default(vec2))
        {
            //Draws without converting world to device
            sb.Draw(Res.Tiles.Texture,
                deviceRect,
                rect_tiles
                , color * a, rotation, origin.toXNA(), SpriteEffects.None, 0.0f);
        }
        public float ScreenTileWidthMultiplier(float addl = 0)
        {
            float w = Game.GraphicsDevice.Viewport.Width;
            return (float)Math.Round((float)w / ((float)Viewport.TilesWidth - addl));//Important - or else you get a pixel glitch
        }
        public float DrawText_Fit_H(SpriteBatch sb, SpriteFont font, string text, float width_pixels, vec2 pos, 
            vec4 color, int outline = 0, vec4 outline_color = default(vec4), string proto_string = "", bool center=true)
        {
            //Returns the scale used to scale the font
            return DrawText_Fit( sb, font, text, width_pixels, Game.GraphicsDevice.Viewport.Height, pos, color, outline, outline_color, center, proto_string);
        }
        public float DrawText_Fit_V(SpriteBatch sb, SpriteFont font, string text, float height_pixels, 
            vec2 pos, vec4 color, int outline = 0, vec4 outline_color = default(vec4), string proto_string = "", bool center=true)
        {
            //Returns the scale used to scale the font
            return DrawText_Fit( sb, font, text, Game.GraphicsDevice.Viewport.Width, height_pixels, pos, color, outline, outline_color, center, proto_string);
        }

        public float DrawText_Fit(SpriteBatch sb, 
            SpriteFont font, string text, float width_pixels, float height_pixels, vec2 pos, vec4 color, int outline = 0, vec4 outline_color = default(vec4), 
            bool center_h = false, string proto_string = "")
        {
            //Proto_string = A string that we pass in to keep width/height consistent.
            float w_vp_dv = 0;

            //Scale the text to fit the given w/h.  Auto-adjust the other dimension
            try
            {
                string test_str = String.IsNullOrEmpty(proto_string) ? text : proto_string;

                Vector2 stringwh = font.MeasureString(text);

                float h1 = 0;
                float v1 = 0;
                float actualWidth_Pixels = 0;

                h1 = DrawText_Fit_HScale(font, test_str, width_pixels);
                v1 = DrawText_Fit_VScale(font, test_str, height_pixels);
                if (h1 < v1)
                {
                    w_vp_dv = h1;
                    actualWidth_Pixels = width_pixels;// height_pixels * w_vp_dv;
                }
                else
                {
                    w_vp_dv = v1;
                    actualWidth_Pixels = stringwh.X;
                }

                if (center_h)
                {
                    float ppx = Viewport.WidthPixels / Game.GraphicsDevice.Viewport.Width;
                    pos.x += actualWidth_Pixels * 0.5f - (stringwh.X * ppx) * w_vp_dv * 0.5f;
                }

                vec2 wp_device = Viewport.WorldToDevice(pos);

                DrawTextOutline(sb, font, text, outline, wp_device, w_vp_dv, outline_color);

                sb.DrawString(font, text, wp_device.toXNA(), color.toXNAColor(), 0, 
                    new Vector2(0, 0), w_vp_dv, SpriteEffects.None, 0.0f);
            }
            catch (Exception ex)
            {
                //Can't draw things like, e.g., "NaN' so this will happen when a float is infinity &c
            }
            return w_vp_dv;
        }
        public float DrawText_Fit_H_OR_V_Scale(SpriteFont font, bool h, string text, float width_or_height_pixels)
        {
            //Return the scale used for the font/fit algorithm
            if (h)
            {
                return DrawText_Fit_HScale(font, text, width_or_height_pixels);
            }
            else
            {
                return DrawText_Fit_VScale(font, text, width_or_height_pixels);
            }
        }
        public float DrawText_Fit_VScale(SpriteFont font, string text, float height_pixels)
        {
            Vector2 font_size_device = font.MeasureString(text);

            return ((height_pixels / Viewport.HeightPixels) * Game.GraphicsDevice.Viewport.Height) / font_size_device.Y;
        }
        public float DrawText_Fit_HScale(SpriteFont font, string text, float width_pixels)
        {
            Vector2 font_size_device = font.MeasureString(text);
            return ((width_pixels / Viewport.WidthPixels) * Game.GraphicsDevice.Viewport.Width) / font_size_device.X;
        }

        public vec2 GetFontSizePixels(SpriteFont font, string text, float scale)
        {
            //Returns the size of font in screen pixels given the calculated scale
            Vector2 fsd = font.MeasureString(text);

            float ratio = Viewport.WidthPixels / Game.GraphicsDevice.Viewport.Width;

            vec2 res = new vec2(fsd.X * ratio, fsd.Y * ratio);
            return res;
        }
        public void DrawTextOutline(SpriteBatch sb, SpriteFont font, string text, int outline, vec2 pos_device, float scale, vec4 outline_color)
        {
            if (outline > 0)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        sb.DrawString(font, text, pos_device.toXNA() + 
                            new Vector2(x*outline, y*outline),
                            outline_color.toXNAColor(), 0, new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
                    }
                }
 

            }
        }
        public void DrawUIFrame(SpriteBatch sb, string sprite_name, int iFrame, vec2 xy, vec2 wh, vec4 color)
        {
            Sprite sp = Res.Tiles.GetSprite(sprite_name);
            if (sp != null && sp.Frames !=null && sp.Frames.Count > iFrame)
            {
                Frame fr = sp.Frames[iFrame];

                DrawUIFrame(sb, fr, xy, wh, color);
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
        }
        public void DrawUIFrame(SpriteBatch sb, Frame fr, vec2 xy, vec2 wh, vec4 color)
        {
            sb.Draw(
                Res.Tiles.Texture,
                Viewport.WorldToDevice(Viewport.Pos + xy, wh),
                fr.R,
                color.toXNAColor(),
                0,
                default(vec2).toXNA(),
                SpriteEffects.None,
                0.0f);
        }

    }
}
