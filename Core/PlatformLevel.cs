using System;
using System.Collections.Generic;
using TiledSharp;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Core
{
    public enum BlockType { None, Stone, Wood, Dirt, Hedge, MonsterGrass, Vines, StoneDecal }
    //  public enum PortalMode { None, InAndOut, OutOnly, InOnly }
    public enum DoorTransitionEffect
    {
        Blend, //Blend a and b
        WalkIntoWall, //player walks into the wall by pressing up
        MoveCamera, //When colliding we hide player then move the location of the exit portal 
        //WalkIntoWall_MoveCamera,
        Fade // Fade a to b the level 
    }

    public class Spike : GameObject
    {
        //Dir Dir = Dir.L;
        Cutscene cs;
        public Spike(WorldBase w) : base(w) { }
        public override void Update(Input inp, float dt, bool bDoPhysics = false)
        {
            base.Update(inp, dt, bDoPhysics);

            if (cs == null || cs.Ended())
            {
                cs = new Cutscene()
                .Then(3.0f, (s, delta) =>
                {
                    //idle
                    return s.Duration > 0;
                }, null)
                .Then(0.1f, (s, delta) =>
                {
                    //show/ preview
                    return s.Duration > 0;
                }, null)
                .Then(2.0f, (s, delta) =>
                {
                    //preview wait
                    return s.Duration > 0;
                }, null)
                .Then(0.1f, (s, delta) =>
                {
                    //drive
                    return s.Duration > 0;
                }, null)
                .Then(0.1f, (s, delta) =>
                {
                    //drive wait
                    return s.Duration > 0;
                }, null)
                .Then(0.1f, (s, delta) =>
                {
                    //go back
                    return s.Duration > 0;
                }, null)
                 ;
            }

            cs.Update(dt);
        }
        public float IdleTime = 0;
        public float MaxIdleTime = 3.0f;
        public float ShowTime = 0;
        public float MaxShowTime = 0.1f;
        public float ShowWaitTime = 0;
        public float MaxShowWaitTime = 1.1f;
        public float DriveTime = 0;
        public float MaxDriveTime = 0.2f;
        public float HoldTime = 0;
        public float MaxHoldTime = 3.0f;

    }
    public enum SpecialItemChestType
    {
        Normal_Small,
        Silver_Small,
        Normal_Big,
        Gold_Big,
        Plinth_Sword,
        Plinth_Shield,
        Plinth_Bow
    }//kind of chest were in
    public enum Dir { L, R, B, T }
    public enum LockType { None, Key, Electronic }
    public class Door : GameObject
    {
        public bool IsEntering(Guy guy)
        {
            return (guy.Vel.x > 0 && Dir == Dir.L) ||//L
                    (guy.Vel.x < 0 && Dir == Dir.R) || //R
                    (guy.Vel.y > 0 && Dir == Dir.T) || // B
                    (guy.Vel.y < 0 && Dir == Dir.B); //T
        }
        public Door(WorldBase w) : base(w) { }
        public bool AlwaysOpen = false; //for invisible dors
        public string OpenSprite = "";
        public string CloseSprite = "";
        public bool Open = false;
        public LockType LockType = LockType.None;
        public Dir Dir = 0; //0=L, 1=R, 2=T, 3=B
        public DoorTransitionEffect PortalTransitionEffect = DoorTransitionEffect.Blend;

        public bool Locked = false;
        public bool Entered = false;    // If hte door was entered by the player *used for generating map*

        public bool IsPortalDoor = true; // iF the door is on a room boundary then it's a portal.
        public ivec2 OrigTilePos;

        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);
            w.Write(Locked);
            w.Write(AlwaysOpen);
            w.Write(IsElectronic);
            w.Write(Entered);
        }
        public override void Deserialize(BinaryReader w)
        {
            base.Deserialize(w);
            Locked = w.ReadBoolean();
            AlwaysOpen = w.ReadBoolean();
            IsElectronic = w.ReadBoolean();
            Entered = w.ReadBoolean();

        }
        public override void CreateNew()
        {
            if (Dir == Dir.L || Dir == Dir.R)
            {
                if (TileId == Res.Door_Nolock_TileId)
                {
                    //Open, Close, Lock, Gold Lock
                }
                else if (TileId == Res.Door_Lock_TileId)
                {
                    Locked = true;
                    LockType = LockType.Key;
                }
                else if (TileId == Res.Doorless_Portal_TileId)
                {
                    Open = true;
                    AlwaysOpen = true;
                }
                else if (TileId == Res.SwitchDoorTileId)
                {
                    Open = false;
                    AlwaysOpen = false;
                    Locked = true;
                    LockType = LockType.Electronic;
                    IsElectronic = true;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            else if (Dir == Dir.T || Dir == Dir.B)
            {
                if (TileId == Res.Door_Nolock_TileId)
                {
                }
                else if (TileId == Res.Door_Lock_TileId)
                {
                    Locked = true;
                }
                else if (TileId == Res.Doorless_Portal_TileId)
                {
                    Open = true;
                    AlwaysOpen = true;
                }
                else if (TileId == Res.SwitchDoorTileId)
                {
                    Open = false;
                    AlwaysOpen = false;
                    Locked = true;
                    LockType = LockType.Electronic;
                    IsElectronic = true;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }

            }
        }
        public override void UpdateSprite()
        {
            Door d = this;
            int door_wh = 6;

            int iOpen = 0, iClose = 1, iLock = 2,
                iElectronicLock = 4,
                iMetalClose = 5, iMetalOpen = 6;

            int iFrame = 0;
            string strSprite = "";

            //Set sprite and bound box
            if (Dir == Dir.L || Dir == Dir.R)
            {
                strSprite = Res.SprDoor_LR;
                d.BoxRelative = new Box2f(0, 0, door_wh, Res.Tiles.TileHeightPixels);
            }
            else
            {
                strSprite = Res.SprDoor_TB;
                d.BoxRelative = new Box2f(0, 0, Res.Tiles.TileWidthPixels, door_wh);
            }

            //Set Frame
            if (Locked == false)
            {
                if (AlwaysOpen == false)
                {
                    if (Open)
                    {
                        if (IsElectronic)
                        {
                            iFrame = iMetalOpen;
                        }
                        else
                        {
                            iFrame = iOpen;
                        }
                    }
                    else
                    {
                        if (IsElectronic)
                        {
                            iFrame = iMetalClose;
                        }
                        else
                        {
                            iFrame = iClose;
                        }
                    }
                }
                else
                {
                    iFrame = -1;
                }
            }
            else
            {
                if (IsElectronic)
                {
                    iFrame = iElectronicLock;
                }
                else
                {
                    iFrame = iLock;
                }
            }


            if (iFrame >= 0)
            {
                d.SetSprite(strSprite, false, iFrame);
            }


            //Flip box or sprite.
            if (Dir == Dir.L)//L
            {
                d.SpriteEffects = SpriteEffects.FlipHorizontally;
                d.BoxRelative = Box2f.FlipBoxH(d.BoxRelative, Res.Tiles.TileWidthPixels);
            }
            else if (Dir == Dir.R)//R
            {
            }
            else if (Dir == Dir.T)//B
            {
                d.SpriteEffects = SpriteEffects.FlipVertically;
                d.BoxRelative = Box2f.FlipBoxV(d.BoxRelative, Res.Tiles.TileHeightPixels);
            }
            else if (Dir == Dir.B)//T
            {
            }
        }
    }
    public class Arrow : GameObject
    {
        public Arrow(WorldBase w) : base(w) { }
        public float StickTime = 0;
        public static float MaxStickTime = 10.0f;
    }
    public class Bomb : GameObject
    {
        public float BlastRadiusPixels = 16 * 2.5f;
        public static float BaseScale = 0.7f;
        public float MaxBlowtime = 5.0f;
        public static float BlinkRate = 0.1f;
        public float Blowtime = 5;
        public bool Blink = false;
        public float NextBlink = 5 * Bomb.BlinkRate;

        public bool Exploded = false;

        public Bomb(WorldBase w) : base(w) { }
        public string ExplodeSound = "";

        public bool IsEnemyBomb = false; // if an enemy bomb, blow up the player instead of the enemy
        public bool ExplodeOnImpact = false;
    }
    public enum PickupItemType { Bomb, Potion, Marble, Arrow }
    public class PickupItem : GameObject
    {
        public float DieTime = 100000.0f;//Remove after 10 seconds
        public float PickupTime = 0.0f;// Amount of time until the player can pick the tiem up

        public PickupItemType PickupItemType { get; private set; } = PickupItemType.Bomb;
        public PickupItem(WorldBase w, PickupItemType type) : base(w)
        {
            //this.PhysicsShape = PhysicsShape.Rectangle;//There's no point.
            this.PhysicsResponse = PhysicsResponse.StayPut;
            //TODO
            PickupItemType = type;
        }
    }
    public class TreasureChest : GameObject
    {
        public SpecialItem SpecialItem = null;//Optional - if we have a special item then show a cutscene
        public float SparkleTime = 0;
        public float MaxSparkleTime = 0.3f;
        public bool Open = false;
        public int Money = 0;

        public TreasureChest(WorldBase w, string sprite, vec2 pos) : base(w, sprite, pos)
        {
        }
        public override void CreateNew()
        {

        }
        public override void UpdateSprite()
        {
            if (Open)
            {
                SetFrame(1);
            }
            else
            {
                SetFrame(0);
            }
        }
        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);
            w.Write(Open);
        }
        public override void Deserialize(BinaryReader w)
        {
            base.Deserialize(w);
            Open = w.ReadBoolean();

            UpdateSprite();
        }
    }
    public class Guy : GameObject
    {
        public bool OnSlope = false;

        public bool IsNpc = false;
        public List<List<string>> NpcDialog = new List<List<string>>();
        public string NpcName = "";


        public bool GrappleInitialScan = false;
        public vec2 GrappleNormal = new vec2(0, -1);
        public Cell GrappleCell = null;
        public int GrappleSide = -1; //0=L,R,T,B,  4= BRTL, TRBL, TLBR, BLTR
        public float Grapple = 0.0f;
        public int GrappleDir = 0;//0=left 1=right

        public GameObject ItemHeld = null;
        public GameObject LastItemThrown = null;//This is set when we call ThrowItem

        public AIPhysics AIPhysics = AIPhysics.Character;

        public float HurtTime = 0;
        public float HurtTimeMax = 0.1f;//this is the invincibility time.

        public float WaterDampGrav = 0.2f;//Modify these to change grav/acc when in water.
        public float WaterDamp = 0.5f;
        public float NormalDampGrav = 1;//Modify these to change grav/acc when in water.
        public float NormalDamp = 1;

        public bool InWater = false;
        public float SwimStrokeTime = 0;
        public float SwimStrokeTimeMax = 0.33f;//3 jumps per second
        public WaterType InWaterType = WaterType.Water;

        public bool CanJump = true;


        public float Knockback = 0.0f;

        public bool IsPlayer() { return this.AIState == AIState.Player; }
        public List<string> HitSounds = new List<string>();
        public List<string> DieSounds = new List<string>();
        public List<string> GrowlSounds = new List<string>();

        public float MaxAcc = 999;

        public Joystick Joystick = null;    //This controls the guy both for AI and for the Player

        //For Monsters Only
        public float WakeRadiusPixels = 16 * 3;
        public bool CanDefend = false;
        public float DefendRadiusPixels = 16 * 1.5f;
        public float AttackTime = 0;
        public float MaxAttackTime = 1.0f;//


        public vec2 cposR;
        public vec2 cposB;
        public vec2 cposT;
        public vec2 cposL;
        public vec2 cposC;
        public vec2 cposrelB = new vec2(8, 16);
        public vec2 cposrelT = new vec2(8, 5); //Make this smaller so we can get into crevices easier
        public vec2 cposrelL = new vec2(2, 14);
        public vec2 cposrelR = new vec2(14, 14);
        public vec2 cposrelC = new vec2(8, 8);
        public string IdleSprite;
        public string WalkSprite;
        public string CrouchSprite = "";
        public string ItemHeldWalkSprite;
        public string ClimbSprite;
        public string HangSprite;
        public string JumpSprite;
        public string MountSprite;
        public string SpringJumpSprite = "";
        public string WalkAttackSprite = "";
        public string CrouchAttackSprite = "";
        public string FallSprite = "";
        public string SleepSprite = "";
        public string DefendSprite = "";
        public string TalkSprite = "";

        public Box2f BoxRelativeCrouch = new Box2f(0, 0, 16, 16);//Hack

        public new float Friction = 18.0f;
        public float MaxVel = 170.0f;
        public bool OnGround = true;
        public bool LastOnGround = true;
        public float TimeOnGround = 0;

        public bool Jumping = false;//this is just for showing the animation
                                    // public bool Hanging = false;
        public bool Climbing = false;
        public bool Crouching = false;
        //public bool Bouncing = false;
        //public GameObject BouncedObject = null;
        public SpriteEffects ClimbFace = SpriteEffects.None;//This tells us left/right
                                                            // public vec2 ClimbPos = new vec2(0, 0);
                                                            // public vec2 ClimbPosStart = new vec2(0, 0);
        public bool Waving = false;
        public int ClimbType = 0;//0=mount, 1=climb(up)
        public float ClimbSpeed = 2000.0f;
        // public float ClimbLen = 0.0f;
        //  public float ClimbLenMax = 0.0f;
        public float Airtime = 0.0f;
        public float MaxAirtime = 0.4f;
        public float JumpSpeed = 780.0f;
        public float SpringJumpSpeed = 1280.0f;//Jump speed with spring boots
        public float CurJumpSpeed = 0;

        public void StartJump(float dt)
        {
            ContinueJump(dt);
            Airtime = 0;
        }
        public void ContinueJump(float dt)
        {
            if (Jumping)
            {
                Airtime += dt;
                if (Airtime < this.MaxAirtime)
                {
                    //Simply add jump ve.
                    Vel += new vec2(0, -CurJumpSpeed) * dt;
                }
                else
                {
                    Jumping = false;
                }
            }
        }

        public AIState AIState = AIState.None;
        public float AIActionTime = 0; //amount of time to perform this action
        public void FaceObject(GameObject ob)
        {
            float d = Box.Center().x - ob.Box.Center().x;
            if (d > 0)
            {
                SpriteEffects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                SpriteEffects = SpriteEffects.None;
            }
        }
        //public vec2 LastPos = new vec2();
        public Guy(WorldBase w, string spr, AIState ai) : base(w, spr)
        {
            AIState = ai;


            if (ai == AIState.Player)
            {
                Joystick = new Joystick(w.Screen, true);
            }
            else
            {
                Joystick = new Joystick(w.Screen, false);
            }

        }

        public void HoldItem(GameObject ob)
        {
            if (ob == null)
            {
                //Apply trans, remove parent, remove item
                ItemHeld.Pos = Pos + ItemHeld.Pos;
                ItemHeld.Parent = null;
                ItemHeld = null;
                //Set these animations to NULL to prevent invalid state changes
                if (String.IsNullOrEmpty(WalkSprite) == false)
                {
                    SetSprite(WalkSprite);
                }

                return;
            }

            if (string.IsNullOrEmpty(ItemHeldWalkSprite) == false)
            {
                SetSprite(ItemHeldWalkSprite);
            }


            ItemHeld = ob;
            ob.Parent = this;
            UpdateHeldItem();
        }
        public void UpdateHeldItem()
        {
            if (ItemHeld != null)
            {
                float len = 8;
                //I guess this is adding the item's pos from the origin of the player
                ItemHeld.Pos = MathUtils.DecomposeRotation(Rotation) * len;


            }
        }

        public override void CalcBoundBox()
        {
            if (PhysicsShape == PhysicsShape.Ball)
            {
                Box = new Box2f(WorldPos() - PhysicsBallRadiusPixels, WorldPos() + PhysicsBallRadiusPixels);
            }
            else
            {
                if (Crouching == true)
                {
                    Box = new Box2f(WorldPos() + BoxRelativeCrouch.Min, WorldPos() + BoxRelativeCrouch.Max);
                }
                else
                {
                    Box = new Box2f(WorldPos() + BoxRelative.Min, WorldPos() + BoxRelative.Max);
                }
            }

            ApplyRotateBoundBox();
        }



        public void SetWalkSprite()
        {
            //set based on state.
            if (ItemHeld != null)
            {
                SetSprite(ItemHeldWalkSprite);
            }
            else
            {
                SetSprite(WalkSprite);
            }
        }

        public void LimitAcc()
        {
            if (Acc.Len2() > MaxAcc * MaxAcc)
            {
                Acc = Acc.Normalized() * MaxAcc;
            }
        }

        public void PlayHitSound()
        {
            if (HitSounds.Count == 0)
            {
                return;
            }
            int n = Globals.RandomInt(0, HitSounds.Count);

            Res.Audio.PlaySound(HitSounds[n]);
        }
        public void PlayDieSound()
        {
            if (DieSounds.Count == 0)
            {
                return;
            }
            int n = Globals.RandomInt(0, DieSounds.Count);

            Res.Audio.PlaySound(DieSounds[n]);
        }
        public bool DoPlayGrowl = false;
        public void PlayGrowlSound()
        {
            int n = Globals.RandomInt(0, GrowlSounds.Count);

            Res.Audio.PlaySound(GrowlSounds[n]);
        }

        public void CalcRelPos()
        {
            cposR = Pos + cposrelR;
            cposB = Pos + cposrelB;
            cposT = Pos + cposrelT;
            cposL = Pos + cposrelL;

            cposC = Pos + cposrelC;
        }

        public void SetAllMotionSprites(string s)
        {
            IdleSprite = s;
            WalkSprite = s;
            CrouchSprite = s;
            ItemHeldWalkSprite = s;
            ClimbSprite = s;
            HangSprite = s;
            JumpSprite = s;
            MountSprite = s;
            SpringJumpSprite = s;
            WalkAttackSprite = s;
            CrouchAttackSprite = s;
            FallSprite = s;
            SleepSprite = s;
            DefendSprite = s;
            TalkSprite = s;
        }

        public void AISetRandomDir()
        {
            bool dir = Globals.RandomBool();
            if (dir)
            {
                Joystick.AIRight = true;
                Joystick.AILeft = false;
            }
            else
            {
                Joystick.AIRight = false;
                Joystick.AILeft = true;
            }
        }




    }
    public class Duck : Guy
    {
        public Duck(WorldBase w, string spr, AIState ai) : base(w, spr, ai)
        {

        }
    }
    public enum SwordModifier { None, Tar, Water, Lava, Obsidian }
    public enum Weapon { None, Bomb, Bow, Sword }
    public class Player : Guy
    {
        public Player(WorldBase w, string spr, AIState ai) : base(w, spr, ai) { }

        public bool HasInteractedThisFrame = false;//set to true if the player cliekd something this frame already so we don't swing the sword

        public float LastCollideGrassMax = 0.2f;
        public float LastCollideGrass = 0.2f; //Every x seconds we collide with the grass again

        public static float PlayerBaseEmitRadius() { return 16 * 1; }
        public static vec4 PlayerBaseEmitColor() { return new vec4(0.2f, 0.2f, 0.2f, 1.0f); }

        public static float PowerSwordEmitRadius() { return 16 * 5; }
        public static vec4 PowerSwordEmitColor() { return new vec4(0.85f, 0.85f, 0.85f, 1.0f); }

        public static vec4 FireSwordEmitColor() { return new vec4(1, 1, .2f, 1); }

        public bool NoWeaponAction()
        {
            return SwordOut == false && BowOut == false && ItemHeld == null;
        }

        public float ShieldDeflectionMaxAngle = 90; // Maximum of 90 degrees since we can "fly" with shielld
        public static float SpringBootsMinTime = 0.05f;
        public float BombTime = 0;
        public float MaxBombTime = 0.06f;


        public bool ShieldOut = false;//If shield is out
        public bool SwordOut = false;//If sq     is out

        public bool SwordEnabled = false;
        public bool BombsEnabled = false;
        public int NumBombs = 3;
        public int MaxBombs = 99;
        public bool JumpBootsEnabled = false;
        public bool PowerSwordEnabled = false;

        public bool BowOut = false;
        public float BowDrawTime = 0;
        public float BowDrawTimeMax = 0.5f;

        public bool BowEnabled = false;
        public int NumArrows = 99;
        public int MaxArrows = 99;

        public bool ShieldEnabled = false;
        public int SmallKeys = 0;
        public int MaxSmallKeys = 99;
        public int Money = 0;
        public int MaxMoney = 99;

        public SwordModifier SwordModifier = SwordModifier.None;   // if sword has tar on it.
        public bool SwordOnFire = false;
        public float SwordOnFireTime = 0;
        public float SwordOnFireTimeMax = 0.1f;

        public Weapon SelectedSubweapon = Weapon.None;

        public override void Serialize(BinaryWriter w)
        {
            w.Write(SwordEnabled);
            w.Write(ShieldEnabled);
            w.Write(BombsEnabled);
            w.Write(BowEnabled);
            w.Write(JumpBootsEnabled);
            w.Write(PowerSwordEnabled);

            w.Write(NumBombs);
            w.Write(MaxBombs);
            w.Write(NumArrows);
            w.Write(MaxArrows);

            w.Write(SmallKeys);
            w.Write(Money);
            w.Write(MaxMoney);

            base.Serialize(w);
        }
        public override void Deserialize(BinaryReader r)
        {
            SwordEnabled = r.ReadBoolean();
            ShieldEnabled = r.ReadBoolean();
            BombsEnabled = r.ReadBoolean();
            BowEnabled = r.ReadBoolean();
            JumpBootsEnabled = r.ReadBoolean();
            PowerSwordEnabled = r.ReadBoolean();

            NumBombs = r.ReadInt32();
            MaxBombs = r.ReadInt32();
            NumArrows = r.ReadInt32();
            MaxArrows = r.ReadInt32();

            SmallKeys = r.ReadInt32();
            Money = r.ReadInt32();
            MaxMoney = r.ReadInt32();

            base.Deserialize(r);
        }
        public float SwordLastSwipeTime = 0;
        public float SwordSwingMaxAngleRadians = 1;//radians
        public int SwordSwingDir = 0;//0=updown 1=downup
        public float SwordSwingDelayMaxSwing0 = 0.25f; //Max delay, e.g. if we have a strong pickaxe or something
        public float SwordSwingDelayMaxSwing1 = 0.1f; //Max delay, e.g. if we have a strong pickaxe or something
        public float SwordSwingDelayMaxSwing2 = 0.1f; //Max delay, e.g. if we have a strong pickaxe or something
        public float SwordSwingDelayMax = 0.1f; //Max delay, e.g. if we have a strong pickaxe or something
        public float SwordSwingDelay = 0.0f; //Dealy in mining
        public float ControlSwordDelay = 0.025f;   //time til the player has control over the sword
        public float ControlSwordDelayMax = 0.025f;//time til the player has control over the sword
        public vec2 ShieldClickNormal;
        public vec2 SwordClickNormal;

        public float PowerSwordCharge = 0;
        public float PowerSwordChargeWait = 1;
        public float PowerSwordChargeWaitMax = 1;
        public float PowerSwordChargeBase = 3f;
        public float PowerSwordChargeBaseMax = 3f;
        public float PowerSwordChargeRise = 0.5f;//3.5s to full sword charge
        public float PowerSwordChargeRiseMax = 0.5f;
        public float PowerSwordChargePulse = 0;
        public float PowerSwordChargePulseDirection = 0;
        public float PowerSwordChargePulseSpeed = 0;
        public bool PowerSwordCharged = false;



    }
    public enum CutsceneType
    {
        None, // Just open, for money
        Powerup, //Show a short open without drama
        KeyItem //Key items, show a cutscene.
    }
    public class SpecialItem
    {
        public Sprite Sprite = null;
        public string Name = "Cool New Thing";
        public List<string> Description = new List<string>() { "This is your new thing.  It works by presing A." };
        public int ItemId;
        public SpecialItemChestType SpecialItemChestType = SpecialItemChestType.Gold_Big;
        public CutsceneType CutsceneType = CutsceneType.KeyItem;
        public SpecialItem(string name, List<string> desc, int itemid, Sprite spr, SpecialItemChestType ct, bool sparkle)
        {
            Name = name;
            Sprite = spr;
            Description = desc;
            ItemId = itemid;
            SpecialItemChestType = ct;
            Sparkle = sparkle;
        }

        public bool Sparkle = false;    //if chest does sparkle.

        public Func<World, float, bool> AfterInfoDialogClosed = null;//World, Deltatime
    }
    public class ButtonSwitch : GameObject
    {
        public bool Pressed = false;
        public ButtonSwitch(WorldBase w) : base(w) { }

        public override void CreateNew()
        {

        }
        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);
            w.Write(Pressed);
        }
        public override void Deserialize(BinaryReader w)
        {
            base.Deserialize(w);
            Pressed = w.ReadBoolean();
            UpdateSprite();
        }
        public override void UpdateSprite()
        {
            if (Pressed) { SetFrame(1); }
            else SetFrame(0);
        }
    }
    public class Tile : GameObject
    {
        public bool CanMine = false;
        public bool FallThrough = false;
        public bool CanClimb = false;
        public BlockType BlockType = BlockType.Stone;
        public Action<Cell, int, TileBlock> Create = null;

        public bool CanBomb = false;

        public List<string> RandomSprites = null;//IF SET then we use a random set of sprites to generate.

        public Tile(WorldBase w, Sprite sprite, vec2 xy, vec2 wh, int hp, Action<Cell, int, TileBlock> create) : base(w, sprite, xy)
        {
            Health = MaxHealth = hp;
            Box = new Box2f(xy, xy + wh);
            Create = create;
        }
    }
    public class Sign : GameObject
    {
        public Sign(WorldBase w, List<string> text) : base(w) { Text = text; }
        public List<string> Text = new List<string>() { "" };
    }
    public class TileBlock
    {
        public Tile Tile = null; // Sprite for this block.  This also containst he TILE ID.  This is a REFERENCE to the tile.  Not a copy
        public Sprite Sprite = null;
        public int FrameIndex = 0;
        public float Health = 100; // Max Health is 100 for all blocks
        public bool Blocking = false;
        public bool CanMine = false;
        public bool CanBomb = false;
        public bool IsConduit = false;
        public bool FallThrough = false;
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        public Box2f Box;   // So we need custom boxes for things like ladders, &c
        public vec2 Pos { get { return Box.Min; } private set { } }

        public int SlopeTileId = -1;//One of 4 slopes TL TR BL BR
        public bool IsSlope() { return SlopeTileId != -1; }
    }
    public enum WaterType { Water, Lava, Tar }
    public class Cell
    {
        public int LastUpdatedCellFrame = -1;
        public List<GameObject> ObjectsFrame = new List<GameObject>();
        public List<TileBlock> Layers;

        //public vec4 WaterColor = new vec4(0.1f, 0.1f, 1.0f, 0.6f); 
        public float Water = 0.0f;
        public bool WaterTravelFrame = false;//
        public bool WaterOnRight = false;
        public bool WaterOnLeft = false;
        public WaterType WaterType = WaterType.Lava;

        public vec2 Pos()
        {
            return Parent.Box.Min;
        }
        public Box2f Box()
        {
            return Parent.Box;
        }
        public Box2f DeviceBoundBox()
        {
            Box2f gridBox = new Box2f(
                Parent.Level.World.Screen.Viewport.WorldToDevice(Parent.Box.Min),
                Parent.Level.World.Screen.Viewport.WorldToDevice(Parent.Box.Max)
                );
            return gridBox;
        }
        public Node Parent { get; private set; }
        public Cell(Node parent, int nLayers)
        {
            Parent = parent;
            Layers = new List<TileBlock>(nLayers);
            for (int i = 0; i < nLayers; ++i)
            {
                Layers.Add(null);
            }

        }

        public vec4 LightColor = new vec4(1, 1, 1, 1);   // the color
        public int MarchFrame = 0;
        public int MarchFrameLight = 0;
        //public float LightValue = 0;    // 0 - 100 = 0 = black 100 = transparent

        public ivec2 GetTilePosLocal()
        {
            return new ivec2(
                (int)(Parent.Box.Min.x / Res.Tiles.TileWidthPixels),
                (int)(Parent.Box.Min.y / Res.Tiles.TileHeightPixels)
                );
        }
        public ivec2 GetTilePosGlobal()
        {
            return Parent.Level.Room.Min + GetTilePosLocal();
        }
    }
    public class Node
    {
        public PlatformLevel Level { get; private set; }
        //Node in a binary box tree - we don't use a quadtree because our grids are not square
        // public bool IsLeaf = false;
        public Box2f Box { get; set; } // Box, in Game-world pixels (not device pixels)
        public Node(PlatformLevel level, Box2f box) { Level = level; Box = box; }
        public Node[] Children;
        public Cell Cell = null;
    }
    public class TileGrid
    {
        public PlatformLevel Level { get; private set; }
        public Node RootNode { get; set; }
        public Dictionary<ivec2, Cell> CellDict;
        int dbg_numnodes = 0;
        int dbg_numcells = 0;
        int NumLayers = 0;

        public bool TouchCell(Cell c)
        {
            if (c == null)
            {
                return false;
            }

            Box2f gridBox = new Box2f(
                Level.World.Screen.Viewport.WorldToDevice(c.Parent.Box.Min),
                Level.World.Screen.Viewport.WorldToDevice(c.Parent.Box.Max)
                );

            if (gridBox.ContainsPointInclusive(Level.World.Screen.Game.Input.LastTouch))
            {
                return true;
            }

            return false;
        }
        public TileGrid(PlatformLevel level, int tilesW, int tilesH, int nLayers)
        {
            Level = level;
            NumLayers = nLayers;

            CellDict = new Dictionary<ivec2, Cell>(new ivec2EqualityComparer());

            RootNode = new Node(level, GetGridExtents(tilesW, tilesH));
            DivideGrid(RootNode);
        }

        public void DivideGrid(Node parent)
        {
            if (parent.Box.Width() <= 0)
            {
                System.Diagnostics.Debugger.Break();
            }
            if (parent.Box.Height() <= 0)
            {
                System.Diagnostics.Debugger.Break();
            }

            //Double sanity - we must always be evenly divisible by tiles.
            if (((int)parent.Box.Width() % Res.Tiles.TileWidthPixels) != 0)
            {
                System.Diagnostics.Debugger.Break();
            }
            if (((int)parent.Box.Height() % Res.Tiles.TileHeightPixels) != 0)
            {
                System.Diagnostics.Debugger.Break();
            }

            vec2 boxwh = (parent.Box.Max - parent.Box.Min);
            int tilesXParent = (int)((boxwh.x / Res.Tiles.TileWidthPixels));
            int tilesYParent = (int)((boxwh.y / Res.Tiles.TileHeightPixels));
            int tilesXMid = (int)((boxwh.x / Res.Tiles.TileWidthPixels) * 0.5f);
            int tilesYMid = (int)((boxwh.y / Res.Tiles.TileHeightPixels) * 0.5f);

            if (tilesXParent == 1 && tilesYParent == 1)
            {
                ivec2 cellPos = new ivec2(
                    (int)(parent.Box.Min.x / Res.Tiles.TileWidthPixels),
                    (int)(parent.Box.Min.y / Res.Tiles.TileHeightPixels)
                    );
                parent.Cell = new Cell(parent, NumLayers);

                if (CellDict.ContainsKey(cellPos))
                {
                    //Error: cell already found
                    System.Diagnostics.Debugger.Break();
                }
                else
                {
                    CellDict.Add(cellPos, parent.Cell);
                }

                dbg_numcells++;
            }
            else
            {
                Box2f A;
                Box2f B;

                if (tilesXParent > tilesYParent)
                {
                    float midx = parent.Box.Min.x + (float)tilesXMid * Res.Tiles.TileWidthPixels;

                    A = new Box2f(new vec2(parent.Box.Min.x, parent.Box.Min.y), new vec2(midx, parent.Box.Max.y));
                    B = new Box2f(new vec2(midx, parent.Box.Min.y), new vec2(parent.Box.Max.x, parent.Box.Max.y));
                }
                else
                {
                    float midy = parent.Box.Min.y + (float)tilesYMid * Res.Tiles.TileHeightPixels;

                    A = new Box2f(new vec2(parent.Box.Min.x, parent.Box.Min.y), new vec2(parent.Box.Max.x, midy));
                    B = new Box2f(new vec2(parent.Box.Min.x, midy), new vec2(parent.Box.Max.x, parent.Box.Max.y));
                }

                parent.Children = new Node[2];
                parent.Children[0] = new Node(Level, A);
                parent.Children[1] = new Node(Level, B);

                dbg_numnodes += 2;

                int i = 0;
                foreach (Node n in parent.Children)
                {
                    DivideGrid(n);
                    i++;
                }
            }


        }
        public Cell GetCellForPoint(ivec2 gridpos)
        {
            vec2 v = new vec2(
                (float)gridpos.x * (float)Res.Tiles.TileWidthPixels + (float)Res.Tiles.TileWidthPixels * 0.5f,
                (float)gridpos.y * (float)Res.Tiles.TileHeightPixels + (float)Res.Tiles.TileHeightPixels * 0.5f
                );
            return GetCellForPoint(v);
        }
        public Cell GetCell(ivec2 xy)
        {
            Cell cell = null;
            //Gets the cell at the grid pos
            CellDict.TryGetValue(xy, out cell);
            return cell;
        }
        public Cell GetCellForPoint(vec2 pos)
        {
            Node parent = RootNode;
            Cell ret = null;

            int nSanity = 0;//Instead of using a while true loop we do this to prevent catastrophic failure
            while (nSanity < 1000)
            {
                foreach (Node n in parent.Children)
                {
                    if (n.Box.ContainsPointInclusive(pos))
                    {
                        if (n.Cell != null)
                        {
                            ret = n.Cell;
                            nSanity = 1005;
                        }
                        else
                        {
                            parent = n;
                        }

                        break;
                    }
                }

                nSanity++;
            }


            return ret;
        }
        public Cell[] GetSurroundingCells(Cell c, bool corners = false)
        {
            //If corners is false, you skip the corners of the 3x3 grid

            Cell[] n = new Cell[9];
            n[4] = c;
            if (c == null) { return n; }

            for (int j = -1; j <= 1; ++j)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    if (i == 0 && j == 0)
                    {
                        //Skip center
                    }
                    else if (corners == false && ((i == -1 && j == -1) || (i == 1 && j == 1) || (i == -1 && j == 1) || (i == 1 && j == -1)))
                    {
                        //Skip corners if they're false
                    }
                    else
                    {
                        int ind = (j + 1) * 3 + (i + 1);
                        n[ind] = GetNeighborCell(c, i, j);
                    }

                }
            }

            return n;
        }
        public Box2f GetGridExtents(int tilesW, int tilesH)
        {
            Box2f b = new Box2f(
                new vec2(0, 0),
                new vec2(tilesW * Res.Tiles.TileWidthPixels, tilesH * Res.Tiles.TileHeightPixels));
            return b;
        }
        public List<Cell> GetCellManifoldForBox(Box2f b)
        {
            int x = (int)(b.Min.x / Res.Tiles.TileWidthPixels);
            int y = (int)(b.Min.y / Res.Tiles.TileHeightPixels);

            int w = (int)Math.Ceiling(b.Width() / Res.Tiles.TileWidthPixels);
            int h = (int)Math.Ceiling(b.Height() / Res.Tiles.TileHeightPixels);

            List<Cell> ret = new List<Cell>();

            ivec2 vtmp;
            for (int iy = y; iy <= (y + h); ++iy)
            {
                for (int ix = x; ix <= (x + w); ++ix)
                {
                    vtmp = new ivec2(ix, iy);
                    Cell c = null;
                    if (CellDict.TryGetValue(vtmp, out c))
                    {
#if DEBUG
                        if (ret.Contains(c))
                        {

                            //SANITY CHEC
                            System.Diagnostics.Debugger.Break();
                        }
#endif
                        ret.Add(c);
                    }
                    else
                    {
                        int n = 0;
                        n++;
                    }
                }
            }

            return ret;
        }
        public Cell GetCellAbove(Cell c)
        {
            return GetNeighborCell(c, 0, -1);
        }
        public Cell GetNeighborCell(Cell c, int x, int y)
        {
            //X increases right
            //Y increases down
            if (c == null)
            {
                return null;
            }
            vec2 pt = new vec2(
                c.Parent.Box.Min.x + Res.Tiles.TileWidthPixels * (float)x + Res.Tiles.TileWidthPixels * 0.5f,
                c.Parent.Box.Min.y + Res.Tiles.TileHeightPixels * (float)y + Res.Tiles.TileHeightPixels * 0.5f);
            Cell ret = GetCellForPoint(pt);
            return ret;
        }
    }
    public class Room
    {
        public int RoomId;
        public ivec2 Min = new ivec2(0, 0);
        public ivec2 Max = new ivec2(0, 0);
        //public List<ivec2> Found = new List<ivec2>();
        public HashSet<ivec2> Found = new HashSet<ivec2>(new ivec2EqualityComparer());
        public HashSet<ivec2> Border = new HashSet<ivec2>(new ivec2EqualityComparer());
        public HashSet<ivec2> Doors = new HashSet<ivec2>(new ivec2EqualityComparer());
        public int WidthTiles { get; private set; } = -1;
        public int HeightTiles { get; private set; } = -1;

        public Room()
        {
            Min.x = Min.y = Int32.MaxValue;
            Max.x = Max.y = -Int32.MaxValue;
        }
        public void Validate()
        {
            if (Min.x > Max.x || Min.y > Max.y)
            {
                System.Diagnostics.Debugger.Break();
            }
            if (Found.Count == 0)
            {
                //Don't know whyt his would happen.
                System.Diagnostics.Debugger.Break();
            }
            if (Found.Count > 90000)//For the first area we're at 5670 so still, pretty big
            {
                //Level is Too Big
                //You probably forgot a portal or delimiter wall somewhere.
                System.Diagnostics.Debugger.Break();
            }

            //include the border for doors
            //Plus 1 - the magnitue doesn't include the end tile
            //Max.x += 1;
            //Max.y += 1;

            //Subtract 1 for the outside border.
            WidthTiles = Max.x - Min.x + 1;
            HeightTiles = Max.y - Min.y + 1;
        }

    }
    public class TileDefs
    {
        public static Dictionary<int, Tile> TileLUT = null;
        public static Dictionary<int, Func<Cell, int, GameObject>> ObjLUT = null;//TILED TileID mapped to Create function (Cell, Layer, Return GameObject)
        public static Dictionary<int, SpecialItem> SpecialItemLUT = null;
        public static Dictionary<int, Dictionary<string, List<string>>> SignLUT = null;
        public static List<int> DoorTilesLUT = null;

        private WorldBase World;
        public TileDefs(WorldBase w)
        {
            this.World = w;
            BuildTileLUT();
            BuildObjLUT();
            BuildSpecialItemLUT();
            //BuildSignLUT();
            DoorTilesLUT = new List<int>() {
                    Res.Doorless_Portal_TileId,
                    Res.Door_Lock_LeftOnly_TileId,
                    Res.Door_Lock_Monster_TileId,
                    Res.Door_Lock_RightOnly_TileId,
                    Res.Door_Lock_TileId,
                    Res.Door_Nolock_TileId,
                    Res.SwitchDoorTileId
                };
        }
        private void BuildObjLUT()
        {
            if (ObjLUT != null)
            {
                return;
            }

            ObjLUT = new Dictionary<int, Func<Cell, int, GameObject>>();
            BuildMonsterObjLUT();
            BuildItemObjLUT();
            BuildTriggerObjLUT();
            BuildNPCObjLut();
        }
        public void BuildItemObjLUT()
        {
            ObjLUT.Add(Res.TorchTileId, (cell, ilayer) =>
            {
                GameObject g = new GameObject(World, Res.SprTorch, cell.Pos());
                g.Pos = cell.Pos();
                g.Animate = true;
                g.BlocksLight = false;
                g.EmitLight = true;
                g.EmitRadiusInPixels = 16 * 4;
                g.EmitColor = new vec4(
                    Globals.Random(0, 1),
                    Globals.Random(0, 1),
                    Globals.Random(0, 1),
                    1.0f);
                g.EmitColor.SetMinLightValue(1.5f);

                return g;
            });
            ObjLUT.Add(Res.LanternTileId, (cell, ilayer) =>
            {
                GameObject g = new GameObject(World, Res.SprLantern, cell.Pos());
                g.Pos = cell.Pos();
                g.Animate = true;
                g.BlocksLight = false;
                g.EmitLight = true;
                g.EmitRadiusInPixels = 16 * 9;
                g.EmitColor = new vec4(
                    Globals.Random(0, 1),
                    Globals.Random(0, 1),
                    Globals.Random(0, 1),
                    1.0f);

                g.EmitColor.SetMinLightValue(1.5f);

                return g;
            });
            ObjLUT.Add(Res.SmallChestTileId, (cell, ilayer) =>
            {
                TreasureChest g = new TreasureChest(World, Res.SprSmallChest, cell.Pos());
                g.Pos = cell.Pos();
                g.Animate = false;
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Money = 1;// Globals.RandomInt(3, 10);

                return g;
            });
            ObjLUT.Add(Res.SilverSmallChestTileId, (cell, ilayer) =>
            {
                TreasureChest g = new TreasureChest(World, Res.SprSilverSmallChest, cell.Pos());
                g.Pos = cell.Pos();
                g.Animate = false;
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Money = Globals.RandomInt(10, 20);

                return g;
            });
            ObjLUT.Add(Res.GoldSmallChestTileId, (cell, ilayer) =>
            {
                TreasureChest g = new TreasureChest(World, Res.SprGoldSmallChest, cell.Pos());
                g.Pos = cell.Pos();
                g.Animate = false;
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Money = Globals.RandomInt(20, 50);

                return g;
            });
            ObjLUT.Add(Res.SavePointTileId, (cell, ilayer) =>
            {
                GameObject g = new GameObject(World, Res.SprSavePoint, cell.Pos());
                g.Pos = cell.Pos();
                g.Animate = false;
                g.SetFrame(1);
                g.BoxRelative = new Box2f(3, 3, 16 - 2 * 2, 16 - 3 * 2);

                g.BlocksLight = false;
                g.EmitLight = true;
                g.EmitRadiusInPixels = 16 * 4;
                g.EmitColor = new vec4(
                    Globals.Random(.7f, 1),
                    Globals.Random(.6f, 1),
                    Globals.Random(.1f, 1),
                    1.0f);

                return g;
            });
            ObjLUT.Add(Res.SwitchButtonTileId, (cell, ilayer) =>
            {
                ButtonSwitch g = new ButtonSwitch(World);
                g.Pos = cell.Pos();
                g.Animate = false;
                g.SetSprite(Res.SprButtonSwitch);
                g.SetFrame(0);
                g.BoxRelative = new Box2f(4, 12, 16 - 8, 4);
                g.BlocksLight = false;
                g.EmitLight = false;

                return g;
            });
            ObjLUT.Add(Res.BrazierTileId, (cell, ilayer) =>
            {
                GameObject g = new GameObject(World);
                g.Pos = cell.Pos();
                g.Animate = true;
                g.SetSprite(Res.SprBrazier);
                g.SetFrame(0);
                g.BoxRelative = new Box2f(3, 2, 16 - 6, 14);
                g.BlocksLight = false;
                g.EmitLight = true;
                g.EmitRadiusInPixels = Res.Tiles.TileWidthPixels * 3;
                g.EmitColor = new vec4(0.93f, 0.85f, 0, 1);
                return g;
            });
            ObjLUT.Add(Res.AppleTileId, (cell, ilayer) =>
            {
                GameObject g = new GameObject(World);
                g.Pos = cell.Pos();
                g.Animate = false;
                g.SetSprite(Res.SprApple);
                g.SetFrame(0);
                g.Origin = new vec2(8, 8);
                g.BoxRelative = new Box2f(-2, -2, 4, 4);
                g.PhysicsResponse = PhysicsResponse.Bounce_And_Roll;
                g.PhysicsShape = PhysicsShape.Ball;
                g.PhysicsBallRadiusPixels = 3;
                g.Gravity = World.Gravity;
                g.Health = 1;
                return g;
            });

        }
        public void BuildMonsterObjLUT()
        {
            ObjLUT.Add(Res.GuyTileId, (cell, ilayer) =>
            {
                Player g = new Player(World, Res.SprGuyWalk, AIState.Player);
                g.Origin = new vec2(Res.Tiles.TileWidthPixels * 0.5f, Res.Tiles.TileHeightPixels * 0.5f);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-4, -6, 8, 14);
                g.BoxRelativeCrouch = new Box2f(-4, 0, 8, 7);
                g.Animate = true;
                g.Speed = 80.0f;
                g.Power = 1;
                g.MaxAcc = 200;
                g.Knockback = 60.0f;
                g.Gravity = World.Gravity;

                g.HurtTimeMax = 0.4f;

                g.cposrelL = new vec2(g.Origin.x + g.BoxRelative.Min.x, 14);
                g.cposrelR = new vec2(g.Origin.x + g.BoxRelative.Max.x, 14);

                g.BlocksLight = false;
                g.EmitLight = true;
                g.EmitRadiusInPixels = Player.PlayerBaseEmitRadius();
                g.EmitColor = Player.PlayerBaseEmitColor();

                g.ItemHeldWalkSprite = Res.SprGuyWalk;
                g.WalkSprite = Res.SprGuyWalk;
                g.CrouchSprite = Res.SprGuyWalk;
                g.JumpSprite = Res.SprGuyJump;
                g.HangSprite = Res.SprGuyWalk;
                g.MountSprite = Res.SprGuyWalk;
                g.ClimbSprite = Res.SprGuyWalk;
                g.SpringJumpSprite = Res.SprGuyJump;
                g.WalkAttackSprite = Res.SprGuyWalk;
                g.CrouchAttackSprite = Res.SprGuyWalk;
                g.FallSprite = Res.SprGuyWalk;

                return g;
            });
            ObjLUT.Add(Res.ZombieTileId, (cell, ilayer) =>
            {
                Guy g = new Guy(World, Res.SprZombieWalk, AIState.Wander);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-4, -6, 8, 14);
                g.Origin = new vec2(Res.Tiles.TileWidthPixels * 0.5f, Res.Tiles.TileHeightPixels * 0.5f);
                g.Animate = true;
                g.Power = 1;
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Health = 2;
                g.Speed = 10.0f;
                g.MaxAcc = 50;
                g.Knockback = 20.0f;
                g.Gravity = World.Gravity;

                g.Friction = 8.0f;//Make this less than the player so we can knockback easier

                g.HitSounds.Add(Res.SfxZombHit0);
                g.HitSounds.Add(Res.SfxZombHit1);

                g.GrowlSounds.Add(Res.SfxZombGrowl0);
                g.GrowlSounds.Add(Res.SfxZombGrowl1);

                g.SetAllMotionSprites(Res.SprZombieWalk);

                return g;
            });
            ObjLUT.Add(Res.SkeleTileId, (cell, ilayer) =>
            {
                Guy g = new Guy(World, Res.SprSkeleWalk, AIState.Wander);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-4, -6, 8, 14);
                g.Origin = new vec2(Res.Tiles.TileWidthPixels * 0.5f, Res.Tiles.TileHeightPixels * 0.5f);
                g.Animate = true;
                g.Power = 1;
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Health = 30;
                g.Speed = 30.0f;
                g.MaxAcc = 50;
                g.Knockback = 20.0f;
                g.Gravity = World.Gravity;

                g.Friction = 8.0f;//Make this less than the player so we can knockback easier

                g.HitSounds.Add(Res.SfxZombHit0);
                g.HitSounds.Add(Res.SfxZombHit1);

                g.GrowlSounds.Add(Res.SfxZombGrowl0);
                g.GrowlSounds.Add(Res.SfxZombGrowl1);
                g.SetAllMotionSprites(Res.SprSkeleWalk);

                return g;
            });
            ObjLUT.Add(Res.GlowfishTileId, (cell, ilayer) =>
            {
                string MainSprite = Res.SprGlowfish_Swim;

                Guy g = new Guy(World, MainSprite, AIState.SwimLeftRight);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-4, -4, 8, 8);
                g.Animate = true;
                g.Power = 1;
                g.Gravity = World.Gravity;// new vec2(0, 0);//we HAVE gravity, but only when OUT of water

                //Lives in water and swims. Reverse gravity.  if in water - 
                g.NormalDamp = 1;
                g.NormalDampGrav = 1;// g.WaterDampGrav;
                g.WaterDamp = 1.0f;
                g.WaterDampGrav = 0.0f; //No gravity influence if in water

                g.BlocksLight = false;
                g.EmitLight = true;
                g.EmitColor = new vec4(0.6f, 0.6f, 0.6f, 1.0f);
                // g.EmitColor.SetMinLightValue(1.5f);
                g.EmitRadiusInPixels = Res.Tiles.TileWidthPixels * 5;
                g.Health = 30;
                g.Speed = 10.0f;
                g.MaxAcc = 1;
                g.Knockback = 20.0f;
                g.CanJump = false;
                g.AISetRandomDir();

                g.Friction = 0.0f;//Make this less than the player so we can knockback easier

                g.HitSounds.Add(Res.SfxZombHit0);
                g.HitSounds.Add(Res.SfxZombHit1);

                g.GrowlSounds.Add(Res.SfxZombGrowl0);
                g.GrowlSounds.Add(Res.SfxZombGrowl1);
                g.SetAllMotionSprites(MainSprite);

                return g;
            });
            ObjLUT.Add(Res.RockMonsterTileId, (cell, ilayer) =>
            {
                Guy g = new Guy(World, Res.SprRockMonsterWalk, AIState.Wander);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-4, -4, 8, 8);
                g.Animate = true;
                g.Power = 1;
                g.BlocksLight = false;
                g.EmitLight = false;
                // g.EmitRadiusInPixels = 16 * 3;
                g.Health = 80;
                g.Speed = 4.0f;
                g.MaxAcc = 20;
                g.Knockback = 10.0f;
                g.CanJump = false;
                g.WaterDamp = g.WaterDampGrav = 1.0f; //Lives in water, no speed reduction
                g.Gravity = World.Gravity;

                g.Friction = 8.0f;//Make this less than the player so we can knockback easier

                g.HitSounds.Add(Res.SfxZombHit0);
                g.HitSounds.Add(Res.SfxZombHit1);

                g.GrowlSounds.Add(Res.SfxZombGrowl0);
                g.GrowlSounds.Add(Res.SfxZombGrowl1);
                g.SetAllMotionSprites(Res.SprRockMonsterWalk);

                return g;
            });

            Func<Cell, int, string, Guy> GrubBase = (Cell cell, int ilayer, string sprite) =>
            {
                Guy g = new Guy(World, sprite, AIState.MoveLRConstant);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-5, -8, 10, 9);
                g.AIPhysics = AIPhysics.Grapple;
                g.Animate = true;
                g.Power = 1;
                g.Origin = new vec2(8, 15.1f);
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Health = 80;
                g.MaxAcc = 20;
                g.Knockback = 0.0f;
                g.CanJump = false;

                g.WaterDamp = g.WaterDampGrav = 1.0f; //Lives in water, no speed reduction
                g.Gravity = 0.0f;
                g.GrappleDir = ((Globals.Random(0, 1) > 0.5f) ? 1 : 0);//1 = move clockwise / right

                g.Friction = 0.0f;//Make this less than the player so we can knockback easier
                g.HitSounds.Add(Res.SfxGrubHit);
                g.DieSounds.Add(Res.SfxGrubDie);
                g.SetAllMotionSprites(sprite);

                return g;
            };

            ObjLUT.Add(Res.EnemGrubGrassTileId, (cell, ilayer) =>
            {
                Guy grub = GrubBase(cell, ilayer, Res.SprGrub);
                grub.Speed = 0.5f;// * ;
                grub.Power = 1;
                grub.Health = 2;
                return grub;
            });
            ObjLUT.Add(Res.EnemGrubWaterTileId, (cell, ilayer) =>
            {
                Guy grub = GrubBase(cell, ilayer, Res.SprGrubWater);
                grub.Speed = 0.55f;// * ;
                grub.Power = 2;
                grub.Health = 3;
                grub.EmitLight = true;
                grub.EmitColor = new vec4(0.0f, 0.2f, 1.0f, 1.0f);
                grub.EmitRadiusInPixels = Res.Tiles.TileWidthPixels * 3;
                return grub;
            });
            ObjLUT.Add(Res.EnemGrubRockTileId, (cell, ilayer) =>
            {
                Guy grub = GrubBase(cell, ilayer, Res.SprGrubRock);
                grub.Speed = 0.6f;// * ;
                grub.Power = 4;
                grub.Health = 5;
                return grub;
            });
            ObjLUT.Add(Res.EnemGrubLavaTileId, (cell, ilayer) =>
            {
                Guy grub = GrubBase(cell, ilayer, Res.SprGrubLava);
                grub.Speed = 0.6f;// * ;
                grub.Power = 5;
                grub.Health = 6;

                grub.EmitLight = true;
                grub.EmitColor = new vec4(1.0f, 0.8f, 0.0f, 1.0f);
                grub.EmitRadiusInPixels = Res.Tiles.TileWidthPixels * 3;

                return grub;
            });


            ObjLUT.Add(Res.PlantBombGuyTileId, (cell, ilayer) =>
            {
                Guy g = new Guy(World, Res.SprPlantBombDudeIdle, AIState.Sleep);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-6, -4, 8, 12);
                g.AIPhysics = AIPhysics.PlantBombGuy;
                g.Animate = true;
                g.Power = 1;
                g.Origin = new vec2(8, 15.1f);
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Health = 80;
                g.Speed = 0.5f;// * ;
                g.MaxAcc = 20;
                g.Knockback = 0.0f;
                g.CanJump = false;
                g.WaterDamp = g.WaterDampGrav = 1.0f; //Lives in water, no speed reduction
                g.Gravity = 0.0f;

                g.GrappleDir = ((Globals.Random(0, 1) > 0.5f) ? 1 : 0);//1 = move clockwise / right

                g.Friction = 0.0f;//Make this less than the player so we can knockback easier

                g.HitSounds.Add(Res.SfxPlantBombGuyDamage);

                g.IdleSprite = Res.SprPlantBombDudeIdle;
                g.WalkAttackSprite = Res.SprPlantBombDudeAttack;
                g.SleepSprite = Res.SprPlantBombDudeSleep;
                g.DefendSprite = Res.SprPlantBombDudeCover;

                //MUST set this to null to prevent attack from not showing when we throw an item
                g.WalkSprite = Res.SprPlantBombDudeAttack;
                g.ItemHeldWalkSprite = Res.SprPlantBombDudeAttack;

                g.WakeRadiusPixels = 16 * 3;
                g.CanDefend = true;
                g.DefendRadiusPixels = 16 * 1.5f;
                g.AttackTime = g.MaxAttackTime = 1.0f;//This is more of an initial delay when the player encounters, because we throw new when the bomb dies

                return g;
            });

        }
        public void BuildTriggerObjLUT()
        {
            //Triggers
            for (int i = 0; i < 10; ++i)
            {
                int xx = i;
                ObjLUT.Add(Res.TriggerTileId0 + xx, (cell, ilayer) =>
                {
                    GameObject g = new GameObject(World);
                    g.Pos = cell.Pos();
                    g.Animate = false;
                    g.Visible = false;
                    g.Frame = null;
                    g.TileId = Res.TriggerTileId0 + xx;
                    return g;
                });
            }

        }
        private void BuildNPCObjLut()
        {
            ObjLUT.Add(Res.CapeGuyTileId, (cell, ilayer) =>
            {
                Guy g = new Guy(World, Res.SprCapeGuyWalk, AIState.Wander);
                g.Pos = cell.Pos();
                g.BoxRelative = new Box2f(-4, -6, 8, 14);
                g.Origin = new vec2(Res.Tiles.TileWidthPixels * 0.5f, Res.Tiles.TileHeightPixels * 0.5f);
                g.Animate = true;
                g.Power = 1;
                g.BlocksLight = false;
                g.EmitLight = false;
                g.Health = 2;
                g.Speed = 10.0f;
                g.MaxAcc = 30;
                g.Knockback = 20.0f;
                g.Gravity = World.Gravity;
                g.IsNpc = true;
                g.Friction = 8.0f;//Make this less than the player so we can knockback easier
                g.NpcDialog = new List<List<string>>() {
                    new List<string>() { "Do you think we should get the bow or the bomb first?", "...","I don't get out much." }
                };
                g.NpcName = "DAN";

                g.SetAllMotionSprites(Res.SprCapeGuyWalk);
                g.TalkSprite = Res.SprCapeGuyTalk;

                return g;
            });
        }
        public void BuildSpecialItemLUT()
        {
            SpecialItem se = null;

            SpecialItemLUT = new Dictionary<int, SpecialItem>();
            SpecialItemLUT.Add(Res.BootsTileId, new SpecialItem(
                "Spring Boots",
                new List<string>() {
                    "*SPRING *BOOTS",
                    "Spring boots let Joe jump twice as high. ",
                    "Hold *jump before hitting the ground to spring into the air."
                }, Res.BootsTileId, Res.Tiles.GetSprite(Res.SprBoots), SpecialItemChestType.Normal_Big, true));
            SpecialItemLUT.Add(Res.SwordItemTileId, new SpecialItem(
                "KiranSword",
                new List<string>() {
                    "*Kiran *Sword",
                    "The sword of legend warrior Kiran.  Holding it makes you feel the power of old." +
                    "Click left Mouse button to use.",
                }, Res.SwordItemTileId, Res.Tiles.GetSprite(Res.SprSwordItem), SpecialItemChestType.Plinth_Sword, true));
            SpecialItemLUT.Add(Res.ShieldItemTileId, new SpecialItem(
                "KiranShield",
                new List<string>() {
                    "*Kiran *Shield",
                    "The shield of the sage warrior." ,
                    "Hold the Right Mouse button to block.",
                }, Res.ShieldItemTileId, Res.Tiles.GetSprite(Res.SprShield), SpecialItemChestType.Plinth_Shield, true));
            SpecialItemLUT.Add(Res.BowItemTileId, new SpecialItem(
                "DracBow",
                new List<string>() {
                                "*Drac *Bow",
                                "The bow of the Drac warrior." ,
                                "Click and hold left mouse button to use.",
                                "Aim with the mouse cursor.",
                }, Res.BowItemTileId, Res.Tiles.GetSprite(Res.SprBow), SpecialItemChestType.Plinth_Bow, true));

            SpecialItemLUT.Add(Res.BombTileId, new SpecialItem(
                "Bomb",
                new List<string>() {
                    "*BOMB",
                    "Bombs blast through rock too tough for a pickaxe. ",
                    "Press E to throw a bomb.",
                    "Caution! Bombs are dangerous."
                }, Res.BombTileId, Res.Tiles.GetSprite(Res.SprBomb), SpecialItemChestType.Normal_Big, true));


            se = new SpecialItem(
                "Power Sword",
                new List<string>() {
                    "*POWER *SWORD",
                    "Power sword releases energy waves that attack from a distance. ",
                    "Hold the attack button to charge your sword.",
                    "Release the attack button to fire.",
                }, Res.PowerSwordTileId, Res.Tiles.GetSprite(Res.SprPowerSwordItem), SpecialItemChestType.Normal_Big, false);
            se.AfterInfoDialogClosed = (World w, float dt) =>
            {
                //Extinguish all torches so player uses powersword to navigate through area.
                foreach (GameObject ob_torch in w.Level.GameObjects)
                {
                    if (ob_torch.TileId == Res.TorchTileId)
                    {
                        ob_torch.EmitLight = false;
                        ob_torch.SetSprite(Res.SprTorchOut);
                    }
                }
                Res.Audio.PlaySound(Res.SfxTorchout);
                return false;
            };

            SpecialItemLUT.Add(Res.PowerSwordTileId, se);

            //Sub-Special items
            se = new SpecialItem(
                "Bomb_Powerup",
                new List<string>() { "Bomb capacity increased by 1" }, Res.BombPowerupTileId,
                Res.Tiles.GetSprite(Res.SprBomb), SpecialItemChestType.Normal_Big, false);
            se.CutsceneType = CutsceneType.Powerup;
            SpecialItemLUT.Add(Res.BombPowerupTileId, se);

            se = new SpecialItem("Small Key",
                    new List<string>() { "You found a Small key." }, Res.SmallKey_TileId,
                    Res.Tiles.GetSprite(Res.SprSmallKey), SpecialItemChestType.Normal_Small, false);
            se.CutsceneType = CutsceneType.Powerup;
            SpecialItemLUT.Add(Res.SmallKey_TileId, se);


        }
        public void BuildTileLUT()
        {
            if (TileLUT != null)
            {
                return;
            }

            TileLUT = new Dictionary<int, Tile>();

            vec2 curTilePos = new vec2(0, 0);
            vec2 curTileWH = new vec2(0, 0);

            //Black - Nogo / Border
            AddTileToLut(Res.NoGoTileId, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprBlackNogo),
                curTilePos, curTileWH,
                30, (cell, iLayer, tb) => { })
            {
                CanMine = false,
                BlockType = BlockType.None,
                BlocksLight = true,
                Blocking = true,
                CanBomb = false
            });


            AddTileToLut(Res.TreeTileId, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprTreeTiles),
                curTilePos, curTileWH,
                30, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.Wood, BlocksLight = false, Blocking = false, CanBomb = true });



            AddTileToLut(Res.CaveVineTileId, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprCaveVineTiles),
                curTilePos, curTileWH,
                5, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.Vines, BlocksLight = false, Blocking = false, CanBomb = true });
            AddTileToLut(Res.BlockTileID_CaveBack,
                new Tile(World, Res.Tiles.GetSprite(Res.SprCaveBackgroundBlockTiles),
                curTilePos, curTileWH, 0, (cell, iLayer, tb) => { })
                { CanMine = false, BlockType = BlockType.None });
            AddTileToLut(Res.BlockTileID_WoodBackground,
                new Tile(World, Res.Tiles.GetSprite(Res.SprWoodBackgroundBlockTiles),
                curTilePos, curTileWH, 0, (cell, iLayer, tb) => { })
                { CanMine = false, BlockType = BlockType.None });

            AddTileToLut(Res.SandRockBackTileId, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprSandRockBack),
                curTilePos, curTileWH,
                0, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None });



            //AddTileToLut(Res.MonsterGrassTileId, new Tile(
            //    this.World,
            //    Res.Tiles.GetSprite(Res.SprGrassMonsterTiles),
            //    curTilePos, curTileWH,
            //    2, (cell, iLayer, tb) => { })
            //{ CanMine = true, BlockType = BlockType.MonsterGrass, BlocksLight = false, CanBomb = true });

            AddTileToLut(Res.SeaweedTileId, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprSeaweed),
                curTilePos, curTileWH,
                2, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.MonsterGrass, BlocksLight = false, CanBomb = true });


            int ladderWidth = 5;//5 pixels ladder
            AddTileToLut(Res.LadderTileId_R, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprLadderTiles),
                curTilePos, curTileWH,
                0, (cell, iLayer, tb) =>
                {
                    tb.SpriteEffects = SpriteEffects.FlipHorizontally;
                    tb.Box = new Box2f(tb.Box.Min + new vec2(tb.Box.Width() - ladderWidth, 0), tb.Box.Max);
                })
            { CanMine = false, BlockType = BlockType.None, CanClimb = true, BlocksLight = false });
            AddTileToLut(Res.LadderTileId_L, new Tile(
                this.World,
                Res.Tiles.GetSprite(Res.SprLadderTiles),
                curTilePos, curTileWH,
                0, (cell, iLayer, tb) =>
                {
                    tb.SpriteEffects = SpriteEffects.None;
                    tb.Box = new Box2f(tb.Box.Min, tb.Box.Max - new vec2(ladderWidth, 0));
                })
            { CanMine = false, BlockType = BlockType.None, CanClimb = true, BlocksLight = false });



            AddTileToLut(Res.BlockTileId_Rock, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_Rock),
                curTilePos, curTileWH, 10, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.Stone, CanBomb = true });

            AddTileToLut(Res.BlockTileId_Hedge, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_Hedge),
                curTilePos, curTileWH, 10, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.Hedge, CanBomb = true });

            AddTileToLut(Res.BlockTileId_Copper, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_Copper),
                curTilePos, curTileWH, 30, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.Stone, CanBomb = true });

            AddTileToLut(Res.BlockTileId_SandRock, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_Sandrock),
                curTilePos, curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.Stone });
            AddTileToLut(Res.BlockTileId_GrassDirt, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_GrassDirt),
                curTilePos, curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.Stone });
            AddTileToLut(Res.BlockTileId_Obsidian, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_Obsidian),
                curTilePos, curTileWH, 5, (cell, iLayer, tb) => { })
            { CanMine = true, BlockType = BlockType.Stone, CanBomb = true });
            //**Necroid Rock = The main thing to bomb
            AddTileToLut(Res.BlockTileId_GreenDot, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_GreenDot),
                curTilePos, curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.Stone, CanBomb = true });

            AddTileToLut(Res.BlockTileId_WaterGrass, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_WaterGrass), curTilePos,
                curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None });
            AddTileToLut(Res.BlockTileId_WaterGrassBack, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_WaterGrassBack), curTilePos,
                curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None });

            AddTileToLut(Res.BlockTileId_Crag, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_Crag), curTilePos,
                curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None });


            AddTileToLut(Res.BlockTileID_DirtBackground, new Tile(World, Res.Tiles.GetSprite(Res.SprBlock_DirtBackground), curTilePos,
                curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None });


            //AddTileToLut(Res.MineshaftExitTileId, new Tile(World, Res.Tiles.GetSprite(Res.SprMineshaftExit),
            //    curTilePos, curTileWH, 10, (cell, iLayer, tb) => { })
            //{ CanMine = false, BlockType = BlockType.None, Blocking = false, BlocksLight = false });

            //Decals need to be objects, not tiles.  Because they need to emit light sometimes
            AddDecalTileGroup(Res.GlowFlowerTileId, true, true, BlockType.MonsterGrass,
                new List<string> { Res.SprGlowFlower0, Res.SprGlowFlower1 }, true, new vec4(1, 1, 1, 1), 4 * Res.Tiles.TileWidthPixels);
            AddDecalTileGroup(Res.RockTileId, true, true, BlockType.StoneDecal,
                new List<string> { Res.SprRock0, Res.SprRock1, Res.SprRock2 });
            AddDecalTileGroup(Res.MushroomTileId, true, true, BlockType.StoneDecal,
                new List<string> { Res.SprShroom0, Res.SprShroom1, Res.SprShroom2 });
            AddDecalTileGroup(Res.FlowerTileId, true, true, BlockType.MonsterGrass,
                new List<string> { Res.SprFlower0, Res.SprFlower1, Res.SprFlower2, Res.SprFlower3 });
            AddDecalTileGroup(Res.MonsterGrassTileId, true, true, BlockType.MonsterGrass,
                new List<string> { Res.SprGrass0, Res.SprGrass1, Res.SprGrass2, Res.SprGrass3 });

            AddTileToLut(Res.Sky_Level0, new Tile(World, Res.Tiles.GetSprite(Res.Spr_Sky_Level0), curTilePos,
                curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None, BlocksLight = false });
            AddTileToLut(Res.Sky_Level1, new Tile(World, Res.Tiles.GetSprite(Res.Spr_Sky_Level1), curTilePos,
    curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None, BlocksLight = false });
            AddTileToLut(Res.Sky_Level2, new Tile(World, Res.Tiles.GetSprite(Res.Spr_Sky_Level2), curTilePos,
curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None, BlocksLight = false });
            AddTileToLut(Res.Sky_Level3, new Tile(World, Res.Tiles.GetSprite(Res.Spr_Sky_Level3), curTilePos,
curTileWH, 100, (cell, iLayer, tb) => { })
            { CanMine = false, BlockType = BlockType.None, BlocksLight = false });

            AddDecalTileGroup(Res.Tree_Doodad, true, true, BlockType.MonsterGrass,
    new List<string> { Res.SprTree_Doodad, Res.SprGrass_Doodad1, Res.SprGrass_Doodad2 });
            AddDecalTileGroup(Res.Grass_Doodad1, true, true, BlockType.MonsterGrass,
    new List<string> { Res.SprTree_Doodad, Res.SprGrass_Doodad1, Res.SprGrass_Doodad2 });
            AddDecalTileGroup(Res.Grass_Doodad2, true, true, BlockType.MonsterGrass,
new List<string> { Res.SprTree_Doodad, Res.SprGrass_Doodad1, Res.SprGrass_Doodad2 });

            AddDecalTileGroup(Res.Star_0, true, true, BlockType.MonsterGrass,
new List<string> { Res.SprParticleSmall });


        }

        private void AddSign(string level, int signTileId, List<string> text)
        {
            Dictionary<string, List<string>> ret = null;

            if (!SignLUT.TryGetValue(signTileId, out ret))
            {
                SignLUT.Add(signTileId, new Dictionary<string, List<string>>());
                SignLUT.TryGetValue(signTileId, out ret);
            }

            List<string> strings = null;
            if (ret.TryGetValue(level, out strings))
            {
                //you already added it.
                System.Diagnostics.Debugger.Break();
            }

            ret.Add(level, text);
        }

        private void AddDecalTileGroup(int tileid, bool canmine, bool canbomb, BlockType bt, List<string> sprites, bool emit = false, vec4 emitColor = default(vec4), int emitRadius = 0)
        {
            Tile t = new Tile(World, null, new vec2(0, 0), new vec2(0, 0), 5,
                (cell, iLayer, tb) =>
                {
                    int nn = 0;
                    nn++;
                })
            {
                CanMine = canmine,
                BlockType = bt,
                RandomSprites = sprites,
                EmitLight = emit,
                EmitColor = emitColor,
                EmitRadiusInPixels = emitRadius,
                CanBomb = canbomb,
                Health = 1,
                MaxHealth = 1
            };
            AddTileToLut(tileid, t);

        }
        private void AddTileToLut(int id, Tile t)
        {
            t.TileId = id;
            TileLUT.Add(id, t);
        }
    }
    public class TileMap
    {
        public int MapWidthTiles { get; private set; }
        public int MapHeightTiles { get; private set; }

        public string LevelName { get; set; }
        public List<List<List<int>>> GenTiles;
        public ivec2 PlayerStartXY = new ivec2(Int32.MaxValue, Int32.MaxValue);

        public TileMap(int width, int height)
        {
            MapWidthTiles = width;
            MapHeightTiles = height;
            PlayerStartXY = new ivec2(Int32.MaxValue, Int32.MaxValue);
            InitGenTileGrid();
        }

        public TileMap(string level_name)
        {
            LevelName = level_name;
            var map = new TmxMap("Content\\" + level_name + ".tmx");

            MapWidthTiles = map.Width;
            MapHeightTiles = map.Height;

            //Create teh world data
            PlayerStartXY = new ivec2(Int32.MaxValue, Int32.MaxValue);
            InitGenTileGrid();
            ParseTmxMap(map);
        }
        public void InitGenTileGrid()
        {
            //Create an empty tile map.
            GenTiles = new List<List<List<int>>>();
            for (int iRow = 0; iRow < MapHeightTiles; ++iRow)
            {
                GenTiles.Add(new List<List<int>>());

                for (int iCol = 0; iCol < MapWidthTiles; ++iCol)
                {
                    List<int> layers = new List<int>();
                    for (int iLayer = 0; iLayer < PlatformLevel.LayerCount; ++iLayer)
                    {
                        layers.Add(PlatformLevel.EMPTY_TILE);
                    }
                    GenTiles[iRow].Add(layers);// 3 layers **0 is out of bounds** so -1 is unset/null
                }
            }
        }
        public void ParseTmxMap(TmxMap map)
        {
            var version = map.Version;


            List<int> KeyTiles = new List<int>
            {
                Res.SlopeTile_BL               ,
                Res.SlopeTile_BR               ,
                Res.SlopeTile_TL               ,
                Res.SlopeTile_TR               ,
                Res.BorderTileId               ,
                Res.Sun_20Percent              ,
                Res.Sun_5Percent               ,
                Res.SavePointTileId            ,
                Res.Bombable_Tile_TileId       ,
                Res.FallThrough_Tile_TileId    ,
                Res.Water100TileId             ,
                Res.Water50TileId              ,
                Res.Lava100TileId              ,
                Res.Lava50TileId               ,
                Res.SavePointTileId,
                Res.SwitchButtonTileId  ,
                Res.SwitchDoorTileId   ,
                Res.SwitchConduitTileId,
                Res.Tar80TileId               ,
            };

            foreach (TmxLayer layer in map.Layers)
            {
                int layerId = -1;

                if (layer.Name.Equals("Foreground")) { layerId = PlatformLevel.Foreground; }
                else if (layer.Name.Equals("Midground")) { layerId = PlatformLevel.Midground; }
                else if (layer.Name.Equals("Midback")) { layerId = PlatformLevel.Midback; }
                else if (layer.Name.Equals("Background")) { layerId = PlatformLevel.Background; }
                else if (layer.Name.Equals("Liquid")) { layerId = PlatformLevel.Liquid; }
                else if (layer.Name.Equals("Conduit")) { layerId = PlatformLevel.Conduit; }

                if (layerId == -1)
                {
                    System.Diagnostics.Debugger.Break();
                }
                else
                {

                    foreach (TmxLayerTile tile in layer.Tiles)
                    {
                        if (tile.Gid == Res.GuyTileId)
                        {
                            //here is our start point, flood fill this area.
                            PlayerStartXY.x = tile.X;
                            PlayerStartXY.y = tile.Y;
                        }

                        //Set to empty if we're not presetn.  Most tiles are 0, we use -1 for empty
                        int val = tile.Gid;
                        if (tile.Gid == 0)
                        {
                            val = PlatformLevel.EMPTY_TILE;
                        }
                        else if (TileDefs.TileLUT.ContainsKey(tile.Gid))
                        {
                        }
                        else if (KeyTiles.Contains(tile.Gid))
                        {
                        }
                        else if (TileDefs.DoorTilesLUT.Contains(tile.Gid))
                        {
                        }
                        else if (TileDefs.ObjLUT.ContainsKey(tile.Gid))
                        {
                        }
                        //else if (TileDefs.SpecialItemLUT.ContainsKey(tile.Gid))
                        //{
                        //}
                        //else if (TileDefs.SignLUT.ContainsKey(tile.Gid))
                        //{
                        //}
                        else
                        {
                            val = PlatformLevel.EMPTY_TILE;
                        }
                        TrySetGenTile(tile.X, tile.Y, layerId, val);
                    }
                }

            }

        }
        public int TileXY(ivec2 v, int layer)
        {
            return TileXY(v.x, v.y, layer);
        }
        public int TileXY(int col, int row, int layer)
        {
            //**RETURN 0 FOR OUT OF BOUNDS
            if (row >= GenTiles.Count || row < 0)
            {
                return 0;
            }
            if (col >= GenTiles[row].Count || col < 0)
            {
                return 0;
            }
            if (layer >= GenTiles[row][col].Count)
            {
                return 0;
            }
            return GenTiles[row][col][layer];
        }
        public void TrySetGenTile(int iCol, int iRow, int iLayer, int iTile)
        {
            if (iRow < 0 || iRow >= MapHeightTiles)
            {
                return;
            }
            if (iCol < 0 || iCol >= MapWidthTiles)
            {
                return;
            }

            GenTiles[iRow][iCol][iLayer] = iTile;//already set, but debug here
        }
    }
    public class PlatformLevel
    {
        public vec4 GlobalLight { get; private set; } //Global light value set with the Sun 10% object.

        public static int Background = 0;
        public static int Midback = 1;
        public static int Midground = 2;
        public static int Foreground = 3;
        public static int Liquid = 4;
        public static int Conduit = 5;
        public static int LayerCount = 6;
        public static int EMPTY_TILE = -1;

        public Guy MainGuy = null;  //This is set initially and persists.
        public World World { get; private set; }
        public TileGrid Grid;
        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
        public List<GameObject> Projectiles { get; private set; } = new List<GameObject>();
        public TileMap GenTiles;
        public Room Room { get; private set; }

        int NumFloodFill = 0;

        public PlatformLevel(World world, TileMap gt)
        {
            World = world;

            //Load the Map Data
            this.GenTiles = gt;

            // this.Grid = tg;
            MakeRoom(this.GenTiles.PlayerStartXY);
        }
        //  byte[] Data = new byte[];
        public class SaveTile
        {
            //The object may have moved, bu tthis is the ORIGINAL position int he TmX map  it's just the key
            public ivec2 TilePos;//THis is the global xy of the cell reltaive to the TMX map
            public int Layer;//Layer ID
            public int ClassId;
            public byte[] Data; // Bytes
        }
        public int GetClassId(GameObject ob)
        {
            if (ob is Door) return 1;
            if (ob is TreasureChest) return 2;
            //  if (ob is Player) return 3; // Do not save player - save it separately.
            if (ob is ButtonSwitch) return 4;

            // System.Diagnostics.Debugger.Break();
            // if (ob is SpecialItem) return 4;
            return 0;
        }
        List<SaveTile> SaveTiles = new List<SaveTile>();
        public void SerializeLevel()
        {
            //loop obs
            //find the ob by the cell/level key 
            //if not present create new, else dump old, replace
            //all objects are id by grid_x, grid_y, layer
            foreach (GameObject ob in GameObjects)
            {
                int id = GetClassId(ob);

                //Objects of id 0 are not saved/serialized.  TO save them add them to GetClassID
                if (id > 0)
                {
                    if (ob.SaveLayer == -1)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                    {
                        SaveTile t = SaveTiles.Find(x =>
                            x.TilePos.x == ob.SavePos.x
                            && x.TilePos.y == ob.SavePos.y
                            && x.Layer == ob.SaveLayer);
                        if (t == null)
                        {
                            t = new SaveTile();
                            t.TilePos = ob.SavePos;
                            t.Layer = ob.SaveLayer;
                            t.ClassId = GetClassId(ob);
                            SaveTiles.Add(t);
                        }

                        ob.Serialize(writer);
                        //https://stackoverflow.com/questions/7528140/binary-writer-returns-byte-array-of-null
                        t.Data = (writer.BaseStream as MemoryStream).ToArray();
                    }
                }

            }
        }
        public bool SaveGame(string file)
        {
            try
            {
                SerializeLevel();

                //Save all tile blocks
                byte[] data = null;
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    writer.Write((Int32)SaveTiles.Count);//int
                                                         //Convert to bytes
                    foreach (SaveTile t in SaveTiles)
                    {
                        writer.Write(t.TilePos.x);
                        writer.Write(t.TilePos.y);
                        writer.Write(t.Layer);
                        writer.Write(t.Data.Length);
                        writer.Write(t.Data);
                    }
                    data = (writer.BaseStream as MemoryStream).GetBuffer();
                }

                System.IO.File.WriteAllBytes(file, data);
            }
            catch (Exception ex)
            {
                Globals.IgnoreException(ex);
                return false;
            }
            return true;
        }
        public void GoThroughDoor(Door door)
        {
            //Go to the other side of this door.

            //Get position of the other side of this doorway.
            int dx = 0;
            int dy = 0;

            if (door.Dir == Dir.L) { dx = 1; dy = 0; }
            if (door.Dir == Dir.R) { dx = -1; dy = 0; }
            if (door.Dir == Dir.B) { dx = 0; dy = 1; }
            if (door.Dir == Dir.T) { dx = 0; dy = -1; }

            ivec2 guy_pos = door.OrigTilePos;
            guy_pos.x += dx;
            guy_pos.y += dy;

            //Set the last player pos
            int isPLayer = this.GenTiles.TileXY(this.GenTiles.PlayerStartXY, Midground);
            if (isPLayer != Res.GuyTileId)
            {
                //**Error - the player's tile has changed.
                System.Diagnostics.Debugger.Break();
            }
            this.GenTiles.TrySetGenTile(this.GenTiles.PlayerStartXY.x, this.GenTiles.PlayerStartXY.y, Midground, EMPTY_TILE);
            this.GenTiles.PlayerStartXY = guy_pos;
            this.GenTiles.TrySetGenTile(this.GenTiles.PlayerStartXY.x, this.GenTiles.PlayerStartXY.y, Midground, Res.GuyTileId);

            MakeRoom(guy_pos);
        }
        private void MakeRoom(ivec2 startxy)
        {
            if (startxy.x != Int32.MaxValue)
            {
                //Cleanup
                Room = null;
                Grid = null;
                GameObjects = new List<GameObject>();
                GC.Collect();

                //Find the boundary x/y of the map so we can make a grid
                Room = new Room();
                NumFloodFill = 0;

                FloodFillFromPointRecursive(startxy, Room);

                Room.Validate();

                //Make the grid
                Grid = new TileGrid(this, Room.WidthTiles, Room.HeightTiles, LayerCount);

                //MUST COME BEFORE GENERATE GAME OBJECTS
                GenerateDoors();

                //Generate objects  / Doors
                GenerateWorldObjects();

                MakeWallTorches();


            }
            else
            {
                //YOU DIDNT SET THE PLAYER ANYWHERE
                //Failed to find the guy tile.
                System.Diagnostics.Debugger.Break();
            }
        }
        private void MakeWallTorches()
        {
            //Make torches on wall
            //must come after the main terrain generation since most torches we create are on the midback wall (before midground)
            //Also applies an offset to torches because most walls don't actually hit tile border.
            foreach (GameObject ob in GameObjects)
            {
                if (ob.TileId == Res.TorchTileId)
                {
                    if (ob.CreatedCell != null)
                    {
                        Cell l = Grid.GetNeighborCell(ob.CreatedCell, -1, 0);
                        Cell r = Grid.GetNeighborCell(ob.CreatedCell, 1, 0);

                        bool L = (l != null) && (l.Layers[Midground] != null) && (l.Layers[Midground].Blocking = true) && (l.Layers[Midground].IsSlope() == false);
                        bool R = (r != null) && (r.Layers[Midground] != null) && (r.Layers[Midground].Blocking = true) && (r.Layers[Midground].IsSlope() == false);

                        vec2 off = new vec2(0, -4);
                        if (L || R)
                        {
                            ob.SetSprite(Res.SprTorchWall);
                        }

                        if (L && R)
                        {
                            //Flip horizontally random if both sides
                            if (Globals.RandomBool())
                            {
                                ob.SpriteEffects = SpriteEffects.FlipHorizontally;
                                off.x = 4;
                            }
                            else
                            {
                                off.x = -4;
                            }
                        }
                        else if (R)
                        {
                            ob.SpriteEffects = SpriteEffects.FlipHorizontally;
                            off.x = 4;
                        }
                        else if (L)
                        {
                            off.x = -4;
                        }


                        ob.Pos += off;
                    }
                }
            }


        }
        public GameObject GetTriggerById(int triggerid)
        {
            GameObject trigger = GameObjects.Find(x => x.TileId == triggerid);
            return trigger;
        }
        public void GenerateDoors()
        {
            foreach (ivec2 v in Room.Doors)
            {
                //Check left right top bot

                //Check to see if the door is a portal door, if border on left/r or top/bot. otherwise it's just a normal door.
                //6/23 this check didn't work because the corner border cells aren't included in the flood fill *included them
                bool LR = false, TB = false;
                LR = Room.Border.Contains(v + new ivec2(0, -1)) && Room.Border.Contains(v + new ivec2(0, 1));
                TB = Room.Border.Contains(v + new ivec2(-1, 0)) && Room.Border.Contains(v + new ivec2(1, 0));
                bool bIsPortal = (LR == true || TB == true);

                Door made = null;

                if (bIsPortal == false)
                {
                    int n = 0;
                    n++;
                }

                made = CheckMakeDoor(v, Dir.L, new ivec2(-1, 0), bIsPortal);
                if (made == null)
                {
                    made = CheckMakeDoor(v, Dir.R, new ivec2(1, 0), bIsPortal);
                    if (made == null)
                    {
                        made = CheckMakeDoor(v, Dir.T, new ivec2(0, -1), bIsPortal);
                        if (made == null)
                        {
                            made = CheckMakeDoor(v, Dir.B, new ivec2(0, 1), bIsPortal);
                            if (made == null)
                            {
                                System.Diagnostics.Debugger.Break();
                            }
                        }
                    }
                }

            }

        }
        public SaveTile GetSaveTile(ivec2 cell_pos_global, int layer)
        {
            SaveTile t = SaveTiles.Find(x =>
                x.TilePos.x == cell_pos_global.x
                && x.TilePos.y == cell_pos_global.y
                && x.Layer == layer
                );
            return t;
        }
        public Door CheckMakeDoor(ivec2 door_pos, Dir dir, ivec2 check_pos, bool bIsPortal)
        {
            Door d = null;
            ivec2 v_n = door_pos + check_pos;
            if (Room.Found.Contains(v_n))
            {
                int door = this.GenTiles.TileXY(door_pos.x, door_pos.y, Midground);
                //Dir = L, R B T
                if (TileDefs.DoorTilesLUT.Contains(door))
                {
                    ivec2 viOrig = new ivec2(door_pos.x - Room.Min.x, door_pos.y - Room.Min.y);
                    Cell check_cell = Grid.GetCell(viOrig + check_pos);

                    //SaveData. 
                    d = new Door(World);
                    d.OrigTilePos = door_pos;
                    d.TileId = door;
                    d.Pos = check_cell.Pos();
                    d.Dir = dir;
                    d.IsPortalDoor = bIsPortal;

                    //Doors only on midground
                    //Pass the original cell so we can save the door
                    AddGameObject(d, door, check_cell, Midground, door_pos, Midground);
                }
            }

            return d;
        }

        public void FloodFillFromPointRecursive(ivec2 pt_origin, Room room)
        {
            //Flood fill an area demarcated by the boundary.
            List<ivec2> toCheck = new List<ivec2>();
            toCheck.Add(pt_origin);

            if (pt_origin.x < 0 || pt_origin.y < 0 || pt_origin.x > this.GenTiles.MapWidthTiles || pt_origin.y > this.GenTiles.MapHeightTiles)
            {
                //Player is outside room, flood fill can't work.
                System.Diagnostics.Debugger.Break();
                return;
            }

            while (toCheck.Count > 0)
            {
                NumFloodFill++;//degbug
                ivec2 pt = toCheck[toCheck.Count - 1];
                toCheck.RemoveAt(toCheck.Count - 1);

                if (pt.x < 0 || pt.y < 0 || pt.x > this.GenTiles.MapWidthTiles || pt.y > this.GenTiles.MapHeightTiles)
                {
                    continue;
                }

                int iTile = this.GenTiles.TileXY(pt.x, pt.y, Midground);

                if (room.Found.Contains(pt))
                {
                    int n = 0;
                    n++;
                }
                else if (iTile == Res.BorderTileId)
                {
                    if (!room.Border.Contains(pt))
                    {
                        room.Border.Add(pt);

                        //Include Corner border tiles.
                        //This is needed to see if a door that lies on a border corner is a portal door
                        //otherwise we wouldn't include cornder borders in the flood fill
                        FloodFillAddNeighborBorder(pt + new ivec2(-1, 0), room.Border);
                        FloodFillAddNeighborBorder(pt + new ivec2(1, 0), room.Border);
                        FloodFillAddNeighborBorder(pt + new ivec2(0, -1), room.Border);
                        FloodFillAddNeighborBorder(pt + new ivec2(0, 1), room.Border);
                    }
                }
                else if (TileDefs.DoorTilesLUT.IndexOf(iTile) >= 0)
                {
                    ///So the TODO here is to be able to figure out which side of the border the door is on
                    if (!room.Border.Contains(pt))
                    {
                        room.Doors.Add(pt);
                    }
                }
                else
                {
                    room.Found.Add(pt);

                    if (pt.x < room.Min.x) { room.Min.x = pt.x; }
                    if (pt.y < room.Min.y) { room.Min.y = pt.y; }
                    if (pt.x > room.Max.x) { room.Max.x = pt.x; }
                    if (pt.y > room.Max.y) { room.Max.y = pt.y; }

                    //Were not a border.
                    toCheck.Add(pt + new ivec2(-1, 0));
                    toCheck.Add(pt + new ivec2(1, 0));
                    toCheck.Add(pt + new ivec2(0, -1));
                    toCheck.Add(pt + new ivec2(0, 1));
                }


            }


        }
        public void FloodFillAddNeighborBorder(ivec2 v, HashSet<ivec2> border)
        {
            if (this.GenTiles.TileXY(v.x, v.y, Midground) == Res.BorderTileId)
            {
                if (!border.Contains(v))
                {
                    border.Add(v);
                }
            }
        }

        public Tile GetTile(int tileId)
        {
            Tile tile = null;
            TileDefs.TileLUT.TryGetValue(tileId, out tile);

            return tile;
        }
        public void DestroyTile(Cell c, int iLayer)
        {
            //Note : GLOBAL tile pos
            ivec2 tilepos = c.GetTilePosGlobal();

            this.GenTiles.TrySetGenTile(tilepos.x, tilepos.y, iLayer, EMPTY_TILE);
            c.Layers[iLayer] = null;

            RepairSurroundingTiles(c, tilepos.x, tilepos.y, iLayer);

        }
        private void RepairSurroundingTiles(Cell center, int gridx_global, int gridy_global, int iLayer)
        {
            //Repair tiled sprites if we destroyed them.

            for (int j = -1; j <= 1; ++j)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    int iCol = gridx_global + i;
                    int iRow = gridy_global + j;

                    Cell neighbor = Grid.GetNeighborCell(center, i, j);
                    if (neighbor != null)
                    {
                        if (neighbor.Layers[iLayer] != null && neighbor.Layers[iLayer].Tile != null)
                        {
                            // Sprite neighborSprite = neighbor.Layers[iLayer].Tile.Sprite;
                            Tile neighborTile = neighbor.Layers[iLayer].Tile;
                            if (neighborTile != null)
                            {
                                int id = this.GenTiles.TileXY(iCol, iRow, iLayer);
                                int iFrame = 0;

                                if (id == Res.SlopeTile_BR)
                                {
                                    iFrame = 50;
                                }
                                else if (id == Res.SlopeTile_BL)
                                {
                                    iFrame = 51;
                                }
                                else if (id == Res.SlopeTile_TR)
                                {
                                    iFrame = 52;
                                }
                                else if (id == Res.SlopeTile_TL)
                                {
                                    iFrame = 53;
                                }
                                else
                                {
                                    iFrame = GetSpriteTileFrame(iCol, iRow, iLayer, neighborTile);
                                }

                                neighbor.Layers[iLayer].FrameIndex = iFrame;
                            }
                        }

                    }


                }
            }
        }
        public void GenerateWorldObjects()
        {
            //we have doors in our set, do not reset gameobjects
            //gentilesprites

            vec2 curTileWH = Res.Tiles.GetWHVec();
            GlobalLight = new vec4(0, 0, 0, 1);

            //We add +1 here to include the outside border which isn't included in the parse routine
            for (int iRow = 0; iRow < Room.HeightTiles; ++iRow)
            {
                for (int iCol = 0; iCol < Room.WidthTiles; ++iCol)
                {
                    vec2 curTilePos = new vec2(iCol * Res.Tiles.TileWidthPixels, iRow * Res.Tiles.TileWidthPixels);
                    Cell cell = Grid.GetCellForPoint(curTilePos + new vec2(Res.Tiles.TileWidthPixels * 0.5f, Res.Tiles.TileHeightPixels * 0.5f));
                    if (cell == null)
                    {
                        //Error - this shouldn't be null
                        System.Diagnostics.Debugger.Break();
                        return;
                    }

                    //Translate row/col to the room area.
                    int RoomCol = Room.Min.x + iCol;
                    int RoomRow = Room.Min.y + iRow;

                    ivec2 rcpos = new ivec2(RoomCol, RoomRow);

                    bool bIsNoGo = CheckIsNogoTile(rcpos);


                    if (bIsNoGo == false)
                    {
                        for (int iLayer = 0; iLayer < LayerCount; ++iLayer)
                        {
                            int iTileId = this.GenTiles.TileXY(RoomCol, RoomRow, iLayer);

                            if (iTileId != EMPTY_TILE)
                            {
                                Tile tile = null;
                                Func<Cell, int, GameObject> obCreate = null;
                                SpecialItem si = null;

                                if (iTileId == Res.SwitchConduitTileId)
                                {
                                    cell.Layers[Conduit] = new TileBlock();
                                    cell.Layers[Conduit].IsConduit = true;
                                }
                                else if (iTileId == Res.Sun_5Percent)
                                {
                                    GlobalLight += new vec4(0.05f, 0.05f, 0.05f, 0.05f);
                                }
                                else if (iTileId == Res.Sun_20Percent)
                                {
                                    GlobalLight += new vec4(0.2f, 0.2f, 0.2f, 0.0f);
                                }
                                else if (iTileId == Res.GuyTileId)
                                {
                                    //Separate guy object so we
                                    //Keep the main guy between levels
                                    if (MainGuy == null)
                                    {
                                        TileDefs.ObjLUT.TryGetValue(iTileId, out obCreate);
                                        GameObject ob = obCreate(cell, iLayer);
                                        MainGuy = (ob as Guy);
                                    }

                                    //Critical that we actually change the player's location
                                    MainGuy.Pos = cell.Pos();

                                    if (MainGuy == null)
                                    {
                                        System.Diagnostics.Debugger.Break();
                                    }
                                    AddGameObject(MainGuy, iTileId, cell, iLayer, cell.GetTilePosGlobal(), iLayer);
                                }
                                else if (iTileId == Res.Bombable_Tile_TileId)
                                {
                                    //Set the Midground tile to be destructible
                                    if (cell.Layers[Midground] != null)
                                    {
                                        cell.Layers[Midground].CanBomb = true;
                                    }
                                    else
                                    {
                                        //Error - You either A) have a desrtuctible tile NOT on the foreground layer or
                                        // B) you don't have a midground tile here.
                                        //System.Diagnostics.Debugger.Break();
                                    }
                                }
                                else if (iTileId == Res.FallThrough_Tile_TileId)
                                {
                                    //Set the Midground tile to be destructible
                                    if (cell.Layers[Midground] != null)
                                    {
                                        cell.Layers[Midground].FallThrough = true;
                                    }
                                    else
                                    {
                                        //Error - You either A) have a desrtuctible tile NOT on the foreground layer or
                                        // B) you don't have a midground tile here.
                                        System.Diagnostics.Debugger.Break();
                                    }
                                }
                                else if (iTileId == Res.SlopeTile_BL ||
                                    iTileId == Res.SlopeTile_BR ||
                                    iTileId == Res.SlopeTile_TL ||
                                    iTileId == Res.SlopeTile_TR)
                                {
                                    //We call BuildSlopes after processing in order to do this

                                    tile = CreateSlopeTile(iTileId, RoomCol, RoomRow, iLayer);
                                    if (tile != null)
                                    {
                                        int iFrame = 0;
                                        if (iTileId == Res.SlopeTile_BR)
                                        {
                                            iFrame = 50;
                                        }
                                        else if (iTileId == Res.SlopeTile_BL)
                                        {
                                            iFrame = 51;
                                        }
                                        else if (iTileId == Res.SlopeTile_TR)
                                        {
                                            iFrame = 52;
                                        }
                                        else if (iTileId == Res.SlopeTile_TL)
                                        {
                                            iFrame = 53;
                                        }

                                        CreateTile(cell, tile, iFrame, iLayer, curTilePos, curTileWH);

                                        //Set the slope tile
                                        if (cell.Layers[iLayer] != null)
                                        {
                                            cell.Layers[iLayer].SlopeTileId = iTileId;
                                        }
                                    }
                                    else
                                    {
                                        //else we have an empty slope
                                    }

                                }
                                else if (TileDefs.TileLUT.TryGetValue(iTileId, out tile))
                                {
                                    int iFrame = GetSpriteTileFrame(RoomCol, RoomRow, iLayer, tile);
                                    CreateTile(cell, tile, iFrame, iLayer, curTilePos, curTileWH);
                                }
                                else if (TileDefs.ObjLUT.TryGetValue(iTileId, out obCreate))
                                {
                                    //The layer doesn't matter for GameObjects because they're always on the 
                                    GameObject ob = obCreate(cell, iLayer);
                                    AddGameObject(ob, iTileId, cell, iLayer, cell.GetTilePosGlobal(), iLayer);
                                }
                                //else if (TileDefs.SignLUT.TryGetValue(iTileId, out obSigns))
                                //{
                                //    List<string> texts = null;
                                //    if (obSigns.TryGetValue(this.GenTiles.LevelName, out texts))
                                //    {
                                //        Sign sign = new Sign(World, texts);
                                //        sign.Pos = cell.Pos();
                                //        sign.SetSprite(Res.SprSign);
                                //        AddGameObject(sign, iTileId, cell, iLayer, cell.GetTilePosGlobal(), iLayer);
                                //    }
                                //}
                                else if (TileDefs.SpecialItemLUT.TryGetValue(iTileId, out si))
                                {
                                    //Create a chest
                                    TreasureChest ob = new TreasureChest(World, "", cell.Pos());

                                    //Note: We shouldn't be using small chests for special item.
                                    int tilee = 0;
                                    if (si.SpecialItemChestType == SpecialItemChestType.Normal_Big)
                                    {
                                        ob.SetSprite(Res.SprBigChest);
                                    }
                                    else if (si.SpecialItemChestType == SpecialItemChestType.Gold_Big)
                                    {
                                        ob.SetSprite(Res.SprGoldChest);
                                    }
                                    else if (si.SpecialItemChestType == SpecialItemChestType.Normal_Small)
                                    {
                                        ob.SetSprite(Res.SprSmallChest);
                                    }
                                    else if (si.SpecialItemChestType == SpecialItemChestType.Silver_Small)
                                    {
                                        ob.SetSprite(Res.SprSilverSmallChest);
                                    }
                                    else if (si.SpecialItemChestType == SpecialItemChestType.Plinth_Shield)
                                    {
                                        ob.SetSprite(Res.SprPlinthShield);
                                    }
                                    else if (si.SpecialItemChestType == SpecialItemChestType.Plinth_Bow)
                                    {
                                        ob.SetSprite(Res.SprPlinthBow);
                                    }
                                    else if (si.SpecialItemChestType == SpecialItemChestType.Plinth_Sword)
                                    {
                                        ob.SetSprite(Res.SprPlinthSword);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debugger.Break();
                                    }

                                    ob.SpecialItem = si;

                                    AddGameObject(ob, tilee, cell, iLayer, cell.GetTilePosGlobal(), iLayer);
                                }
                                else if (iTileId == Res.Water100TileId)
                                {
                                    cell.Water = 1.0f;
                                    cell.WaterType = WaterType.Water;
                                }
                                else if (iTileId == Res.Water50TileId)
                                {
                                    cell.Water = 0.5f;
                                    cell.WaterType = WaterType.Water;
                                }
                                else if (iTileId == Res.Lava100TileId)
                                {
                                    cell.Water = 1.0f;
                                    cell.WaterType = WaterType.Lava;
                                }
                                else if (iTileId == Res.Lava50TileId)
                                {
                                    cell.Water = 0.5f;
                                    cell.WaterType = WaterType.Lava;
                                }
                                else if (iTileId == Res.Tar80TileId)
                                {
                                    cell.Water = 0.8f;
                                    cell.WaterType = WaterType.Tar;
                                }


                            }
                        }
                    }
                    else
                    {
                        //Create black tile
                        Tile tile = null;
                        TileDefs.TileLUT.TryGetValue(Res.NoGoTileId, out tile);
                        CreateTile(cell, tile, 0, Midground, curTilePos, curTileWH);
                    }

                }
            }


        }
        private bool CheckIsNogoTile(ivec2 rcpos)
        {
            bool bNogo = true;
            if (Room.Found.Contains(rcpos))
            {
                bNogo = false;
            }
            else
            {
                //Check to see that it's not a non-portal door.
                if (Room.Doors.Contains(rcpos))
                {
                    foreach (GameObject g in GameObjects)
                    {
                        Door d = g as Door;
                        if (d != null)
                        {
                            if (d.OrigTilePos.x == rcpos.x && d.OrigTilePos.y == rcpos.y)
                            {
                                if (d.IsPortalDoor == false)
                                {
                                    bNogo = false;
                                }
                            }
                        }
                    }
                }
            }

            return bNogo;
        }
        private Tile CreateSlopeTile(int tileid, int iCol, int iRow, int iLayer)
        {
            //Get the surrounding 4 tiles and return the first tileset that matches the slope
            Tile outTile = null;

            int top = this.GenTiles.TileXY(iCol, iRow - 1, iLayer);
            int bot = this.GenTiles.TileXY(iCol, iRow + 1, iLayer);
            int left = this.GenTiles.TileXY(iCol - 1, iRow, iLayer);
            int right = this.GenTiles.TileXY(iCol + 1, iRow, iLayer);

            if (tileid == Res.SlopeTile_BR)
            {
                if (CheckNeighborTileIs3x3(bot))
                {
                    TileDefs.TileLUT.TryGetValue(bot, out outTile);
                }
                else if (CheckNeighborTileIs3x3(right))
                {
                    TileDefs.TileLUT.TryGetValue(right, out outTile);
                }
            }
            else if (tileid == Res.SlopeTile_BL)
            {
                if (CheckNeighborTileIs3x3(bot))
                {
                    TileDefs.TileLUT.TryGetValue(bot, out outTile);
                }
                else if (CheckNeighborTileIs3x3(left))
                {
                    TileDefs.TileLUT.TryGetValue(left, out outTile);
                }
            }
            else if (tileid == Res.SlopeTile_TR)
            {
                if (CheckNeighborTileIs3x3(top))
                {
                    TileDefs.TileLUT.TryGetValue(top, out outTile);
                }
                else if (CheckNeighborTileIs3x3(right))
                {
                    TileDefs.TileLUT.TryGetValue(right, out outTile);
                }
            }
            else if (tileid == Res.SlopeTile_TL)
            {
                if (CheckNeighborTileIs3x3(top))
                {
                    TileDefs.TileLUT.TryGetValue(top, out outTile);
                }
                else if (CheckNeighborTileIs3x3(left))
                {
                    TileDefs.TileLUT.TryGetValue(left, out outTile);
                }
            }


            return outTile;
        }
        private bool CheckNeighborTileIs3x3(int tileid)
        {
            Tile neighbor_tile = null;
            if (TileDefs.TileLUT.TryGetValue(tileid, out neighbor_tile))
            {
                if (neighbor_tile.Sprite.Tiling != Tiling.Grid3x3)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        private bool IsSaved(Cell c, int layer)
        {
            return GetSaveTile(c.GetTilePosGlobal(), layer) != null;
        }
        private void AddGameObject(GameObject ob, int tileid, Cell cell, int iLayer, ivec2 vi_save_pos, int i_save_layer)
        {
            ob.TileId = tileid;

            //Critical that we make our boundbox and our update.
            ob.Update(World.Screen.Game.Input, 0.0f);
            ob.CalcBoundBox();

            ob.SavePos = vi_save_pos;
            ob.SaveLayer = i_save_layer;
            ob.CreatedCell = cell;
            ob.CreatedLayer = iLayer;

            //After creating it, deserialize it
            SaveTile ds = GetSaveTile(vi_save_pos, iLayer);
            if (ds != null)
            {
                if (ob is Player)
                {
                }
                else
                {
                    ob.Deserialize(new BinaryReader(new MemoryStream(ds.Data)));
                }
            }
            else
            {
                ob.CreateNew();
            }

            ob.UpdateSprite();

            GameObjects.Add(ob);
        }
        private int GetSpriteTileFrame(int iCol, int iRow, int iLayer, Tile tile)
        {

            int iFrame = 0;
            //52 52 53 54
            if (tile.Sprite == null)
            {
                //This is a random sprite group.
                iFrame = 0;
            }
            else if (tile.Sprite.Tiling == Tiling.Single)
            {
                iFrame = 0;
            }
            else if (tile.Sprite.Tiling == Tiling.Grid3x3)
            {

                iFrame = GetTileIndex3x3(iCol, iRow, iLayer, tile.Sprite.SeamlessIds, true);
            }
            else if (tile.Sprite.Tiling == Tiling.Horizontal3x1)
            {
                iFrame = GetTileIndex3H(iCol, iRow, iLayer, tile.TileId);
            }
            else if (tile.Sprite.Tiling == Tiling.Vertical1x3)
            {
                iFrame = GetTileIndex3V(iCol, iRow, iLayer, tile.TileId);
            }

            return iFrame;
        }
        public void CreateTile(Cell cell, Tile tile, int iFrame, int iLayer, vec2 curTilePos, vec2 curTileWH)
        {
            cell.Layers[iLayer] = new TileBlock();
            cell.Layers[iLayer].FrameIndex = iFrame;
            cell.Layers[iLayer].Tile = tile;
            if (tile.RandomSprites != null && tile.RandomSprites.Count > 0)
            {
                int iSpriteIndex = Globals.RandomInt(0, tile.RandomSprites.Count);
                cell.Layers[iLayer].Sprite = Res.Tiles.GetSprite(tile.RandomSprites[iSpriteIndex]);
            }
            else
            {
                cell.Layers[iLayer].Sprite = tile.Sprite;
            }
            cell.Layers[iLayer].Box = cell.Box();//Make a copy of box & modify 
            cell.Layers[iLayer].Blocking = tile.Blocking;
            cell.Layers[iLayer].CanMine = tile.CanMine;
            cell.Layers[iLayer].CanBomb = tile.CanBomb;
            cell.Layers[iLayer].FallThrough = tile.FallThrough;
            cell.Layers[iLayer].Health = tile.MaxHealth;
            tile.Create?.Invoke(cell, iLayer, cell.Layers[iLayer]);
        }
        public int GetTileIndex3H(int col, int row, int layer, int tile_id, bool bContinue = true)
        {
            //Works for both 3H (monster grass) and 3v (trees)

            bool[] arr = new bool[3];
            for (int i = -1; i <= 1; ++i)
            {
                int txy = this.GenTiles.TileXY(col + i, row, layer);
                if (bContinue && txy == 0) { txy = tile_id; }

                arr[i + 1] = txy == tile_id;
            }

            return CrankPattern(TilePatterns3H, arr, 3, 1, 1);
        }
        public int GetTileIndex3V(int col, int row, int layer, int tile_id, bool bContinue = true)
        {
            //Works for both 3H (monster grass) and 3v (trees)

            bool[] arr = new bool[3];
            for (int i = -1; i <= 1; ++i)
            {
                int txy = this.GenTiles.TileXY(col, row + i, layer);
                if (bContinue && txy == 0) { txy = tile_id; }

                arr[i + 1] = txy == tile_id;
            }

            return CrankPattern(TilePatterns3V, arr, 3, 1, 1);
        }
        public int GetTileIndex3x3(int col, int row, int layer, List<int> seamless_ids, bool bContinue = true)
        {
            //The tiles should all be the same in the spritesheett
            //Create a boolean array
            bool[] arr = new bool[9];
            for (int j = -1; j <= 1; ++j)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    int ind = (j + 1) * 3 + (i + 1);

                    //Out of bounds check
                    //((col + i) == (Room.Min.x-1)) || ((col + i) == (Room.Max.x + 1)) || ((row + j) == (Room.Min.y-1)) || ((row + j) == (Room.Max.y+1)))
                    if (Room.Found.Contains(new ivec2(col + i, row + j)) == false)
                    {
                        //Seam the level border
                        arr[ind] = true;
                    }
                    else
                    {
                        int txy = this.GenTiles.TileXY(col + i, row + j, layer);
                        if (bContinue && txy == 0) { txy = seamless_ids[0]; }//continue outside the level border, 0 is null/no tile for TILED tiles.

                        //If there is a slope tile on the corresponding connected side
                        if (txy == Res.SlopeTile_BR && ((i == 0 && j == -1) || (i == -1 && j == 0) || (i == -1 && j == -1)))
                        {
                            arr[ind] = true;
                        }
                        else if (txy == Res.SlopeTile_BL && ((i == 0 && j == -1) || (i == 1 && j == 0) || (i == 1 && j == -1)))
                        {
                            arr[ind] = true;
                        }
                        else if (txy == Res.SlopeTile_TR && ((i == 0 && j == 1) || (i == -1 && j == 0) || (i == -1 && j == 1)))
                        {
                            arr[ind] = true;
                        }
                        else if (txy == Res.SlopeTile_TL && ((i == 0 && j == 1) || (i == 1 && j == 0) || (i == 1 && j == 1)))
                        {
                            arr[ind] = true;
                        }
                        else
                        {
                            //Else find the tile in the seamless tile id's
                            arr[ind] = seamless_ids.Contains(txy);//txy==tileid
                        }
                    }
                }
            }

            int pat = CrankPattern(TilePatterns3x3, arr, 9, 7, 4);
            if (pat == 21)
            {
                int n = 0;
                n++;
            }
            return pat;
        }
        public int CrankPattern(MultiValueDictionary<int, List<int>> list, bool[] arr, int PatternTileCount, int Defaultvalue, int CenterPat)
        {
            //This algorithm matches an input pattern of tiles, to the a configured pattern, to generate
            //a sprite.  This is essentially an automatic tiling algorithm.
            //Loop arr, match with every pattern in "list" and return the corresponding key
            //Center pat is the center index - the pattern we ignore  = 4 for 3x3 and 1 for 1x3 or 3x1
            if (arr.Length != PatternTileCount)
            {
                //Error
                System.Diagnostics.Debugger.Break();
            }

            int ret = Defaultvalue;
            foreach (int frame in list.Keys)
            {
                HashSet<List<int>> values = null;

                if (list.TryGetValue(frame, out values))
                {
                    foreach (List<int> pat in values)
                    {
                        if (pat.Count != PatternTileCount)
                        {
                            //Must have 9 tiles in the pattern.
                            System.Diagnostics.Debugger.Break();
                        }

                        bool match = true;
                        for (int iPat = 0; iPat < PatternTileCount; ++iPat)
                        {
                            if (iPat == CenterPat)
                            {
                                //Don't test the center square. That's what we're calculating.
                                continue;
                            }

                            if (pat[iPat] == 2)
                            {
                                //2 = Don't care.  
                            }
                            else if ((pat[iPat] == 1) != arr[iPat])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match == true)
                        {
                            return frame;
                        }
                    }

                }
            }

            return Defaultvalue;
        }

        static MultiValueDictionary<int, List<int>> TilePatterns3H = new MultiValueDictionary<int, List<int>>() {
            //Horizontal patterns, like grass
                {0, new List<int>{ 0,1,0 }},
                {1, new List<int>{ 0,1,1 }},
                {2, new List<int>{ 1,1,1 }},
                {3, new List<int>{ 1,1,0 }},
        };
        static MultiValueDictionary<int, List<int>> TilePatterns3V = new MultiValueDictionary<int, List<int>>() {
            //Vertical patterns, trees
                {0, new List<int>{
                    0,
                    1,
                    0 }},
                {1, new List<int>{
                    1,
                    1,
                    0 }},
                {2, new List<int>{
                    1,
                    1,
                    1 }},
                {3, new List<int>{
                    0,
                    1,
                    1 }},
        };
        static MultiValueDictionary<int, List<int>> TilePatterns3x3 = new MultiValueDictionary<int, List<int>>() {
                {0, new List<int>{
                    2,0,2,
                    0,1,1,
                    2,1,1 }},
                {1, new List<int>{
                    2,0,2,
                    1,1,1,
                    1,1,1 }},
                {2, new List<int>{
                    2,0,2,
                    1,1,0,
                    1,1,2 }},
                {3, new List<int>{
                    0,0,0,
                    0,1,0,
                    2,1,2 }},
                {3, new List<int>{
                    2,0,2,
                    0,1,0,
                    2,1,2 }},
                 {4, new List<int>{
                    2,0,2,
                    0,1,1,
                    2,0,2 }},
                 {5, new List<int>{
                    2,0,2,
                    1,1,0,
                    2,0,2 }},

                 //Row2
                {6, new List<int>{
                    2,1,1,
                    0,1,1,
                    2,1,1 }},
                {7, new List<int>{
                    1,1,1,
                    1,1,1,
                    1,1,1 }},
                 {8, new List<int>{
                    1,1,2,
                    1,1,0,
                    1,1,2 }},
                 {9, new List<int>{
                    2,1,2,
                    0,1,0,
                    2,0,2 }},
                  {10, new List<int>{
                    1,1,0,
                    1,1,1,
                    1,1,1 }},
                 {11, new List<int>{
                    0,1,1,
                    1,1,1,
                    1,1,1 }},
                //Row 3
                { 12, new List<int>{
                    2,1,1,
                    0,1,1,
                    2,0,2 }},
                 { 13, new List<int>{
                    1,1,1,
                    1,1,1,
                    2,0,2 }},

                 { 14, new List<int>{
                    1,1,2,
                    1,1,0,
                    2,0,2 }},


                 { 15, new List<int>{ //*This goes with 4 and 5
                    2,0,2,
                    1,1,1,
                    2,0,2 }},

                 { 16, new List<int>{
                    1,1,1,
                    1,1,1,
                    0,1,0 }},

                 //Too many combos, use 2
                  { 17, new List<int>{
                    2,1,2,
                    0,1,0,
                    2,1,2 }},

                  //Row 4
                  { 18, new List<int>{
                    2,1,0,
                    0,1,1,
                    2,1,1 }},
                  { 19, new List<int>{
                    0,1,2,
                    1,1,0,
                    1,1,2 }},
                  { 20, new List<int>{
                    2,1,0,
                    0,1,1,
                    2,1,0 }},

                  { 21, new List<int>{
                    0,1,2,
                    1,1,0,
                    0,1,2 }},

                  { 22, new List<int>{
                    1,1,0,
                    1,1,1,
                    1,1,0 }},
                  { 23, new List<int>{
                    0,1,1,
                    1,1,1,
                    0,1,1 }},
                  //Row 5
                  { 24, new List<int>{
                    2,0,2,
                    1,1,1,
                    1,1,0 }},
                  { 25, new List<int>{
                    2,0,2,
                    1,1,1,
                    0,1,1 }},
                  { 26, new List<int>{
                    2,0,2,
                    1,1,1,
                    0,1,0 }},

                  { 27, new List<int>{
                    2,1,0,
                    0,1,1,
                    2,0,2 }},

                  { 28, new List<int>{
                    0,1,2,
                    1,1,0,
                    2,0,2 }},

                  {29, new List<int>{
                    2,0,2,
                    0,1,0,
                    2,0,2 }},

           
                  //Rpw 6
                  { 30, new List<int>{
                    2,0,2,
                    0,1,1,
                    2,1,0 }},
                  { 31, new List<int>{
                    2,0,2,
                    1,1,0,
                    0,1,2 }},

                  { 32, new List<int>{
                    0,1,0,
                    1,1,1,
                    0,1,0 }},
                { 33, new List<int>{
                        2,1,1,
                        0,1,1,
                        2,1,0 }},
                { 34, new List<int>{
                        1,1,2,
                        1,1,0,
                        0,1,2 }},
                { 35, new List<int>{
                    0,1,0,
                    1,1,1,
                    2,0,2 }},

                 //rOW 7
                { 36, new List<int>{
                        0,1,0,
                        1,1,1,
                        1,1,0 }},
                { 37, new List<int>{
                        0,1,0,
                        1,1,1,
                        0,1,1 }},
                { 38, new List<int>{
                        0,1,1,
                        1,1,1,
                        2,0,2 }},

                { 39, new List<int>{
                        1,1,0,
                        1,1,1,
                        2,0,2 }},
                { 40, new List<int>{
                        1,1,1,
                        1,1,1,
                        1,1,0 }},
                { 41, new List<int>{
                        1,1,1,
                        1,1,1,
                        0,1,1 }},
                //Row 8
                { 42, new List<int>{
                        1,1,0,
                        1,1,1,
                        0,1,0 }},
                { 43, new List<int>{
                        0,1,1,
                        1,1,1,
                        0,1,0 }},
                { 44, new List<int>{
                        0,1,0,
                        1,1,1,
                        1,1,1 }},
             { 49, new List<int>{
                        0,1,2,
                        1,1,1,
                        2,1,0 }},
                { 48, new List<int>{
                        2,1,0,
                        1,1,1,
                        0,1,2 }},




            };



    }
}
