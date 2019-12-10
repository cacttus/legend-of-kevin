using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;

namespace Core
{
    public enum Platform { Android, Desktop, WindowsPhone, iOS }
    public enum TouchState { Up, Press, Down, Release }
    public enum MenuState { Closed, PickObject, PlaceObject, PickWand, GameOver }
    public enum WorldState { Play, Pause, GameOver }
    public enum ShowScreen { None, Game, Title, Tutorial }

    public class GfxRect
    {
        public Rectangle Rect;
        public Color Color;
        public GfxRect(GfxRect rc) { Rect = new Rectangle(rc.Rect.X, rc.Rect.Y, rc.Rect.Width, rc.Rect.Height); Color = new Color(rc.Color.R, rc.Color.G, rc.Color.B, rc.Color.A); }
        public GfxRect() { }
        public GfxRect(Rectangle r, Color c) { Rect = r; Color = c; }
    }
    public static class Globals
    {
        public static void IgnoreException(Exception e)
        {

        }
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public static vec2 RandomDirection()
        {
            vec2 ret = new vec2();
            ret.x = (float)Globals.Rng.NextDouble() - 0.5f;
            ret.y = (float)Globals.Rng.NextDouble() - 0.5f;
            ret.Normalize();
            return ret;
        }
        public static bool FuzzyEquals(float a, float b, float e = 0.001f)
        {
            return (((a - e) <= b) && ((a + e) >= b));
        }
        public static Random Rng { get; set; } = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static bool Pick(Rectangle r, vec2 pos)
        {
            if (pos.x < r.X) { return false; }
            if (pos.y < r.Y) { return false; }
            if (pos.x > (r.X + r.Width)) { return false; }
            if (pos.y > (r.Y + r.Height)) { return false; }
            return true;
        }
        public static float Random(float min, float max)
        {
            return min + ((float)Rng.NextDouble() * (max - min));
        }
        public static int RandomInt(int min, int max)
        {
            return min + (Rng.Next() % (max - min));
        }
        public static bool RandomBool()
        {
            return Random(0, 1) >= 0.5f;
        }
        public enum WorldState { Play, Pause, GameOver }
    }
    public class DelayedAction
    {
        public DelayedAction(Action action, float delay, bool repeat = true)
        {
            TimeRemaining = delay;
            Action = action;
            MaxDelay = delay;
            Delay = delay;
            Repeat = repeat;
        }

        public float MaxDelay { get; set; }
        public Action Action { get; private set; }
        public float Delay { get; private set; }
        public float TimeRemaining { get; set; }
        public bool Stopped = false;

        public bool Repeat = false;
        public bool Update(float deltaTime)
        {
            if (Stopped == false)
            {
                TimeRemaining -= deltaTime;

                if (TimeRemaining <= 0)
                {
                    Action();
                    if (Repeat)
                    {
                        TimeRemaining = MaxDelay;
                    }
                    else
                    {
                        Stopped = true;
                    }
                    return false;
                }
            }

            return true;
        }
    }
    public class ScreenShake
    {
        public vec2 ScreenShakeOffset = new vec2(0, 0);
        public vec2 Dir = new vec2(0, 0);
      //  vec2 Max ;//Pixels
        vec2 CurMax = 0;//Pixels
        float Speed = 0;
        float Damp = 0.89f;//The length of shake time
        Viewport Viewport;
        public ScreenShake(Viewport vp)
        {
            Viewport =vp;
        }
        public void Shake( float speed, float damping = 0.89f)
        {
            Speed = speed;
            Damp = damping;
 
            Dir = Globals.RandomDirection();

           // CurMax = Dir * speed;
        }
        public void Update(float dt)
        {
            if (Speed < 0.1f) {Speed = 0; }

            if (Speed > 0)
            {
                Speed *= Damp;

                ScreenShakeOffset = Globals.RandomDirection() * Speed;// Dir * Speed * dt;

                //if ((ScreenShakeOffset - Viewport.Pos).Len2() >= CurMax.Len2())
                //{
                //    Dir = -Dir;
                //    CurMax = Dir * Speed;
                //}

            }
            else
            {
                ScreenShakeOffset = 0;
            }
        }
    }

}
