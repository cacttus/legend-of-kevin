using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class CollisionFlags
    {
        public static int CollideTop = 0x01;
        public static int CollideBot = 0x02;
        public static int CollidePixel = 0x04;
    }
    public enum AIState { None, Player, Wander, Idle, SwimLeftRight, MoveLRConstant, Sleep, Defend, Attack, Talk }
    public enum PhysicsShape { None, Ball }
    public enum PhysicsResponse { Slide_And_Roll, Bounce_And_Roll, StayPut, StickIntoGround }
    public enum AIPhysics { Character, Grapple, PlantBombGuy }
    public enum DeflectType { Physical, ExactReverse } // type of direction to deflect oject when hit with shield

    public class GameObject : Touchable
    {
        public float CreateTime = 0;
        public vec2 LastCollideNormal = new vec2(0, 0);

        public List<Rectangle> HitBoxes = new List<Rectangle>();
        public bool IsFacingRight() { return SpriteEffects == SpriteEffects.None; }
        public bool IsFacingLeft() { return SpriteEffects == SpriteEffects.FlipHorizontally; }

        public bool IsDeleted = false;

        public float ShakeTime = 0;
        public float ShakeAmountPixels = 0;
        public vec2 ShakeOffset = new vec2(0, 0);

        public bool IsElectronic = false;
        public bool CollidedWithSomething = false;

        public ivec2 SavePos = new ivec2(0, 0);//The position in TMX map we created this, so we can save it
        public int SaveLayer = -1;
        public Cell CreatedCell = null;//Cell on which this was created.
        public int CreatedLayer = -1;

        public virtual void UpdateSprite()
        {
            //Update sprite 
        }
        public virtual void CreateNew()
        {
            //set the objects initial state, before saving
        }
        public virtual void Serialize(BinaryWriter w)
        {
            //save object
        }
        public virtual void Deserialize(BinaryReader w)
        {
            //load object
        }

        public bool IsAnimationComplete()
        {
            if (Animate == true)
            {
                if (Loop == false)
                {
                    if (Sprite != null)
                    {
                        if (Frame != null)
                        {
                            if (Sprite.Frames != null)
                            {
                                if (Sprite.Frames.Count > 0)
                                {
                                    if (Sprite.Frames[Sprite.Frames.Count - 1] == Frame)
                                    {
                                        if (_fNextFrame <= 0)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return false;
        }

        public bool CanAttack { get; set; } = true;
        public int TileId { get; set; } = -1;   //The TIle ID from the TILED map that we use to identify this object

        public float Friction = 0.8f;
        //  public bool IsPortal = false;//When collided with we change level via TilId;
        public vec2 Gravity = new vec2(0, 0);

        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public void CalcRotationDelta()
        {
            //simple rotation delta func
            RotationDelta = (Vel.x / MaxAngularVelocity);
        }
        public float Bounciness = 1.0f; // How much we bounce the velocity.
        public float MaxAngularVelocity = 3.141593f * 2;
        public PhysicsResponse PhysicsResponse = PhysicsResponse.Bounce_And_Roll;
        public PhysicsShape PhysicsShape = PhysicsShape.None;
        public float PhysicsBallRadiusPixels = 1.0f;//Radius of physics ball
        public bool CanDeflectWithShield = false;
        public DeflectType DeflectType = DeflectType.Physical;
        public bool DeflectedWithShield = false;


        public Cell CellFrame { get; set; } = null;//updated manually

        public float Speed { get; set; } = 1.5f;
        public float Power { get; set; } = 0;
        public float Health { get; set; } = 5;
        public float MaxHealth { get; set; } = 5;
        public int Value { get; set; } = 1;//Price
        public int CollisionFlags { get; set; } = 0;// CollisionFlags.CollideTop;

        //how many 'vapors' to draw behind the guy.
        public List<vec2> LastPos = new List<vec2>();
        public List<float> LastRot = new List<float>();
        public List<vec2> LastScale = new List<vec2>();
        public int VaporTrail = 0;

        public Box2f Box;
        public Box2f BoxRelative = new Box2f(0, 0, 16, 16);//Hack

        public float AnimationSpeed { get; set; } = 1.0f; // Multiplier for animation speed.


        public vec2 Vel;
        public vec2 Acc;
        public Sprite Sprite { get; set; }
        public Frame Frame { get; set; }
        private vec2 _pos = new vec2(0, 0);

        //These are needed because you can't set a value type in a proepty
        public void setPos(vec2 p) { _pos = p; }
        public void setPosX(float v)
        {
            _pos.x = v;
        }
        public void setPosY(float v)
        {
            _pos.y = v;
        }

        public vec2 Pos
        {
            get
            {
                return this._pos;
            }
            set
            {
                _pos = value;
            }
        }
        public vec2 Size = new vec2(1, 1);
        public float frame = 0;
        public bool Animate
        {
            get;
            set;
        } = false;
        public bool Loop { get; set; } = true;//loop aniamtino
        public bool Blocking = false;

        public bool RotateToTrajectory = false;//for arrows

        public vec2 ScaleDelta = 0.0f;
        public vec2 Scale = 1.0f;
        public bool ScalePingpongX = false;
        public bool ScalePingpongY = false;
        public vec2 ScalePingpongXRange = new vec2(0, 1);
        public vec2 ScalePingpongYRange = new vec2(0, 1);

        public float Fade { get; set; } = 0.0f;//Fade animation
        public float Alpha { get; set; } = 1.0f;//Addition of fade
        public vec4 Color { get; set; } = new vec4(1, 1, 1, 1);
        public float RotationDelta { get; set; } = 0.0f;
        public float Rotation { get; set; } = 0.0f;
        public vec2 Origin { get; set; } = new vec2(0, 0);
        public Rectangle Bounds = new Rectangle(0, 0, 0, 0);

        public GameObject Parent = null;// Apply a relative transform to this gameobject

        public bool BlocksLight = true;
        public bool EmitLight = false;
        public vec4 EmitColor = new vec4(1, 1, 1, 1);
        public float EmitRadiusInPixels = 16.0f * 3;
        public Box2f GetEmitterBox()
        {
            Box2f ret = new Box2f(new vec2(
                this.Pos - this.EmitRadiusInPixels
                ), new vec2(
                    this.Pos + this.EmitRadiusInPixels
                    ));
            return ret;
        }

        public virtual void CalcBoundBox()
        {
            if (PhysicsShape == PhysicsShape.Ball)
            {
                Box = new Box2f(WorldPos() - PhysicsBallRadiusPixels, WorldPos() + PhysicsBallRadiusPixels);
            }
            else
            {
                Box = new Box2f(WorldPos() + BoxRelative.Min, WorldPos() + BoxRelative.Max);
            }

            ApplyRotateBoundBox();
        }
        protected void ApplyRotateBoundBox()
        {
            if (Rotation != 0)
            {
                vec2 wp = WorldPos();
                //Rotate the box points and apply a new bound box to the rotated points
                //Generate the box based on the rotated box
                vec2 p0 = Box.TopLeft() - wp;
                vec2 p1 = Box.TopRight() - wp;
                vec2 p2 = Box.BotLeft() - wp;
                vec2 p3 = Box.BotRight() - wp;

                mat2 m = mat2.GetRot(Rotation);
                //rotate box along with object
                p0 = m * p0;
                p1 = m * p1;
                p2 = m * p2;
                p3 = m * p3;

                Box.GenResetExtents();
                Box.ExpandByPoint(p0 + wp);
                Box.ExpandByPoint(p1 + wp);
                Box.ExpandByPoint(p2 + wp);
                Box.ExpandByPoint(p3 + wp);
            }


        }
        public WorldBase World { get; private set; }

        private float _fNextFrame = 0;

        public bool Touch()
        {
            Box2f gridBox = new Box2f(
                 World.Screen.Viewport.WorldToDevice(Box.Min),
                 World.Screen.Viewport.WorldToDevice(Box.Max));

            if (gridBox.ContainsPointInclusive(World.Screen.Game.Input.LastTouch))
            {
                return true;
            }
            return false;
        }

        public GameObject(WorldBase w)
        {
            World = w;
            CreateTime = w.Time;
        }
        public GameObject(WorldBase w, string SpriteName)
        {
            World = w;
            CreateTime = w.Time;
            SetSprite(SpriteName);
        }

        public GameObject(WorldBase w, string SpriteName, vec2 xy)
        {
            World = w;
            CreateTime = w.Time;
            SetSprite(SpriteName);
            Pos = xy;
        }
        public GameObject(WorldBase w, Sprite sprite, vec2 xy)
        {
            World = w;
            CreateTime = w.Time;
            SetSprite(sprite);

            Pos = xy;
        }

        public void SetSpriteIfNot(string name, bool loop = true)
        {
            if (Sprite == null || Sprite.Name != name)
            {
                SetSprite(name, loop);
            }
        }
        //public void SetSprite(string name, bool loop = true)
        //{
        //    Sprite spr = World.Screen.Game.Res.Tiles.GetSprite(name);
        //    SetSprite(spr);
        //}
        public void SetSprite(string name, bool loop = true, int iFrame = -1)
        {
            if (string.IsNullOrEmpty(name))
            {
                //Happens if we set guysprite on a duck
                return;
            }
            Sprite spr = Res.Tiles.GetSprite(name);
            SetSprite(spr, loop);
            if (iFrame >= 0)
            {
                SetFrame(iFrame);
            }
        }
        public void SetSprite(Sprite sprite, bool loop = true)
        {
            Sprite = sprite;
            if (Sprite != null && Sprite.Frames.Count > 0)
            {
                Frame = Sprite.Frames[0];
                _fNextFrame = Frame.Delay;
            }
            else
            {
                _fNextFrame = 0;
            }
            Loop = loop;
        }
        public void SetFrame(int index)
        {
            if (Sprite == null) { return; }
            if (Sprite.Frames.Count <= index) { return; }
            Frame = Sprite.Frames[index];
        }
        public bool CollidesWidth_Inclusive(vec2 point)
        {
            //Grid REct
            float l = Pos.x; ;// * World.Tiles.SpriteWidth;
            float t = Pos.y; ;// * World.Tiles.SpriteWidth;
            float r = l + 1;
            float b = t + 1;
            if (point.x >= l && point.x <= r && point.y >= t && point.y <= b)
            {
                return true;
            }
            return false;
        }
        public bool CollidesWith(GameObject g)
        {
            //Grid REct
            float l = Pos.x; ;// * World.Tiles.SpriteWidth;
            float t = Pos.y; ;// * World.Tiles.SpriteWidth;
            float r = l + 1;
            float b = t + 1;

            float gl = g.Pos.x;//* World.Tiles.SpriteWidth;
            float gt = g.Pos.y;// World.Tiles.SpriteWidth;
            float gr = gl + 1;
            float gb = gt + 1;

            if (gr < l) { return false; }
            else if (gb < t) { return false; }
            else if (gl > r) { return false; }
            else if (gt > b) { return false; }
            else
            {
                return true;
            }
        }
        public bool OutsideWindow()
        {
            float w = (float)World.Screen.Game.GraphicsDevice.Viewport.Width;
            float h = (float)World.Screen.Game.GraphicsDevice.Viewport.Height;

            float tile_w = w / World.Screen.Viewport.TilesWidth;
            //R2 Rect
            float l = Pos.x * (float)tile_w;
            float t = Pos.y * (float)tile_w;
            float r = l + (float)tile_w;
            float b = t + (float)tile_w;

            if (r < 0) { return true; }
            if (b < 0) { return true; }
            if (l > w) { return true; }
            if (t > h) { return true; }

            return false;
        }

        public override Rectangle GetDest()
        {
            return World.Screen.Viewport.WorldToDevice(Pos, Size);
        }
        public vec2 WorldPos()
        {
            if (Parent != null)
            {
                return Pos + Parent.Pos + Origin;
            }
            else
            {
                return Pos + Origin;
            }
        }
        public virtual void Update(Input inp, float dt, bool bDoPhysics = false)
        {
            base.Update(inp);

            //Vapor Trail, if any
            if (VaporTrail > 0)
            {
                LastPos.Add(WorldPos());
                LastRot.Add(Rotation);
                LastScale.Add(Scale);
                if (LastPos.Count > VaporTrail)
                {
                    LastPos.RemoveAt(0);
                    LastRot.RemoveAt(0);
                    LastScale.RemoveAt(0);
                }
            }

            if (ShakeTime > 0)
            {
                ShakeTime -= dt;
                if (ShakeTime < 0) { ShakeTime = 0; ShakeOffset.x = ShakeOffset.y = 0; }
                else
                {
                    ShakeOffset.x = Globals.Random(-ShakeAmountPixels, ShakeAmountPixels);
                    ShakeOffset.y = Globals.Random(-ShakeAmountPixels, ShakeAmountPixels);
                }
            }

            //Animation
            if (Animate == true && Frame == null && Sprite != null)
            {
                Frame = Sprite.Frames[0];
            }
            if (bDoPhysics)
            {
                //Don't make automatic physics.
                Vel += Acc * dt;
                Vel += Gravity;
                Pos += Vel /*Dir * Speed*/ * dt;

            }

            //Bounds.X = (int)Pos.X;
            //Bounds.Y = (int)Pos.Y;


            Alpha -= Fade * dt;
            if (Alpha < 0)
            {
                Alpha = 0;
            }



            if (ScalePingpongX)
            {
                if (Scale.x > ScalePingpongXRange.y)
                {
                    Scale.x = ScalePingpongXRange.y;
                    ScaleDelta.x = -ScaleDelta.x;
                }
                if (Scale.x < ScalePingpongXRange.x)
                {
                    Scale.x = ScalePingpongXRange.x;
                    ScaleDelta.x = -ScaleDelta.x;
                }
            }
            if (ScalePingpongY)
            {
                if (Scale.y > ScalePingpongYRange.y)
                {
                    Scale.y = ScalePingpongYRange.y;
                    ScaleDelta.y = -ScaleDelta.y;
                }
                if (Scale.y < ScalePingpongYRange.x)
                {
                    Scale.y = ScalePingpongYRange.x;
                    ScaleDelta.y = -ScaleDelta.y;
                }
            }
            Scale.x -= ScaleDelta.x * dt;
            Scale.y -= ScaleDelta.y * dt;
            if (Scale.x < 0) { Scale.x = 0; }
            if (Scale.y < 0) { Scale.y = 0; }

            if (RotateToTrajectory == true)
            {
                vec2 d = Pos + Vel;
                Rotation = MathUtils.GetRotationFromLine(d.x, d.y, Pos.x, Pos.y) - 3.141593f * 0.5f;
            }

            Rotation += RotationDelta * dt;
            Rotation = Rotation % (float)(Math.PI * 2.0f);

            if (Animate == true)
            {

                _fNextFrame -= dt * AnimationSpeed;

                int i = 0;
                while (_fNextFrame <= 0 && i < 500)
                {
                    if (Sprite != null && Frame != null)
                    {
                        if (Sprite.Frames.Count > 0)
                        {
                            if ((Frame.Index == Sprite.Frames.Count - 1) && Loop == false)
                            {
                                //, No loop, Don't change frame
                            }
                            else if (Frame == null)
                            {
                                Frame = Sprite.Frames[0];
                                _fNextFrame = 0;
                            }
                            else
                            {
                                int frame = (Frame.Index + 1) % Sprite.Frames.Count;
                                Frame = Sprite.Frames[frame];
                                _fNextFrame += Frame.Delay;
                            }

                        }

                    }
                    i++;
                }
            }


        }
        //public void Draw(SpriteBatch sb)
        //{

        //}
    }
}
