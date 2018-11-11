using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Core
{
    public enum Tiling { Single, Vertical1x3, Horizontal3x1, Grid3x3 }
    public class Frame
    {
        public Sprite Sprite { get; private set; }
        public Frame(Sprite s)
        {
            Sprite = s;
        }
        public float TilesWidth() { return (float)R.Width / (float)Sprite.Tiles.TileWidthPixels; }
        public float TilesHeight() { return(float) R.Height / (float)Sprite.Tiles.TileHeightPixels; }

        public int Index { get; set; } = 0;
        public Rectangle R { get; set; }
        public float Delay { get; set; } = 0;
        public vec2 PixelCenter()
        {
            vec2 v = new vec2(
                0 + (float)R.Width * 0.5f,
                0 + (float)R.Height * 0.5f
                );
            return v;
        }
    }
    public class Sprite
    {
        public Frame GetFrame(int i) { if (i < Frames.Count) return Frames[i]; return null; }
        public Tiling Tiling = Tiling.Single;
        public List<int> SeamlessIds { get; set; } = new List<int>();
        public Tiles Tiles { get; private set; }
        public string Name { get; set; } = "";
        public List<Frame> Frames { get; set; } = new List<Frame>();
        public Sprite(string name, Tiles tiles) { Name = name; Tiles = tiles; }
        public float DurationSeconds = 0;//Total duration of all frames combined
    }
    public class Tiles
    {
        public int TileWidthPixels { get; private set; } = 16;
        public int TileHeightPixels { get; private set; } = 16;
        int _iSpacing = 1;
        int _iOffset = 1;
        public vec2 GetWHVec() { return new vec2(TileWidthPixels, TileHeightPixels);  }
        public Texture2D Texture { get; set; } = null;

        public Dictionary<string, int> TileIds;
        public Dictionary<int, Sprite> IdToSprite;
        public Tiles()
        {
        }
        public int GetTileId(string name)
        {
            if (TileIds == null)
            {
                //Uh Oh - this must have a value
                System.Diagnostics.Debugger.Break();
            }
            int val = 0;
            if(TileIds.TryGetValue(name, out val)==false)
            {
                System.Diagnostics.Debugger.Break();
            }
            return val;
        }

        public Dictionary<string,Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();
        public Sprite GetSprite(string n)
        {
            Sprite s = null;
            Sprites.TryGetValue(n, out s);
            return s;
        }

        public int AtlasTilesWidth()
        {
            //Number of Horizontal tiles in the atlas texture
            int w = Texture.Width / (TileWidthPixels + _iSpacing + _iOffset);
            return w;
        }
        public int AtlasTilesHeight()
        {
            int h = Texture.Height / (TileHeightPixels + _iSpacing + _iOffset);
            return h;
        }
        public Rectangle FrameRect(Rectangle tiles)
        {
            return FrameRect(tiles.X, tiles.Y, tiles.Width, tiles.Height);
        }
        public Rectangle FrameRect(int x, int y, int w, int h)
        {
            return new Rectangle(
                      _iOffset + (int)x * (TileWidthPixels + _iSpacing)
                    , _iOffset + (int)y * (TileHeightPixels + _iSpacing)
                    , (w * TileWidthPixels) + ((_iSpacing) * (w - 1))
                    , (h * TileHeightPixels) + ((_iSpacing) * (h - 1))
                    );
        }
        
        public Sprite AddSprite(string name, List<Rectangle> frames, float duration_seconds, Tiling tiling = Tiling.Single, int TileId = -1)
        {
            Sprite s = new Sprite(name,this);
            s.DurationSeconds = duration_seconds;
            int i = 0;
            foreach (Rectangle iframe in frames)
            {
                Frame f = new Frame(s);
                f.R = FrameRect((int)iframe.X, (int)iframe.Y, iframe.Width, iframe.Height);
                f.Delay = duration_seconds / (float)frames.Count;
                f.Index = i++;
                s.Frames.Add(f);
            }

            s.Tiling = tiling;

            Sprites.Add(name, s);



            return s;
        }
        public Frame GetSpriteFrame(string n, int idx)
        {
            Sprite sp = GetSprite(n);
            if (sp != null)
            {
                if (sp.Frames.Count > idx)
                {
                    return sp.Frames[idx];
                }
            }
            return null;
        }
    }

}
