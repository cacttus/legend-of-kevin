using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Core
{
    public enum ParticleLife { Alpha_Zero, Scale_Zero,
        AnimationComplete, Seconds }
    public class Particle : GameObject
    {
        public Particle(WorldBase w) : base(w) { }
        // public bool DestroyAfterAnimationComplete = false;
        public ParticleLife ParticleLife;
        public float LifeSeconds = 5;
    }
    public abstract class WorldBase
    {
        public vec2 Gravity = new vec2(0, 400.0f);

        public Screen Screen { get; private set; }
        public List<GameObject> Particles { get; private set; } = new List<GameObject>();

        public float Time { get; private set; } = 0;//**Elapsed Game time in secobnds
        public int iFrameStamp { get; private set; } = 0;

        public WorldBase(Screen screen)
        {
            Screen = screen;
        }
        public virtual void Update(float dt, bool bFocusViewport = true)
        {
            iFrameStamp++;
            Time += dt;

            for (int i=Particles.Count-1; i>=0; i--)
            {
                Particle p = Particles[i] as Particle;
                p.Update(Screen.Game.Input, dt, true);

                p.LifeSeconds -= dt;

                if(p.LifeSeconds <= 0)
                {
                    Particles.RemoveAt(i);
                }
                else if (Screen.Viewport.ObjectIsOutsideViewport(p))
                {
                    Particles.RemoveAt(i);
                }
                else if (p.ParticleLife == ParticleLife.AnimationComplete )
                {
                    if (p.IsAnimationComplete())
                    {
                        Particles.RemoveAt(i);
                    }
                }
                else if (p.ParticleLife == ParticleLife.Alpha_Zero)
                {
                    if (p.Alpha <= 0.0001f)
                    {
                        Particles.RemoveAt(i);
                    }
                }
                else if (p.ParticleLife == ParticleLife.Scale_Zero)
                {
                    if (p.Scale.x <= 0.0001f || p.Scale.y < 0.0001f)
                    {
                        Particles.RemoveAt(i);
                    }
                }
            }
        }
        public virtual void Draw(SpriteBatch sb)
        {
            DrawObs<GameObject>(sb, Particles);
        }
        public void DrawObs<T>(SpriteBatch sb, List<T> obs)
        {
            foreach (T ob1 in obs)
            {
                GameObject ob = ob1 as GameObject;
                if (ob != null && ob.Visible == true)
                {
                    if (ob.Frame == null)
                    {
                        //Not sur why we're getting this
                        //due to the explodable blocks, or blocks that have exploded
                       // System.Diagnostics.Debugger.Break();
                    }
                    else
                    {
                        Screen.DrawFrame(sb, ob.Frame, ob.Pos,
                            new vec2(ob.Frame.R.Width, ob.Frame.R.Height),
                            (ob.Color * ob.Alpha).toXNAColor(), ob.Scale, ob.Rotation, ob.Origin, ob.SpriteEffects);
                    }

                }
            }
        }
        public void CreateParticles(string spritename, ParticleLife life,  int count, vec2 pos, float minspeed, float maxspeed,
            vec4 color, float fade = 0.0f, float size = 0.0f, float rotateMin = 0.0f, float rotateMax = 0.0f,
            vec2 origin = default(vec2), vec2 gravity = default(vec2), bool randomvelocity = true, vec2 velocity = default(vec2),
            float sizeStart = 1, bool bUseAperature = false, vec2 ap_dir = default(vec2), float ap_angle = 0.0f, float ap_speed_min = 0.0f, float ap_speed_max = 0.0f,
            bool emit = false, vec4 emitColor = default(vec4), float emitRadiusPIxels = 16, bool use_physical_rotation = false, bool bAnimate = false)
        {

            for (int i = 0; i < count; ++i)
            {
                Particle p = new Particle(this);
                p.SetSprite(spritename, life != ParticleLife.AnimationComplete);

                p.Fade = fade;
                p.Animate = bAnimate;
                p.ParticleLife = life;

                p.ScaleDelta = size;
                p.Scale = sizeStart;
                p.Pos = new vec2(pos.x, pos.y);
                p.Color = color;
                p.Origin = origin;
                p.Acc = gravity;

                if (emit)
                {
                    p.EmitLight = true;
                    p.EmitColor = emitColor;
                    p.EmitRadiusInPixels = emitRadiusPIxels;
                }

                if (bUseAperature)
                {
                    float ang = Globals.Random(-ap_angle / 2, ap_angle / 2);
                    //https://matthew-brett.github.io/teaching/rotation_2d.html
                    //x2=cosβx1−sinβy1
                    //y2 =sinβx1+cosβy1

                    float cosa = (float)Math.Cos(ang);
                    float sina = (float)Math.Sin(ang);

                    p.Vel = new vec2(
                        cosa * ap_dir.x - sina * ap_dir.y,
                        sina * ap_dir.x + cosa * ap_dir.y);
                    p.Vel *= Globals.Random(ap_speed_min, ap_speed_max);
                }
                else if (randomvelocity)
                {
                    p.Vel = Globals.RandomDirection() * Globals.Random(minspeed, maxspeed);
                }
                else
                {
                    p.Vel = velocity;
                }


                if (use_physical_rotation)
                {
                    p.CalcRotationDelta();
                }
                else
                {
                    p.RotationDelta = Globals.Random(rotateMin, rotateMax);

                }

                   Particles.Add(p);
            }

        }

    }
}
