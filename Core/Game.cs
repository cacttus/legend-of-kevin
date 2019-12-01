using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Core
{
    /* map depends on save system
     * we generate the map on a sub-grid of tiles
     * start with player pos
     * for all portals in cur level
     * get saved portal 
     *  List<ivec2> checked, tocheck
     *  get traversal left, or right
     *   if Entered - continue through portal.
     *   else if both add both sides to map.
     *   flood fill
     *   
     *   
     * */
    public enum GameState { Play, Pause, LevelTransition, PlayerDeath_Begin, PlayerDeath_Animate, PlayerDeath_ShowContinue, PlayerDeath_Resetting }
    public enum DrawState { Normal, None, PlayerOnly }
    public enum HoverState { Normal, Interact, Aim, Attack, Talk }//Cursor state when interacting with object

    public class PortalTransition
    {
        public DoorTransitionEffect PortalTransitionEffect { get; set; } = DoorTransitionEffect.Blend;
        public RenderTarget2D Prev;
        public RenderTarget2D Next;
        Game Game;
        public float MaxTime = 1.5f;//seconds
        public float Time = 0;
        public World World;
        public vec2 ViewportPosPrev;
        public vec2 ViewportPosNext;

        public PortalTransition(World w, DoorTransitionEffect ef)
        {
            World = w;
            Game = World.Screen.Game;
            Time = MaxTime;
            PortalTransitionEffect = ef;

        }
        public bool Update(float dt)
        {
            Time -= dt;
            if (Time <= 0)
            {
                return true;
            }
            return false;
        }

        public void Draw(Screen screen)
        {
            if (PortalTransitionEffect == DoorTransitionEffect.Fade ||
                PortalTransitionEffect == DoorTransitionEffect.Blend)
            {
                float a = 0, b = 0;
                if (PortalTransitionEffect == DoorTransitionEffect.Fade)
                {
                    b = 1.0f - Globals.Clamp(Time / (MaxTime / 2), 0, 1);
                    a = Globals.Clamp(Time / (MaxTime / 2), 0, 1);
                }
                else if (PortalTransitionEffect == DoorTransitionEffect.Blend)
                {
                    a = Time / MaxTime;
                    b = 1.0f - a;
                }

                screen.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

                if (Prev != null)
                {
                    screen.SpriteBatch.Draw(Prev,
                    new Rectangle(0, 0, screen.Game.GraphicsDevice.Viewport.Width, screen.Game.GraphicsDevice.Viewport.Height),
                    new Rectangle(0, 0, Prev.Width, Prev.Height),
                    Color.White * a, 0, new Vector2(0, 0), SpriteEffects.None, 0.0f);
                }
                if (Next != null)
                {
                    screen.SpriteBatch.Draw(Next,
                    new Rectangle(0, 0, screen.Game.GraphicsDevice.Viewport.Width, screen.Game.GraphicsDevice.Viewport.Height),
                    new Rectangle(0, 0, Next.Width, Next.Height),
                    Color.White * b, 0, new Vector2(0, 0), SpriteEffects.None, 0.0f);
                }
                screen.SpriteBatch.End();

            }
            else if (PortalTransitionEffect == DoorTransitionEffect.MoveCamera)
            {

                World.Screen.Viewport.Pos = ViewportPosPrev + (this.ViewportPosNext - this.ViewportPosPrev) * (1 - Time / MaxTime);
                World.Screen.Viewport.CalcBoundBox();

                World.Update(0.0f, false);
                (screen as GameScreen).DrawGameWorldWithEffects();
            }



        }


        public void RenderPrevWorld(World w)
        {
            ViewportPosPrev = w.Screen.Viewport.Pos;
            RenderWorldToBuffer(ref Prev, w);
        }
        public void RenderNextWorld(World w)
        {
            ViewportPosNext = w.Screen.Viewport.Pos;
            RenderWorldToBuffer(ref Next, w);
        }

        private void RenderWorldToBuffer(ref RenderTarget2D tar, World w)
        {
            tar = new RenderTarget2D(
                Game.GraphicsDevice,
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            //Draw 
            World.Screen.Game.GraphicsDevice.SetRenderTarget(tar);
            World.Screen.Game.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            World.Screen.Game.GraphicsDevice.Clear(Color.Black);
            World.Screen.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            World.DrawGameWorld(World.Screen.SpriteBatch);
            World.Screen.SpriteBatch.End();
            World.Screen.Game.GraphicsDevice.SetRenderTarget(null);
        }
    }
    public class CutsceneSequence
    {
        public float Duration = 0;//this is an optional parameter
        public Func<CutsceneSequence, float, bool> Update;
        public Action<CutsceneSequence, SpriteBatch> Draw;
        public CutsceneSequence(float duration, Func<CutsceneSequence, float, bool> upd, Action<CutsceneSequence, SpriteBatch> draw)
        {
            Update = upd;
            Draw = draw;
            Duration = duration;
        }
    }
    public class Cutscene
    {
        public List<CutsceneSequence> Sequences = new List<CutsceneSequence>();
        public Cutscene()
        {
        }
        public bool Ended()
        {
            return Sequences.Count == 0;
        }
        public Cutscene Then(float d, Func<CutsceneSequence, float, bool> upd, Action<CutsceneSequence, SpriteBatch> draw = null)
        {
            CutsceneSequence s = new CutsceneSequence(d, upd, draw);
            Sequences.Add(s);
            return this;
        }
        public Cutscene Wait(float seconds)
        {
            //Wait for the specified number of seconds
            return Then(seconds, (s, dt) =>
            {
                return s.Duration > 0;
            }, null);
        }
        public void Draw(SpriteBatch sb)
        {
            if (Sequences.Count > 0)
            {
                Sequences[0].Draw?.Invoke(Sequences[0], sb);
            }
        }
        public bool Update(float dt)
        {
            //Return true as long as we are still running
            if (Sequences.Count == 0)
            {
                return false;
            }

            CutsceneSequence cur = Sequences[0];
            cur.Duration -= dt;
            if (cur.Update(cur, dt) == false)
            {
                Sequences.RemoveAt(0);
            }

            if (Sequences.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
    public class Dialog
    {
        public class TextBlock
        {
            public vec4 Color;
            public string Text;
            public vec2 Pos;
            public float Alpha = 1.0f; // When moving text off the dialog
            public TextBlock(string text, vec2 pos, vec4 color)
            {
                Text = text;
                Pos = pos;
                Color = color;
            }
        }

        public bool Halt = false;  //Halt player input until done
        private List<string> Text;//the List<> means that every new string will get a cursor (user needs to press OK).
        public World World;
        GameObject TextCursor;
        float _fNextChar = 0;
        float _fTextSpeed = 0.05f; //basic text speed
        float _fTextSpeedSkip = 0.025f; // PLayer pressees OK to skip the text
        int _iChar = 0, _iMessage = 0;
        bool WaitForUser = false;
        SpriteFont Font;
        private vec4 SpecialColor = new vec4(.2f, .721f, .2f, 1); //Highlighted color WEAPON
        private vec4 SpecialColor2 = new vec4(.1f, .121f, .16f, 1); //Highlighted color NPC NAME
        private vec4 BaseColor = new vec4(.00186f, .0023f, .101f, 1);
        List<List<TextBlock>> Lines = new List<List<TextBlock>>();
        bool bSpecial = false;
        Guy NpcTalking = null;

        public Dialog(World w, SpriteFont f)
        {
            World = w;
            Font = f;
        }
        public void Quit()
        {
            Text = null;
        }
        public bool IsEnabled()
        {
            return Text != null && Text.Count > 0;
        }
        public float MaxLineWidthPixels()
        {
            float box_pad_LR = 0.1f;
            return (World.Screen.Viewport.TilesWidth - 2) * Res.Tiles.TileWidthPixels * (1.0f - box_pad_LR * 2.0f);
        }
        public void ShowDialog(List<string> text, Guy NPC = null)
        {
            //Halt = halt;
            Text = new List<string>(text);//We edit this , so make a new 
            _fNextChar = _fTextSpeed;
            _iChar = 0;
            _iMessage = 0;
            Lines = new List<List<TextBlock>>();
            WaitForUser = false;
            NpcTalking = NPC;

            //Add npc name if present
            if (Text.Count > 0 && NPC != null && String.IsNullOrEmpty(NPC.NpcName) == false)
            {
                Text[0] = "^" + NPC.NpcName + ": " + Text[0];
            }
        }
        private vec2 GetTextBasePos()
        {
            return World.Screen.Viewport.Pos + new vec2(
                    Res.Tiles.TileWidthPixels * 1.35f,
                    World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 1.7f
                    );
        }
        public float GetTextVHeightPixels()
        {
            float v_pad = 0.15f;
            float v_height = Res.Tiles.TileHeightPixels - (Res.Tiles.TileHeightPixels * v_pad) * 2.0f;

            return v_height;
        }
        public float GetTextLineHeightPixels()
        {
            float v_pad = 0.20f;
            float v_height = Res.Tiles.TileHeightPixels - (Res.Tiles.TileHeightPixels * v_pad) * 2.0f;

            return v_height;
        }
        public void Update(float dt)
        {
            if (IsEnabled())
            {
                if (TextCursor == null)
                {
                    TextCursor = new GameObject(World, Res.SprMoreTextCursor);
                    TextCursor.Animate = true;
                }
                TextCursor.Update(World.Screen.Game.Input, dt);

                AdvanceText(dt);

                //Move The blocks up if we have too much text.
                if (Lines.Count >= 3)
                {
                    vec2 vp_off = GetTextBasePos();
                    for (int iLine = Lines.Count - 1; iLine >= 0; iLine--)
                    {
                        bool removeLine = false;
                        foreach (TextBlock block in Lines[iLine])
                        {
                            block.Pos.y -= 1.0f;

                            float miny = GetTextBasePos().y - GetTextLineHeightPixels();

                            float block_pos_abs = block.Pos.y + vp_off.y;

                            if (block_pos_abs < miny)
                            {
                                removeLine = true;
                            }

                            //fade the text that falls off the top
                            if (block_pos_abs < GetTextBasePos().y + 0.1f)
                            {
                                block.Alpha = (block_pos_abs - miny) / (GetTextBasePos().y - miny) * 0.8f;
                            }
                        }

                        if (removeLine)
                        {
                            Lines.RemoveAt(iLine);
                        }
                    }
                }

            }
        }
        private void AdvanceText(float dt)
        {
            //This advances the MESSAGE text.
            if (WaitForUser == false)
            {
                _fNextChar -= dt;
                if (_fNextChar <= 0)
                {
                    if (World.GetPlayer().Joystick.Ok.PressOrDown())
                    {
                        _fNextChar = _fTextSpeedSkip;
                        this.World.GetPlayer().HasInteractedThisFrame = true;
                    }
                    else
                    {
                        _fNextChar = _fTextSpeed;
                    }

                    if (_iChar >= Text[_iMessage].Length)
                    {
                        _iChar = Text[_iMessage].Length - 1;
                        WaitForUser = true;
                    }
                    else
                    {
                        if (Char.IsWhiteSpace(Text[_iMessage][_iChar]) == false)
                        {
                            Res.Audio.PlaySound(Res.SfxTextBlip);
                        }

                        AddChar(Text[_iMessage][_iChar]);
                    }
                    _iChar++;
                }
            }

            if (WaitForUser == true)
            {
                if (Halt == false)
                {
                    Guy guy = World.GetPlayer();
                    if (guy.Joystick.Ok.Press() || guy.Joystick.Action.Press() || this.World.Rmb.PressOrDown() || this.World.Lmb.PressOrDown())
                    {
                        this.World.GetPlayer().HasInteractedThisFrame = true;

                        _iChar = 0;
                        _iMessage += 1;
                        WaitForUser = false;

                        StartNewLine(new vec2(0, GetNextLineY()));

                        if (_iMessage >= Text.Count)
                        {
                            Text = null;//Hide the textbox

                            if (NpcTalking != null)
                            {
                                NpcTalking.AIState = AIState.Wander;
                                NpcTalking.SetSprite(NpcTalking.WalkSprite);
                            }
                        }
                    }
                }

            }
        }
        private void StartNewLine(vec2 xy)
        {
            List<TextBlock> tbl = new List<TextBlock>();
            Lines.Add(tbl);
            StartNewBlock(xy, bSpecial ? SpecialColor : BaseColor);
        }
        private void StartNewBlock(vec2 xy, vec4 color)
        {
            if (Lines.Count == 0)
            {
                //**ERROR
                System.Diagnostics.Debugger.Break();
                return;
            }
            Lines[Lines.Count - 1].Add(new TextBlock("", xy, color));
        }
        private void AddChar(char c)
        {

            //Compute Line Length
            //we need to use pixels because # chars simply doesn't work.
            float curLineWidthPixels = 0;
            if (Lines.Count > 0)
            {
                curLineWidthPixels = GetLineWidthPixels(Lines[Lines.Count - 1]);
            }
            else
            {
                StartNewLine(new vec2(0, 0));
            }

            if (Lines.Count > 0 && Lines[Lines.Count - 1].Count > 0 && curLineWidthPixels >= MaxLineWidthPixels())
            {
                //Start a new line. also use a - if we need to wrap words
                List<TextBlock> lastLine = Lines[Lines.Count - 1];
                TextBlock lastBlock = lastLine[lastLine.Count - 1];

                char c_last = lastBlock.Text[lastBlock.Text.Length - 1];
                char c_next = _iChar < Text[_iMessage].Length ? Text[_iMessage][_iChar] : ' ';

                if (Char.IsWhiteSpace(c_last) == false && Char.IsWhiteSpace(c_next) == false)
                {
                    lastBlock.Text = lastBlock.Text.Substring(0, lastBlock.Text.Length - 1) + '-';
                }

                StartNewLine(new vec2(0, GetNextLineY()));

                if (Char.IsWhiteSpace(c_last) == false && Char.IsWhiteSpace(c_next) == false)
                {
                    //Move the hyphenated char to the next line
                    Lines[Lines.Count - 1][Lines[Lines.Count - 1].Count - 1].Text += c_last;
                }

            }

            if (c == '*')
            {
                bSpecial = true;
                StartNewBlock(new vec2(GetCurLineX(), GetCurLineY()), SpecialColor);
            }
            else if (c == '^')
            {
                bSpecial = true;
                StartNewBlock(new vec2(GetCurLineX(), GetCurLineY()), SpecialColor2);
            }
            else
            {
                if (Char.IsWhiteSpace(c) && bSpecial)
                {
                    bSpecial = false;
                    StartNewBlock(new vec2(GetCurLineX(), GetCurLineY()), BaseColor);
                }

                Lines[Lines.Count - 1][Lines[Lines.Count - 1].Count - 1].Text += c;
            }

        }
        public float GetLineWidthPixels(List<TextBlock> line)
        {
            float w = 0;
            foreach (TextBlock b in line)
            {
                if (String.IsNullOrEmpty(b.Text))
                {
                    //We get NAN if text is empty for MeasureString...why.. 
                    continue;
                }
                Vector2 deviceWidth = Font.MeasureString(b.Text);

                float scale = World.Screen.DrawText_Fit_H_OR_V_Scale(Font, false, b.Text, GetTextVHeightPixels());

                deviceWidth *= scale;//This gives us the actual device pixels width.
                vec2 wh_pixels = World.Screen.Viewport.ScreenRasterToScreenPixels(new vec2(deviceWidth));

                w += wh_pixels.x;
            }
            return w;
        }
        public float GetNextLineY()
        {
            if (Lines.Count == 0)
            {
                return 0;
            }
            if (Lines[Lines.Count - 1].Count == 0)
            {
                return 0;
            }
            float lowest_y = Lines[Lines.Count - 1][Lines[Lines.Count - 1].Count - 1].Pos.y;
            return lowest_y + GetTextLineHeightPixels();
        }
        public float GetCurLineY()
        {
            if (Lines.Count == 0)
            {
                return 0;
            }
            if (Lines[Lines.Count - 1].Count == 0)
            {
                return 0;
            }
            return Lines[Lines.Count - 1][Lines[Lines.Count - 1].Count - 1].Pos.y;
        }
        public float GetCurLineX()
        {
            if (Lines.Count == 0)
            {
                return 0;
            }
            if (Lines[Lines.Count - 1].Count == 0)
            {
                return 0;
            }
            return GetLineWidthPixels(Lines[Lines.Count - 1]);
        }
        public void Draw(SpriteBatch sb)
        {
            if (IsEnabled())
            {
                DrawDialogBackground(sb);

                //Draw the cursor
                if (WaitForUser && Halt == false)
                {
                    vec2 wh = new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels);
                    World.Screen.DrawUIFrame(sb,
                        TextCursor.Frame,
                        new vec2(
                            World.Screen.Viewport.WidthPixels - (Res.Tiles.TileWidthPixels) * 2.0f,
                            World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 0.8f),
                        wh, new vec4(1, 1, 1, 1));
                }

                vec2 vp_off = GetTextBasePos();
                //Draw the text
                foreach (List<TextBlock> line in Lines)
                {
                    foreach (TextBlock block in line)
                    {
                        World.Screen.DrawText_Fit_V(sb, Font, block.Text,
                            GetTextVHeightPixels(), vp_off + block.Pos, block.Color * block.Alpha, 2, new vec4(1, 1, 1, 1) * block.Alpha, "", false);
                    }
                }



            }
        }
        public void DrawDialogBackground(SpriteBatch sb)
        {
            Sprite spr = Res.Tiles.GetSprite(Res.SprTextBk);
            vec2 wh = new vec2(
                    Res.Tiles.TileWidthPixels,
                     Res.Tiles.TileHeightPixels
                    );
            World.Screen.DrawUIFrame(sb,
                Res.SprTextBk, 0,
                new vec2((Res.Tiles.TileWidthPixels) * (1), World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 2),
                wh, new vec4(1, 1, 1, 1));
            World.Screen.DrawUIFrame(sb,
                Res.SprTextBk, 3,
                new vec2((Res.Tiles.TileWidthPixels) * (1), World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 1),
                wh, new vec4(1, 1, 1, 1));

            for (int i = 2; i < World.Screen.Viewport.TilesWidth - 2; ++i)
            {
                World.Screen.DrawUIFrame(sb,
                    Res.SprTextBk, 1,
                    new vec2(
                        (Res.Tiles.TileWidthPixels) * (i),
                    World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 2),
                    wh, new vec4(1, 1, 1, 1));
                World.Screen.DrawUIFrame(sb,
                    Res.SprTextBk, 4,
                    new vec2(
                        (Res.Tiles.TileWidthPixels) * (i),
                    World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 1),
                    wh, new vec4(1, 1, 1, 1));
            }

            World.Screen.DrawUIFrame(sb,
                Res.SprTextBk, 2,
                new vec2(World.Screen.Viewport.WidthPixels - (Res.Tiles.TileWidthPixels) * 2,
                World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 2),
                wh, new vec4(1, 1, 1, 1));
            World.Screen.DrawUIFrame(sb,
                Res.SprTextBk, 5,
                new vec2(World.Screen.Viewport.WidthPixels - (Res.Tiles.TileWidthPixels) * 2,
                World.Screen.Viewport.HeightPixels - (Res.Tiles.TileHeightPixels) * 1),
                wh, new vec4(1, 1, 1, 1));
        }

    }
    public class Projectile : GameObject
    {
        public Projectile(WorldBase w) : base(w) { }
    }
    public enum MenuTab { Inventory, Map, Options }
    public class World : WorldBase
    {
        public GameState GameState
        {
            get;
            set;
        } = GameState.Play;
        DrawState DrawState = DrawState.Normal;//For cuscenese

        public HoverState HoverState = HoverState.Normal;

        public bool bLoadingRoom = false;


        public PlatformLevel Level { get; private set; }

        SoundEffectInstance jumpSound = null;
        SoundEffectInstance climbSound = null;
        SoundEffectInstance landSound = null;
        SoundEffectInstance chargeSound = null;
        SoundEffectInstance drawBowSound = null;

        Cutscene Cutscene = null;
        public Dialog Dialog { get; private set; } = null;

        List<Cell> ViewportCellsFrame;
        public List<GameObject> ViewportObjsFrame = new List<GameObject>();
        public List<GameObject> MouseObjectsFrame = new List<GameObject>();
        //  public List<GameObject> HoverObjectsFrame = new List<GameObject>();

        List<GameObject> EmittersFrame = new List<GameObject>();
        HashSet<GameObject> GuyCells = new HashSet<GameObject>();
        public vec2 LastLightVPPos;//Prevents the border-screen "flicker" that we get from the player moving too fast.
        float Last_DT;

        float LastParticle = 0.0f;//For Lava Particles

        public float LavaScreenDistort { get; private set; } = 0.0f;
        float LavaScreenDistortFadeSpeed = 3.0f; //seconds to full distort

        ButtonBase DebugButton = new ButtonBase();
        ButtonBase CheatButton = new ButtonBase();
        private bool ShowDebug = false;
        private float Fps;
        bool bShowMenu = false;
        bool bShowUI = true;
        bool bShowCursor = true;
        public vec4 ScreenOverlayColor { get; private set; } = new vec4(1, 1, 1, 1);//This color multiplies the entire scene

        vec4 JumpBootsMenuColor = new vec4(1, 1, 1, 1);
        vec4 PickaxeMenuColor = new vec4(1, 1, 1, 1);
        vec4 BombMenuColor = new vec4(1, 1, 1, 1);
        vec4 PowerSwordMenuColor = new vec4(1, 1, 1, 1);
        vec4 ShieldMenuColor = new vec4(1, 1, 1, 1);
        vec4 BowMenuColor = new vec4(1, 1, 1, 1);

        float LastShieldDeflect = 0;
        Door LastSwordCollideDoor = null;

        float SaveTime;
        bool bSaving = false;

        public ButtonBase Rmb = new ButtonBase();
        public ButtonBase Lmb = new ButtonBase();

        GameObject ShieldObject = null;

        //Menu
        MenuTab menutab = MenuTab.Inventory;
        float ShowMenuFade = 0.0f;
        float MaxShowMenuFade = 1.0f;
        Box2f MenuTab0Box = new Box2f(13, 12, 32, 8);
        Box2f MenuTab1Box = new Box2f(47, 12, 32, 8);
        Box2f MenuTab2Box = new Box2f(47 + 34, 12, 32, 8);
        Box2f MenuMapBox = new Box2f(16, 22, 130, 60);
        TileDefs td;

        bool StartRunning = false;

        public World(Screen screen) : base(screen)
        {
            Dialog = new Dialog(this, Res.Font);
            Screen.Game.AdMan.HideAd("MainAd");

            GenerateLevel();
            DoCheats();
            if (Res.ShownTutorial == false)
            {
                PlayIntroCutscene();
                Res.ShownTutorial = true;
            }
            else
            {
                StartRunning = true;
            }
        }
        private void PlayIntroCutscene()
        {

            Cutscene = new Cutscene()
                .Then(0, (s, dt) =>
                {
                    IntroTutorial = true;
                    GetPlayer().Joystick.IsUser = false;
                    GetPlayer().SetSprite(Res.SprGuyWalk, true, 1);
                    return false;
                })
                .Then(2, (s, dt) =>
                {
                    //Wait an initial 2 seconds.
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return s.Duration > 0;
                })
                .Then(0, (s, dt) =>
                {
                    Dialog.ShowDialog(new List<string>
                    {
                        "Welcome to Rocket Jump",
                        "This is Bill, the Rocket Jump champion."
                    }, null);
                    return false;
                })
                .Then(0, (s, dt) =>
                {
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return Dialog.IsEnabled();
                })
                .Then(0, (s, dt) =>
                {
                    GetPlayer().SetSprite(Res.SprGuyWave, true);
                    GetPlayer().Animate = true;
                    GetPlayer().Waving = true;
                    return false;
                })
                .Then(3, (s, dt) =>
                {
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return s.Duration > 0;
                })
                .Then(0, (s, dt) =>
                {
                    GetPlayer().SetSprite(Res.SprGuyWalk, true, 1);
                    GetPlayer().Animate = false;
                    GetPlayer().Waving = false;
                    Dialog.ShowDialog(new List<string>
                    {
                        "Bill is performing the Rocket Jump today.",
                        "Press Spacebar to make Bill jump."
                    }, null);

                    return false;
                })
                .Then(0, (s, dt) =>
                {
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return Dialog.IsEnabled();
                })
                .Then(3f, (s, dt) =>
                {
                    GetPlayer().Joystick.AIJump = true;
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return s.Duration > 0;
                })
                .Then(0, (s, dt) =>
                {
                    GetPlayer().Joystick.AIJump = false;
                    Dialog.ShowDialog(new List<string>
                    {
                        "Jump again when Bill hits the ground to jump higher."
                        ,"After the 4th jump, Bill launches into space!"
                    }, null);
                    return false;
                })
                .Then(0, (s, dt) =>
                {
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return Dialog.IsEnabled();
                })
                .Then(.8f, (s, dt) =>
                {
                    GetPlayer().Joystick.AIJump = true;
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return s.Duration > 0;
                })
                .Then(0, (s, dt) =>
                {
                    GetPlayer().Joystick.AIJump = false;
                    Dialog.ShowDialog(new List<string>
                    {
                    "Try to jump the furthest you can!"
                        ,"And don't hit the water!"
                    }, null);
                    return false;
                })
                .Then(0, (s, dt) =>
                {
                    if (Rmb.Press())
                    {
                        Dialog.Quit();
                        return false;//Skip
                    }
                    return Dialog.IsEnabled();
                })
                .Then(0, (s, dt) =>
                {
                    IntroTutorial = false;
                    GetPlayer().Joystick.IsUser = true;
                    StartRunning = true;
                    return false;
                });
            ;

        }
        private void CreateDucks()
        {
            for (int i = 0; i < 100; ++i)
            {
                Duck g = new Duck(this, Res.SprDuck, AIState.SwimLeftRight);
                g.Animate = true;
                g.BoxRelative = new Box2f(-4, -4, 8, 8);
                g.Gravity = new vec2(0, 0);

                if (Globals.Random(0, 1) >= 0.5f)
                {
                    g.Joystick.AIRight = true;
                    g.Joystick.AILeft = false;
                }
                else
                {
                    g.Joystick.AIRight = false;
                    g.Joystick.AILeft = true;
                }

                float ebase = 20;
                float emax = 50;

                g.Pos.x = Globals.Random(0, 1) * Res.Tiles.TileWidthPixels * (float)this.RoomWidthTiles;
                g.Pos.y = (ebase * Res.Tiles.TileHeightPixels) + Globals.Random(0, 1) * Res.Tiles.TileHeightPixels * (float)(this.RoomHeightTiles - ebase - emax);

                Level.GameObjects.Add(g);
            }
        }
        private int RoomWidthTiles = 300;
        private int RoomHeightTiles = 200;
        private void GenerateLevel()
        {

            td = new TileDefs(this);
            //Legend of Kevin Map
            //TileMap tm = new TileMap("World-0");
            //Our Map.
            TileMap tm = new TileMap(RoomWidthTiles, RoomHeightTiles);

            int playerX = 1;
            int playerY = tm.MapHeightTiles - 5;
            tm.PlayerStartXY = new ivec2(playerX, playerY);
            tm.TrySetGenTile(playerX, playerY, PlatformLevel.Midground, Res.GuyTileId);

            //Atmosphere
            for (int gy = 0; gy < tm.MapHeightTiles; ++gy)
            {
                for (int gx = 0; gx < tm.MapWidthTiles; ++gx)
                {
                    int skyId = Res.Sky_Level0;
                    if (gy > tm.MapHeightTiles - 30)
                    {
                        skyId = Res.Sky_Level0;
                    }
                    else if (gy > tm.MapHeightTiles - 60)
                    {
                        skyId = Res.Sky_Level1;
                    }
                    else if (gy > tm.MapHeightTiles - 90)
                    {
                        skyId = Res.Sky_Level2;
                    }
                    else if (gy > tm.MapHeightTiles - 110)
                    {

                        skyId = Res.Sky_Level3;
                    }
                    else
                    {
                        skyId = Res.Sky_Level4;
                    }
                    tm.TrySetGenTile(gx, gy, PlatformLevel.Background, skyId);
                }
            }

            int water_start = 40;
            int water_end = 70;
            //Decals above ground
            for (int gy = playerY; gy < playerY + 1; ++gy)
            {
                for (int gx = 0; gx < tm.MapWidthTiles; ++gx)
                {
                    if (gx < water_start || gx > water_end)
                    {
                        float rt = Globals.Random(0, 1);
                        if (rt < 0.3)
                        {
                            //empty
                        }
                        else if (rt < 0.5)
                        {
                            tm.TrySetGenTile(gx, gy, PlatformLevel.Midback, Res.Tree_Doodad);
                        }
                        else if (rt < 0.7)
                        {
                            tm.TrySetGenTile(gx, gy, PlatformLevel.Midback, Res.Grass_Doodad1);
                        }
                        else
                        {
                            tm.TrySetGenTile(gx, gy, PlatformLevel.Midback, Res.Grass_Doodad2);
                        }
                    }

                }
            }

            //Ground
            for (int gy = playerY + 1; gy < tm.MapHeightTiles; ++gy)
            {
                for (int gx = 0; gx < tm.MapWidthTiles; ++gx)
                {
                    if (gx > water_start && gx < water_end)
                    {
                        if (gy > playerY + 1)
                        {
                            if (gy == tm.MapHeightTiles - 1)
                            {
                                //Add a "base" ground to the bottom for the water.
                                tm.TrySetGenTile(gx, gy, PlatformLevel.Midground, Res.BlockTileId_GrassDirt);
                            }
                            else
                            {
                                tm.TrySetGenTile(gx, gy, PlatformLevel.Midground, Res.Water100TileId);
                            }
                        }
                        else
                        {
                            tm.TrySetGenTile(gx, gy, PlatformLevel.Midground, Res.Water50TileId);
                        }
                    }
                    else
                    {
                        tm.TrySetGenTile(gx, gy, PlatformLevel.Midground, Res.BlockTileId_GrassDirt);
                    }
                }
            }

            Level = new PlatformLevel(this, tm);

            //add stars
            for (int i = 0; i < 1000; ++i)
            {
                GameObject star = new GameObject(this);
                if (Globals.Random(0, 1) > 0.7)
                {
                    star.SetSprite(Res.SprParticleBig, false);
                }
                else
                {
                    star.SetSprite(Res.SprParticleSmall, false);
                }
                float dx = Globals.Random(0, 1) * Res.Tiles.TileWidthPixels * tm.MapWidthTiles;
                float dy = Globals.Random(0, 1) * Res.Tiles.TileHeightPixels * 110;

                // Fade stars that are in the exosphere 
                if (dy > tm.MapHeightTiles - 110)
                {
                    star.Alpha = (float)(dy - (float)110) / (float)110;
                    float rc = 0.4f * Globals.Random(0, 1);
                    star.Color = new vec4(1, 1, .6f + rc, 1);
                }

                star.Pos.x = dx;
                star.Pos.y = dy;
                Level.GameObjects.Add(star);
            }

            CreateDucks();
        }
        private void Subdt(float dt, ref float value)
        {
            value -= dt;
            if (value <= 0) { value = 0; }
        }
        public override void Update(float dt, bool bFocusViewport = true)
        {
            base.Update(dt);//Particles

            //Debug
            UpdateDebug(dt);

            Rmb.Update(Mouse.GetState().RightButton == ButtonState.Pressed);
            Lmb.Update(Mouse.GetState().LeftButton == ButtonState.Pressed);

            // Cutscene
            if (Cutscene != null)
            {
                if (Cutscene.Update(dt) == false)
                {
                    Cutscene = null;
                }
            }
            else if (GameState != GameState.LevelTransition)
            {
                UpdateMenu(dt);
            }

            if (GameState == GameState.PlayerDeath_ShowContinue)
            {
                //Allow player to reset game
                if (Keyboard.GetState().IsKeyDown(Keys.Space) || Rmb.PressOrDown() || Lmb.PressOrDown())
                {
                    GameState = GameState.PlayerDeath_Resetting;
                    Res.Audio.PlaySound(Res.SfxContinue);

                    float hs = 0;
                    try
                    {
                        hs = (this.Screen.Game as MainGame).GetHighScore();
                    }
                    catch (Exception ex)
                    {
                    }

                    float d = GetDist(GetPlayer());

                    if (d > hs)
                    {
                        (this.Screen.Game as MainGame).SetHighScore(d);
                    }
                    (this.Screen as GameScreen).Reset = true;
                }
            }
            //else if(GameState==GameState.Play)
            {//
             //if((Screen.Game as MainGame).GraphicsDeviceManager.IsFullScreen)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Screen.Game.Exit();
                    }
                }

            }

            //Update Game
            UpdatePhysics(dt);

            if (bFocusViewport)
            {
                //Set the viewport to the guy.
                UpdateViewport(dt);
            }


            //put this first to prevent player from interacting with stuff when dialog is showing
            Dialog.Update(dt);

            UpdatePostPhysics(dt);


            UpdateUI();

            Last_DT = dt;//FPS
        }
        private void UpdateUI()
        {
            if (bShowMenu)
            {
                if (menutab == MenuTab.Options)
                {

                    if (OptionWindowItems != null)
                    {

                        foreach (UiItem item in OptionWindowItems)
                        {
                            if (IsMouseOverScreenBox(item.Box))
                            {
                                item.Hover = true;
                                item.TouchState = Lmb.TouchState;

                                if (item.TouchState == TouchState.Release)
                                {
                                    item.OnRelease();
                                }
                            }
                            else
                            {
                                item.Hover = false;
                            }
                        }
                    }

                }
            }
        }

        private void UpdateDebug(float dt)
        {
            CheatButton.Update(Keyboard.GetState().IsKeyDown(Keys.F4));
            if (CheatButton.Press())
            {
                DoCheats();
            }

            DebugButton.Update(Keyboard.GetState().IsKeyDown(Keys.F3));
            if (DebugButton.Press()) { ShowDebug = !ShowDebug; }
            if (iFrameStamp % 20 == 0)
            {
                Fps = 1.0f / Last_DT;
            }
        }
        private void DoCheats()
        {
            Player player = GetPlayer();
            player.SelectedSubweapon = Weapon.Bomb;
            player.SwordEnabled = true;
            player.JumpBootsEnabled = true;
            player.PowerSwordEnabled = true;
            player.ShieldEnabled = true;
            player.BombsEnabled = true;
            player.NumBombs = player.MaxBombs;
            player.SmallKeys = 10;
            player.BowEnabled = true;
            player.NumArrows = 99;
            player.MaxArrows = 99;
        }

        private void UpdateMenu(float dt)
        {
            Guy g = GetPlayer();

            if (g.Joystick.Menu.Press())
            {
                if (bShowMenu == false)
                {
                    Res.Audio.PlaySound(Res.SfxShowMenu);
                    ShowMenuFade = MaxShowMenuFade;
                    bShowMenu = true;
                    GameState = GameState.Pause;
                }
                else
                {
                    Res.Audio.PlaySound(Res.SfxHideMenu);
                    bShowMenu = false;
                    GameState = GameState.Play;
                }

                //Menu tab clicking
                //just hack it

            }
            if (bShowMenu == true)
            {
                ShowMenuFade -= dt;

                if (Lmb.Press())
                {
                    if (IsMouseOverScreenBox(MenuTab0Box))
                    {
                        menutab = MenuTab.Inventory;
                    }
                    if (IsMouseOverScreenBox(MenuTab1Box))
                    {
                        menutab = MenuTab.Map;
                    }
                    if (IsMouseOverScreenBox(MenuTab2Box))
                    {
                        menutab = MenuTab.Options;
                    }
                }

            }
        }
        private void UpdatePhysics(float dt)
        {
            //updateobjects, update game objects
            //update the play game state
            EmittersFrame.Clear();
            Player player = GetPlayer();

            //Need this to come first for the ball physics deflections.
            if (ShieldObject != null)
            {
                //Basically, update the boudn box
                ShieldObject.Update(Screen.Game.Input, dt);
                ShieldObject.CalcBoundBox();
            }

            DebugContactPointsToDraw = new List<vec2>();
            //Update objects
            for (int iObj = Level.GameObjects.Count - 1; iObj >= 0; iObj--)
            {
                if (Level.GameObjects[iObj].IsDeleted)
                {
                    continue;
                }

                GameObject ob = Level.GameObjects[iObj];

                Guy g = ob as Guy;
                if (g != null)
                {
                    //Always update joystick even if paused
                    g.Joystick.Update(dt);

                    if (StartRunning)
                    {
                        g.Joystick.Right.TouchState = TouchState.Down;
                    }
                }

                if (GameState == GameState.Play)
                {
                    ob.Update(Screen.Game.Input, dt, false);

                    //Update guy physics
                    if (g != null)
                    {
                        UpdateOb_Guy(g, dt);

                        Player p = g as Player;
                        if (p != null)
                        {
                            UpdateOb_Player(p, dt);
                        }
                    }
                    else
                    {
                        //Update bomb logic
                        if (ob as Bomb != null)
                        {
                            UpdateOb_Bomb(ob as Bomb, player, dt);
                        }
                        if (ob as Arrow != null)
                        {
                            UpdateOb_Arrow(ob as Arrow, dt);
                        }
                        if (ob as PickupItem != null)
                        {
                            UpdateOb_PickupItem(ob as PickupItem, player, dt);
                        }
                        if (ob as Projectile != null)
                        {
                            UpdateOb_Projectile(ob as Projectile, dt);
                        }
                        if (ob.TileId == Res.BrazierTileId)
                        {
                            Player p = GetPlayer();
                            if (p.SwordModifier == SwordModifier.Tar && p.SwordOnFire == false)
                            {
                                vec2 hp = GetSwordHitPoint(p);
                                if (ob.Box.ContainsPointInclusive(hp))
                                {
                                    p.SwordOnFire = true;
                                    Res.Audio.PlaySound(Res.SfxSwordLightFire);
                                    //p.EmitLight = true;
                                    p.EmitColor = Player.FireSwordEmitColor();
                                    p.EmitRadiusInPixels = 16 * 4;
                                }
                            }

                        }

                        //Update all Ball style physics
                        if (ob.Parent == null)
                        {
                            if (ob.PhysicsShape == PhysicsShape.Ball)
                            {
                                ob.CollidedWithSomething = false;
                                ob.CollidedWithSomething = BallPhysics(ob, dt);

                                //Shield Bounce
                                if (ob.CanDeflectWithShield /*&& !ob.DeflectedWithShield*/)
                                {
                                    //Make sure that we're coming towards teh shield.
                                    if (ob.Vel.Dot(player.ShieldClickNormal) < 0)
                                    {
                                        if (player.ShieldOut && player.ShieldEnabled && ShieldObject != null)
                                        {
                                            float r2 = (ob.PhysicsBallRadiusPixels + ShieldObject.PhysicsBallRadiusPixels);
                                            r2 *= r2;
                                            if ((ob.Box.Center() - ShieldObject.Box.Center()).Len2() < r2)
                                            {
                                                //Instead of anything fancy, Multiplying V * -1 makes the projectile go back on its trajectory exactly and hit the guy.
                                                if (ob.DeflectType == DeflectType.ExactReverse)
                                                {
                                                    ob.Vel *= -1;
                                                }
                                                else
                                                {
                                                    vec2 v = ob.Box.Center() - ShieldObject.Box.Center();
                                                    v = v.Normalized() * ob.Vel.Len();
                                                    ob.Vel = v;
                                                }

                                                if (Time - LastShieldDeflect > 0.1f)
                                                {
                                                    float kickback = 0;
                                                    if (ob is Bomb)
                                                    {
                                                        //Not a bomb explosion but the bomb object hitting shield
                                                        kickback = 10;
                                                    }

                                                    PerformShieldDeflect(kickback);


                                                    //Res.Audio.PlaySound(Res.SfxShieldDeflect);
                                                    LastShieldDeflect = Time;
                                                }
                                                ob.DeflectedWithShield = true;
                                            }
                                        }


                                    }

                                }


                            }
                            else
                            {
                                ob.Vel += ob.Acc * dt;// * waterdamp;
                                ob.Vel += ob.Gravity * dt;// * waterdampgrav;
                                                          //No physics.
                            }
                        }
                    }
                }

                //Collect emitters
                if (ob.EmitLight)
                {
                    Box2f b = ob.GetEmitterBox();
                    if (Screen.Viewport.Box.BoxIntersect_EasyOut_Inclusive(b))
                    {
                        EmittersFrame.Add(ob);
                    }
                }

                ob.CalcBoundBox();
            }


            //Remove deleted gameobjects
            for (int iObj = Level.GameObjects.Count - 1; iObj >= 0; iObj--)
            {
                if (Level.GameObjects[iObj].IsDeleted)
                {
                    //Check for player death.
                    if (Level.GameObjects[iObj] is Player)
                    {
                        if (GameState == GameState.Play)
                        {
                            GameState = GameState.PlayerDeath_Begin;
                        }
                    }
                    else
                    {
                        Level.GameObjects.RemoveAt(iObj);
                    }
                }
            }
        }
        private void PerformShieldDeflect(float power)
        {
            Player p = GetPlayer();
            vec2 n = (ShieldObject.Box.Center() - p.Box.Center()).Normalized();

            Res.Audio.PlaySound(Res.SfxShieldDeflect);
            CreateBlastParticles(ShieldObject.Box.Center(), new vec4(1, 1, .8914f, 1), 10, "SprParticleBig", n, MathHelper.ToRadians(30));

            p.Vel = -n * power;


        }
        private void UpdateOb_Arrow(Arrow arrow, float dt)
        {
            if (arrow.StickTime > 0)
            {
                arrow.StickTime -= dt;

                if (arrow.StickTime < Arrow.MaxStickTime * 0.2f)
                {
                    arrow.Alpha = arrow.StickTime / (Arrow.MaxStickTime * 0.2f);
                }

                if (arrow.StickTime <= 0)
                {
                    FlagDeleteObject(arrow);
                }
            }
            else
            {
                //not stuck
                foreach (GameObject ob in Level.GameObjects)
                {
                    if (ob as Guy != null)
                    {
                        if (ob as Player == null)
                        {
                            ////(arrow.WorldPos() - ob.WorldPos()).Len2() < (ob.Box.Width() * 0.5f) * (ob.Box.Width() * 0.5f))
                            if (ob.Box.ContainsPointInclusive(arrow.WorldPos()))
                            {
                                arrow.Origin = ob.Box.Center() - ob.Pos + arrow.Origin * 0.5f;// (arrow.Pos+arrow.Origin) - (ob.Pos+ob.Origin);//Make pos relative
                                arrow.Parent = ob;
                                arrow.Pos = 0;//**Must set to zero
                                arrow.RotateToTrajectory = false;
                                arrow.StickTime = Arrow.MaxStickTime;

                                AttackBadGuy(ob as Guy, 2);

                                break;
                            }
                        }

                    }

                }

            }

        }
        private void FlagDeleteObject(GameObject ob)
        {
            //instead of dleeting things in the obj loops we flag them for deletion
            ob.IsDeleted = true;

        }
        private bool IsMouseOverObjectBox(GameObject ob)
        {
            return IsMouseOverWorldBox(ob.Box);
        }
        private bool IsMouseOverWorldBox(Box2f b)
        {
            //return true if mouse point isinside of pixel-screen world box

            vec2 vMouse = new vec2(Mouse.GetState().Position);// Screen.Game.Input.LastTouch;

            Box2f box = new Box2f(
                Level.World.Screen.Viewport.WorldToDevice(b.Min),
                Level.World.Screen.Viewport.WorldToDevice(b.Max));
            if (box.ContainsPointInclusive(vMouse))
            {
                return true;
            }

            return false;
        }
        private bool IsMouseOverScreenBox(Box2f b)
        {
            //return true if mouse point isinside of pixel-screen world box

            vec2 vMouse = new vec2(Mouse.GetState().Position);// Screen.Game.Input.LastTouch;

            Box2f box = new Box2f(
                Level.World.Screen.Viewport.ScreenToDevice(b.Min),
                Level.World.Screen.Viewport.ScreenToDevice(b.Max));
            if (box.ContainsPointInclusive(vMouse))
            {
                return true;
            }

            return false;
        }
        private void CollectClickedOrHoveredObjects()
        {
            MouseObjectsFrame = new List<GameObject>();
            // HoverObjectsFrame = new List<GameObject>();

            HoverState = HoverState.Normal;
            Player player = GetPlayer();
            //Run through and click objects
            if (player.ItemHeld != null)
            {
                HoverState = HoverState.Aim;
            }
            if (player.BowOut == true)
            {
                HoverState = HoverState.Aim;
            }

            foreach (GameObject ob in ViewportObjsFrame)
            {
                if (IsMouseOverObjectBox(ob))
                {
                    if (HoverState == HoverState.Normal)
                    {
                        if (ob is Door || ob is TreasureChest || ob.TileId == Res.SavePointTileId)
                        {
                            HoverState = HoverState.Interact;
                        }
                        else if (ob is Guy)
                        {
                            if ((ob as Guy).AIState != AIState.Player)
                            {
                                if ((ob as Guy).IsNpc == true)
                                {
                                    HoverState = HoverState.Talk;
                                }
                                else
                                {
                                    HoverState = HoverState.Attack;

                                }
                            }
                        }
                    }



                    MouseObjectsFrame.Add(ob);

                }
            }
        }
        private void UpdatePostPhysics(float dt)
        {
            //Collect visible Objs, now that the player has moved
            CollectVisibleCells(dt);

            CollectClickedOrHoveredObjects();

            //Create Lava/Water particles AND update the lava screen distort
            UpdateLava(dt);

            Player player = GetPlayer();
            //Mine the world, Attack
            if (GameState == GameState.Play)
            {

                //Must come before swingsword in order to not swing sword when we're doing other thngs
                CollidePortalsAndTriggers(player, dt);

                //Kill things
                if (player.SwordEnabled && player.ItemHeld == null)
                {

                    if (player.SwordSwingDelay < player.SwordSwingDelayMax && player.SwordSwingDelay > 0)
                    {
                        DamageTiles(player.Power, GetSwordHitPoint(player));
                    }
                }


                if (iFrameStamp % 2 == 0 || bLoadingRoom)
                {
                    UpdateLiquid(dt);
                }

            }
            else if (GameState == GameState.PlayerDeath_Begin)
            {
                PlayPlayerDeathCutscene();
            }

            //Update light every 3 frames, OR when the player has moved too fast in the current frame
            //Prevents the border-screen "flicker" that we get from the player moving too fast.
            if ((iFrameStamp % 3 == 0) ||
                (Math.Abs(LastLightVPPos.x - Screen.Viewport.Pos.x) >= Res.Tiles.TileWidthPixels * 0.5f) ||
                (Math.Abs(LastLightVPPos.y - Screen.Viewport.Pos.y) >= Res.Tiles.TileHeightPixels * 0.5f)
                || bLoadingRoom)
            {
                LastLightVPPos = Screen.Viewport.Pos;
                //Gather lava cells, or cells with water that emits light
                //Add particles as emitters, if they emit
                CollectAdditionalEmitters();
                DoLight();
            }

        }
        private void PlayPlayerDeathCutscene()
        {
            vec2 GuyPos = GetPlayer().Pos;
            float rot = 0;
            float animDuration = 2.5f;
            float rotSpdDelta = animDuration;
            float rotSpd = animDuration * 4;

            //Action<CutsceneSequence, SpriteBatch> playerdead_draw = (s, sb) =>
            //{
            //    Frame fr = Res.Tiles.GetSpriteFrame(Res.SprGuyDead, 0);
            //    DrawObject(sb, p, new vec4(1, 1, 1, 1));

            //};

            Cutscene = new Cutscene()
                .Then(0, (s, dt) =>
                {
                    Res.Audio.PlaySound(Res.SfxPickupItem);
                    GameState = GameState.PlayerDeath_Animate;
                    //  DrawState = DrawState.PlayerOnly;
                    return false;
                })
                .Then(animDuration, (s, dt) =>
                {
                    rot += rotSpd * dt;
                    rotSpd -= rotSpd * rotSpdDelta * dt;
                    Player p = GetPlayer();
                    p.Rotation = rot;
                    p.Animate = false;
                    p.SetSprite(Res.SprGuyDead, false);
                    return s.Duration > 0;
                })
                .Then(0, (s, dt) =>
                {
                    bShowMenu = false;
                    GameState = GameState.PlayerDeath_ShowContinue;
                    return s.Duration > 0;
                });
        }
        private void OpenOrUnlockDoor(Player player, Door obOtherDoor)
        {
            if (obOtherDoor.Locked)
            {
                if (obOtherDoor.IsElectronic == false && player.SmallKeys > 0)
                {
                    UnlockDoor(obOtherDoor);
                    player.SmallKeys -= 1;
                }
                else
                {
                    obOtherDoor.ShakeTime = 0.3f;
                    obOtherDoor.ShakeAmountPixels = 1.0f;
                    if (obOtherDoor.IsElectronic)
                    {
                        Res.Audio.PlaySound(Res.SfxElectronicNogo);
                    }
                    else
                    {
                        Res.Audio.PlaySound(Res.SfxDoorLocked);
                    }
                }
            }
            else
            {
                if (obOtherDoor.Open == false)
                {
                    obOtherDoor.Open = true;
                    obOtherDoor.UpdateSprite();
                    Res.Audio.PlaySound(Res.SfxDoorOpen);
                }
                else
                {
                    obOtherDoor.Open = false;
                    obOtherDoor.UpdateSprite();
                    Res.Audio.PlaySound(Res.SfxDoorClose);
                }
            }


        }
        private void UnlockDoor(Door d)
        {
            Res.Audio.PlaySound(Res.SfxDoorUnlock);
            d.Locked = false;
            d.UpdateSprite();
            Level.SerializeLevel();
        }
        private void UpdateOb_PickupItem(PickupItem ob, Player player, float dt)
        {

            ob.DieTime -= dt;
            ob.PickupTime -= dt;
            if (ob.PickupTime < 0) { ob.PickupTime = 0; }
            if (ob.DieTime < 3.0f)
            {
                ob.Alpha = (ob.DieTime / 3.0f);
            }
            if (ob.DieTime <= 0)
            {
                FlagDeleteObject(ob);
                //Level.GameObjects.RemoveAt(iObj);
            }
            else
            {
                if (ob.PickupTime <= 0)
                {
                    if (player.Box.BoxIntersect_EasyOut_Inclusive(ob.Box))
                    {
                        vec4 particleColor = new vec4(1, 1, 1, 1);
                        //Level.GameObjects.RemoveAt(iObj);
                        FlagDeleteObject(ob);
                        if (ob.PickupItemType == PickupItemType.Bomb)
                        {
                            player.NumBombs += 1;
                            if (player.NumBombs > player.MaxBombs) { player.NumBombs = player.MaxBombs; }
                            particleColor = new vec4(0, 0, 0, 1);
                            Res.Audio.PlaySound(Res.SfxPickupItem);
                        }
                        else if (ob.PickupItemType == PickupItemType.Arrow)
                        {
                            player.NumArrows += 1;
                            if (player.NumArrows > player.MaxArrows) { player.NumArrows = player.MaxArrows; }
                            particleColor = new vec4(0, 0, 0, 1);
                            Res.Audio.PlaySound(Res.SfxPickupItem);
                        }
                        else if (ob.PickupItemType == PickupItemType.Potion)
                        {
                            player.Health += 1;
                            if (player.Health > player.MaxHealth) { player.Health = player.MaxHealth; }
                            particleColor = new vec4(1, .2f, .2f, 1);
                        }
                        else if (ob.PickupItemType == PickupItemType.Marble)
                        {
                            player.Money += 1;
                            if (player.Money > player.MaxMoney) { player.Money = player.MaxMoney; }
                            particleColor = new vec4(1, 1f, 1f, 1);
                            Res.Audio.PlaySound(Res.SfxPickupItem);
                        }
                        CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 5, ob.Box.Center(), -1, -70, particleColor, 0.9f, 4.7f, 0, 0, Res.Tiles.GetWHVec() * 0.5f);

                    }
                }

            }
        }
        private void UpdateOb_Projectile(Projectile ob, float dt)
        {
            if (Level.Grid.RootNode.Box.BoxIntersect_EasyOut_Inclusive(ob.Box) == false)
            {
                FlagDeleteObject(ob);
                //Outside room
            }
            else
            {
                ob.Pos += ob.Vel * dt;
                if (DamageTiles(ob.Power, ob.Box))
                {
                    //Destroy
                    FlagDeleteObject(ob);
                }
            }
        }
        private void CollectVisibleCells(float dt)
        {
            //**Must call after physics 
            ViewportCellsFrame = Level.Grid.GetCellManifoldForBox(Screen.Viewport.Box);
            ViewportObjsFrame.Clear();
            foreach (GameObject ob in Level.GameObjects)
            {
                if (Screen.Viewport.Box.ContainsPointInclusive(ob.Box.Center()))
                {
                    ViewportObjsFrame.Add(ob);
                }
            }

        }
        private void UpdateOb_Guy(Guy g, float dt)
        {
            g.UpdateHeldItem();

            //make animation faster based on vel
            ///fl

            float vl = g.Vel.Len2() / (g.MaxVel * g.MaxVel);
            g.AnimationSpeed = 1 + vl;

            UpdateGuyState(g, dt);
            DoPhysics(g, dt);
            CheckGuyInWater(g, dt);

            if (g is Player)
            {
                foreach (GameObject ob in Level.GameObjects)
                {
                    if ((ob is Duck) && ob.IsDeleted == false)
                    {
                        if ((g.Pos - ob.Pos).Len2() <= (7 * 7))
                        {
                            Res.Audio.PlaySound(Res.SfxDuckQuack);
                            FlagDeleteObject(ob);
                            if (g.Vel.y > 0)
                            {
                                //Go back up if we are going down.
                                g.Vel.y *= -3f;
                            }
                        }
                    }
                }
            }

            if (g.InWater)
            {
                EndGame();
            }
        }
        public void EndGame()
        {
            //Fix this
            GameState = GameState.PlayerDeath_Begin;
            GetPlayer().Joystick.IsUser = false;
            GetPlayer().VaporTrail = 0;
        }
        int LastMouseWheel = 0;
        bool F12Down = false;
        private void UpdateOb_Player(Player p, float dt)
        {
            //reset interaction flag
            p.HasInteractedThisFrame = false;

            //Udpate Grass collision time
            p.LastCollideGrass -= dt;
            if (p.LastCollideGrass < 0) { p.LastCollideGrass = 0; }

            //Update Subweapoln
            // UpdateSubWeapon(p, dt);

            if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                if (F12Down == false)
                {
                    F12Down = true;
                    (Screen.Game as MainGame).SetGraphicsOptions(!(this.Screen.Game as MainGame).GraphicsDeviceManager.IsFullScreen, null);
                }
            }
            else
            {
                F12Down = false;
            }


            if (Mouse.GetState().ScrollWheelValue != LastMouseWheel)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt))
                {
                    float diff = Mouse.GetState().ScrollWheelValue - LastMouseWheel;
                    Screen.Viewport.TilesHeight += diff * 0.01f;

                    if (Screen.Viewport.TilesHeight < 4)
                    {
                        Screen.Viewport.TilesHeight = 4;
                    }
                    if (Screen.Viewport.TilesHeight > 12)
                    {
                        Screen.Viewport.TilesHeight = 12;
                    }
                }
                LastMouseWheel = Mouse.GetState().ScrollWheelValue;
                Weapon last = p.SelectedSubweapon;
                if (p.SelectedSubweapon == Weapon.Bomb)
                {
                    p.SelectedSubweapon = Weapon.Bow;
                }
                else if (p.SelectedSubweapon == Weapon.Bow)
                {
                    p.SelectedSubweapon = Weapon.Sword;
                }
                else if (p.SelectedSubweapon == Weapon.Sword)
                {
                    p.SelectedSubweapon = Weapon.Bomb;
                }
                else if (p.SelectedSubweapon == Weapon.None)
                {
                    p.SelectedSubweapon = Weapon.Bomb;
                }
                if (p.SelectedSubweapon != last)
                {
                    Res.Audio.PlaySound(Res.SfxChangeSubweapon);
                }
            }


            //Update Fire Sword Particles
            //if (p.SwordOnFire)
            //{
            //    vec2 hp = GetSwordHitPoint(p);
            //    p.SwordOnFireTime -= dt;
            //    if (p.SwordOnFireTime <= 0)
            //    {
            //        p.SwordOnFireTime = p.SwordOnFireTimeMax;
            //        int make = Globals.RandomInt(4, 8);
            //        for (int i = 0; i < make; i++)
            //        {
            //            CreateParticles(Res.SprParticleSmall, ParticleLife.Scale_Zero, 1, hp + new vec2(Globals.Random(-3, 3), Globals.Random(-3, 3)), 2, 6, Player.FireSwordEmitColor(),
            //                0.8f, 6.06f, -3.14f, 3.14f, new vec2(8, 8), -Gravity,
            //                false, new vec2(Globals.Random(-500, 500), -100), 2.0f, true,
            //                new vec2(Globals.Random(-1, 1), -1), (float)Math.PI * 0.5f, 0, 0, true, Player.FireSwordEmitColor(), 0,
            //                false, false);
            //        }
            //    }
            //}
            ////Liquid Sword Modifier - Lava, Tar, Water
            ////Update Sword Luquid Modifier state.
            //if (p.SwordOut && p.SwordEnabled)
            //{
            //    vec2 hp = GetSwordHitPoint(p);
            //    Cell swordcc = Level.Grid.GetCellForPoint(hp);
            //    if (swordcc != null)
            //    {
            //        float h = swordcc.Box().Max.y - hp.y;
            //        if (h <= swordcc.Water * 16.0f)
            //        {
            //            SwordModifier prev = p.SwordModifier;

            //            if (swordcc.WaterType == WaterType.Lava)
            //            {
            //                if (prev != SwordModifier.Lava)
            //                {
            //                    if (p.SwordModifier == SwordModifier.None)
            //                    {
            //                        p.SwordModifier = SwordModifier.Lava;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Water)
            //                    {
            //                        p.SwordModifier = SwordModifier.Obsidian;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Tar)
            //                    {
            //                        p.SwordModifier = SwordModifier.Lava;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Obsidian)
            //                    {
            //                        // p.SwordModifier = SwordModifier.Lava;
            //                    }
            //                    else
            //                    {
            //                        System.Diagnostics.Debugger.Break();
            //                    }
            //                }
            //            }
            //            else if (swordcc.WaterType == WaterType.Tar)
            //            {
            //                if (prev != SwordModifier.Tar)
            //                {
            //                    if (p.SwordModifier == SwordModifier.None)
            //                    {
            //                        p.SwordModifier = SwordModifier.Tar;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Water)
            //                    {
            //                        p.SwordModifier = SwordModifier.None;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Lava)
            //                    {
            //                        p.SwordModifier = SwordModifier.Lava;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Obsidian)
            //                    {

            //                    }
            //                    else
            //                    {
            //                        System.Diagnostics.Debugger.Break();
            //                    }
            //                }
            //            }
            //            else if (swordcc.WaterType == WaterType.Water)
            //            {
            //                if (prev != SwordModifier.Water)
            //                {
            //                    if (p.SwordModifier == SwordModifier.None)
            //                    {
            //                        p.SwordModifier = SwordModifier.Water;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Lava)
            //                    {
            //                        p.SwordModifier = SwordModifier.Obsidian;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Tar)
            //                    {
            //                        p.SwordModifier = SwordModifier.None;
            //                    }
            //                    else if (p.SwordModifier == SwordModifier.Obsidian)
            //                    {

            //                    }
            //                    else
            //                    {
            //                        System.Diagnostics.Debugger.Break();
            //                    }
            //                }
            //            }
            //            else if (p.SwordModifier == SwordModifier.Obsidian)
            //            {

            //            }
            //            else
            //            {
            //                System.Diagnostics.Debugger.Break();
            //            }

            //            if (p.SwordModifier != prev)
            //            {
            //                //End the charge if we are charging.
            //                EndPowerSwordCharge(p);

            //                //Play enter water sound to indicate we changed the sword.
            //                Res.Audio.PlaySound(Res.SfxEnterWater);
            //            }

            //        }
            //    }

            // }
        }
        private vec2 GetThrowNormal(Guy guy, GameObject ob)
        {
            return GetAimNormal(guy, ob.WorldPos());
        }
        private vec2 GetAimNormal(Guy guy, vec2 world_pos)
        {
            vec2 p0 = new vec2(Mouse.GetState().Position);
            vec2 p1 = Screen.Viewport.WorldToDevice(world_pos);

            vec2 dv = (p0 - p1).Normalized();

            return dv;
        }
        private void ThrowHeldItem(Guy guy, float dt, vec2 dir, float speed)
        {
            if (guy.ItemHeld != null)
            {
                GameObject ob = guy.ItemHeld;
                if (ob.Parent != null)
                {
                    Res.Audio.PlaySound(Res.SfxThrow);
                    guy.HoldItem(null);

                    guy.LastItemThrown = ob;
                    ob.CalcRotationDelta();// RotationDelta = 3.141593f * 2.0f * (ob_bomb.Vel.x > 0 ? 1.0f : -1.0f);

                    ob.Vel = dir * speed * dt;
                }
            }
        }

        private void UpdateOb_Bomb(Bomb ob_bomb, Guy player, float dt)
        {
            //Update Bomb
            if (ob_bomb != null)
            {
                ob_bomb.Blowtime -= dt;
                if (ob_bomb.Blowtime <= 0 || (ob_bomb.ExplodeOnImpact && ob_bomb.CollidedWithSomething == true))
                {
                    //BLOW UP - Destroy Necroid Rock + other stuff
                    if (player.ItemHeld == ob_bomb)
                    {
                        player.HoldItem(null);
                    }
                    FlagDeleteObject(ob_bomb);

                    if (string.IsNullOrEmpty(ob_bomb.ExplodeSound) == false)
                    {
                        Res.Audio.PlaySound(ob_bomb.ExplodeSound);
                    }

                    ob_bomb.Exploded = true;

                    Screen.ScreenShake.Shake(10, 0.89f);

                    Box2f bomb_blast_box = new Box2f(ob_bomb.WorldPos() - ob_bomb.BlastRadiusPixels, ob_bomb.WorldPos() + ob_bomb.BlastRadiusPixels);

                    //Make smoke particles
                    for (int i = 0; i < 20; ++i)
                    {
                        vec2 rr = RandomRegion(bomb_blast_box);

                        //Make sure the smoke particles are withint he blast radius
                        vec2 dir = (rr - bomb_blast_box.Center());
                        float len = dir.Len();
                        if (len > ob_bomb.BlastRadiusPixels)
                        {
                            rr = bomb_blast_box.Center() + dir.Normalized() * ob_bomb.BlastRadiusPixels;
                        }

                        MakeSmokeParticle(rr);
                    }

                    //Get all cells withint he bomb radius, and blow them up
                    List<Cell> cells = Level.Grid.GetCellManifoldForBox(bomb_blast_box);
                    foreach (Cell c in cells)
                    {
                        float r2 = (c.Box().Center() - ob_bomb.WorldPos()).Len2();

                        if (r2 < ob_bomb.BlastRadiusPixels * ob_bomb.BlastRadiusPixels)
                        {
                            bool blewup = false;
                            //Destroy all possible objects in that cell.
                            for (int iLayer = 0; iLayer < PlatformLevel.LayerCount; ++iLayer)
                            {
                                if (c.Layers[iLayer] != null)
                                {
                                    if (c.Layers[iLayer].CanBomb)
                                    {
                                        DamageTileBlock(c, iLayer, c.Layers[iLayer], ob_bomb.Power, false);
                                        blewup = true;
                                    }
                                }
                            }

                            if (blewup)
                            {
                                for (int i = 0; i < 4; ++i)
                                {
                                    MakeSmokeParticle(RandomRegion(c.Box()));
                                }
                            }
                        }
                    }

                    //Kill Game Objects & Deflect blast with shield
                    foreach (GameObject ob in Level.GameObjects)
                    {
                        if (ob as Guy != null)
                        {
                            bool playerBlocked = false;

                            //Bombs can hurt player.
                            if (ob as Player != null)
                            {
                                //Try to deflect bomb blast with shield
                                Player p = ob as Player;
                                if (p.ShieldOut)
                                {
                                    if (ShieldObject.Box.BoxIntersect_EasyOut_Inclusive(bomb_blast_box))
                                    {

                                        vec2 v0 = (ob_bomb.Box.Center() - p.Box.Center()).Normalized();
                                        vec2 v1 = (ShieldObject.Box.Center() - p.Box.Center()).Normalized();
                                        float a = Math.Abs((float)Math.Acos(v1.Dot(v0)));
                                        float b = MathHelper.ToRadians(p.ShieldDeflectionMaxAngle);
                                        if (a < b)
                                        {
                                            playerBlocked = true;
                                            PerformShieldDeflect(280);
                                        }
                                    }
                                }
                            }
                            if (playerBlocked == false)
                            {
                                if (ob.Box.BoxIntersect_EasyOut_Inclusive(bomb_blast_box))
                                {
                                    AttackBadGuy(ob as Guy, ob_bomb.Power);
                                }
                            }
                        }
                    }

                }
                else
                {
                    ob_bomb.NextBlink -= dt;
                    if (ob_bomb.NextBlink <= 0)
                    {
                        ob_bomb.NextBlink = ob_bomb.Blowtime * Bomb.BlinkRate;
                        ob_bomb.Blink = !ob_bomb.Blink;
                    }

                    if (ob_bomb.Blink == true)
                    {
                        ob_bomb.EmitColor = ob_bomb.Color = new vec4(1, 0, 0, 1);
                    }
                    else
                    {
                        ob_bomb.EmitColor = ob_bomb.Color = new vec4(1, 1, 1, 1);
                    }
                    ob_bomb.EmitLight = true;
                    ob_bomb.EmitRadiusInPixels = 16 * 3;

                    float scale_time = 0.3f;//the last x seconds, scale up to multiply
                    float multiply_size = 1.0f;
                    if (ob_bomb.Blowtime < scale_time)
                    {
                        float scl = 1 + (1.0f - (ob_bomb.Blowtime / scale_time)) * multiply_size; //grow to twice the size

                        ob_bomb.Scale = scl * Bomb.BaseScale;
                    }

                }


            }
        }
        private vec2 RandomRegion(Box2f minmax)
        {
            vec2 rpos = new vec2(0, 0);
            rpos.x = minmax.Min.x + (minmax.Max.x - minmax.Min.x) * Globals.Random(0, 1);
            rpos.y = minmax.Min.y + (minmax.Max.y - minmax.Min.y) * Globals.Random(0, 1);
            return rpos;
        }
        private void MakeSmokeParticle(vec2 pos, float scaleDelta = -1.06f, float scaleStart = 1.0f)
        {
            CreateParticles(Res.SprParticleCloud, ParticleLife.Alpha_Zero, 1, pos, 2, 6, new vec4(1, 1, 1, 1),
            0.8f, scaleDelta, -3.14f, 3.14f, new vec2(8, 8), new vec2(0, 0),
            true, default(vec2), scaleStart, false, default(vec2), 0, 0, 0, false, default(vec4), 0, true);
        }
        private void CollectAdditionalEmitters()
        {
            //Gather lava cells, or cells with water that emits light
            foreach (Cell c in Level.Grid.CellDict.Values)
            {
                float lavaEmitRadiusPixels = 16 * 2;
                float tarEmitRadiusPixels = 16 * 2;

                if (c.Water > 0)
                {
                    if (c.WaterType == WaterType.Lava)
                    {
                        Box2f emitBox = new Box2f(
                            new vec2(c.Pos() - lavaEmitRadiusPixels),
                            new vec2(c.Pos() + lavaEmitRadiusPixels));

                        if (Screen.Viewport.Box.BoxIntersect_EasyOut_Inclusive(emitBox))
                        {
                            //Create new emitter and add.
                            GameObject ob = new GameObject(this);
                            ob.Pos = c.Pos();
                            ob.EmitRadiusInPixels = lavaEmitRadiusPixels;
                            ob.EmitColor = WaterColorFromType(c.WaterType);
                            ob.EmitLight = true;
                            ob.CalcBoundBox();
                            EmittersFrame.Add(ob);
                        }
                    }
                    else if (c.WaterType == WaterType.Tar)
                    {
                        Box2f emitBox = new Box2f(
                            new vec2(c.Pos() - tarEmitRadiusPixels),
                            new vec2(c.Pos() + tarEmitRadiusPixels));

                        if (Screen.Viewport.Box.BoxIntersect_EasyOut_Inclusive(emitBox))
                        {
                            //Create new emitter and add.
                            GameObject ob = new GameObject(this);
                            ob.Pos = c.Pos();
                            ob.EmitRadiusInPixels = tarEmitRadiusPixels;
                            ob.EmitColor = WaterColorFromType(c.WaterType);
                            ob.EmitLight = true;
                            ob.CalcBoundBox();
                            EmittersFrame.Add(ob);
                        }
                    }

                }
            }

            //Add particles as emitters, if they emit
            foreach (GameObject ob in Particles)
            {
                if (ob.EmitLight)
                {
                    EmittersFrame.Add(ob);
                }
            }

        }
        private void CreateBlastParticles(vec2 pos, vec4 color, int count = 20, string sprite = Res.SprParticleBig, vec2 aperature_n = default(vec2), float aperature_r = 6.28f)
        {
            CreateParticles(sprite, ParticleLife.Scale_Zero, count,
                pos, 70.5f, 1000.0f,
                color, 0.0f, 0.3f, 90, 150, Res.Tiles.GetWHVec() * 0.5f,
                default(vec2), false, default(vec2), 0.1f,
                true, aperature_n, aperature_r, 30, 90,
                false, default(vec4), 0, false, false);
        }
        private void UpdateLava(float dt)
        {
            LastParticle -= dt;

            if (LastParticle <= 0)
            {
                bool bMade = false;
                LastParticle = Globals.Random(3.0f, 6.0f);
                int nCount = 3;
                foreach (Cell c in ViewportCellsFrame)
                {
                    if (nCount <= 0) break;
                    if (c.Water > 0.1f)
                    {
                        if (c.WaterType == WaterType.Lava || c.WaterType == WaterType.Tar)
                        {
                            for (int i = 0; i < 1; ++i)
                            {
                                if (Globals.RandomBool())
                                {
                                    vec4 color = WaterColorFromType(c.WaterType);

                                    float addl = Globals.Random(-0.3f, 0.3f);
                                    color.x += addl;//r,g
                                    color.y += addl;//r,g
                                    color.Clamp(0, 1);

                                    CreateParticles(Res.SprParticleBig, ParticleLife.Scale_Zero, 1,
                                        c.Box().Center() + Globals.Random(-8, 8), 0, 0,
                                        color, 0.0f, 0.3f, 10, 50, Res.Tiles.GetWHVec() * 0.5f,
                                        Gravity * 0.6f, false, default(vec2), 0.3f,
                                        true, new vec2(0, -1), PI / 6, 10, 80,
                                        true, color, Res.Tiles.TileWidthPixels * 1.5f);
                                    bMade = true;
                                    nCount--;
                                }
                            }
                        }

                    }
                }

                if (bMade)
                {
                    Res.Audio.PlaySound(Res.SfxLavaBall);
                }
            }


        }
        List<vec2> DebugContactPointsToDraw;
        private bool BallPhysics(GameObject obj, float dt)
        {
            //Physics for ball objects
            obj.Vel += obj.Acc * dt;// * waterdamp;
            obj.Vel += obj.Gravity * dt;// * waterdampgrav;

            if (obj.Vel.Len2() <= 0)
            {
                return false;
            }

            float r = obj.PhysicsBallRadiusPixels;

            Box2f speedbox = new Box2f();
            speedbox.GenResetExtents();
            speedbox.ExpandByPoint(obj.Pos + obj.Origin + obj.Vel - r);
            speedbox.ExpandByPoint(obj.Pos + obj.Origin + obj.Vel + r);
            speedbox.ExpandByPoint(obj.Pos + obj.Origin - r);
            speedbox.ExpandByPoint(obj.Pos + obj.Origin + r);


            obj.Pos += obj.Vel * dt;// * waterdamp;
            vec2 cpos = obj.WorldPos();

            List<Cell> Tiles = Level.Grid.GetCellManifoldForBox(speedbox);
            Tiles.Sort((x, y) => (x.Box().Center() - cpos).Len2().CompareTo((y.Box().Center() - cpos).Len2()));

            vec2 n = new vec2(0, 0);
            vec2 slide_dir = new vec2(0, 0);

            bool bDidCollide = false;
            foreach (Cell cell in Tiles)
            {
                TileBlock block = cell.Layers[PlatformLevel.Midground];
                if (block == null)
                {
                    continue;
                }

                if (block.Tile.Blocking)
                {
                    if (block.Box.BoxIntersect_EasyOut_Inclusive(obj.Box))
                    {
                        bool collided = CollideBallOnBlock(block, r, obj.Vel, ref cpos, ref n, ref slide_dir);

                        bDidCollide = bDidCollide || collided;
                        //Response
                        //The collided check is needed because for the sides we test against the block cube, but slopes test the circle
                        if (collided)
                        {
                            if (obj.PhysicsResponse == PhysicsResponse.Slide_And_Roll)
                            {
                                obj.Vel = SlideSlope(obj.Pos, obj.Vel, n);

                                // obj.Vel = slide_dir * obj.Vel.Len();
                                obj.Vel = obj.Vel * obj.Friction;
                                obj.CalcRotationDelta();
                            }
                            else if (obj.PhysicsResponse == PhysicsResponse.Bounce_And_Roll)
                            {
                                obj.Vel = obj.Vel - n * (obj.Vel.Dot(n)) * 2.0f;
                                obj.Vel = obj.Vel * obj.Friction;
                                obj.Vel *= obj.Bounciness;
                                obj.CalcRotationDelta();
                            }
                            else if (obj.PhysicsResponse == PhysicsResponse.StayPut)
                            {
                                obj.Vel = obj.Vel - n * (obj.Vel.Dot(n)) * 2.0f;
                                obj.Vel = obj.Vel * obj.Friction;
                            }
                            else if (obj.PhysicsResponse == PhysicsResponse.StickIntoGround)
                            {
                                if (obj as Arrow != null)
                                {
                                    (obj as Arrow).StickTime = Arrow.MaxStickTime;
                                }
                                if (obj.RotateToTrajectory == true)
                                {
                                    obj.RotateToTrajectory = false;
                                }
                                obj.Vel = 0;
                                obj.Gravity = 0;
                            }
                            obj.LastCollideNormal = n;

                            //Recompute
                            obj.Pos = cpos - obj.Origin;
                            cpos = obj.WorldPos();// obj.Pos +obj.Origin;
                            obj.CalcBoundBox();

                            //ball_box = new Box2f(cpos - r, cpos + r);
                        }



                    }//If box

                }// if blocking
            }


            obj.CalcBoundBox();
            return bDidCollide;
        }
        private vec2 GetNormalForCollidedBlock(TileBlock block, vec2 cpos, float r)
        {
            vec2 n = new vec2(0, 0);
            if (block.IsSlope())
            {
                if (block.SlopeTileId == Res.SlopeTile_BR)
                {
                    n.Construct(-1, -1);
                    n.Normalize();
                }
                else if (block.SlopeTileId == Res.SlopeTile_BL)
                {
                    n.Construct(1, -1);
                    n.Normalize();
                }
            }
            else
            {
                int side = GetClosestPenetrateSide(cpos, r, block.Box);

                if (side == 0)
                {
                    n.Construct(-1, 0);
                }
                else if (side == 1)
                {
                    n.Construct(1, 0);
                }
                else if (side == 2)
                {
                    n.Construct(0, -1);
                }
                else if (side == 3)
                {
                    n.Construct(0, 1);
                }

            }

            return n;
        }
        private bool CollideBallOnBlock(TileBlock block, float r, vec2 vel, ref vec2 cpos, ref vec2 n, ref vec2 slide_dir, bool force_place = false)
        {
            bool collided = false;
            if (block.IsSlope())
            {
                if (block.SlopeTileId == Res.SlopeTile_BR)
                {
                    vec2 a = new vec2(block.Box.Min.x, block.Box.Max.y);
                    vec2 b = new vec2(block.Box.Max.x, block.Box.Min.y);
                    float pctx = (cpos.x - block.Box.Min.x) / Res.Tiles.TileWidthPixels;
                    if (pctx >= 0.0f && pctx <= 1.0f)
                    {
                        float topy = a.y + (b.y - a.y) * pctx;

                        if (force_place || ((cpos.y + r) > topy))
                        {
                            collided = true;
                            cpos.y = topy - r;
                            n.Construct(-1, -1);
                            n.Normalize();
                        }
                    }
                }
                else if (block.SlopeTileId == Res.SlopeTile_BL)
                {
                    vec2 a = new vec2(block.Box.Min.x, block.Box.Min.y);
                    vec2 b = new vec2(block.Box.Max.x, block.Box.Max.y);
                    float pctx = (cpos.x - block.Box.Min.x) / Res.Tiles.TileWidthPixels;
                    if (pctx >= 0.0f && pctx <= 1.0f)
                    {
                        float topy = a.y + (b.y - a.y) * pctx;

                        if (force_place || ((cpos.y + r) > topy))
                        {
                            collided = true;
                            cpos.y = topy - r;
                            n.Construct(1, -1);
                            n.Normalize();

                        }
                    }
                }
            }
            else
            {
                int side = GetClosestPenetrateSide(cpos, r, block.Box);

                if (side == 0)
                {
                    //  if(Math.Abs(cpos.x - block.Pos.x) < r)
                    {
                        collided = true;
                        cpos.x = block.Pos.x - r - 0.001f;
                        n.Construct(-1, 0);
                        slide_dir.Construct(0, vel.y < 0 ? -1 : 1);
                    }
                }
                else if (side == 1)
                {
                    // if (Math.Abs(cpos.x - (block.Pos.x+Res.Tiles.TileWidthPixels)) < r)
                    {
                        collided = true;
                        n.Construct(1, 0);
                        cpos.x = block.Pos.x + Res.Tiles.TileWidthPixels + r + 0.001f;
                        slide_dir.Construct(0, vel.y < 0 ? -1 : 1);
                    }
                }
                else if (side == 2)
                {
                    // if (Math.Abs(cpos.y - block.Pos.y) < r)
                    {
                        collided = true;
                        cpos.y = block.Pos.y - r - 0.001f;
                        n.Construct(0, -1);
                        slide_dir.Construct(vel.x < 0 ? -1 : 1, 0);
                    }
                }
                else if (side == 3)
                {
                    //   if (Math.Abs(cpos.y - (block.Pos.y + Res.Tiles.TileHeightPixels)) < r)
                    {
                        collided = true;
                        cpos.y = block.Pos.y + Res.Tiles.TileHeightPixels + r + 0.001f;
                        n.Construct(0, 1);
                        slide_dir.Construct(vel.x < 0 ? -1 : 1, 0);
                    }
                }

            }


            return collided;
        }
        private int GetClosestPenetrateSide(vec2 pos, float r, Box2f box)
        {
            //0 = Left, 1 = Right, 2=  Top, Bottom = 3
            float cA = (float)Math.Abs(pos.x + r - box.Min.x);//Left
            float cB = (float)Math.Abs(pos.x - r - box.Max.x);//
            float cC = (float)Math.Abs(pos.y + r - box.Min.y);
            float cD = (float)Math.Abs(pos.y - r - box.Max.y);

            if (cA <= cB && cA <= cC && cA <= cD) { return 0; }
            if (cB <= cA && cB <= cC && cB <= cD) { return 1; }
            if (cC <= cB && cC <= cA && cC <= cD) { return 2; }
            if (cD <= cB && cD <= cC && cD <= cA) { return 3; }

            return 0;
        }
        public Box2f GetSwordHitBox(Player player)
        {
            vec2 hp = GetSwordHitPoint(player);
            vec2 po = GetMovableItemOrigin(player);
            Box2f b = new Box2f();
            b.GenResetExtents();
            b.ExpandByPoint(hp);
            b.ExpandByPoint(po);
            return b;
        }
        private bool PlayerIsNearObject(GameObject ob, Player player)
        {
            bool b = (ob.Box.Center() - player.Box.Center()).Len2() <= (Res.Tiles.TileWidthPixels * Res.Tiles.TileWidthPixels);

            return b;
        }
        private bool MousePress(GameObject ob)
        {
            return (Screen.Game.Input.Global.PressOrDown() && MouseObjectsFrame.Contains(ob));
        }
        private bool MouseRelease(GameObject ob)
        {
            return (Screen.Game.Input.Global.PressOrDown() && MouseObjectsFrame.Contains(ob));
        }
        private void CollidePortalsAndTriggers(Player player, float dt)
        {

            //This really only collides portals.
            bool bActionButtonPressed = player.Joystick.Ok.Press();

            //CollidedTriggersFrame = new List<GameObject>();
            for (int i = ViewportObjsFrame.Count - 1; i >= 0; i--)
            {
                //colliding with guy box was giving erroneous results (for most things)
                //We collide box with doors
                Door obDoor = ViewportObjsFrame[i] as Door;
                if (obDoor != null)
                {

                    //Open the door
                    if (obDoor.AlwaysOpen == false)
                    {
                        if (this.Screen.Game.Input.Global.Press())
                        {
                            //Check the distance is less than or equal to 1 tile
                            if (PlayerIsNearObject(obDoor, player))
                            {
                                if (MousePress(obDoor))
                                {
                                    OpenOrUnlockDoor(player, obDoor);
                                    player.HasInteractedThisFrame = true;
                                }

                            }
                        }
                        else if (Screen.Game.Input.Global.Down())
                        {
                            //Open door with sword
                            if (player.SwordOut)
                            {
                                if (LastSwordCollideDoor != obDoor)
                                {
                                    if (obDoor.Open == false)//To prevent the annoying open/close busines just allow sword to OPEN doors, not close
                                    {

                                        Box2f b = GetSwordHitBox(player);

                                        if (obDoor.Box.BoxIntersect_EasyOut_Inclusive(b))
                                        {
                                            OpenOrUnlockDoor(player, obDoor);

                                            LastSwordCollideDoor = obDoor;
                                        }
                                    }

                                }

                            }
                        }
                        else if (Screen.Game.Input.Global.ReleaseOrUp())
                        {
                            LastSwordCollideDoor = null;

                        }

                    }

                    //Warp, if the door is open
                    if (obDoor.Box.BoxIntersect_EasyOut_Inclusive(player.Box))
                    {
                        //Either guy is moving in the direction of the door, OR the guy is sitting/standing on it and he clicked it
                        if (obDoor.IsEntering(player) || MousePress(obDoor))
                        {
                            if (obDoor.Open || obDoor.AlwaysOpen)
                            {
                                if (obDoor.IsPortalDoor)
                                {
                                    (Screen as GameScreen).TransitionDoor = obDoor;
                                }
                            }
                        }
                    }

                }
                else if (ViewportObjsFrame[i] as Sign != null)
                {
                    if (ViewportObjsFrame[i].Box.ContainsPointInclusive(player.cposC))
                    {
                        if (MouseRelease(ViewportObjsFrame[i]))
                        {
                            player.HasInteractedThisFrame = true;
                            ReadSign(ViewportObjsFrame[i] as Sign, bActionButtonPressed);
                        }
                    }
                }
                else if (ViewportObjsFrame[i] as ButtonSwitch != null)
                {
                    if (ViewportObjsFrame[i].Box.BoxIntersect_EasyOut_Inclusive(player.Box))
                    {
                        if ((ViewportObjsFrame[i] as ButtonSwitch).Pressed == false)
                        {
                            if (player.Vel.y > 0)
                            {
                                (ViewportObjsFrame[i] as ButtonSwitch).Pressed = true;
                                (ViewportObjsFrame[i] as ButtonSwitch).UpdateSprite();

                                Res.Audio.PlaySound(Res.SfxSwitchButtonPress);
                                //Okay now find all through the conduit

                                List<Cell> activatd = new List<Cell>();
                                List<GameObject> items = new List<GameObject>();
                                GatherConduitObjects(Level.Grid.GetCellForPoint(ViewportObjsFrame[i].Box.Center()), activatd, items);

                                ActivateElectronics(items);

                            }
                        }
                    }
                }
                else if (ViewportObjsFrame[i].TileId == Res.SavePointTileId)
                {
                    if (PlayerIsNearObject(ViewportObjsFrame[i], player))
                    {
                        if (MouseRelease(ViewportObjsFrame[i]))
                        {
                            player.HasInteractedThisFrame = true;

                            GameState = GameState.Pause;
                            Res.Audio.PlaySound(Res.SfxSavePoint1);
                            ViewportObjsFrame[i].SetFrame(0);
                            bSaving = true;
                            Dialog.ShowDialog(new List<string>() { "Saving Game..." });
                            Dialog.Halt = true;
                            SaveTime = Time;
                        }
                    }
                }
                else if (ViewportObjsFrame[i] as Guy != null)
                {
                    //Talk / Talk to guy . talk to npc
                    Guy obGuy = ViewportObjsFrame[i] as Guy;

                    if (PlayerIsNearObject(obGuy, player))
                    {
                        if (MouseRelease(obGuy))
                        {
                            if (Dialog.IsEnabled() == false)
                            {
                                if (obGuy.NpcDialog != null && obGuy.NpcDialog.Count > 0)
                                {
                                    obGuy.AIState = AIState.Talk;
                                    obGuy.Vel = 0;
                                    obGuy.Joystick.AIDown = false;
                                    obGuy.Joystick.AILeft = false;
                                    obGuy.Joystick.AIRight = false;
                                    obGuy.Joystick.AIUp = false;
                                    obGuy.Joystick.AIJump = false;
                                    obGuy.Joystick.AIOk = false;
                                    obGuy.Joystick.AIAction = false;
                                    obGuy.FaceObject(player);
                                    obGuy.SetSprite(obGuy.TalkSprite);
                                    obGuy.Animate = true;

                                    int r = Globals.RandomInt(0, obGuy.NpcDialog.Count);
                                    Dialog.ShowDialog(obGuy.NpcDialog[r], obGuy);
                                    player.HasInteractedThisFrame = true;
                                }
                            }
                        }
                    }
                }
            }
            //  LastCollidedTriggersFrame = new List<GameObject>(CollidedTriggersFrame);
        }
        private void ActivateElectronics(List<GameObject> items)
        {
            //Run through listo f electronics we activated through the conduit and "activate" them
            float itemtime = 0.7f;
            int nActivated = 0;
            Cutscene = new Cutscene()
              .Then(0, (s, dt) =>
              {
                  GameState = GameState.Pause;
                  return false;
              }, null)
            .Then(0, (s, dt) =>
            {
                itemtime -= dt;
                if (items.Count > 0 && itemtime <= 0.0001f)
                {
                    GameObject ob = items[0];
                    items.RemoveAt(0);

                    itemtime = 0.7f;

                    if (ob as Door != null)
                    {
                        if ((ob as Door).Locked == true && (ob as Door).LockType == LockType.Electronic)
                        {
                            UnlockDoor((ob as Door));
                            nActivated++;
                        }
                    }
                    //else.. do other electronic activation
                }

                return items.Count > 0;
            }, null)
            .Then(1, (s, dt) =>
            {
                return s.Duration > 0;
            }, null)
            .Then(0, (s, dt) =>
            {
                if (nActivated > 0)
                {
                    Res.Audio.PlaySound(Res.SfxPuzzleSolved);
                }

                GameState = GameState.Play;
                return false;
            }, null)
            ;
        }
        private void GatherConduitObjects(Cell cell, List<Cell> activated, List<GameObject> items)
        {
            //Run through a conduit and get all items that intersect conduit line
            if (activated.Contains(cell))
            {
                return;
            }
            activated.Add(cell);

            Cell[] surround = Level.Grid.GetSurroundingCells(cell);
            foreach (Cell c in surround)
            {
                if (c != null)
                {
                    if (c.Layers[PlatformLevel.Conduit] != null && c.Layers[PlatformLevel.Conduit].IsConduit)
                    {
                        GatherConduitObjects(c, activated, items);
                    }

                    List<GameObject> obs = GetObjectsForBox(c.Box());
                    foreach (GameObject ob in obs)
                    {
                        if (!items.Contains(ob))
                        {
                            if (ob.IsElectronic == true)
                            {
                                items.Add(ob);
                            }
                        }
                    }
                }
            }

        }
        public List<GameObject> GetObjectsForBox(Box2f b)
        {
            List<GameObject> ret = new List<GameObject>();
            foreach (GameObject ob in Level.GameObjects)
            {
                if (ob.Box.BoxIntersect_EasyOut_Inclusive(b))
                {
                    ret.Add(ob);
                }
            }
            return ret;
        }

        private void ReadSign(Sign ob, bool bActionButtonPressed)
        {
            if (bActionButtonPressed)
            {
                Dialog.ShowDialog(new List<string>(ob.Text));
            }
        }

        private void PlayGotSpecialItemCutscene(Player guy, TreasureChest ob_chest)
        {
            float FadeIn = 1;
            float MoveUpStart = Res.Tiles.TileHeightPixels * 0.2f;
            float MoveUp = 0;

            //Make the guy align with the chest.
            GetPlayer().Pos = ob_chest.Pos;

            vec2 ChestPos = ob_chest.Pos;
            vec2 GuyPos = GetPlayer().Pos;

            //Guy open chest animation
            float GuyCAnimTimeMax = 0.5f;
            float GuyCAnimTime = GuyCAnimTimeMax;
            int ifr = 0;
            Action<CutsceneSequence, SpriteBatch> beforeget_draw = (s, sb) =>
            {
                //Draw the guy opening the chest
                Frame fr;
                fr = ob_chest.Frame;
                Screen.DrawFrame(sb, fr,
                    ChestPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));

                fr = Res.Tiles.GetSpriteFrame(Res.SprGuyOpenChest, ifr);

                Screen.DrawFrame(sb, fr,
                    GuyPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));
            };


            Action<CutsceneSequence, SpriteBatch> draw = (s, sb) =>
            {
                //Draw the guy jumping with item above
                Frame fr;
                fr = ob_chest.Frame;
                Screen.DrawFrame(sb, fr,
                    ChestPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));

                fr = Res.Tiles.GetSpriteFrame(Res.SprGuyItemHeldWalk, 0);
                Screen.DrawFrame(sb, fr,
                    GuyPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));

                fr = ob_chest.SpecialItem.Sprite.GetFrame(0);
                Screen.DrawFrame(sb, fr,
                    ob_chest.Pos - new vec2(0, MoveUpStart) + new vec2(0, MoveUp),
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White * (1.0f - FadeIn), new vec2(1, 1));
            };
            float A = 0.0f;

            Cutscene = new Cutscene()
                .Then(0, (s, dt) =>
                {
                    Res.Audio.PlaySound(Res.SfxGetKeyItem);
                    GameState = GameState.Pause;
                    DrawState = DrawState.None;
                    return false;
                }, beforeget_draw)
                .Then(2.3f, (s, dt) =>
                {
                    GuyCAnimTime -= dt;
                    if (GuyCAnimTime <= 0)
                    {
                        GuyCAnimTime = GuyCAnimTimeMax;
                        if (ifr == 0) ifr = 1; else ifr = 0;
                    }
                    //wait for music
                    return s.Duration > 0;
                }, beforeget_draw)
                .Then(4, (s, dt) =>
                {
                    FadeIn -= dt;
                    MoveUp -= 0.26f;
                    MoveUp = Globals.Clamp(MoveUp, -Res.Tiles.TileHeightPixels, 0);
                    FadeIn = Globals.Clamp(FadeIn, 0, 1);
                    return s.Duration > 0;
                }, draw)
                .Then(0, (s, dt) =>
                {
                    Res.Audio.PlaySound(Res.SfxShowNewItem);
                    Dialog.ShowDialog(ob_chest.SpecialItem.Description);
                    GameState = GameState.Pause;
                    return false;
                }, null)
                .Then(3, (s, dt) =>
                {
                    A += dt * 1.3f;
                    A = Globals.Clamp(A, 0, 1);
                    DrawState = DrawState.Normal;//go back to drawing world.
                    bShowMenu = true;
                    menutab = MenuTab.Inventory;

                    if (ob_chest.SpecialItem.ItemId == Res.BootsTileId)
                    {
                        guy.JumpBootsEnabled = true;
                        JumpBootsMenuColor = new vec4(1, 1, 1, A);
                    }
                    else if (ob_chest.SpecialItem.ItemId == Res.SwordItemTileId)
                    {
                        guy.SwordEnabled = true;
                        PickaxeMenuColor = new vec4(1, 1, 1, A);
                    }
                    else if (ob_chest.SpecialItem.ItemId == Res.BombTileId)
                    {
                        guy.BombsEnabled = true;
                        BombMenuColor = new vec4(1, 1, 1, A);
                    }
                    else if (ob_chest.SpecialItem.ItemId == Res.PowerSwordTileId)
                    {
                        guy.PowerSwordEnabled = true;
                        PowerSwordMenuColor = new vec4(1, 1, 1, A);
                    }
                    else if (ob_chest.SpecialItem.ItemId == Res.ShieldItemTileId)
                    {
                        guy.ShieldEnabled = true;
                        ShieldMenuColor = new vec4(1, 1, 1, A);
                    }
                    else if (ob_chest.SpecialItem.ItemId == Res.BowItemTileId)
                    {
                        guy.BowEnabled = true;
                        BowMenuColor = new vec4(1, 1, 1, A);
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }

                    return s.Duration > 0;
                }, draw)
                .Then(0, (s, dt) =>
                {
                    return Dialog.IsEnabled();
                }, draw)
                .Then(0, (s, dt) =>
                {
                    //Do a custom action after we GOT the item, but the dialog is closed.
                    if (ob_chest.SpecialItem != null && ob_chest.SpecialItem.AfterInfoDialogClosed != null)
                    {
                        return ob_chest.SpecialItem.AfterInfoDialogClosed(this, dt);
                    }
                    return false;
                }, draw)
                .Then(0, (s, dt) =>
                {
                    bShowMenu = false;
                    GameState = GameState.Play;

                    return s.Duration > 0;
                });
        }
        private void PlayGotPowerupCutscene(Player guy, TreasureChest ob)
        {
            float FadeIn = 1;
            float MoveUpStart = Res.Tiles.TileHeightPixels * 0.2f;
            float MoveUp = 0;

            //Make the guy align with the chest.
            GetPlayer().Pos = ob.Pos;

            vec2 ChestPos = ob.Pos;
            vec2 GuyPos = GetPlayer().Pos;

            //Guy open chest animation
            float GuyCAnimTimeMax = 0.5f;
            float GuyCAnimTime = GuyCAnimTimeMax;
            int ifr = 0;
            Action<CutsceneSequence, SpriteBatch> beforeget_draw = (s, sb) =>
            {
                //Draw the guy opening the chest
                Frame fr;
                fr = ob.Frame;
                Screen.DrawFrame(sb, fr,
                    ChestPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));

                fr = Res.Tiles.GetSpriteFrame(Res.SprGuyOpenChest, ifr);

                Screen.DrawFrame(sb, fr,
                    GuyPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));
            };

            Action<CutsceneSequence, SpriteBatch> draw = (s, sb) =>
            {
                //Draw the guy jumping with the item above.
                Frame fr;
                fr = ob.Frame;
                Screen.DrawFrame(sb, fr,
                    ChestPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));

                fr = Res.Tiles.GetSpriteFrame(Res.SprGuyItemHeldWalk, 0);
                Screen.DrawFrame(sb, fr,
                    GuyPos,
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1));

                fr = ob.SpecialItem.Sprite.GetFrame(0);
                Screen.DrawFrame(sb, fr,
                    ob.Pos - new vec2(0, MoveUpStart) + new vec2(0, MoveUp),
                    new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels),
                    Color.White * (1.0f - FadeIn), new vec2(1, 1));
            };

            Cutscene = new Cutscene()
                .Then(0, (s, dt) =>
                {
                    GameState = GameState.Pause;
                    DrawState = DrawState.Normal;//Don't hide background
                    return false;
                }, beforeget_draw)
                .Then(1.0f, (s, dt) =>
                {
                    GuyCAnimTime -= dt;
                    if (GuyCAnimTime <= 0)
                    {
                        GuyCAnimTime = GuyCAnimTimeMax;
                        if (ifr == 0) ifr = 1; else ifr = 0;
                    }
                    //wait for music
                    return s.Duration > 0;
                }, beforeget_draw)
                .Then(0, (s, dt) =>
                {
                    //Res.Audio.PlaySound(Res.SfxGetPowerup);
                    Res.Audio.PlaySound(Res.SfxGetPowerup);
                    return false;
                }, draw)

                .Then(1, (s, dt) =>
                {
                    FadeIn -= dt;
                    MoveUp -= 1.26f;
                    MoveUp = Globals.Clamp(MoveUp, -Res.Tiles.TileHeightPixels, 0);
                    FadeIn = Globals.Clamp(FadeIn, 0, 1);
                    return s.Duration > 0;
                }, draw)
                .Then(0, (s, dt) =>
                {

                    Dialog.ShowDialog(ob.SpecialItem.Description);
                    GameState = GameState.Pause;
                    return false;
                }, draw)
                .Then(0, (s, dt) =>
                {
                    return Dialog.IsEnabled();
                }, draw)
                .Then(0, (s, dt) =>
                {
                    //Upgrade the player
                    if (ob.SpecialItem.ItemId == Res.BombPowerupTileId)
                    {
                        guy.NumBombs += 1;
                    }
                    else if (ob.SpecialItem.ItemId == Res.SmallKey_TileId)
                    {
                        guy.SmallKeys += 1;
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }

                    GameState = GameState.Play;
                    return s.Duration > 0;
                });

        }
        private void CheckGuyInWater(Guy guy, float dt)
        {
            if (guy.InWater)
            {
                guy.SwimStrokeTime -= dt;
                if (guy.SwimStrokeTime < 0)
                {
                    guy.SwimStrokeTime = 0;
                }
            }

            Cell guycc = Level.Grid.GetCellForPoint(guy.cposC);
            if (guycc != null)
            {
                float h = guycc.Box().Max.y - guy.cposC.y;
                if (h <= guycc.Water * 16.0f)
                {
                    if (guy.InWater == false)
                    {
                        if (guy.IsPlayer())
                        {
                            Res.Audio.PlaySound(Res.SfxEnterWater);
                        }

                        CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 7, guy.cposC, 50, 90,
                            this.WaterColorFromType(guycc.WaterType), 2.0f, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f,
                            Gravity, true);

                        guy.SwimStrokeTime = guy.SwimStrokeTimeMax;
                        guy.InWater = true;
                        guy.InWaterType = guycc.WaterType;
                    }
                }
                else
                {
                    if (guy.InWater == true)
                    {
                        CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 7, guy.cposC, 10, 120,
                        this.WaterColorFromType(guycc.WaterType), 2.0f, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f,
                        Gravity, true);

                        if (guy.IsPlayer())
                        {
                            Res.Audio.PlaySound(Res.SfxExitWater);
                        }
                        guy.InWater = false;
                    }
                }
            }

            if (guy.InWater == true && guy.AIState == AIState.Player)
            {
                if (guy.InWaterType == WaterType.Lava)
                {
                    //Lava - damage
                }
                else if (guy.InWaterType == WaterType.Water)
                {
                    //Water.. Remove health
                }
                else if (guy.InWaterType == WaterType.Tar)
                {
                    //Water.. Remove health
                }
            }


        }
        private void UpdateLiquid(float dt)
        {
            //return;

            //Spread Liquid
            //Preprocess the water travel frame
            foreach (Cell c in Level.Grid.CellDict.Values)
            {
                c.WaterTravelFrame = false;
            }
            float WaterMin = 0.001f;
            foreach (Cell c in Level.Grid.CellDict.Values)
            {
                if (c.Water > 0.0f)
                {
                    float moveAmt = 4.0f * dt;
                    if (c.WaterType == WaterType.Lava)
                    {
                        moveAmt = dt * 2.15f;
                    }
                    else if (c.WaterType == WaterType.Tar)
                    {
                        moveAmt = dt * 3.15f;
                    }

                    float move = moveAmt > c.Water ? c.Water : moveAmt;

                    Cell cnb = Level.Grid.GetNeighborCell(c, 0, 1);
                    if (cnb != null && cnb.Layers[PlatformLevel.Midground] == null)
                    {
                        if (cnb.Water < WaterMin)
                        {
                            cnb.WaterType = c.WaterType;
                        }
                        if (cnb.Water < 1.0f)
                        {
                            float add = (cnb.Water + move) < 1 ? move : (cnb.Water + move) - 1;
                            cnb.Water += add;
                            move -= add;
                            if (move < 0) move = 0;
                            c.Water -= add;
                            cnb.WaterTravelFrame = true;
                        }
                    }

                    //Perform a greater distribution based on whether left, or right has more water.
                    //If they're equalt o 0.1 or something then distribute both
                    Cell cnl = Level.Grid.GetNeighborCell(c, -1, 0);
                    Cell cnr = Level.Grid.GetNeighborCell(c, 1, 0);
                    bool distributeLeft = false;
                    bool distributeRight = false;
                    if (cnl != null)
                    {
                        if (cnl.Layers[PlatformLevel.Midground] == null)
                        {

                            if (cnl.Water < 1.0f && cnl.Water < c.Water)
                            {
                                distributeLeft = true;
                            }
                        }
                    }
                    if (cnr != null)
                    {
                        if (cnr.Layers[PlatformLevel.Midground] == null)
                        {

                            if (cnr.Water < 1.0f && cnr.Water < c.Water)
                            {
                                distributeRight = true;

                                //If we asleo are left, then even it out
                                if (distributeLeft)
                                {
                                    if (cnl.Water - cnr.Water > 0.1f)
                                    {
                                        distributeRight = true;
                                        distributeLeft = false;
                                    }
                                    else if (cnr.Water - cnl.Water > 0.1f)
                                    {
                                        distributeLeft = true;
                                        distributeRight = false;
                                    }
                                    else
                                    {
                                        distributeLeft = true;
                                        distributeRight = true;
                                    }
                                }
                            }
                        }
                    }
                    if (distributeLeft)
                    {
                        if (cnl.Water < WaterMin)
                        {
                            cnl.WaterType = c.WaterType;
                        }

                        float add = (cnl.Water + move) < 1 ? move : (cnl.Water + move) - 1;
                        add = (cnl.Water + add) > c.Water ? (c.Water - cnl.Water) / 2 : add;

                        if (distributeLeft && distributeRight)
                        {
                            //Distribute on both sides if they're both open
                            add /= 2;
                        }

                        cnl.Water += add;
                        move -= add;
                        if (move < 0) move = 0;
                        c.Water -= add;

                    }
                    if (distributeRight)
                    {
                        if (cnr.Water < WaterMin)
                        {
                            cnr.WaterType = c.WaterType;
                        }
                        float add = (cnr.Water + move) < 1 ? move : (cnr.Water + move) - 1;
                        add = (cnr.Water + add) > c.Water ? (c.Water - cnr.Water) / 2 : add;

                        cnr.Water += add;
                        move -= add;
                        if (move < 0) move = 0;
                        c.Water -= add;

                    }
                    if (c.Water < WaterMin)//Fixes dumb bugs
                    {
                        c.Water = 0;
                    }
                    if (c.Water > 1.0f)
                    {
                        c.Water = 1.0f;//visual artifact
                    }
                    if (c.Water > 0 && cnl != null && (cnl.Water > 0.0f) &&
                    ((cnl.WaterType == WaterType.Lava && c.WaterType == WaterType.Water) ||
                    (cnl.WaterType == WaterType.Water && c.WaterType == WaterType.Lava)))
                    {
                        //Play sizzle sound
                        Res.Audio.PlaySound(Res.SfxSizzle);
                        // if(cnl.Water + c.Water >= 1.0f)
                        {
                            Level.CreateTile(c, Level.GetTile(Res.BlockTileId_Obsidian), 0, PlatformLevel.Midground,
                                c.Pos(), Res.Tiles.GetWHVec());
                        }
                        cnl.Water = 0;
                        c.Water = 0;
                    }
                    if (c.Water > 0 && cnr != null && (cnr.Water > 0.0f) &&
                    ((cnr.WaterType == WaterType.Lava && c.WaterType == WaterType.Water) ||
                    (cnr.WaterType == WaterType.Water && c.WaterType == WaterType.Lava)))
                    {
                        Res.Audio.PlaySound(Res.SfxSizzle);
                        // if (cnr.Water + c.Water >= 0.0f)
                        {
                            Level.CreateTile(c, Level.GetTile(Res.BlockTileId_Obsidian), 0, PlatformLevel.Midground,
                            c.Pos(), Res.Tiles.GetWHVec());
                        }
                        cnr.Water = 0;
                        c.Water = 0;
                    }
                    if (c.Water > 0 && cnb != null && (cnb.Water > 0.0f) &&
                    ((cnb.WaterType == WaterType.Lava && c.WaterType == WaterType.Water) ||
                    (cnb.WaterType == WaterType.Water && c.WaterType == WaterType.Lava)))
                    {
                        Res.Audio.PlaySound(Res.SfxSizzle);

                        // if (cnb.Water + c.Water >= 1.0f)
                        {
                            Level.CreateTile(cnb, Level.GetTile(Res.BlockTileId_Obsidian), 0, PlatformLevel.Midground,
                            cnb.Pos(), Res.Tiles.GetWHVec());
                        }
                        cnb.Water = 0;
                        c.Water = 0;
                    }
                }

                if (c.Water < WaterMin)//Fixes dumb bugs
                {
                    c.Water = 0;
                }
                if (c.Water > 1.0f)
                {
                    c.Water = 1.0f;//visual artifact
                }
            }


            //Just do again or sometin
            foreach (Cell c in ViewportCellsFrame)
            {
                Cell cnl = Level.Grid.GetNeighborCell(c, -1, 0);
                Cell cnr = Level.Grid.GetNeighborCell(c, 1, 0);
                // Cell cnt = Level.Grid.GetNeighborCell(c, 0, -1);
                // Cell cnb = Level.Grid.GetNeighborCell(c, 0, 1);
                c.WaterOnLeft = false;
                c.WaterOnRight = false;
                //c.WaterAbove = false;
                //c.WaterBelow = false;
                if ((cnl != null && cnl.Layers[PlatformLevel.Midground] != null) || cnl == null || (cnl != null && cnl.Water > 0))
                {
                    c.WaterOnLeft = true;
                }
                if ((cnr != null && cnr.Layers[PlatformLevel.Midground] != null) || cnr == null || (cnr != null && cnr.Water > 0))
                {
                    c.WaterOnRight = true;
                }

            }

        }
        private void DoLight()
        {
            vec4 sunlight = new vec4(0.89f, 0.89f, 0.89f, 1.0f);
            vec4 ambient = Level.GlobalLight;// new vec4(0.01f, 0.01f, 0.01f, 1.0f);

            //Pre-Light - set to zero for hidden background
            //This "bleeds" the empty cells, and the sky into the world.
            foreach (Cell c in ViewportCellsFrame)
            {
                c.LightColor = ambient;

                c.MarchFrame = c.MarchFrameLight = 0;
                if (c.Layers[PlatformLevel.Background] == null ||
                    c.Layers[PlatformLevel.Background].Tile.BlocksLight == false)
                {
                    GameObject sun = new GameObject(this);
                    sun.Pos = c.Pos();
                    sun.EmitColor = sunlight;
                    sun.EmitLight = true;
                    sun.EmitRadiusInPixels = 16 * 4;
                    EmittersFrame.Add(sun);
                    //c.LightColor = sunlight;
                }

            }

            //This is the big one.. march everything.
            int iLight = 0;
            foreach (GameObject emitter in EmittersFrame)
            {
                BeginMarchLight(emitter, iLight);
                iLight++;
            }

        }
        private void BeginMarchLight(GameObject emitter, int iLight)
        {
            Cell c = Level.Grid.GetCellForPoint(emitter.Pos + Res.Tiles.GetWHVec() * 0.5f);
            vec2 center = emitter.GetEmitterBox().Center();

            MarchLight(center, c, iFrameStamp + iLight, emitter.EmitColor, 1.0f, emitter.EmitRadiusInPixels, 0);
        }
        private void MarchLight(vec2 dpos, Cell c, int lightFrame, vec4 light, float multiplier, float atten2, int dir)
        {
            //dpos is attempting to make this look smoother
            if (c == null)
            {
                return;
            }
            if (c.MarchFrameLight == lightFrame)
            {
                //already marched
                return;
            }

            vec2 cCenter = c.Box().Center();

            c.MarchFrameLight = lightFrame;
            c.LightColor += light * multiplier;
            c.LightColor.w = 1.0f; //alpha
            if (c.LightColor.x > 1.0f) c.LightColor.x = 1.0f;
            if (c.LightColor.y > 1.0f) c.LightColor.y = 1.0f;
            if (c.LightColor.z > 1.0f) c.LightColor.z = 1.0f;

            float len = (cCenter - dpos).Len();
            float att = len / atten2;

            if (c.Layers[PlatformLevel.Midground] == null || ((c.Layers[PlatformLevel.Midground].Tile != null) && (c.Layers[PlatformLevel.Midground].Tile.BlocksLight == false)))
            {
                multiplier -= att;
            }
            else
            {
                multiplier -= att * 3.0f;//0;//= att * 4.0f; //remove more for in the world.
            }

            if (multiplier > 0)
            {
                //Marching just 4 tiels gives a strange result. however it's inaccurate because we shoulnd't be able to see through corners
                vec2 next = dpos + (cCenter - dpos).Normalized() * len;

                if (dir == 0)
                {
                    MarchLight(next, Level.Grid.GetNeighborCell(c, -1, 0), lightFrame, light, multiplier, atten2, 1);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 1, 0), lightFrame, light, multiplier, atten2, 2);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, -1), lightFrame, light, multiplier, atten2, 3);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, 1), lightFrame, light, multiplier, atten2, 4);
                }
                else if (dir == 1)
                {
                    MarchLight(next, Level.Grid.GetNeighborCell(c, -1, 0), lightFrame, light, multiplier, atten2, 1);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, -1), lightFrame, light, multiplier, atten2, 1);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, 1), lightFrame, light, multiplier, atten2, 1);
                }
                else if (dir == 2)
                {
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 1, 0), lightFrame, light, multiplier, atten2, 2);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, -1), lightFrame, light, multiplier, atten2, 2);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, 1), lightFrame, light, multiplier, atten2, 2);
                }
                else if (dir == 3)
                {
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 1, 0), lightFrame, light, multiplier, atten2, 3);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, -1, 0), lightFrame, light, multiplier, atten2, 3);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, -1), lightFrame, light, multiplier, atten2, 3);
                }
                else if (dir == 4)
                {
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 1, 0), lightFrame, light, multiplier, atten2, 4);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, -1, 0), lightFrame, light, multiplier, atten2, 4);
                    MarchLight(next, Level.Grid.GetNeighborCell(c, 0, 1), lightFrame, light, multiplier, atten2, 4);
                }

            }

        }
        public Player GetPlayer()
        {
            GameObject g = Level.GameObjects.Find(x => (x as Player) != null);
            return (g as Player);
        }

        bool IntroTutorial = false;
        float jumpStartPos = -1;
        int JumpState = 0;//For Rocket Jump
        private void UpdateGuyState(Guy guy, float dt)
        {
            guy.Acc = 0.0f;

            if (guy.Joystick == null)
            {
                //Must have a joystick
                System.Diagnostics.Debugger.Break();
                return;
            }

            Player player = guy as Player;

            //Hurt Time
            if (guy.HurtTime > 0)
            {
                guy.HurtTime -= dt;
                if (guy.HurtTime < 0)
                {
                    guy.HurtTime = 0;
                }
            }

            //Nonsense update to the land sound
            if (landSound != null && landSound.State == SoundState.Stopped)
            {
                landSound = null;
            }

            if (guy.Joystick.Jump.PressOrDown())
            {
                bool bCanSwim = (guy.InWater && guy.SwimStrokeTime <= 0) &&
                    (!guy.InWater || (guy.InWater && guy.Joystick.Jump.Press()));

                if (guy.OnGround /*|| guy.Hanging */ || guy.Climbing || bCanSwim)//press only for swims
                {
                    //guy.Hanging = false;
                    guy.Climbing = false;
                    guy.Jumping = true;
                    guy.Crouching = false;
                    if (guy.ItemHeld == null)
                    {
                        guy.SetSpriteIfNot(guy.JumpSprite);
                    }

                    if (bCanSwim)
                    {
                        //player is in water and pressed spacebar to swim
                        guy.Vel = new vec2(0, -guy.JumpSpeed) * dt;
                        guy.Vel *= 0.5f;
                        guy.SwimStrokeTime = guy.SwimStrokeTimeMax;
                        if (guy.OnGround == false)
                        {
                            int n = 0; n++;
                        }
                    }
                    else
                    {
                        if (player != null /*&& guy.ItemHeld == null*/ && guy.OnGround /*&& guy.TimeOnGround < Player.SpringBootsMinTime*/)
                        {
                            // use TimeOnGround and JumpState tocheck the next jump's velocity

                            Func<int, float> ms = (x) =>
                            {
                                float ret = (float)((float)x / (float)1000);
                                return ret;
                            };

                            int precision = 0; // 0=Failure
                            float precision_mul = 0; // 0=Failure
                            guy.VaporTrail = 0;
                            float statef = 0.0f;
                            if (this.JumpState == 0)
                            {
                                //We have begun the jump
                                precision = 1;
                                if (jumpSound == null && guy.IsPlayer())
                                {
                                    jumpSound = Res.Audio.PlaySound(Res.SfxJump);
                                }
                                statef = 0.5f;
                                precision_mul = 1.0f;
                                this.JumpState++;
                            }
                            else
                            {
                                //Jump state is greater, Match with time on ground.
                                if (guy.TimeOnGround < ms(30))
                                {
                                    precision = 5;// PERFECT
                                    precision_mul = 1.06f;
                                    ScreenOverlayText = "PERFECT";
                                }
                                else if (guy.TimeOnGround < ms(60))
                                {
                                    precision = 4; // Awesome!
                                    precision_mul = 1.05f;
                                    ScreenOverlayText = "Awesome!";
                                }
                                else if (guy.TimeOnGround < ms(90))
                                {
                                    precision = 3; // Great!
                                    precision_mul = 1.04f;

                                    ScreenOverlayText = "Great!";
                                }
                                else if (guy.TimeOnGround < ms(120))
                                {
                                    precision = 2; //Good!
                                    precision_mul = 1.03f;

                                    ScreenOverlayText = "Good!";
                                }
                                else if (guy.TimeOnGround < ms(200))
                                {
                                    precision = 1;
                                    precision_mul = 1.0f;
                                    ScreenOverlayText = "Ok!";
                                }
                                else
                                {
                                    //Failure 
                                    this.JumpState = 0;
                                    ScreenOverlayText = "Failure";
                                    //Play Fail Animation here.
                                }

                                if (this.JumpState == 1)
                                {
                                    if (jumpSound == null && guy.IsPlayer())
                                    {
                                        jumpSound = Res.Audio.PlaySound(Res.SfxJump);
                                    }
                                    statef = 0.5f;
                                    this.JumpState++;
                                }
                                else if (this.JumpState == 2)
                                {
                                    if (jumpSound == null && guy.IsPlayer())
                                    {
                                        Res.Audio.PlaySound(Res.SfxBootsJump);
                                    }
                                    statef = 0.7f;
                                    guy.VaporTrail = 1;
                                    this.JumpState++;

                                }
                                else if (this.JumpState == 3)
                                {
                                    if (guy.IsPlayer())
                                    {
                                        Screen.ScreenShake.Shake(0.99f, 0.40f);
                                        Res.Audio.PlaySound(Res.SfxBombexplode);

                                    }
                                    statef = 2.6f;
                                    guy.VaporTrail = 4;
                                    this.JumpState++;
                                    jumpStartPos = guy.Pos.x;
                                    guy.CalcRotationDelta();
                                    CreateBlastParticles(guy.Box.Center() + new vec2(0, guy.Box.Height() * 0.5f),
                                        new vec4(1, 1, .8914f, 1), 30, Res.SprParticleBig, new vec2(0, 1), MathHelper.ToRadians(180));

                                    if (guy.IsFacingLeft())
                                    {
                                        guy.RotationDelta *= -1.0f;
                                    }

                                }
                            }

                            if (IntroTutorial)
                            {
                                //Reset jump statei f intro trutorial.
                                JumpState = 0;
                            }

                            //Don't show it
                            ScreenOverlayText = "";

                            //Do a spring boot jump
                            guy.CurJumpSpeed = guy.SpringJumpSpeed * precision_mul * statef;

                            guy.Vel += new vec2(0, -(guy.CurJumpSpeed)) * dt;

                            if (Math.Abs(guy.Vel.y) < Gravity.y * dt)
                            {
                                //The guy's velocity isn't enough to get him off the ground when gravity is applied.
                                int n = 0;
                                n++;
                            }

                            //guy.RotationDelta = 3.14159f * 2.0f;

                            if (guy.IsPlayer())
                            {
                                guy.SetSpriteIfNot(guy.SpringJumpSprite);
                            }
                        }


                    }
                }
                else if (guy.Airtime > 0.0f)
                {
                    guy.Vel += new vec2(0, -guy.CurJumpSpeed) * dt;
                    guy.Airtime -= dt;
                }
            }
            else if (guy.OnGround == false)
            {
                if (guy.Jumping && guy.IsPlayer())
                {
                    if (jumpSound != null)
                    {
                        jumpSound.Stop();
                        jumpSound = null;
                    }

                }
                guy.Airtime = 0;
            }

            if (guy.Climbing == true)
            {
                float dir = -1.0f;
                bool continueClimb = false;
                if (guy.Joystick.Left.PressOrDown())
                {
                    //If we are hanging left, then move left, else, stop hanging
                    if (guy.IsFacingRight() == false)
                    {
                        continueClimb = true;
                    }
                }
                if (guy.Joystick.Right.PressOrDown())
                {
                    //If we are hanging left, then move left, else, stop hanging
                    if (guy.IsFacingRight() == true)
                    {
                        continueClimb = true;
                    }
                }
                if (guy.Joystick.Up.PressOrDown())
                {
                    continueClimb = true;
                }
                if (guy.Joystick.Down.PressOrDown())
                {
                    continueClimb = true;
                    dir = 1.0f;
                }

                if (climbSound != null && climbSound.State == SoundState.Stopped)
                {
                    climbSound = null;
                }

                if (continueClimb)
                {
                    if (climbSound == null && guy.IsPlayer())
                    {
                        climbSound = Res.Audio.PlaySound(Res.SfxClimb);
                    }
                    guy.Vel = new vec2(0, guy.ClimbSpeed * dt * dir);// (Guy.ClimbPos - Guy.ClimbPosStart).Normalized() * ;
                }
                else
                {
                    if (climbSound != null)
                    {
                        climbSound.Stop();
                        climbSound = null;
                    }
                    guy.Vel = new vec2(0, 0);//Don't climb, no button pressed
                }
            }

            //Left + right left/right move left/right 
            //Move Left & Right (No climb or hang)
            //If guy is on ground then the player has more control over his movement

            if (guy.OnGround)
            {

                float SwitchMultiplier = 0.9f;
                if (guy.Joystick.Left.Press() == true)
                {
                    if (guy.Vel.x > 0)
                    {
                        guy.Vel += -guy.Vel * SwitchMultiplier;
                    }
                }
                if (guy.Joystick.Right.Press() == true)
                {
                    if (guy.Vel.x < 0)
                    {
                        guy.Vel += -guy.Vel * SwitchMultiplier;
                    }
                }

                if (guy.Joystick.Down.PressOrDown())
                {
                    if (player != null && (player.ShieldOut || player.SwordOut || player.BowOut))
                    {
                        guy.SetSpriteIfNot(guy.CrouchAttackSprite);
                    }
                    else
                    {
                        guy.SetSpriteIfNot(guy.CrouchAttackSprite);
                    }
                    guy.Crouching = true;
                }
                else if (guy.Joystick.Down.Release())
                {
                    if (guy.ItemHeld == null)
                    {
                        if (player != null && (player.ShieldOut || player.SwordOut || player.BowOut))
                        {
                            guy.SetSpriteIfNot(guy.WalkAttackSprite);
                        }
                        else
                        {
                            guy.SetSpriteIfNot(guy.WalkSprite);
                        }
                    }

                    guy.Crouching = false;
                }

            }

            //If crouching, lower speed
            float walkorcrouchspeed = guy.Speed;
            if (guy.Crouching == true)
            {
                walkorcrouchspeed *= 0.2f;
            }

            //Basic left/right  movement
            if (guy.Joystick.Left.PressOrDown())
            {
                guy.Acc += new vec2(-walkorcrouchspeed, 0);
                guy.SpriteEffects = SpriteEffects.FlipHorizontally;
            }
            else if (guy.Joystick.Right.PressOrDown())
            {
                guy.Acc += new vec2(walkorcrouchspeed, 0);
                guy.SpriteEffects = SpriteEffects.None;
            }


            //If we were climbing but we flipped direction
            if (guy.Climbing && guy.ClimbFace != guy.SpriteEffects)
            {
                guy.Climbing = false;
                guy.SetWalkSprite();
            }

            //Friction  ** Only applied for player
            if (guy.OnGround == true /*&& (guy.AIState==AIState.Player)*/ &&
                (guy.Joystick.Left.PressOrDown() == false) && (guy.Joystick.Right.PressOrDown() == false))
            {
                //Player let go of contros
                //If sliding, then add friction
                guy.Vel -= (guy.Vel * guy.Friction * dt);
            }

            //Handle Stasis, and idle animation states.
            if (player != null && IsChargingSword(player))
            {
                guy.Animate = true;
            }
            else if (guy.Climbing)
            {
                if (guy.Joystick.Right.PressOrDown() ||
                    guy.Joystick.Left.PressOrDown() ||
                    guy.Joystick.Up.PressOrDown() ||
                    guy.Joystick.Down.PressOrDown())
                {
                    guy.Animate = true;
                    guy.Loop = true;
                }
                else
                {
                    guy.Animate = false;
                }
            }
            else if (guy.Jumping)
            {
                guy.Animate = false;
                guy.Loop = false;
                if (guy.Sprite.Frames.Count > 0)
                {
                    guy.SetFrame(0);
                }
            }
            else if ((guy.OnGround || guy.Crouching) && !guy.Waving)
            {
                if (guy.Joystick.Right.PressOrDown() || guy.Joystick.Left.PressOrDown())
                {
                    guy.Animate = true;
                    guy.Loop = true;
                }
                else if (guy.AIState == AIState.Talk)
                {
                    guy.Animate = true;
                    guy.Loop = true;
                }
                else
                {
                    guy.Animate = false;
                    guy.Loop = false;
                    if (guy.Sprite.Frames.Count > 1)
                    {
                        guy.SetFrame(1);
                    }
                }

            }
            else
            {
                //falling
                guy.Animate = true;
                guy.Loop = true;
            }


        }
        private bool IsChargingSword(Player guy)
        {
            return guy.PowerSwordChargeWait <= 0 && Screen.Game.Input.Global.PressOrDown();
        }
        private bool LargestPenetrateSide(float a, float b, float c, float d)
        {
            return (a > 0) && ((b > 0 && a > b) || b <= 0) && ((c > 0 && a > c) || c <= 0) && ((d > 0 && a > d) || d <= 0);
        }
        private void LimitVelocity(Guy guy)
        {
            float maxLenx = 90;
            float maxLeny = 120;//We nedd to separate cuz of jump
            if (guy.Vel.x > maxLenx)
            {
                guy.Vel.x = maxLenx;
            }
            if (guy.Vel.y > maxLeny)
            {
                guy.Vel.y = maxLeny;
            }

        }
        private void DoPhysics(Guy guy, float dt)
        {
            //Debug = 60fps
            dt = 0.016935f;

            guy.CalcRelPos();
            bool LeftButtonDown = guy.Joystick.Left.PressOrDown();
            bool RightButtonDown = guy.Joystick.Right.PressOrDown();
            bool UpButtonDown = guy.Joystick.Up.PressOrDown();

            //Dampen movement in liquid
            //gravity is too harsh we need to dampen  more
            float waterdamp, waterdampgrav;
            if (guy.InWater)
            {
                waterdamp = guy.WaterDamp;
                waterdampgrav = guy.WaterDampGrav;
            }
            else
            {
                waterdamp = guy.NormalDamp;
                waterdampgrav = guy.NormalDampGrav;
            }

            //physics forces
            if (/*guy.Hanging == false &&*/ guy.Climbing == false/* && guy.OnGround==false*/)
            {
                guy.LimitAcc();
                guy.Vel += guy.Acc * dt * waterdamp;
                guy.Vel += guy.Gravity * dt * waterdampgrav;
            }

            LimitVelocity(guy);
            guy.Pos += guy.Vel * dt * waterdamp;

            Box2f speedbox = new Box2f();
            speedbox.GenResetExtents();
            speedbox.ExpandByPoint(guy.cposR);
            speedbox.ExpandByPoint(guy.cposB);
            speedbox.ExpandByPoint(guy.cposT);
            speedbox.ExpandByPoint(guy.cposL);
            speedbox.ExpandByPoint(guy.cposL);
            speedbox.ExpandByPoint(guy.Pos);
            speedbox.ExpandByPoint(guy.Pos + guy.Vel);

            bool LContained = false;//Variables to tell whether to stop climbing
            bool RContained = false;//Variables to tell whether to stop climbing
            guy.LastOnGround = guy.OnGround;
            guy.OnGround = false;//Initially set to false
            guy.OnSlope = false;

            Player player = guy as Player;

            if (player != null)
            {
                List<Cell> GuyTiles = Level.Grid.GetCellManifoldForBox(speedbox);
                //Sort by nearest.  THIS IS CRITICAL to prevent us from colliding with further tiles.
                GuyTiles.Sort((x, y) => (x.Box().Center() - guy.cposC).Len2().CompareTo((y.Box().Center() - guy.cposC).Len2()));

                foreach (Cell cell in GuyTiles)
                {
                    GuyCollideMidbackCell(guy, cell, ref LContained, ref RContained, UpButtonDown);
                    GuyCollideMidgroundCell(guy, cell, ref LContained, ref RContained, LeftButtonDown, RightButtonDown, UpButtonDown);
                    GuyCollideForegroundCell(player, cell, LeftButtonDown, RightButtonDown);
                }
                KeepGuyInWorld(guy, ref LContained, ref RContained, LeftButtonDown, RightButtonDown);

            }
            else
            {
                if (guy.AIPhysics == AIPhysics.Character)
                {
                    List<Cell> GuyTiles = Level.Grid.GetCellManifoldForBox(speedbox);
                    //Sort by nearest.  THIS IS CRITICAL to prevent us from colliding with further tiles.
                    GuyTiles.Sort((x, y) => (x.Box().Center() - guy.cposC).Len2().CompareTo((y.Box().Center() - guy.cposC).Len2()));

                    foreach (Cell cell in GuyTiles)
                    {
                        GuyCollideMidgroundCell(guy, cell, ref LContained, ref RContained, LeftButtonDown, RightButtonDown, UpButtonDown);
                    }
                    UpdateAI_Character(guy, dt, LContained, RContained);
                }
                else if (guy.AIPhysics == AIPhysics.Grapple)
                {
                    UpdateAI_Grapple(dt, guy);
                }
                else if (guy.AIPhysics == AIPhysics.PlantBombGuy)
                {
                    //UpdateAI_PlantBombGuy(guy, dt);

                }

                UpdateAI_Common(guy, dt);
                KeepGuyInWorld(guy, ref LContained, ref RContained, LeftButtonDown, RightButtonDown);
            }

            //Check to end a climb, then HOP off the cliff
            HopCliff(guy, LContained, RContained, dt);

            if (guy.OnGround == false)
            {
                guy.TimeOnGround = 0;
            }
            else if (guy.OnGround == true)
            {
                guy.TimeOnGround += dt;
            }

        }
        private void UpdateAI_Grapple(float dt, Guy guy)
        {
            //Update Animtion (if grub)
            if (guy.ScaleDelta.y == 0)
            {
                guy.ScaleDelta.y = 0.7f;
                guy.ScalePingpongY = true;
                guy.ScalePingpongYRange = new vec2(0.83f, 1.1f);
                guy.ScalePingpongX = true;
                guy.ScaleDelta.x = 0.55f;
                guy.ScalePingpongXRange = new vec2(0.73f, 1.1f);
            }

            //Grapple scan
            if (guy.GrappleInitialScan == false)
            {
                //If we are not currently grappling, then scan for cells to cling to.
                //If there are no cells, update gravity.
                Cell c = Level.Grid.GetCellForPoint(guy.Box.Center());
                Cell found = null;
                int dir = -1;
                GetGroundCell4(c, ref found, ref dir);

                if (found != null)
                {
                    guy.GrappleInitialScan = true;
                    guy.GrappleCell = found;
                    guy.GrappleSide = GetGrappleSide(guy.Box.Center(), found.Layers[PlatformLevel.Midground]);
                    guy.GrappleNormal = GetNormalForSide(guy.GrappleSide);
                }
            }

            //Move along edges
            if (guy.GrappleCell != null && HasMidgroundLand(guy.GrappleCell))
            {
                guy.Vel = 0;
                guy.Acc = 0;

                vec2 a = new vec2(0, 0);
                vec2 b = new vec2(0, 0);
                GetSidePoints(guy.GrappleSide, guy.GrappleCell.Box(), ref a, ref b, guy.GrappleDir == 1);

                //if(guy.Speed < 0)

                //Move the char to the next point, capping it if it goes over
                guy.Grapple += guy.Speed * dt;//TODO: arbitrary
                if (guy.Grapple > 1.0f) { guy.Grapple = 1.0f; }
                vec2 grapplePos = (a + (b - a) * guy.Grapple);
                guy.Pos = grapplePos - guy.Origin;// guy.Vel.Len();

                //if char hits the next point, then let's move him to the next side.
                if ((grapplePos - b).Len2() < 0.01f)
                {

                    //Get the info for the side point A+B and the next side we can move to
                    ivec2 nr_plane = new ivec2(0, 0);
                    ivec2 nr_corner = new ivec2(0, 0);
                    int next_self = 0;
                    int next_corner = 0;
                    int next_plane = 0;
                    int slope_c_0 = 0, slope_p_0 = 0;
                    GetGrappleSideInfo(guy.GrappleSide, guy.GrappleCell.Box(),
                    Midground(guy.GrappleCell).SlopeTileId,
                    ref next_self,
                    ref next_plane, ref next_corner,
                    ref nr_plane, ref nr_corner,
                    ref slope_p_0, ref slope_c_0, guy.GrappleDir == 1);

                    guy.Grapple = 0.0f;

                    //Check  corner cell.
                    Cell c;
                    c = Level.Grid.GetNeighborCell(guy.GrappleCell, nr_corner.x, nr_corner.y);
                    if (HasMidgroundLand(c))
                    {
                        if (Midground(c).SlopeTileId == SideToSlope(slope_c_0))
                        {
                            guy.GrappleSide = slope_c_0;
                        }
                        else
                        {
                            guy.GrappleSide = next_corner;
                        }
                        guy.GrappleCell = c;
                    }
                    else
                    {
                        //If there's no corner, check top.
                        c = Level.Grid.GetNeighborCell(guy.GrappleCell, nr_plane.x, nr_plane.y);
                        if (HasMidgroundLand(c))
                        {
                            if (Midground(c).SlopeTileId == SideToSlope(slope_p_0))
                            {
                                guy.GrappleSide = slope_p_0;
                            }
                            else
                            {
                                //w're on a square plane and on the same plane unless we were on a slope
                                guy.GrappleSide = next_plane;
                            }
                            //Keep the same GrappleSide - we're going to continue
                            guy.GrappleCell = c;
                        }
                        else
                        {
                            //Otherwise, change grapple side to the same as this cell.
                            guy.GrappleSide = next_self; // Go to top of cell
                        }
                    }

                    guy.GrappleNormal = GetNormalForSide(guy.GrappleSide);
                }

                //Update the bug rotation (if we are a bug)
                vec2 r2 = MathUtils.DecomposeRotation(guy.Rotation).Normalized();
                float dxy = (r2.Dot(guy.GrappleNormal) + 1.0f) / 2.0f;
                if (1.0f - dxy > 0.0001f)
                {
                    float rNeeded = (float)Math.PI * (1 - dxy);//The 100% rotation we need
                    if (guy.GrappleNormal.Perp().Normalized().Dot(r2) < 0)
                    {
                        rNeeded *= -1;
                    }

                    float rDelta = rNeeded * 0.15f;

                    //Min amount
                    float minRot = 0.06f;
                    if (Math.Abs(rDelta) < minRot)
                    {
                        if (minRot > Math.Abs(rNeeded))
                        {
                            rDelta = rNeeded;
                        }
                        else
                        {
                            rDelta = minRot * ((rDelta < 0) ? -1 : 1);
                        }

                    }

                    guy.Rotation += rDelta;

                }
                else
                {
                    // ? 2.3f : 0.0f;
                }
                // float wantRot = (float)Math.Asin(guy.GrappleNormal.x);
                //if (dxy < 0.0f) { guy.RotationDelta *= -1.0f; }

            }
            else
            {
                //Reset grapple initial scan, and also set gravity
                guy.GrappleInitialScan = false;
                guy.Vel += Gravity * dt;
                guy.Pos += guy.Vel * dt;
            }

        }
        public void GetSidePoints(int iSide, Box2f box, ref vec2 a, ref vec2 b, bool clockwise)
        {
            if (iSide == 0)
            {
                a = box.TopLeft();
                b = box.BotLeft();
            }
            else if (iSide == 1)
            {
                a = box.BotRight();
                b = box.TopRight();
            }
            else if (iSide == 2)
            {
                a = box.TopRight();
                b = box.TopLeft();
            }
            else if (iSide == 3)
            {
                a = box.BotLeft();
                b = box.BotRight();
            }
            else if (iSide == 4)//4  BR 
            {
                a = box.TopRight();
                b = box.BotLeft();
            }
            else if (iSide == 5)//5 BL
            {
                a = box.BotRight();
                b = box.TopLeft();
            }
            else if (iSide == 6)//6 TR
            {
                a = box.TopLeft();
                b = box.BotRight();
            }
            else if (iSide == 7)//7 TL
            {
                a = box.BotLeft();
                b = box.TopRight();
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }

            if (clockwise)
            {
                vec2 tmp = a;
                a = b;
                b = tmp;
            }
        }
        public void GetGrappleSideInfo(int iSide, Box2f box, int slopetileid,
            ref int next_self, ref int next_plane, ref int next_corner,
            ref ivec2 nr_plane, ref ivec2 nr_corner,
            ref int slope_p_0, ref int slope_c_0, bool right)
        {
            //Based on the CURRENT CORNER the guy is on and his DIRECTIN (CW / CCW) give us the next plane side integer.
            //Gets grapple info, and also points for a side
            //nr_plane = the same-plane neighbor block relative to the current block/side 
            //nr_corner = the corner neighbor block relative to the current block/side
            //next-same - the next side on the same block  (if NR0 and NR1 = = null)
            //next_corner - the next side on the corner (if nr1 != null)
            //slope_p = the  corner allowed for the in-plane-neighbor
            //slope_c = the  corner allowed for the corner neighbor

            int BR = 4, BL = 5, TR = 6, TL = 7;

            if (right)
            {
                //CLOCKWISE
                if (iSide == 0 || iSide == TR)
                {
                    nr_plane = new ivec2(0, -1);
                    nr_corner = new ivec2(-1, -1);
                    next_corner = 3;
                    next_plane = 0;
                    slope_c_0 = TR;
                    slope_p_0 = BR;
                }
                else if (iSide == 1 || iSide == BL)
                {
                    nr_plane = new ivec2(0, 1);
                    nr_corner = new ivec2(1, 1);
                    next_corner = 2;
                    next_plane = 1;
                    slope_c_0 = BL;
                    slope_p_0 = TL;
                }
                else if (iSide == 2 || iSide == BR)
                {
                    nr_plane = new ivec2(1, 0);
                    nr_corner = new ivec2(1, -1);
                    next_corner = 0;
                    next_plane = 2;
                    slope_c_0 = BR;
                    slope_p_0 = BL;
                }
                else if (iSide == 3 || iSide == TL)
                {
                    nr_plane = new ivec2(-1, 0);
                    nr_corner = new ivec2(-1, 1);
                    next_corner = 1;
                    next_plane = 3;
                    slope_c_0 = TL;
                    slope_p_0 = TR;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }


                if (iSide == 0) { next_self = slopetileid == Res.SlopeTile_BL ? BL : 2; }
                else if (iSide == 1) { next_self = slopetileid == Res.SlopeTile_TR ? TR : 3; }
                else if (iSide == 2) { next_self = slopetileid == Res.SlopeTile_TL ? TL : 1; }
                else if (iSide == 3) { next_self = slopetileid == Res.SlopeTile_BR ? BR : 0; }
                else if (iSide == 4) { next_self = 1; }
                else if (iSide == 5) { next_self = 3; }
                else if (iSide == 6) { next_self = 2; }
                else if (iSide == 7) { next_self = 0; }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            else
            {

                //Note: We are moving COUNTER-CLOCKWISE
                if (iSide == 0 || iSide == BR)
                {
                    nr_plane = new ivec2(0, 1);
                    nr_corner = new ivec2(-1, 1);
                    next_corner = 2;
                    next_plane = 0;
                    slope_c_0 = BR;
                    slope_p_0 = TR;
                }
                else if (iSide == 1 || iSide == TL)
                {
                    nr_plane = new ivec2(0, -1);
                    nr_corner = new ivec2(1, -1);
                    next_corner = 3;
                    next_plane = 1;
                    slope_c_0 = TL;
                    slope_p_0 = BL;
                }
                else if (iSide == 2 || iSide == BL)
                {
                    nr_plane = new ivec2(-1, 0);
                    nr_corner = new ivec2(-1, -1);
                    next_corner = 1;
                    next_plane = 2;
                    slope_c_0 = BL;
                    slope_p_0 = BR;
                }
                else if (iSide == 3 || iSide == TR)
                {
                    nr_plane = new ivec2(1, 0);
                    nr_corner = new ivec2(1, 1);
                    next_corner = 0;
                    next_plane = 3;
                    slope_c_0 = TR;
                    slope_p_0 = TL;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }


                if (iSide == 0) { next_self = slopetileid == Res.SlopeTile_TL ? TL : 3; }
                else if (iSide == 1) { next_self = slopetileid == Res.SlopeTile_BR ? BR : 2; }
                else if (iSide == 2) { next_self = slopetileid == Res.SlopeTile_TR ? TR : 0; }
                else if (iSide == 3) { next_self = slopetileid == Res.SlopeTile_BL ? BL : 1; }
                else if (iSide == 4) { next_self = 3; }
                else if (iSide == 5) { next_self = 0; }
                else if (iSide == 6) { next_self = 1; }
                else if (iSide == 7) { next_self = 2; }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }

            }
        }
        public int GetGrappleSide(vec2 pos, TileBlock block)
        {
            int found = 0;

            List<int> sides = new List<int>();
            //determine which sides to check
            if (block.IsSlope())
            {
                //Get slope distance
                // slope_side  = SlopeToSide(block.SlopeTileId);
                if (block.SlopeTileId == Res.SlopeTile_BL)//5
                {
                    sides.Add(0);
                    sides.Add(3);
                    sides.Add(5);
                }
                else if (block.SlopeTileId == Res.SlopeTile_BR)//4
                {
                    sides.Add(1);
                    sides.Add(3);
                    sides.Add(4);
                }
                else if (block.SlopeTileId == Res.SlopeTile_TL)//7
                {
                    sides.Add(0);
                    sides.Add(2);
                    sides.Add(7);
                }
                else if (block.SlopeTileId == Res.SlopeTile_TR)//6
                {
                    sides.Add(1);
                    sides.Add(2);
                    sides.Add(6);
                }
            }
            else
            {
                sides.Add(0);
                sides.Add(1);
                sides.Add(2);
                sides.Add(3);
            }

            float min = float.MaxValue;
            vec2 a = new vec2(0, 0), b = new vec2(0, 0);
            foreach (int iSide in sides)
            {
                //Just using this for points.
                GetSidePoints(iSide, block.Box, ref a, ref b, false);

                vec2 midpoint = a + (b - a) * 0.5f;
                if (midpoint.Len2() < min)
                {
                    min = midpoint.Len2();
                    found = iSide;
                }


            }

            return found;
        }
        public vec2 GetNormalForSide(int side)
        {
            //0=L,R,T,B,  4= BRTL, TRBL, TLBR, BLTR

            if (side == 0) { return new vec2(-1, 0); }
            if (side == 1) { return new vec2(1, 0); }
            if (side == 2) { return new vec2(0, -1); }
            if (side == 3) { return new vec2(0, 1); }

            if (side == 4) { return new vec2(-1, -1).Normalized(); }//BR
            if (side == 5) { return new vec2(1, -1).Normalized(); }//BL
            if (side == 6) { return new vec2(-1, 1).Normalized(); }//TR            
            if (side == 7) { return new vec2(1, 1).Normalized(); }//TL

            return new vec2(0, -1);

        }
        public int SlopeToSide(int slopetileid)
        {
            //0=L,R,T,B,  
            if (slopetileid == Res.SlopeTile_BR) { return 4; }
            if (slopetileid == Res.SlopeTile_BL) { return 5; }
            if (slopetileid == Res.SlopeTile_TR) { return 6; }
            if (slopetileid == Res.SlopeTile_TL) { return 7; }
            System.Diagnostics.Debugger.Break();
            return 0;
        }
        public int SideToSlope(int sideid)
        {
            //0=L,R,T,3-B, 
            if (sideid == 4) { return Res.SlopeTile_BR; }
            if (sideid == 5) { return Res.SlopeTile_BL; }
            if (sideid == 6) { return Res.SlopeTile_TR; }
            if (sideid == 7) { return Res.SlopeTile_TL; }
            System.Diagnostics.Debugger.Break();
            return 0;
        }
        private TileBlock Midground(Cell c)
        {
            return c.Layers[PlatformLevel.Midground];
        }
        private bool HasMidgroundLand(Cell c)
        {
            return (c != null) && (c.Layers[PlatformLevel.Midground] != null) && (c.Layers[PlatformLevel.Midground].Blocking == true);
        }
        private void GetGroundCell4(Cell c, ref Cell found, ref int dir)
        {
            //Get the first solid/ground cell in in the H / V + of squares
            if (c == null)
            {
                return;
            }

            found = null;
            dir = -1;

            Dictionary<int, ivec2> dirs = new Dictionary<int, ivec2>();
            // dirs.Add(-1, new ivec2(0, 0));
            dirs.Add(0, new ivec2(-1, 0));
            dirs.Add(1, new ivec2(1, 0));
            dirs.Add(2, new ivec2(0, -1));
            dirs.Add(3, new ivec2(0, 1));

            foreach (int key in dirs.Keys)
            {
                ivec2 v = new ivec2(0, 0);
                if (dirs.TryGetValue(key, out v))
                {
                    Cell cn;
                    cn = Level.Grid.GetNeighborCell(c, v.x, v.y);
                    if (cn.Layers[PlatformLevel.Midground] != null && cn.Layers[PlatformLevel.Midground].Blocking)
                    {
                        found = cn;
                        dir = key;
                    }
                }
                if (found != null)
                {
                    break;
                }
            }

        }
        private void HopCliff(Guy guy, bool LContained, bool RContained, float dt)
        {
            ///a/Climb + Hop over ledges
            if (guy.Climbing == true)
            {
                if (LContained == false && guy.IsFacingLeft() == true)
                {
                    guy.Climbing = false;
                    guy.Vel = new vec2(-guy.ClimbSpeed * dt * 0.5f, -guy.ClimbSpeed * dt * 3);
                    guy.SetWalkSprite();
                }
                else if (RContained == false && guy.IsFacingRight() == true)
                {
                    guy.Climbing = false;
                    guy.Vel = new vec2(guy.ClimbSpeed * dt * 0.5f, -guy.ClimbSpeed * dt * 3);
                    guy.SetWalkSprite();
                }
            }

        }
        private void KeepGuyInWorld(Guy guy, ref bool LContained, ref bool RContained, bool LeftButtonDown, bool RightButtonDown)
        {
            //Prevent the guy from going below / Left / Right of world box.
            if ((guy.Pos.y + guy.Origin.y + guy.BoxRelative.Min.y) < Level.Grid.RootNode.Box.Min.y)
            {
                CollideBottom(guy, null, null, Level.Grid.RootNode.Box.Max.y);
            }

            if ((guy.Pos.y + guy.Origin.y + guy.BoxRelative.Max.y) >= Level.Grid.RootNode.Box.Max.y)
            {
                CollideBottom(guy, null, null, Level.Grid.RootNode.Box.Max.y);
            }

            if ((guy.Pos.x + guy.Origin.x + guy.BoxRelative.Min.x) < Level.Grid.RootNode.Box.Min.x)
            {
                CollideRightOrLeft(guy, true, false, false, Level.Grid.RootNode.Box.Min, null, ref LContained, LeftButtonDown, 0);
            }
            if ((guy.Pos.x + guy.Origin.x + guy.BoxRelative.Max.x) >= Level.Grid.RootNode.Box.Max.x)
            {
                CollideRightOrLeft(guy, false, false, true, Level.Grid.RootNode.Box.Max, null, ref RContained, RightButtonDown, 0);
            }
        }
        private void GuyCollideMidbackCell(Guy guy, Cell cell1, ref bool LContained, ref bool RContained, bool UpButtonDown)
        {
            TileBlock tile = cell1.Layers[PlatformLevel.Midback];
            if (tile == null)
            {
                return;
            }

            if (tile.Tile.TileId == Res.LadderTileId_L)
            {
                if (tile.Box.ContainsPointInclusive(guy.cposL))
                {
                    LContained = true;

                    if (UpButtonDown)
                    {
                        if (guy.Climbing == false)
                        {
                            guy.Climbing = true;
                            guy.Crouching = false;
                            guy.ClimbFace = SpriteEffects.FlipHorizontally;
                            //guy.Hanging = false;
                            guy.SetSprite(guy.ClimbSprite);
                            guy.Jumping = false;
                            guy.Rotation = guy.RotationDelta = 0;
                            guy.VaporTrail = 0;
                        }
                    }
                }
            }
            if (tile.Tile.TileId == Res.LadderTileId_R)
            {
                if (tile.Box.ContainsPointInclusive(guy.cposR))
                {
                    RContained = true;
                    if (UpButtonDown)
                    {
                        if (guy.Climbing == false)
                        {
                            guy.Climbing = true;
                            guy.Crouching = false;
                            guy.ClimbFace = SpriteEffects.None;
                            //guy.Hanging = false;
                            guy.SetSprite(guy.ClimbSprite);
                            guy.Jumping = false;
                            guy.Rotation = guy.RotationDelta = 0;
                            guy.VaporTrail = 0;

                        }
                    }
                }

            }

        }
        private void UpdateAI_Character(Guy guy, float dt, bool LContained, bool RContained)
        {
            guy.AIActionTime -= dt;
            if (guy.AIActionTime <= 0)
            {
                guy.AIActionTime = 0;
                if (guy.AIState == AIState.SwimLeftRight)
                {
                    //Do nothing
                }
                if (guy.AIState == AIState.Wander)
                {
                    guy.AIState = AIState.Idle;
                    guy.AIActionTime = Globals.Random(0.5f, 6);
                    guy.Animate = false;
                    guy.Joystick.AILeft = false;
                    guy.Joystick.AIRight = false;
                    guy.SetFrame(0);
                }
                else if (guy.AIState == AIState.Idle)
                {
                    guy.AISetRandomDir();
                    guy.AIState = AIState.Wander;
                    guy.AIActionTime = Globals.Random(5, 8);
                    guy.Animate = true;
                    if (Globals.Random(0, 1) > 0.8)
                    {
                        guy.DoPlayGrowl = true;
                    }
                }
            }
            else
            {

            }

            Player player = GetPlayer();
            if (guy.AIState == AIState.Wander)
            {
                if (guy.Joystick.Right.TouchState == TouchState.Up && guy.Joystick.Left.TouchState == TouchState.Up)
                {
                    guy.AISetRandomDir();
                }

                if (guy.Joystick.AIJump)
                {
                    //Here we release jump at the right time to get over the lbock
                    if (guy.OnGround == true)
                    {
                        guy.Joystick.AIJump = false;
                    }
                    else if (guy.SpriteEffects == SpriteEffects.None)//Facing right
                    {
                        Cell rlcell = Level.Grid.GetCellForPoint(guy.Box.Center() + new vec2(16, 8));
                        if (rlcell == null || rlcell.Layers[PlatformLevel.Midground] == null ||
                            rlcell.Layers[PlatformLevel.Midground].Tile.Blocking == false)
                        {
                            guy.Joystick.AIJump = false;

                        }
                    }
                    else if (guy.SpriteEffects == SpriteEffects.FlipHorizontally)
                    {
                        Cell rlcell = Level.Grid.GetCellForPoint(guy.Box.Center() + new vec2(-16, 8));
                        if (rlcell == null || rlcell.Layers[PlatformLevel.Midground] == null ||
                            rlcell.Layers[PlatformLevel.Midground].Tile.Blocking == false)
                        {
                            guy.Joystick.AIJump = false;

                        }
                    }
                }


                if (LContained)
                {
                    if (guy.OnGround == true /*&& guy.Hanging == false*/ && guy.Climbing == false && guy.CanJump)
                    {
                        bool doJump = Globals.Random(0, 10) > 1;
                        if (doJump == true)
                        {
                            guy.Joystick.AIJump = true;

                        }
                        else
                        {
                            //Reverse Direction
                            guy.Joystick.AIRight = true;
                            guy.Joystick.AILeft = false;
                        }
                    }
                }
                if (RContained)
                {
                    if (guy.OnGround == true /*&& guy.Hanging == false*/ && guy.Climbing == false && guy.CanJump)
                    {
                        bool doJump = Globals.Random(0, 10) > 1;
                        if (doJump == true)
                        {
                            guy.Joystick.AIJump = true;
                        }
                        else
                        {
                            //Reverse Direction
                            guy.Joystick.AIRight = false;
                            guy.Joystick.AILeft = true;
                        }
                    }
                }
            }
            else if (guy.AIState == AIState.Idle)
            {

            }
            else if (guy.AIState == AIState.Talk)
            {
                //Nothing, same as idle really
            }
            else if (guy.AIState == AIState.SwimLeftRight)
            {
                if (LContained)
                {
                    //Reverse Direction
                    guy.Joystick.AIRight = true;
                    guy.Joystick.AILeft = false;
                }
                if (RContained)
                {
                    //Reverse Direction
                    guy.Joystick.AIRight = false;
                    guy.Joystick.AILeft = true;
                }
            }

        }
        private void UpdateAI_Common(Guy guy, float dt)
        {
            if (guy.AIState == AIState.Sleep)
            {
                if (guy.Sprite == null || guy.Sprite.Name != guy.SleepSprite)
                {

                    guy.SetSprite(guy.SleepSprite);
                    guy.ScalePingpongY = true;
                    guy.ScaleDelta.y = 0.1f;
                    guy.ScalePingpongYRange = new vec2(0.81f, 1.0f);
                    guy.ScalePingpongX = true;
                    guy.ScaleDelta.x = 0.04f;

                    guy.ScalePingpongXRange = new vec2(0.98f, 1.0f);
                }

                if ((GetPlayer().Box.Center() - guy.Box.Center()).Len2() < (guy.WakeRadiusPixels * guy.WakeRadiusPixels))
                {
                    CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 10,
                                            guy.Box.Center(), 60, 70,
                                                new vec4(.7f, .7f, 1f, 1),
                                                7.8f, 0, 0, 0,
                                                Res.Tiles.GetWHVec() * 0.5f);

                    guy.AIState = AIState.Attack;

                    guy.ScalePingpongY = false;
                    guy.ScalePingpongX = false;
                    guy.ScaleDelta = 0;
                    guy.Scale = 1.0f;
                }
            }
        }
        //private void UpdateAI_PlantBombGuy(Guy guy, float dt)
        //{
        //    Guy player = GetPlayer();

        //    //Check for radial Defense (plant bomb guy)
        //    if (guy.AIState != AIState.Defend)
        //    {
        //        if (guy.AIState != AIState.Sleep)
        //        {
        //            if (guy.CanDefend)
        //            {
        //                if ((GetPlayer().Box.Center() - guy.Box.Center()).Len2() <= (guy.DefendRadiusPixels * guy.DefendRadiusPixels))
        //                {
        //                    guy.SetSprite(guy.DefendSprite);
        //                    guy.Loop = false;
        //                    guy.AIState = AIState.Defend;
        //                    //Little hop
        //                    guy.ScalePingpongY = true;
        //                    guy.ScaleDelta.y = 1.0f / guy.Sprite.DurationSeconds;
        //                    guy.ScalePingpongYRange = new vec2(0.80f, 1.2f);
        //                    Res.Audio.PlaySound(Res.SfxPlantBombGuyHide);//the opposite sounds better
        //                }
        //            }
        //        }
        //    }

        //    float AttackRange = (float)Math.Pow(Res.Tiles.TileWidthPixels * 5, 2);

        //    if (guy.AIState == AIState.Idle)
        //    {
        //        guy.SetSprite(guy.IdleSprite, false, 0);//Idle. Wait for next attack
        //        OrientMonsterToPlayer(guy, player);

        //        if ((player.Box.Center() - guy.Box.Center()).Len2() < AttackRange)
        //        {
        //            guy.AIState = AIState.Attack;
        //        }
        //    }
        //    else if (guy.AIState == AIState.Defend)
        //    {
        //        OrientMonsterToPlayer(guy, player);

        //        if (guy.IsAnimationComplete())
        //        {
        //            //Stop the little hop
        //            guy.ScalePingpongY = false;
        //            guy.ScaleDelta.y = 0;
        //            guy.Scale = 1;
        //        }

        //        if ((player.Box.Center() - guy.Box.Center()).Len2() > (guy.DefendRadiusPixels * guy.DefendRadiusPixels))
        //        {
        //            Res.Audio.PlaySound(Res.SfxPlantBombGuyUnHide);//the opposite sounds better

        //            guy.AIState = AIState.Attack;
        //        }
        //    }
        //    else if (guy.AIState == AIState.Attack)
        //    {
        //        if ((player.Box.Center() - guy.Box.Center()).Len2() > AttackRange)
        //        {
        //            guy.AIState = AIState.Idle;
        //        }
        //        else
        //        {

        //            OrientMonsterToPlayer(guy, player);

        //            guy.AttackTime -= dt;

        //            if (guy.AttackTime <= 0 && (
        //                guy.LastItemThrown == null || (
        //                (guy.LastItemThrown as Bomb != null) &&
        //                ((guy.LastItemThrown as Bomb).Exploded == true)
        //                )))
        //            {
        //                guy.AttackTime = guy.MaxAttackTime;
        //                guy.SetSprite(guy.WalkAttackSprite, false);
        //                guy.Animate = true;

        //                //Attack - somehow. If we are a plant bomb monster. Attack with plant bombs.  Geez, this is kind of confusing.
        //                //Bomb b = MakeBomb(guy, Res.SprPlantBomb, 0.5f, string.Empty, Res.SfxPlantBombExplode, 2, true, true);
        //                ThrowHeldItem(guy, dt, ((player.Box.Center() - guy.Box.Center()).Normalized() + new vec2(0, -1)).Normalized(), 9600);

        //                //https://blog.forrestthewoods.com/solving-ballistic-trajectories-b0165523348c
        //                float speed = 200;
        //                float x = guy.Box.Center().x - player.Box.Center().x;
        //                float y = 0;
        //                float s2 = speed * speed;

        //                float g = Gravity.y;
        //                float desc = s2 * s2 - g * (g * x * x + 2 * y * s2);
        //                if (desc < 0)
        //                {
        //                    //Not enough speed*
        //                    //Can't sqrt this
        //                    int nx = 0;
        //                    nx++;
        //                }
        //                else
        //                {
        //                    float n = (float)Math.Sqrt(desc);
        //                    float r1n = s2 + n;
        //                    float r2n = s2 - n;

        //                    float r1 = (float)Math.Atan(r1n / (g * x));
        //                    float r2 = (float)Math.Atan(r2n / (g * x));

        //                    //  r1 -= (float)Math.PI;
        //                    //  r2 -= (float)Math.PI;

        //                    if (r1 < r2)
        //                    {
        //                    //    b.Vel.x = (float)Math.Cos(r1);
        //                    //    b.Vel.y = (float)Math.Sin(r1);

        //                    }
        //                    else if (r2 < r1)
        //                    {
        //                   //     b.Vel.x = (float)Math.Cos(r2);
        //                    //    b.Vel.y = (float)Math.Sin(r2);

        //                    }

        //                 //   b.Vel *= speed;
        //                 //   b.Gravity = Gravity;
        //                }
        //                //int nss = 0;
        //                //nss++;


        //            }

        //            if (guy.IsAnimationComplete())
        //            {
        //                guy.SetSprite(guy.IdleSprite, false, 0);//Idle. Wait for next attack
        //            }
        //        }

        //    }

        //}
        public static double FaceObject(Vector2 position, Vector2 target)
        {
            // Rotates one object to face another object (or position)
            return (Math.Atan2(position.Y - target.Y, position.X - target.X) * (180 / Math.PI));
        }
        public static Vector2 MoveTowards(Vector2 position, Vector2 target, float speed)
        {
            // Creates a Vector2 to use when moving object from position to a target, with a given speed
            double direction = (float)(Math.Atan2(target.Y - position.Y, target.X - position.X) * 180 / Math.PI);

            Vector2 move = new Vector2(0, 0);

            move.X = (float)Math.Cos(direction * Math.PI / 180) * speed;
            move.Y = (float)Math.Sin(direction * Math.PI / 180) * speed;

            return move;
        }
        private void OrientMonsterToPlayer(Guy monster, Guy player)
        {
            if ((player.Box.Center() - monster.Box.Center()).x < 0)
            {
                monster.SpriteEffects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                monster.SpriteEffects = SpriteEffects.None;
            }
        }
        private void GuyCollideMidgroundCell(Guy guy, Cell cell1, ref bool LContained, ref bool RContained, bool LeftButtonDown, bool RightButtonDown, bool UpButtonDown)
        {
            TileBlock block = cell1.Layers[PlatformLevel.Midground];

            if (block == null)
            {
                return;
            }
            if (block.Tile.Blocking)
            {
                bool playFallThrough = false;
                if (block.Box.ContainsPointInclusive(guy.cposB))
                {
                    if (block.FallThrough == true)
                    {
                        playFallThrough = true;
                    }
                    else
                    {
                        CollideBottom(guy, cell1, block, block.Pos.y);
                    }
                }
                if (block.Box.ContainsPointInclusive(guy.cposT))
                {
                    //Don't fall through top blocks - keep them blocking
                    CollideTop(guy, cell1, block);
                }
                if (block.Box.ContainsPointInclusive(guy.cposR))
                {
                    if (block.FallThrough == true)
                    {
                        //do nothing don't grapple, nothing
                        //prevents erroneous l/r destroyng the block from below
                    }
                    else
                    {
                        if (block.IsSlope() == false)
                        {
                            CollideRightOrLeft(guy, false, block.Tile.CanClimb, guy.IsFacingLeft(), block.Pos,
                                cell1, ref RContained, RightButtonDown, block.Box.Width());
                        }
                    }
                }
                if (block.Box.ContainsPointInclusive(guy.cposL))
                {
                    if (block.FallThrough == true)
                    {
                        //do nothing don't grapple, nothing
                        //prevents erroneous l/r destroyng the block from below
                    }
                    else
                    {
                        if (block.IsSlope() == false)
                        {
                            CollideRightOrLeft(guy, true, block.Tile.CanClimb, guy.IsFacingLeft(), block.Pos,
                                cell1, ref LContained, LeftButtonDown, block.Box.Width());
                        }
                    }
                }

                if (playFallThrough)
                {
                    //Just destroy the block as though it weren't there.
                    //Don't let the player grab onto another tile.
                    //destroy it.. how
                    Level.DestroyTile(cell1, PlatformLevel.Midground);
                    Res.Audio.PlaySound(Res.SfxFallthrough);
                    CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 10,
                        block.Box.Center(), 60, 70,
                            new vec4(.6f, .7f, .2f, 1), 7.8f, 8, 0, 0,
                        Res.Tiles.GetWHVec() * 0.5f, Gravity);

                    //Show Dissolve Animation
                    CreateParticles(Res.SprFallThroughDissolve, ParticleLife.AnimationComplete, 1, block.Box.Min, 0, 0, new vec4(1f, 1f, 1f, 1), 0, 0, 0, 0,
                        new vec2(0, 0), default(vec2), false, new vec2(0, 0), 1, false, default(vec2), 0, 0, 0, false, default(vec4),
                        16, false, true);
                }

            }
        }
        private void CollideTop(Guy guy, Cell cell1, TileBlock block)
        {
            bool collided = false;
            if (guy.Vel.y < 0)
            {

                if (block.IsSlope() == false)
                {
                    guy.Pos.y = block.Pos.y + Res.Tiles.TileHeightPixels - guy.cposrelT.y + 0.01f;
                    collided = true;
                }
                else if (block.SlopeTileId == Res.SlopeTile_TL)
                {
                    vec2 a = new vec2(cell1.Box().Min.x, cell1.Box().Max.y);
                    vec2 b = new vec2(cell1.Box().Max.x, cell1.Box().Min.y);

                    float pct = (guy.cposT.x - a.x) / cell1.Box().Width();//16..
                    guy.Pos.y = a.y + (b.y - a.y) * pct - guy.cposrelT.y + 0.01f;
                    collided = true;
                }
                else if (block.SlopeTileId == Res.SlopeTile_TR)
                {
                    vec2 a = new vec2(cell1.Box().Min.x, cell1.Box().Min.y);
                    vec2 b = new vec2(cell1.Box().Max.x, cell1.Box().Max.y);

                    float pct = (guy.cposT.x - a.x) / cell1.Box().Width();//16 this is easiliy simplified
                    guy.Pos.y = a.y + (b.y - a.y) * pct - guy.cposrelT.y + 0.01f;
                    collided = true;
                }
                else if (block.SlopeTileId == Res.SlopeTile_BR) { }
                else if (block.SlopeTileId == Res.SlopeTile_BL) { }
                else
                {
                    //This should never hit
                    System.Diagnostics.Debugger.Break();
                }

                if (collided)
                {
                    guy.Vel.y = 0;
                    guy.CalcRelPos();
                    guy.CalcBoundBox();
                }
            }


        }
        private bool IsGround(Cell cell, bool bLeft)
        {
            //Return true if the current cell we are on is ground
            Cell cellRL = Level.Grid.GetNeighborCell(cell, bLeft ? 1 : -1, 0);
            bool bGround = (cellRL != null) && (cellRL.Layers[PlatformLevel.Midground] != null) && (cellRL.Layers[PlatformLevel.Midground].Blocking);
            return bGround;
        }
        private void CollideRightOrLeft(Guy guy, bool bLeft, bool can_climb, bool is_facing_left, vec2 block_pos,
            Cell cell1, ref bool ROrLContained, bool RightOrLeftButtonDown, float block_width)
        {
            //Check to make sure the cell to the right/left is not ground.  This is mostly for slopes because the player
            //will collide with corner tiles.
            if (IsGround(cell1, bLeft))
            {
                return;
            }

            ROrLContained = true;

            if (guy.IsFacingRight() == !bLeft && cell1 != null)
            {
                Cell cabove = Level.Grid.GetCellAbove(cell1);

                if (guy.OnSlope == false && (cabove == null || cabove.Layers[PlatformLevel.Midground] == null ||
                    (can_climb == true && (guy.IsFacingRight() == !bLeft) && (is_facing_left == !bLeft))) && RightOrLeftButtonDown)
                {
                    if (guy.Climbing == true)
                    {
                    }
                    else
                    {
                        if (RightOrLeftButtonDown)
                        {
                            //    //"Caught" ledge without player
                            //    if (bLeft)
                            //    {

                            //        guy.Pos = new vec2(block_pos.x + Res.Tiles.TileWidthPixels - guy.cposrelL.x, block_pos.y - 2);
                            //    }
                            //    else
                            //    {
                            //        guy.Pos = new vec2(block_pos.x - guy.cposrelR.x, block_pos.y - 2);
                            //    }

                            //    guy.Hanging = true;
                            //    guy.ClimbFace = bLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                            //    guy.Climbing = false;
                            //    guy.SetSprite(guy.HangSprite);
                            //    guy.Rotation = guy.RotationDelta = 0;
                            //    guy.VaporTrail = 0;

                            //}
                            //else
                            //{
                            guy.Climbing = true;
                            guy.ClimbFace = bLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                            //   guy.Hanging = false;
                            guy.SetSprite(guy.ClimbSprite);
                            guy.Rotation = guy.RotationDelta = 0;
                            guy.VaporTrail = 0;

                        }

                        guy.Jumping = false;
                    }
                }
                guy.ClimbType = 1;
            }

            //Both the velocity direction check and setting it to zero
            //seems to save us from sticky situations, HOWever
            //it prevents the player from jumping when running up a hill or something
            //since L/R naturally gets stuck in corner blocks
            if (bLeft == false)
            {
                if (guy.Vel.x > 0)
                {
                    guy.Pos.x = block_pos.x - guy.cposrelR.x;
                    guy.Vel.x = 0;
                    guy.CalcRelPos();
                    guy.CalcBoundBox();
                }
            }
            else
            {
                if (guy.Vel.x < 0)
                {
                    guy.Pos.x = block_pos.x + block_width - guy.cposrelL.x;
                    guy.Vel.x = 0;
                    guy.CalcRelPos();
                    guy.CalcBoundBox();

                }
            }

        }
        private void CollideBottom(Guy guy, Cell cell1, TileBlock block, float collide_pos_y)
        {
            //The reason we can jump into angles  is that when y == 0
            //if (guy.Vel.y > 0)
            {
                bool collided = false;
                //Check for slope, or do a basic xy Plane collision
                if (block == null || block.IsSlope() == false)
                {
                    if (guy.Vel.y > 0)
                    {
                        guy.Pos.y = collide_pos_y - guy.cposrelB.y;
                        //guy.Vel.y = 0;
                        collided = true;
                    }
                }
                else if (block.SlopeTileId == Res.SlopeTile_BR)
                {
                    if (guy.Vel.x > 0 || guy.Vel.y > 0)
                    {
                        vec2 a = new vec2(cell1.Box().Min.x, cell1.Box().Max.y);
                        vec2 b = new vec2(cell1.Box().Max.x, cell1.Box().Min.y);

                        float pct = (guy.cposB.x - a.x) / cell1.Box().Width();
                        float newy = a.y + (b.y - a.y) * pct - guy.cposrelB.y;

                        if (guy.Pos.y > newy)
                        {
                            guy.Pos.y = newy;
                            collided = true;
                            guy.OnSlope = true;
                        }
                    }
                }
                else if (block.SlopeTileId == Res.SlopeTile_BL)
                {
                    if (guy.Vel.x < 0 || guy.Vel.y > 0)
                    {
                        vec2 a = new vec2(cell1.Box().Min.x, cell1.Box().Min.y);
                        vec2 b = new vec2(cell1.Box().Max.x, cell1.Box().Max.y);

                        float pct = (guy.cposB.x - a.x) / cell1.Box().Width();
                        float newy = a.y + (b.y - a.y) * pct - guy.cposrelB.y;

                        if (guy.Pos.y > newy)
                        {
                            guy.Pos.y = newy;
                            collided = true;
                            guy.OnSlope = true;
                        }
                    }
                }
                else if (block.SlopeTileId == Res.SlopeTile_TR) { }
                else if (block.SlopeTileId == Res.SlopeTile_TL) { }

                if (collided)
                {
                    guy.OnGround = true;
                    guy.Rotation = guy.RotationDelta = 0;
                    guy.VaporTrail = 0;
                    guy.Vel.y = 0;

                    if (this.JumpState >= 4)
                    {
                        //End of game.
                        EndGame();
                    }

                    if (guy.LastOnGround == false)
                    {
                        if (landSound == null && guy.IsPlayer())
                        {
                            landSound = Res.Audio.PlaySound(Res.SfxLand);
                        }
                        //Set sprite to guy walk.
                        guy.SetSprite(guy.WalkSprite);
                    }

                    if (guy.Jumping == true)
                    {
                        guy.Jumping = false;
                        guy.SetWalkSprite();
                        if (jumpSound != null)
                        {
                            jumpSound.Stop();
                            jumpSound = null;
                        }

                    }
                    guy.Airtime = guy.MaxAirtime;
                    guy.CalcRelPos();
                    guy.CalcBoundBox();
                }

            }


        }
        public vec2 SlideSlope(vec2 p, vec2 v, vec2 n)
        {
            vec2 reflect = v - n * (v.Dot(n)) * 2.0f;
            vec2 slide_dir = (p + v + reflect) - p;
            slide_dir.Normalize();

            vec2 res = slide_dir * v.Len();
            return res;
        }
        private void GuyCollideForegroundCell(Player guy, Cell cell1, bool LeftButtonDown, bool RightButtonDown)
        {
            TileBlock tile = cell1.Layers[PlatformLevel.Foreground];
            if (tile == null)
            {
                return;
            }
            if (guy.IsPlayer())
            {
                if (tile.Box.ContainsPointInclusive(guy.cposC))
                {
                    //Do grass only when player is Walking
                    if (LeftButtonDown || RightButtonDown)
                    {
                        if (guy.LastOnGround == true)//Must use last - because we set OnGround to false
                        {
                            if (tile.Tile.Sprite != null && tile.Tile.TileId == Res.MonsterGrassTileId)
                            {
                                if (guy.LastCollideGrass <= 0.0001f)
                                {
                                    guy.LastCollideGrass = guy.LastCollideGrassMax;
                                    Res.Audio.PlaySound(Res.SfxGrassMove);

                                }
                            }
                        }
                    }
                }
            }
        }

        private bool DamageTiles(float power, Box2f box)
        {
            //Cell[] cells = Level.Grid.GetSurroundingCells(Level.Grid.GetCellForPoint(ob.Box.Center()), true);
            List<Cell> cells = Level.Grid.GetCellManifoldForBox(box);
            return DamageTiles(cells, power);
        }
        private bool DamageTiles(float power, vec2 point)
        {
            Cell c = Level.Grid.GetCellForPoint(point);

            if (c != null)
            {
                return DamageTiles(new List<Cell>() { c }, power);
            }
            return false;
        }
        private bool DamageTiles(List<Cell> cells, float power)
        {
            //Attack the first tile layer, return true if we hit something.
            bool bDamage = false;
            foreach (Cell c in cells)
            {
                if (AttackTileLayer(power, c, PlatformLevel.Foreground))
                {
                    bDamage = true;
                    break;
                }
                else if (AttackTileLayer(power, c, PlatformLevel.Midground))
                {
                    bDamage = true;
                    break;
                }
                else if (AttackTileLayer(power, c, PlatformLevel.Midback))
                {
                    bDamage = true;
                    break;
                }
                else if (AttackTileLayer(power, c, PlatformLevel.Background))
                {
                    bDamage = true;
                    break;
                }
            }

            return bDamage;
        }
        private void AttackBadGuys(Player player)
        {
            //Attack Bad guys
            //vec2 hp = GetSwordHitPoint(player);
            Box2f swordbox = GetSwordHitBox(player);

            //List<GameObject> remove = new List<GameObject>();
            //for (int iObj = ViewportObjsFrame.Count - 1; iObj >= 0; iObj--)
            foreach (GameObject obOther in ViewportObjsFrame)
            {
                //GameObject obOther = ViewportObjsFrame[iObj];
                Guy obOtherGuy = obOther as Guy;
                if (obOtherGuy != null)
                {
                    if (obOtherGuy.IsNpc == false)
                    {
                        if (obOther.CanAttack)
                        {

                            if (obOtherGuy.DoPlayGrowl)
                            {
                                //Only play growl sound if we're visible
                                obOtherGuy.PlayGrowlSound();
                                obOtherGuy.DoPlayGrowl = false;
                            }

                            if (obOther != player)
                            {

                                Box2f b = obOtherGuy.Box;
                                if (b.BoxIntersect_EasyOut_Inclusive(swordbox))
                                {
                                    AttackBadGuy(obOtherGuy, player.Power);

                                }
                            }
                        }
                    }
                }
            }


        }
        private bool AttackBadGuy(Guy obOtherGuy, float power)
        {
            if (obOtherGuy.IsDeleted == true)
            {
                return false;
            }
            bool bDead = false;
            if (obOtherGuy.HurtTime < 0.0001f)
            {
                obOtherGuy.HurtTime = obOtherGuy.HurtTimeMax;
                obOtherGuy.Health -= power;


                if (obOtherGuy.Health <= 0)
                {
                    bDead = true;
                    obOtherGuy.PlayDieSound();

                    FlagDeleteObject(obOtherGuy);

                    for (int iParticle = 0; iParticle < 5; ++iParticle)
                    {
                        MakeSmokeParticle(obOtherGuy.Box.Center() + Globals.RandomDirection() * obOtherGuy.Box.Width() * 0.5f, -0.5f, 0.3f);
                    }

                    for (int iParticle = 0; iParticle < 10; ++iParticle)
                    {
                        MakeBloodParticle(obOtherGuy.Box.Center() + Globals.RandomDirection() * obOtherGuy.Box.Width() * 0.5f);
                    }
                    //Show the naughty message
                    if (obOtherGuy.IsNpc == true)
                    {
                        // PlayNpcKilledNaughtyMessage(obOtherGuy);
                    }
                }
                else
                {
                    obOtherGuy.PlayHitSound();

                    for (int iParticle = 0; iParticle < 5; ++iParticle)
                    {
                        MakeBloodParticle(obOtherGuy.Box.Center() + Globals.RandomDirection() * obOtherGuy.Box.Width() * 0.5f);
                    }
                }
            }

            return bDead;
        }
        private void PlayNpcKilledNaughtyMessage(Guy obOtherGuy)
        {
            float screen_fade_time_max = 2.0f;
            float screen_fade_time = screen_fade_time_max;
            float text_fade_in_time = 2.0f;
            float text_fade_out_time = 1.0f;
            float cur_fade_time = 0;

            Cutscene = new Cutscene()
                .Then(0, (s, dt) =>
                {
                    GameState = GameState.Pause;
                    bShowUI = false;
                    bShowCursor = false;
                    return false;
                }, null)
                .Then(0, (s, dt) =>
                {
                    //fade screen
                    screen_fade_time -= dt;
                    float r = 0.3f + (screen_fade_time / screen_fade_time_max) * 0.7f;

                    ScreenOverlayColor = new vec4(r, r, r, 1.0f);
                    return screen_fade_time > 0.0f;
                }, null)
                .Wait(2.0f)
                //////////////////////////////////////////////////////////////////////////
                .Then(0, (s, dt) =>
                {
                    ScreenOverlayText = "You killed " + obOtherGuy.NpcName + "..";
                    cur_fade_time = text_fade_in_time;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_in_time;
                    return false;
                }, null)
                .Then(0, (s, dt) =>
                {
                    //text fadein
                    cur_fade_time -= dt;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_in_time;
                    return cur_fade_time > 0;
                }, null)
                .Wait(2.0f)
                .Then(0, (s, dt) =>
                {
                    //fadeout
                    cur_fade_time += dt;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_out_time;
                    return cur_fade_time < text_fade_out_time;
                }, null)
                .Wait(1.0f)
                //////////////////////////////////////////////////////////////////////////
                .Then(0, (s, dt) =>
                {
                    ScreenOverlayText = "What did DAN ever do to you?";
                    cur_fade_time = text_fade_in_time;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_in_time;
                    return false;
                }, null)
                .Then(0, (s, dt) =>
                {
                    //text fadein
                    cur_fade_time -= dt;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_in_time;
                    return cur_fade_time > 0;
                }, null)
                .Wait(2.0f)
                .Then(0, (s, dt) =>
                {
                    //fadeout
                    cur_fade_time += dt;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_out_time;
                    return cur_fade_time < text_fade_out_time;
                }, null)
                .Wait(2.0f)
                //////////////////////////////////////////////////////////////////////////
                .Then(0, (s, dt) =>
                {
                    ScreenOverlayText = "     Poor " + obOtherGuy.NpcName + ".     ";
                    cur_fade_time = text_fade_in_time;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_in_time;
                    return false;
                }, null)
                .Then(0, (s, dt) =>
                {
                    //text fadein
                    cur_fade_time -= dt;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_in_time;
                    return cur_fade_time > 0;
                }, null)
                .Wait(2.0f)
                .Then(0, (s, dt) =>
                {
                    //fadeout
                    cur_fade_time += dt;
                    ScreenOverlayTextFade = cur_fade_time / text_fade_out_time;
                    return cur_fade_time < text_fade_out_time;
                }, null)
                .Wait(2.0f)
                //////////////////////////////////////////////////////////////////////////



                .Then(0.0f, (s, dt) =>
                {
                    ScreenOverlayText = "";
                    bShowUI = true;
                    bShowCursor = true;
                    ScreenOverlayColor = new vec4(1, 1, 1, 1);
                    GameState = GameState.Play;
                    return s.Duration > 0;
                }, null)
                ;


        }
        private void MakeBloodParticle(vec2 pos)
        {
            CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 1,
                pos, -60, -70,
                new vec4(.7f, .2f, .2f, 1), 0, 1.5f, 0, 0,
                Res.Tiles.GetWHVec() * 0.5f, Gravity, true, default(vec2), 0.9f);
        }
        private bool AttackTileLayer(float power, Cell c, int iLayer)
        {
            TileBlock t = c.Layers[iLayer];
            if (t != null)
            {
                if (t.CanMine)
                {
                    DamageTileBlock(c, iLayer, t, power, true);

                    return true;
                }
            }

            return false;
        }
        private void DropRandomItem(vec2 atPos)
        {
            Player player = GetPlayer();
            List<PickupItemType> pickupItems = new List<PickupItemType>();

            pickupItems.Add(PickupItemType.Potion);
            pickupItems.Add(PickupItemType.Marble);
            if (player.BombsEnabled)
            {
                pickupItems.Add(PickupItemType.Bomb);
            }
            if (player.BowEnabled)
            {
                pickupItems.Add(PickupItemType.Arrow);
            }

            int n = Globals.RandomInt(0, pickupItems.Count + 1);
            if (n >= pickupItems.Count)
            {
                //drop nothing
                return;
            }

            DropItem(atPos, new vec2(Globals.Random(-20, 20), -100), pickupItems[n]);
        }

        private PickupItem DropItem(vec2 atPos, vec2 vel, PickupItemType type, float PickupTime = 0, float radius_pixels = 4, float scale = 0.5f)
        {
            string sprite = "";
            if (type == PickupItemType.Bomb)
            {
                sprite = Res.SprItemBomb;
            }
            else if (type == PickupItemType.Potion)
            {
                sprite = Res.SprItemPotion;
            }
            else if (type == PickupItemType.Marble)
            {
                sprite = Res.SprMarble;
            }
            else if (type == PickupItemType.Arrow)
            {
                sprite = Res.SprArrow;
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }

            PickupItem new_item = new PickupItem(this, type);
            new_item.Pos = atPos;
            new_item.Gravity = Gravity;
            new_item.Vel = vel;
            new_item.Origin = Res.Tiles.GetWHVec() * 0.5f;
            new_item.PhysicsBallRadiusPixels = radius_pixels;
            new_item.SetSprite(sprite);
            new_item.PhysicsResponse = PhysicsResponse.StayPut;
            new_item.PhysicsShape = PhysicsShape.Ball;
            new_item.Scale = scale;
            new_item.Friction = 0.55f;
            new_item.PickupTime = PickupTime;
            Level.GameObjects.Add(new_item);

            return new_item;
        }
        private void DamageTileBlock(Cell c, int iLayer, TileBlock t, float power, bool bPlaySound)
        {
            t.Health -= power;
            if (t.Tile.BlockType == BlockType.Hedge)
            {
                if (bPlaySound) { Res.Audio.PlaySound(Res.SfxGrassCut); }
                if (t.Health <= 0)
                {
                    Level.DestroyTile(c, iLayer);
                }
                DamageTileFx(t, 2);
            }
            else if (t.Tile.BlockType == BlockType.MonsterGrass)
            {
                if (bPlaySound) { Res.Audio.PlaySound(Res.SfxGrassCut); }
                if (t.Health <= 0)
                {
                    Level.DestroyTile(c, iLayer);
                    DropRandomItem(c.Box().Center());
                }
                DamageTileFx(t, 2);
            }
            else if (t.Tile.BlockType == BlockType.Vines)
            {
                if (bPlaySound) Res.Audio.PlaySound(Res.SfxGrassCut);
                if (t.Health <= 0)
                {
                    Level.DestroyTile(c, iLayer);
                    DropRandomItem(c.Box().Center());
                }
                DamageTileFx(t, 2);

                IterateCellBlocks(c, 0, 1, BlockType.Vines, iLayer, (cn) =>
                {
                    CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 5, cn.Box().Center(), -60, -70, new vec4(0, 1, 0, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
                    Level.DestroyTile(cn, iLayer);
                });
            }
            else if (t.Tile.BlockType == BlockType.Stone)
            {
                if (bPlaySound) Res.Audio.PlaySound(Res.SfxMine);
                if (t.Health <= 0)
                {

                    if (bPlaySound) Res.Audio.PlaySound(Res.SfxMine);
                    if (bPlaySound) Res.Audio.PlaySound(Res.SfxWhack);

                    //Tile is dead
                    Level.DestroyTile(c, iLayer);
                    DamageTileFx(t, 3);
                    DropRandomItem(c.Box().Center());
                }
                else
                {
                    DamageTileFx(t, 3);
                }
            }
            else if (t.Tile.BlockType == BlockType.Wood)
            {
                if (bPlaySound) Res.Audio.PlaySound(Res.SfxChop);
                if (t.Health <= 0)
                {
                    t.Health = 0;

                    //Destroy tree
                    if (bPlaySound) Res.Audio.PlaySound(Res.SfxWhack);

                    Level.DestroyTile(c, iLayer);
                    DamageTileFx(t, 3);

                    IterateCellBlocks(c, 0, -1, BlockType.Wood, iLayer, (cn) =>
                    {
                        CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 5, cn.Box().Center(), -60, -70, new vec4(.8f, .3f, .2f, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
                        Level.DestroyTile(cn, iLayer);
                        DamageTileFx(t, 3);
                    });

                    IterateCellBlocks(c, 0, 1, BlockType.Wood, iLayer, (cn) =>
                    {
                        CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, 5, cn.Box().Center(), -60, -70, new vec4(.8f, .3f, .2f, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
                        Level.DestroyTile(cn, iLayer);
                        DamageTileFx(t, 3);
                    });
                    DropRandomItem(c.Box().Center());
                }
                else
                {
                    DamageTileFx(t, 3);
                }
            }


        }
        private void DamageTileFx(TileBlock t, int n)
        {
            if (t == null) { return; }


            if (t.Health <= 0)
            {
                n *= 2;
            }

            //Monster Grass
            if (t.Tile.BlockType == BlockType.MonsterGrass)
            {
                CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, n, t.Box.Center(), -60, -70, new vec4(0, 1, 0, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
            }
            else if (t.Tile.BlockType == BlockType.Vines)
            {
                //Vines
                CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, n, t.Box.Center(), -60, -70, new vec4(0, 1, 0, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
            }
            else if (t.Tile.BlockType == BlockType.Stone)
            {
                //Rock
                CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, n, t.Box.Center(), -50, -70, new vec4(1, 1, 1, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
                CreateParticles(Res.SprParticleRock, ParticleLife.Seconds, n, t.Box.Center(), -60, -70, new vec4(1f, 1f, 1f, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
            }
            else if (t.Tile.BlockType == BlockType.Wood)
            {
                //Wood
                CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, n, t.Box.Center(), -50, -70, new vec4(1, 1, 1, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
                CreateParticles(Res.SprParticleSmall, ParticleLife.Seconds, n, t.Box.Center(), -60, -70, new vec4(.8f, .3f, .2f, 1), 0, 0, 0, 0, Res.Tiles.GetWHVec() * 0.5f, Gravity);
            }

        }
        private void IterateCellBlocks(Cell c, int dx, int dy, BlockType bt, int iLayer, Action<Cell> act)
        {
            //iterate all blocks in a given direction (for trees, vines, and grass)
            Cell cn = c;
            while (true)
            {
                cn = Level.Grid.GetNeighborCell(cn, dx, dy);
                if (cn == null || cn.Layers[iLayer] == null || cn.Layers[iLayer].Tile.BlockType != bt)
                {
                    break;
                }
                else
                {
                    act(cn);
                }
            }
        }
        private bool TileEmpty(vec2 pos)
        {
            return true;
        }
        private void UpdateViewport(float dt)
        {
            //Viewport Update
            Player p = GetPlayer();
            if (p == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            //Makes ure the viewport doesn't go past these values.
            Screen.Viewport.FollowObject(p);
            Screen.Viewport.LimitScrollBounds(Level.Grid.RootNode.Box);
            Screen.Viewport.CalcBoundBox();
        }
        public override void Draw(SpriteBatch sb)
        {
            //****THIS ISN"T CALLED*****

            //All this stuff is what we render to for screen effects
            // DrawGameWorld(sb);
            // DrawUI(sb);
        }
        public void DrawGameWorld(SpriteBatch sb)
        {
            if (DrawState == DrawState.Normal)
            {
                //All this stuff is what we render to for screen effects
                DrawBackground(sb);

                DrawNogo(sb);//Nogo/anything outside the level box such as backgroudn image

                DrawMidback(sb);
                //Draw objects separating guys in the top layer
                DrawObjects(sb, false);
                DrawObjects(sb, true);
                DrawForeground(sb);
                //Put Liquid BEFORE midground to hide the seamless liquid blocks
                DrawLiquid(sb);
                DrawMidground(sb);//Moved midground to AFTER foreground because we need water to be over bushes &c, but we need it to be behind blocks

                base.Draw(sb);

            }
            else if (DrawState == DrawState.PlayerOnly)
            {
                DrawObject(sb, GetPlayer(), new vec4(1, 1, 1, 1));
            }

            if (Cutscene != null)
            {
                Cutscene.Draw(sb);
            }

        }
        private vec4 WaterColorFromType(WaterType t)
        {
            if (t == WaterType.Water)
            {
                return new vec4(0.1f, 0.1f, 1.0f, 0.6f);
            }
            else if (t == WaterType.Lava)
            {
                return new vec4(0.99f, 0.656f, 0.0f, 0.9f);
            }
            else if (t == WaterType.Tar)
            {
                return new vec4(1 / 29, 1 / 38, 1 / 48, 0.9f);
            }
            return new vec4(1, 1, 1, 1);
        }
        private void DrawLiquidBlock(SpriteBatch sb, Box2f b, vec4 color, bool drawTop)
        {
            sb.Draw(Screen.PixelTexture, b.ToXNARect(), color.toXNAColor());

            if (drawTop)
            {
                vec4 colorTop = color;
                colorTop *= 1.5f;
                colorTop.w = color.w;

                Box2f b2 = b;
                b2.Min.y = b2.Min.y + (b2.Max.y - b2.Min.y) * 0.2f;

                sb.Draw(Screen.PixelTexture, b2.ToXNARect(), colorTop.toXNAColor());
            }

        }
        private void DrawLiquid(SpriteBatch sb)
        {
            int LavaInfluenceCount = 0;

            foreach (Cell c in ViewportCellsFrame)
            {
                if (c.WaterType == WaterType.Lava && c.Water > 0.2f && c.Layers[PlatformLevel.Midground] == null)
                {
                    LavaInfluenceCount++;
                }

                vec4 waterColor = WaterColorFromType(c.WaterType) * c.LightColor;

                Cell above = Level.Grid.GetNeighborCell(c, 0, -1);

                if (c.WaterTravelFrame)
                {
                    bool L = false, R = false;
                    if (above != null)
                    {
                        if (above.WaterOnLeft) { L = true; }
                        if (above.WaterOnRight) { R = true; }
                    }
                    waterColor = WaterColorFromType(above.WaterType);

                    Box2f b = c.Box();
                    //Set the box height based on the water level.
                    b.Max.y = b.Min.y + (b.Max.y - b.Min.y) * (1.0f - c.Water);
                    if (L == false && R == false)
                    {
                        //water traveled above
                    }
                    else
                    {
                        if (L == false) b.Min.x += 8;
                        if (R == false) b.Max.x -= 8;
                    }
                    Box2f gridBox = new Box2f(
                        Screen.Viewport.WorldToDevice(b.Min),
                        Screen.Viewport.WorldToDevice(b.Max)
                        );
                    DrawLiquidBlock(sb, gridBox, waterColor, false);
                    //sb.Draw(Screen.PixelTexture, gridBox.ToXNARect(), waterColor);
                }
                if (c.Water > 0.0f)
                {
                    Box2f b = c.Box();

                    //Set the box height based on the water level.
                    b.Min.y = b.Max.y + (b.Min.y - b.Max.y) * c.Water;

                    if (c.WaterOnLeft == false) { b.Min.x += 8; }
                    if (c.WaterOnRight == false) { b.Max.x -= 8; }
                    Box2f gridBox = new Box2f(
                        Screen.Viewport.WorldToDevice(b.Min),
                        Screen.Viewport.WorldToDevice(b.Max)
                        );

                    DrawLiquidBlock(sb, gridBox, waterColor, above == null || above.Water <= 0.0001f);
                }
                //Draw the "water seam" blocks
                if (c.Layers[PlatformLevel.Midground] != null)
                {
                    //If we have surrounding water, draw it in our tiles "fake" so that it looks seamless
                    Cell below = Level.Grid.GetNeighborCell(c, 0, 1);
                    Cell left = Level.Grid.GetNeighborCell(c, -1, 0);
                    Cell right = Level.Grid.GetNeighborCell(c, 1, 0);

                    float BL = 0.0f, BR = 0.0f, TL = 0.0f, TR = 0.0f;//Heights of the boxes


                    if (left != null && left.Water > 0)
                    {
                        TL = left.Water >= 0.5f ? (left.Water - 0.5f) * 2 : 0.0f;
                        BL = left.Water >= 0.5f ? 1.0f : left.Water * 2;
                    }
                    if (right != null && right.Water > 0)
                    {
                        //TR = true;
                        //BR = true;
                        TR = right.Water >= 0.5f ? (right.Water - 0.5f) * 2 : 0.0f;
                        BR = right.Water >= 0.5f ? 1.0f : right.Water * 2;
                    }
                    if (above != null && above.Water > 0)
                    {
                        if (above.WaterOnLeft) { TL = 1.0f; }
                        if (above.WaterOnRight) { TR = 1.0f; }
                    }
                    Box2f b, gridBox = new Box2f();
                    if (TL > 0)
                    {
                        b = c.Box(); b.Max.x -= 8; b.Max.y -= 8;
                        b.Min.y = b.Max.y + (b.Min.y - b.Max.y) * TL;
                        gridBox.Construct(Screen.Viewport.WorldToDevice(b.Min), Screen.Viewport.WorldToDevice(b.Max));
                        if (left != null) waterColor = WaterColorFromType(left.WaterType) * c.LightColor;

                        sb.Draw(Screen.PixelTexture, gridBox.ToXNARect(), waterColor.toXNAColor());
                    }
                    if (TR > 0)
                    {
                        b = c.Box(); b.Min.x += 8; b.Max.y -= 8;
                        b.Min.y = b.Max.y + (b.Min.y - b.Max.y) * TR;
                        gridBox.Construct(Screen.Viewport.WorldToDevice(b.Min), Screen.Viewport.WorldToDevice(b.Max));
                        if (right != null) waterColor = WaterColorFromType(right.WaterType) * c.LightColor;
                        sb.Draw(Screen.PixelTexture, gridBox.ToXNARect(), waterColor.toXNAColor());
                    }
                    if (BL > 0)
                    {
                        b = c.Box(); b.Max.x -= 8; b.Min.y += 8;
                        b.Min.y = b.Max.y + (b.Min.y - b.Max.y) * BL;
                        gridBox.Construct(Screen.Viewport.WorldToDevice(b.Min), Screen.Viewport.WorldToDevice(b.Max));
                        if (left != null) waterColor = WaterColorFromType(left.WaterType) * c.LightColor;
                        sb.Draw(Screen.PixelTexture, gridBox.ToXNARect(), waterColor.toXNAColor());
                    }
                    if (BR > 0)
                    {
                        b = c.Box(); b.Min.x += 8; b.Min.y += 8;
                        b.Min.y = b.Max.y + (b.Min.y - b.Max.y) * BR;
                        gridBox.Construct(Screen.Viewport.WorldToDevice(b.Min), Screen.Viewport.WorldToDevice(b.Max));
                        if (right != null) waterColor = WaterColorFromType(right.WaterType) * c.LightColor;
                        sb.Draw(Screen.PixelTexture, gridBox.ToXNARect(), waterColor.toXNAColor());
                    }

                }


            }


            float dt = Last_DT;
            if ((float)LavaInfluenceCount / (float)ViewportCellsFrame.Count > 0.2f)
            {
                LavaScreenDistort += LavaScreenDistortFadeSpeed * dt;
            }
            else
            {
                LavaScreenDistort -= LavaScreenDistortFadeSpeed * dt;
            }
            LavaScreenDistort = Globals.Clamp(LavaScreenDistort, 0, 1);

        }
        private void DrawBlock(SpriteBatch sb, Cell c, TileBlock block)
        {
            Screen.DrawFrame(sb, block.Sprite.Frames[block.FrameIndex], c.Pos(),
                Res.Tiles.GetWHVec(), c.LightColor.toXNAColor(),
                new vec2(1, 1), 0, default(vec2),//rot/scl/origin
                block.SpriteEffects
                );
        }
        private void DrawBackground(SpriteBatch sb)
        {
            //HORIZON
            Sprite sprHorizon = Res.Tiles.GetSprite(Res.SprHorizon);
            for (int iHorizon = 0; iHorizon < Math.Ceiling(Screen.Viewport.TilesWidth); ++iHorizon)
            {
                Screen.DrawFrame(sb,
                    sprHorizon.Frames[0],
                    Screen.Viewport.Pos + new vec2(iHorizon * Res.Tiles.TileWidthPixels, 0),
                    new vec2(1 * Res.Tiles.TileWidthPixels, Screen.Viewport.TilesHeight * Res.Tiles.TileHeightPixels),
                    Color.White * 1, new vec2(1, 1));
            }

            //float D = (Screen.Game.GraphicsDevice.Viewport.Width / Screen.Viewport.WidthPixels);

            //Backdrop 1
            Sprite sprBack = Res.Tiles.GetSprite(Res.SprMountainBackdrop);
            float backTilesWidth = sprBack.Frames[0].TilesWidth();
            float backTilesHeight = sprBack.Frames[0].TilesHeight();
            int count = (int)Math.Ceiling(Screen.Viewport.TilesWidth / backTilesWidth);
            float YOffset = 0;
            float YHeight = Res.Tiles.TileHeightPixels * backTilesHeight * 0.4f;
            for (int i = 0; i < count; ++i)
            {
                Screen.DrawFrame(sb,
                    sprBack.Frames[0],
                    Screen.Viewport.Pos + new vec2(i * backTilesWidth * Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels * 1 + YOffset - YHeight),
                    new vec2(backTilesWidth * Res.Tiles.TileWidthPixels, backTilesHeight * Res.Tiles.TileHeightPixels),
                    Color.White, new vec2(1, 1)
                    );
            }

            //Backdrop 2 / trees
            sprBack = Res.Tiles.GetSprite(Res.SprBackground_Trees);
            backTilesWidth = sprBack.Frames[0].TilesWidth() * 0.7f;
            count = (int)Math.Ceiling(Screen.Viewport.TilesWidth / backTilesWidth);
            YHeight = Res.Tiles.TileHeightPixels * backTilesHeight * 0.4f;
            YOffset = (Screen.Viewport.TilesHeight - 3.2f) * Res.Tiles.TileHeightPixels;
            for (int i = 0; i < count; ++i)
            {
                Screen.DrawFrame(sb,
                    sprBack.Frames[0],
                    Screen.Viewport.Pos + new vec2(i * backTilesWidth * Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels * 1 + YOffset - YHeight),
                    new vec2(backTilesWidth * Res.Tiles.TileWidthPixels + 0.1f, backTilesHeight * Res.Tiles.TileHeightPixels * 0.7f),
                    Color.White, new vec2(1, 1)
                    );
            }

            //Background TILES
            foreach (Cell c in ViewportCellsFrame)
            {
                TileBlock block = c.Layers[PlatformLevel.Background];
                if (block == null || block.Tile == null)
                {
                    continue;
                }
                DrawBlock(sb, c, block);
            }
        }
        private void DrawNogo(SpriteBatch sb)
        {

            //Draw the "blank" squares to blank the background out.
            if (Screen.Viewport.Box.Min.x > (Level.Grid.RootNode.Box.Min.x) &&
                Screen.Viewport.Box.Min.y > (Level.Grid.RootNode.Box.Min.y) &&
                Screen.Viewport.Box.Max.x < (Level.Grid.RootNode.Box.Max.x) &&
                Screen.Viewport.Box.Max.y < (Level.Grid.RootNode.Box.Max.y))
            {
                //We are within the level bounds
            }
            else
            {
                Frame f = Res.Tiles.GetSpriteFrame(Res.SprBlackNogo, 0);
                vec2 vpxy = this.Screen.Viewport.Pos;
                //subtract remainder
                float xm = vpxy.x % Res.Tiles.TileWidthPixels;
                float ym = vpxy.y % Res.Tiles.TileHeightPixels;
                if (xm < 0) xm = Res.Tiles.TileWidthPixels - Math.Abs(xm);
                if (ym < 0) ym = Res.Tiles.TileHeightPixels - Math.Abs(ym);

                vec2 vpc = vpxy - new vec2(xm, ym);

                int cw = (int)Math.Ceiling(Screen.Viewport.Box.Width() / Res.Tiles.TileWidthPixels);
                int ch = (int)Math.Ceiling(Screen.Viewport.Box.Height() / Res.Tiles.TileHeightPixels);

                for (int j = 0; j <= ch; ++j)
                {
                    for (int i = 0; i <= cw; ++i)
                    {
                        vec2 p = vpc + new vec2(i * 16 + 8, j * 16 + 8);
                        if (Level.Grid.RootNode.Box.ContainsPointInclusive(p) == false)
                        {
                            vec2 pos = vpc + new vec2(i * 16, j * 16);
                            Screen.DrawFrame(sb, f, pos, new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels), Color.White, new vec2(1, 1));
                        }
                    }
                }
            }

        }
        private void DrawMidback(SpriteBatch sb)
        {
            foreach (Cell c in ViewportCellsFrame)
            {
                TileBlock block = c.Layers[PlatformLevel.Midback];
                if (block == null || block.Tile == null)
                {
                    continue;
                }

                DrawBlock(sb, c, block);
            }
        }
        private void DrawMidground(SpriteBatch sb)
        {
            // float BaseLandY = GetBaseLandYPixels();
            Sprite crack = Res.Tiles.GetSprite(Res.SprCrack);

            foreach (Cell c in ViewportCellsFrame)
            {
                TileBlock block = c.Layers[PlatformLevel.Midground];
                if (block == null || block.Tile == null)
                {
                    continue;
                }

                DrawBlock(sb, c, block);

                if (block.Tile.BlockType == BlockType.Stone)
                {
                    //Block

                    //Tile Cract
                    int iCrack = 4 - (int)Math.Floor(4.0f * ((float)block.Health / block.Tile.MaxHealth));
                    if (iCrack > 0)
                    {
                        Screen.DrawFrame(sb, crack.Frames[iCrack - 1], c.Pos(), Res.Tiles.GetWHVec(), c.LightColor.toXNAColor(), new vec2(1, 1));
                    }
                }

            }



        }
        private void DrawObjects(SpriteBatch sb, bool guys = false)
        {
            foreach (GameObject ob in Level.GameObjects)
            {
                if (((ob as Guy != null) && (guys == true)) || ((ob as Guy == null) && (guys == false)))
                {

                    if (ob.Visible && ob.Frame != null)
                    {

                        Box2f b = ob.Box;
                        if (Screen.Viewport.Box.BoxIntersect_EasyOut_Inclusive(b))
                        {
                            Cell c = Level.Grid.GetCellForPoint(b.Center());

                            vec4 color = new vec4(1, 1, 1, 1);
                            if (c != null)
                            {
                                color = c.LightColor;
                            }


                            Guy g = ob as Guy;
                            if (g != null)
                            {
                                if (g.HurtTime > 0)
                                {
                                    float t = (g.HurtTime / g.HurtTimeMax);

                                    color = color * (1 - t) + new vec4(1, 0, 0, 1) * t;//Lerp
                                }

                            }
                            if (ob.VaporTrail > 0)
                            {
                                for (int iv = ob.VaporTrail - 1; iv >= 0; --iv)
                                {
                                    if (ob.LastPos.Count - iv - 1 >= 0)
                                    {
                                        vec2 p = ob.LastPos[ob.LastPos.Count - iv - 1];
                                        float r = ob.LastRot[ob.LastRot.Count - iv - 1];
                                        vec2 s = ob.LastScale[ob.LastScale.Count - iv - 1];

                                        float a = ob.Alpha * (1.0f - (float)(iv + 1) / (float)(ob.VaporTrail + 1));

                                        Screen.DrawFrame(sb, ob.Frame,
                                        p,
                                        Res.Tiles.GetWHVec(),
                                        (ob.Color * color * a).toXNAColor(),
                                        s,
                                        r,
                                        ob.Origin,
                                        ob.SpriteEffects
                                        );
                                    }

                                }
                            }

                            Player obPlayer = ob as Player;

                            //Draw sword/shield behind guy if they are put away (reverwe order sto look like sword is in front)

                            //if (obPlayer != null && obPlayer.ShieldEnabled && obPlayer.ShieldOut == false)
                            //{
                            //    DrawShield(obPlayer, sb, color);
                            //}
                            //if (obPlayer != null && obPlayer.SwordEnabled && obPlayer.SwordOut == false)
                            //{
                            //    DrawSword(obPlayer, sb, color);
                            //}

                            DrawObject(sb, ob, color);

                            //if (obPlayer != null && obPlayer.BowOut == true)
                            //{
                            //    if (obPlayer != null && obPlayer.BowEnabled && obPlayer.BowOut == true)
                            //    {
                            //        DrawBow(obPlayer, sb, color);
                            //    }
                            //}
                            //else
                            //{
                            //    if (obPlayer != null && obPlayer.SwordEnabled && obPlayer.SwordOut == true)
                            //    {
                            //        DrawSword(obPlayer, sb, color);
                            //    }
                            //    if (obPlayer != null && obPlayer.ShieldEnabled && obPlayer.ShieldOut == true)
                            //    {
                            //        DrawShield(obPlayer, sb, color);
                            //    }
                            //}


                            DrawObjDebug(ob, sb);
                        }
                    }
                }
            }


            if (ShieldObject != null)
            {
                DrawObjDebug(ShieldObject, sb);
            }

        }
        public void DrawObject(SpriteBatch sb, GameObject ob, vec4 color)
        {
            //Draws object with rotation and correct offset.
            Screen.DrawFrame(sb, ob.Frame,
                ob.WorldPos(),
                Res.Tiles.GetWHVec(),
                (ob.Color * color * ob.Alpha).toXNAColor(),
                ob.Scale,
                ob.Rotation,
                ob.Origin + ob.ShakeOffset,
                ob.SpriteEffects
                );
        }
        public void DrawWorldDebug(SpriteBatch sb)
        {
            if (ShowDebug == false)
            {
                return;
            }

            foreach (vec2 vcp in DebugContactPointsToDraw)
            {
                Screen.DrawPointWorld(sb, vcp, 1.0f, Color.Yellow);
            }

            Screen.DrawBoxOutlineWorld(sb, Level.Grid.RootNode.Box, Color.Black);

            if (bShowMenu)
            {
                Screen.DrawBoxOutlineScreen(sb, MenuTab0Box, Color.Red);
                Screen.DrawBoxOutlineScreen(sb, MenuTab1Box, Color.Red);
                Screen.DrawBoxOutlineScreen(sb, MenuTab2Box, Color.Red);
                Screen.DrawBoxOutlineScreen(sb, MenuMapBox, Color.Red);

                if (menutab == MenuTab.Options)
                {
                    foreach (UiItem ui in OptionWindowItems)
                    {
                        Screen.DrawBoxOutlineScreen(sb, ui.Box, Color.Red);

                    }
                }

            }
        }
        private void DrawObjDebug(GameObject ob_obj, SpriteBatch sb)
        {
            if (ShowDebug == false)
            {
                return;
            }

            Screen.DrawPointWorld(sb, ob_obj.Pos, 1.0f, Color.Green);
            Screen.DrawPointWorld(sb, ob_obj.Pos + ob_obj.Origin, 1.0f, Color.Red);

            if (ob_obj as Guy != null)
            {
                if (ob_obj as Player != null)
                {
                    vec2 hp = GetSwordHitPoint(ob_obj as Player);

                    Screen.DrawPointWorld(sb, GetMovableItemOrigin(ob_obj as Player) + (ob_obj as Player).SwordClickNormal * 4, 1.0f, Color.White);
                    Screen.DrawPointWorld(sb, GetMovableItemOrigin(ob_obj as Player) + (ob_obj as Player).ShieldClickNormal * 4, 1.0f, Color.Gray);
                    Screen.DrawPointWorld(sb, hp, 1.0f, Color.Orange);

                    Screen.DrawBoxOutlineWorld(sb, GetSwordHitBox(ob_obj as Player), Color.BlueViolet);


                }

                Screen.DrawPointWorld(sb, (ob_obj as Guy).cposC, 1.0f, Color.Yellow);
                Screen.DrawPointWorld(sb, (ob_obj as Guy).cposB, 1.0f, Color.Yellow);
                Screen.DrawPointWorld(sb, (ob_obj as Guy).cposT, 1.0f, Color.Yellow);
                Screen.DrawPointWorld(sb, (ob_obj as Guy).cposL, 1.0f, Color.Yellow);
                Screen.DrawPointWorld(sb, (ob_obj as Guy).cposR, 1.0f, Color.Yellow);
            }

            Screen.DrawBoxOutlineWorld(sb, ob_obj.Box, Color.AliceBlue);

            if (ob_obj.PhysicsShape == PhysicsShape.Ball)
            {
                Screen.DrawBoxOutlineWorld(sb,
                    new Box2f(ob_obj.WorldPos() - ob_obj.PhysicsBallRadiusPixels, ob_obj.WorldPos() + ob_obj.PhysicsBallRadiusPixels),
                    Color.Orange);
            }

            Cell[] sur = Level.Grid.GetSurroundingCells(Level.Grid.GetCellForPoint(ob_obj.Box.Center()), true);
            foreach (Cell cell in sur)
            {
                if (cell != null)
                {
                    Screen.DrawBoxOutlineWorld(sb, cell.Box(), Color.Red);
                }
            }

            Cell c = Level.Grid.GetCellForPoint(ob_obj.Box.Center());
            if (c != null)
            {
                Screen.DrawBoxOutlineWorld(sb, c.Box(), Color.LightGreen);
            }

        }
        private const float PI = 3.1415927f;
        private vec2 GetSwordHitPoint(Player guy)
        {
            float angle = 0;
            vec2 origin = new vec2(0, 0);
            origin = GetMovableItemOrigin(guy);
            angle = GetWeaponAngle(guy);

            if (guy.SpriteEffects == SpriteEffects.FlipHorizontally)
            {
                angle = angle - PI * 0.5f;
            }
            else
            {
                angle = angle - PI * 0.5f;
            }


            float pickaxelength = Res.Tiles.TileHeightPixels * 0.8f;
            // vec2 v = new vec2(FastCos(angle), FastSin(angle)) * pickaxelength;
            vec2 v = new vec2((float)Math.Cos(angle), (float)Math.Sin(angle)) * pickaxelength;

            v += origin;
            return v;
        }

        private float GetWeaponAngle(Player guy, bool bIsAimItem = false)
        {
            //Gets angle for SWORD OR An aimable item, such as bow 

            //0
            // ----  0 = up = base angle
            //PI
            float angle = 0;

            vec2 n = guy.SwordClickNormal;
            if (bIsAimItem)
            {
                n = GetAimNormal(guy, GetMovableItemOrigin(guy));

            }

            if (guy.ControlSwordDelay <= 0.001f || bIsAimItem)
            {
                //Otherwise - player is controlling sword (bow, etc) with mouse, set the angle to be exactly
                //the angle from origin to mouse point
                angle = (float)Math.Asin(n.y) + (float)Math.PI * 0.5f;
            }
            else
            {
                //Sword  Only
                //For any swipe - we use the sword angle as the MIDPOINT of the SWIPE ARC
                float fBase = (float)Math.Acos(n.Dot(new vec2(0, -1)));
                fBase -= (float)Math.PI * 0.5f;

                if (guy.SwordSwingDir == 0)//UP / Down
                {
                    angle = (1.0f - (guy.SwordSwingDelay / guy.SwordSwingDelayMax)) * ((float)guy.SwordSwingMaxAngleRadians);
                }
                else
                {
                    angle = (guy.SwordSwingDelay / guy.SwordSwingDelayMax) * ((float)guy.SwordSwingMaxAngleRadians);
                }

                angle += fBase;
            }

            if (guy.SpriteEffects == SpriteEffects.FlipHorizontally)
            {
                angle = -angle;
            }

            return angle;
        }


        private vec2 GetMovableItemOrigin(Guy guy, bool otherHand = false)
        {
            vec2 origin = new vec2(0, 0);

            if (guy.Crouching)
            {
                if (guy.SpriteEffects == SpriteEffects.FlipHorizontally)
                {
                    if (otherHand)
                    {
                        origin = new vec2(3, 2);
                    }
                    else
                    {
                        origin = new vec2(-7, 1);
                    }
                }
                else
                {
                    if (otherHand)
                    {
                        origin = new vec2(-3, 2);
                    }
                    else
                    {
                        origin = new vec2(7, 1);
                    }
                }
            }
            else
            {
                if (guy.SpriteEffects == SpriteEffects.FlipHorizontally)
                {
                    if (otherHand)
                    {
                        origin = new vec2(3, 2);
                    }
                    else
                    {
                        origin = new vec2(-5, 1);
                    }
                }
                else
                {
                    if (otherHand)
                    {
                        origin = new vec2(-3, 2);
                    }
                    else
                    {
                        origin = new vec2(5, 1);
                    }
                }
            }

            // origin = origin + guy.Pos - guy.Box.Center();
            origin = mat2.GetRot(guy.Rotation) * origin;
            origin = guy.Box.Center() + origin;
            return origin;
        }
        //private Frame GetBowArrowFrame(Player player)
        //{
        //    int iFrame = 0;
        //    if (player.SwordModifier == SwordModifier.Tar)
        //    {
        //        iFrame = 1;
        //    }

        //    return Res.Tiles.GetSpriteFrame(Res.SprArrow, iFrame);
        //}
        //private Frame GetSwordFrame(Player player)
        //{
        //    int iFrame = 0;
        //    if (player.SwordModifier == SwordModifier.Tar)
        //    {
        //        iFrame = 1;
        //    }
        //    if (player.SwordModifier == SwordModifier.Water)
        //    {
        //        iFrame = 2;
        //    }
        //    if (player.SwordModifier == SwordModifier.Lava)
        //    {
        //        iFrame = 3;
        //    }
        //    if (player.SwordModifier == SwordModifier.Obsidian)
        //    {
        //        iFrame = 4;
        //    }

        //    return Res.Tiles.GetSpriteFrame(Res.SprSword, iFrame);
        //}
        //private void DrawSword(Player player, SpriteBatch sb, vec4 color)
        //{
        //    if (player.SwordEnabled)
        //    {
        //        Frame SFrame = GetSwordFrame(player);
        //        if (player.SwordOut)
        //        {
        //            Frame PSFrame = Res.Tiles.GetSpriteFrame(Res.SprPowerSword, 0);
        //            Frame HandFrame = Res.Tiles.GetSpriteFrame(Res.SprGuyHand, 0);

        //            float angle = GetWeaponAngle(player);
        //            vec2 origin = GetMovableItemOrigin(player);

        //            float blend = 0;
        //            if (player.PowerSwordEnabled)
        //            {
        //                blend = player.PowerSwordChargePulse;
        //                Screen.DrawFrame(sb, PSFrame, origin, Res.Tiles.GetWHVec(), Color.White * (blend), new vec2(1, 1), angle, new vec2(16 / 2, 15), player.SpriteEffects);
        //            }

        //            Screen.DrawFrame(sb, SFrame, origin, Res.Tiles.GetWHVec(), (color * (1 - blend)).toXNAColor(), new vec2(1, 1), angle, new vec2(16 / 2, 15), player.SpriteEffects);

        //            //Draw hand in front of sword
        //            Screen.DrawFrame(sb, HandFrame, origin, Res.Tiles.GetWHVec(), Color.White, new vec2(1, 1), angle, Res.Tiles.GetWHVec() * 0.5f, player.SpriteEffects);

        //            if (player.ShieldOut == false)
        //            {
        //                DrawOtherHand(sb, player, HandFrame);
        //            }
        //        }
        //        else
        //        {
        //            DrawPutAwaySwordOrShield(sb, color, player, SFrame, false);
        //        }
        //    }
        //}
        //private void DrawBow(Player player, SpriteBatch sb, vec4 color)
        //{
        //    if (player.BowEnabled)
        //    {
        //        Frame BowFrame = Res.Tiles.GetSpriteFrame(Res.SprBow, 0);
        //        Frame ArrowFrame = GetBowArrowFrame(player);
        //        if (player.BowOut)
        //        {
        //            Frame HandFrame = Res.Tiles.GetSpriteFrame(Res.SprGuyHand, 0);

        //            float angle = GetWeaponAngle(player, true);

        //            vec2 origin = GetBowOffset(player);
        //            vec2 aimNormal = GetAimNormal(player, origin);
        //            vec2 arrowOff = GetArrowOffsetFromBow(player, aimNormal);

        //            Screen.DrawFrame(sb, BowFrame, origin, Res.Tiles.GetWHVec(), (color).toXNAColor(), new vec2(1, 1), angle, new vec2(16 / 2, 15), player.SpriteEffects);

        //            //Draw Arrow + Arrow Hand
        //            if (player.NumArrows > 0)
        //            {
        //                Screen.DrawFrame(sb, ArrowFrame, origin + arrowOff, Res.Tiles.GetWHVec(), (color).toXNAColor(), new vec2(1, 1), angle, new vec2(16 / 2, 15), player.SpriteEffects);
        //            }

        //            //Bow Hand
        //            vec2 vHoldHandOrigin = aimNormal * 7;
        //            Screen.DrawFrame(sb, HandFrame, origin + vHoldHandOrigin, Res.Tiles.GetWHVec(), Color.White, new vec2(1, 1), angle, Res.Tiles.GetWHVec() * 0.5f, player.SpriteEffects);

        //            //Draw Bow String
        //            vec2 p = aimNormal.Perp();
        //            Screen.DrawLineWorld(sb, origin + arrowOff, origin + (p * 5), 3, new vec4(1, 1, 0.79f, 1).toXNAColor());
        //            Screen.DrawLineWorld(sb, origin + arrowOff, origin - (p * 5), 3, new vec4(1, 1, 0.79f, 1).toXNAColor());

        //            //String Hand
        //            Screen.DrawFrame(sb, HandFrame, origin + arrowOff, Res.Tiles.GetWHVec(), Color.White, new vec2(1, 1), angle, Res.Tiles.GetWHVec() * 0.5f, player.SpriteEffects);
        //        }
        //    }
        //}
        //public vec2 GetBowOffset(Player player)
        //{
        //    vec2 originBase = GetMovableItemOrigin(player);
        //    vec2 origin = new vec2(0, 0);
        //    if (player.IsFacingRight())
        //    {
        //        origin = originBase - new vec2(5, 0);//makeit look more like the player is h oldin bot
        //    }
        //    else
        //    {
        //        origin = originBase + new vec2(5, 0);//makeit look more like the player is h oldin bot
        //    }
        //    return origin;
        //}
        //private vec2 GetArrowOffsetFromBow(Player player, vec2 aimNormal)
        //{
        //    vec2 arrowOff = (aimNormal * -2) + (aimNormal * 8 * player.BowDrawTime / player.BowDrawTimeMax);
        //    return arrowOff;
        //}
        //private void DrawPutAwaySwordOrShield(SpriteBatch sb, vec4 color, Guy guy, Frame SFrame, bool shield)
        //{
        //    float r = 0;
        //    SpriteEffects se = SpriteEffects.None;
        //    vec2 o = guy.Box.Center();
        //    vec2 od = new vec2(0, 0);
        //    //Draw the sword behind the guy.
        //    if (guy.IsFacingRight())
        //    {
        //        if (shield)
        //        {
        //            r = 0;
        //        }
        //        else
        //        {
        //            r = (float)Math.PI * 0.75f;
        //        }
        //        r += guy.Rotation;
        //        se = SpriteEffects.FlipHorizontally;
        //        od += new vec2(-2, 0);
        //    }
        //    else
        //    {
        //        if (shield)
        //        {
        //            r = 0;
        //        }
        //        else
        //        {
        //            r = (float)Math.PI * 1.25f;
        //        }
        //        r += guy.Rotation;
        //        od += new vec2(2, 0);
        //    }

        //    Screen.DrawFrame(sb, SFrame, o + od, Res.Tiles.GetWHVec(), color.toXNAColor(), new vec2(1, 1), r, new vec2(16 / 2, 16 / 2), guy.SpriteEffects);
        //}
        //private void DrawOtherHand(SpriteBatch sb, Guy guy, Frame HandFrame)
        //{
        //    vec2 mo = GetMovableItemOrigin(guy, true);
        //    Screen.DrawFrame(sb, HandFrame, mo, Res.Tiles.GetWHVec(), Color.White, new vec2(1, 1), 0, Res.Tiles.GetWHVec() * 0.5f, guy.SpriteEffects);
        //}
        //private void DrawShield(Player player, SpriteBatch sb, vec4 color)
        //{
        //    if (player.ShieldEnabled)
        //    {

        //        GameObject ob = ShieldObject;
        //        if (ob != null)
        //        {
        //            if (player.ShieldOut)
        //            {

        //                //Draw hand behind shield
        //                Frame HandFrame = Res.Tiles.GetSpriteFrame(Res.SprGuyHand, 0);
        //                Screen.DrawFrame(sb, HandFrame,
        //                    player.Box.Center() + player.ShieldClickNormal * 3.6f,
        //                    Res.Tiles.GetWHVec(), Color.White, new vec2(1, 1), 0, Res.Tiles.GetWHVec() * 0.5f, player.SpriteEffects);


        //                Screen.DrawFrame(sb, ob.Frame,
        //                    ob.WorldPos(),
        //                    Res.Tiles.GetWHVec(),
        //                    (ob.Color * color * ob.Alpha).toXNAColor(),
        //                    ob.Scale,
        //                    ob.Rotation,
        //                    ob.Origin + ob.ShakeOffset,
        //                    ob.SpriteEffects
        //                    );

        //                if (player.SwordOut == false)
        //                {
        //                    DrawOtherHand(sb, player, HandFrame);
        //                }
        //            }
        //            else
        //            {
        //                DrawPutAwaySwordOrShield(sb, color, player, ob.Frame, true);
        //            }

        //        }
        //    }
        //}
        private void DrawForeground(SpriteBatch sb)
        {
            //  float BaseLandY = GetBaseLandYPixels();
            foreach (Cell c in ViewportCellsFrame)
            {
                TileBlock block = c.Layers[PlatformLevel.Foreground];
                if (block == null || block.Tile == null)
                {
                    continue;
                }
                DrawBlock(sb, c, block);

            }
        }
        public float GetDist(Guy p)
        {
            float dist = 0;
            if (jumpStartPos >= 0)
            {
                dist = (float)(p.Pos.x - jumpStartPos) / (float)Res.Tiles.TileWidthPixels;
            }
            return dist;
        }
        public void DrawUI(SpriteBatch sb)
        {

            if (ShowDebug)
            {
                Screen.DrawText_Fit_H(sb, Res.Font, Fps.ToString("F1"), 12, Screen.Viewport.Pos + new vec2(3, 3), new vec4(0, 0, 0, 1), 1, new vec4(1, 1, 1, 1));
            }


            Player player = GetPlayer();

            if (StartRunning == false)
            {
                Screen.DrawText_Fit_H(sb, Res.Font, "Rocket Jump", Screen.Viewport.WidthPixels * 0.75f,
    Screen.Viewport.Pos + new vec2(Screen.Viewport.WidthPixels * 0.25f * 0.5f, Screen.Viewport.HeightPixels * 0.2f),
    new vec4(1, 0.2f, 0.2f, 1), 1, new vec4(0.00f, 0.4f, 1.00f, 1.00f));
            }

            //Draw Mine Count + Mines
            if (GameState == GameState.Play)
            {

                if (bShowMenu)
                {
                }
                else if (bShowUI)
                {
                    float alpha = 0.6f;

                    float uibottom = Screen.Viewport.HeightPixels - 16;

                    Player p = GetPlayer();

                    Screen.DrawText_Fit_H(sb, Res.Font, "Distance", 17, Screen.Viewport.Pos + new vec2(3, 3), new vec4(0, 0, 0, 1), 1, new vec4(1, 1, 1, 1));
                    Screen.DrawText_Fit_H(sb, Res.Font, " " + GetDist(GetPlayer()).ToString("F1") + "m", 12, Screen.Viewport.Pos + new vec2(3, 10), new vec4(0, 0, 0, 1), 1, new vec4(1, 1, 1, 1));


                    DrawUIInventory(sb, Res.SprMarbleUI, Screen.Viewport.WidthPixels - 24, 1, player.Money, player.MaxMoney, alpha, new vec2(6, 6), false, true, 0);

                }
            }
            else if (GameState == GameState.PlayerDeath_ShowContinue)
            {
                Screen.DrawText_Fit_H(sb, Res.Font, "Distance:" + " " + GetDist(GetPlayer()).ToString("F1") + "m", Screen.Viewport.WidthPixels * 0.75f,
                    Screen.Viewport.Pos + new vec2(Screen.Viewport.WidthPixels * 0.25f * 0.5f, Screen.Viewport.HeightPixels * 0.2f),
                    new vec4(0, 0.00f, 0, 1), 1, new vec4(1.00f, 1.00f, 1.00f, 1.00f));

                float hs = 0;
                try
                {
                    hs = (this.Screen.Game as MainGame).GetHighScore();
                }
                catch (Exception ex)
                {
                }
                if (hs > 0)
                {
                    Screen.DrawText_Fit_H(sb, Res.Font, "High Score:" + hs.ToString("F1"), Screen.Viewport.WidthPixels * 0.32f,
                        Screen.Viewport.Pos + new vec2(Screen.Viewport.WidthPixels * 0.68f * 0.5f, Screen.Viewport.HeightPixels * 0.5f),
                        new vec4(0, 0.00f, 0, 1), 1, new vec4(1.00f, 1.00f, 1.00f, 1.00f));
                }


                Screen.DrawText_Fit_H(sb, Res.Font, "Press Spacebar To Continue...", Screen.Viewport.WidthPixels * 0.32f,
                    Screen.Viewport.Pos + new vec2(Screen.Viewport.WidthPixels * 0.68f * 0.5f, Screen.Viewport.HeightPixels * 0.7f),
                    new vec4(1, 0.99f, 0, 1), 1, new vec4(0.50f, 0.50f, 0.50f, 1.00f));
            }

            DrawScreenOverlayText(sb);

            //CURSOR
            if (Screen.Game.GameSystem.GetPlatform() == Platform.Desktop)
            {

                if (bShowCursor)
                {
                    // Player player = GetPlayer();
                    string sprite = "";
                    if (HoverState == HoverState.Normal)
                    {
                        sprite = Res.SprCursorArrow;
                    }
                    else if (HoverState == HoverState.Aim)
                    {
                        sprite = Res.SprCursorCrosshair;
                    }
                    else if (HoverState == HoverState.Interact)
                    {
                        sprite = Res.SprCursorQuestion;
                    }
                    else if (HoverState == HoverState.Attack)
                    {
                        sprite = Res.SprCursorSword;
                    }
                    else if (HoverState == HoverState.Talk)
                    {
                        sprite = Res.SprCursorTalk;
                    }

                    if (string.IsNullOrEmpty(sprite) == false)
                    {
                        sb.Draw(Res.Tiles.Texture, new Rectangle(Mouse.GetState().Position, new Point(32, 32)),
                            Res.Tiles.GetSprite(sprite).Frames[0].R,
                            Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.0f);
                    }
                }
            }
        }

        private class UiItem
        {
            public bool Hover = false;
            public bool PlayClickSoundOnHover = false;
            public TouchState TouchState = TouchState.Up;
            public Box2f Box;
            public UiItem(Box2f box) { Box = box; }
            public Action OnRelease = null;
        }
        private class CheckOption : UiItem
        {
            public bool Checked = false;
            public CheckOption(bool check, Box2f b) : base(b) { Checked = check; }
        }
        private class TextButton : UiItem
        {
            public string Text = "";
            public TextButton(string str, Box2f b) : base(b) { Text = str; }
        }
        List<UiItem> OptionWindowItems = null;

        private void DrawOptionTab(SpriteBatch sb, Player player, bool selected)
        {
            DrawMenuBackground(sb, Res.SprMenuUIOption, selected ? 1 : 0.5f);

            if (selected == true)
            {
                //Screen.DrawUIFrame(sb, Res.SprCheckboxUI, 0, 
                //    MenuMapBox.Min + new vec2(2, 2), 
                //    new vec2(12, 12), new vec4(1, 1, 1, 1));

                if (OptionWindowItems == null)
                {
                    OptionWindowItems = new List<UiItem>();
                    OptionWindowItems.Add(
                        new TextButton("Exit To Desktop", new Box2f(MenuMapBox.Min.x + 2, MenuMapBox.Min.y + 20, 70, 12))
                        {
                            OnRelease = () => { this.Screen.Game.Exit(); }
                            ,
                            PlayClickSoundOnHover = true
                        }
                    );
                }

                foreach (UiItem item in OptionWindowItems)
                {
                    if (item is TextButton)
                    {
                        float c = 0.4f;
                        if (item.TouchState == TouchState.Press || item.TouchState == TouchState.Down)
                        {
                            c = 0.2f;
                        }
                        else if (item.Hover)
                        {
                            c = 1.0f;
                        }
                        Screen.DrawText_Fit(sb, Res.Font, (item as TextButton).Text, item.Box.Width(), item.Box.Height(),
                            Screen.Viewport.Pos + item.Box.Min, new vec4(c, c, c, 1), 1, new vec4(1, 1, 1, 1), false);
                    }
                }

            }


        }
        private void DrawMapTab(SpriteBatch sb, Player player, bool selected)
        {
            DrawMenuBackground(sb, Res.SprMenuUIMap, selected ? 1 : 0.5f);

            if (selected)
            {

                /*
                 * we have: MapBox (screen)
                 * start from the map origin - center of map box for now
                 * when we ENTER a room 
                 *  if player not in room before
                 *      save the map squaers (the border red squares)
                 *          save the actual positions as world grid integers
                 *  
                 * when we EXIT a room
                 *  save the map squares (border squares)
                 *  
                 *  Draw the map in squares. from the red squares surrounding each room
                 *  if a square is outside the map box don't draw
                 *  if a square is cut off by the map box - don't draw, for simplicity
                 *  
                 *  
                 *  start with current room
                 *      List<Box2f> ScanMap() - to get border squares
                 *  foreach door
                 *      if door.entered
                 *          scanmap(door
                 *  
                 * */
            }
        }
        private void DrawMenuBackground(SpriteBatch sb, string MenuBackSprite, float c = 1)
        {
            float bor_px_x = 10;
            float bor_px_y = 10;
            Screen.DrawUIFrame(sb, MenuBackSprite, 0,
                new vec2(bor_px_x, bor_px_y), new vec2(Screen.Viewport.WidthPixels - bor_px_x * 2, Screen.Viewport.HeightPixels - bor_px_y * 2),
                new vec4(1 * c, 1 * c, 1 * c, 1));

        }

        private string ScreenOverlayText = "";
        private float ScreenOverlayTextFade = 0;
        private vec4 ScreenOverlayTextColor = new vec4(1, 0, 0, 1);
        private vec4 ScreenOverlayTextOutline = new vec4(1, 1, 1, 1);

        private void DrawScreenOverlayText(SpriteBatch sb)
        {
            if (ScreenOverlayText != "")
            {
                SpriteFont overlayfont = Res.Font;

                float alpha = 1 - (ScreenOverlayTextFade);
                vec4 color = ScreenOverlayTextColor * alpha;
                vec4 outline = ScreenOverlayTextOutline * alpha;
                float pad = 0.2f;
                float width_pixels = Screen.Viewport.WidthPixels - Screen.Viewport.WidthPixels * pad * 2.0f;
                string proto = "Perfect!";
                float sy = Screen.DrawText_Fit_H_OR_V_Scale(overlayfont, false, proto, width_pixels); //**Note, we're using a constant string to keep the size consistent.
                float font_height_px = Screen.GetFontSizePixels(overlayfont, proto, sy).y;

                Screen.DrawText_Fit_H(sb, overlayfont, ScreenOverlayText,
                    width_pixels,
                    Screen.Viewport.Pos + new vec2(Screen.Viewport.WidthPixels * pad, Screen.Viewport.HeightPixels * 0.5f - font_height_px * 0.5f),
                    color, 1, outline, proto);
            }
        }

        private void DrawUIInventory(SpriteBatch sb, string sprite, float x, float y, int numItem, int maxItem, float ui_alpha, vec2 spritewh, bool drawSelectionBox, bool drawQuantity, int xOffset)
        {
            Screen.DrawUIFrame(sb, sprite, 0, new vec2(x + xOffset, y), spritewh, new vec4(1, 1, 1, ui_alpha));

            string text = "";
            if (drawQuantity)
            {
                text = "x" + numItem;
            }

            vec4 color = new vec4(0, 0, 0, ui_alpha);
            vec4 outline = new vec4(1, 1, 1, ui_alpha);
            if (numItem == maxItem)
            {
                color = new vec4(0.018f, 0.311f, 0.0814f, ui_alpha);
                outline = new vec4(0.718f, 1.0f, 0.7114f, ui_alpha);
            }

            //Screen.DrawText_Fit_H(sb, Res.Font, text, 8,
            //    Screen.Viewport.Pos + new vec2(x + 12,
            //    Screen.Viewport.HeightPixels - 10), color, 1, outline);
            Screen.DrawText_Fit_V(sb, Res.Font, text, 7,
                Screen.Viewport.Pos + new vec2(x + spritewh.x, y + spritewh.y - spritewh.y / 2), color, 1, outline);

            if (drawSelectionBox)
            {
                Screen.DrawUIFrame(sb, Res.SprSelectedItemUI, 0, new vec2(x - 1, y - 1), spritewh + new vec2(12, 2), new vec4(1, 1, 1, ui_alpha));
            }

        }
    }
    public class GameScreen : Screen
    {
        PortalTransition PortalTransition = null;

        World World;
        // public Portal TransitionPortal = null;
        public Door TransitionDoor = null;
        GameState LastGameState;
        RenderTarget2D ScreenTarget;
        public bool Reset { get; set; } = false;

        private void StartNewWorld()
        {
            World = null;
            GC.Collect();
            World = new World(this);
        }
        public override void Init(GameBase game)
        {
            base.Init(game);
            StartNewWorld();
            CreateTarget();
        }
        private void GoToNextLevel()
        {
            if (TransitionDoor != null)
            {
                //Save that the door was opened, and save the state of the game.
                TransitionDoor.Entered = true;
                World.Level.SerializeLevel();

                //**Save previous world imgae
                Res.Audio.PlaySound(Res.SfxExitStairs);
                PortalTransition = new PortalTransition(this.World, TransitionDoor.PortalTransitionEffect);
                PortalTransition.RenderPrevWorld(World);
                LastGameState = World.GameState;
                World.GameState = GameState.LevelTransition;

                //Load the actual next area.

                World.bLoadingRoom = true;
                {
                    World.Level.GoThroughDoor(TransitionDoor);
                    Guy g = World.GetPlayer();
                    //World.WarpPlayer(g.Pos);
                    World.Update(0.0f);
                }
                World.bLoadingRoom = false;

                //**Render the next world image
                PortalTransition.RenderNextWorld(World);
                TransitionDoor = null;
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (Reset)
            {
                Reset = false;
                StartNewWorld();
            }

            if (PortalTransition != null)
            {
                //Update the player's sprite so we can animate.
                Guy player = World.GetPlayer();
                player.Update(Game.Input, dt);


                if (PortalTransition.Update(dt))
                {
                    PortalTransition = null;
                    World.GameState = LastGameState;//Restore game state
                }
            }
            else
            {
                //This can be modded
                GoToNextLevel();
                World.Update(dt);
            }

        }
        private void CreateTarget()
        {
            ScreenTarget = new RenderTarget2D(
                Game.GraphicsDevice,
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }
        public override void Draw()
        {
            if (PortalTransition != null)
            {
                PortalTransition.Draw(this);
            }
            else
            {
                DrawGameWorldWithEffects();

            }

        }
        public void DrawGameWorldWithEffects()
        {
            //Draw to RT
            Game.GraphicsDevice.SetRenderTarget(ScreenTarget);
            Game.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            Game.GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            {
                World.DrawGameWorld(World.Screen.SpriteBatch);
            }
            World.Screen.SpriteBatch.End();
            World.Screen.Game.GraphicsDevice.SetRenderTarget(null);

            //Do lava / water processing
            float scale = 0.0f;
            if (World.LavaScreenDistort > 0.0f)
            {
                scale = World.LavaScreenDistort;
            }

            base.BeginDraw();
            {
                Vector2 o = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) * 0.5f;

                //Draw target
                SpriteBatch.Draw(ScreenTarget,
                    new Rectangle((int)o.X, (int)o.Y, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    new Rectangle(0, 0, ScreenTarget.Width, ScreenTarget.Height),
                   World.ScreenOverlayColor.toXNAColor(), 3.1415f * scale, o, SpriteEffects.None, 0.0f);

                World.DrawUI(SpriteBatch);

                World.DrawWorldDebug(SpriteBatch);
                //  base.DrawMenu();//Must come after world.draw

                World.Dialog.Draw(SpriteBatch);
            }
            base.EndDraw();
        }
    }
    public class MainGame : GameBase
    {
        public GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        GameScreen GameScreen;
        Screen _objCurScreen = null;
        GameData GameData;
        const int c_ScreenResolutionWidthPixels = 800;
        const int c_ScreenResolutionHeightPixels = 600;

        public float GetHighScore()
        {
            GameData.Load();
            return GameData.HighScore;
        }
        public void SetHighScore(float f)
        {
            GameData.HighScore = f;
            GameData.Save();
        }

        public void SetGraphicsOptions(bool? fullscreen, DisplayOrientation? orientation)
        {
            if (GraphicsDevice != null)
            {

                GraphicsDevice.Clear(Color.Black);
            }
            if (fullscreen != null)
            {
                GraphicsDeviceManager.IsFullScreen = fullscreen.Value;
                if (fullscreen.Value)
                {
                    GraphicsDeviceManager.PreferredBackBufferWidth = c_ScreenResolutionWidthPixels;
                    float ar = (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    //update aspect ratio to fit your screen.
                    GraphicsDeviceManager.PreferredBackBufferHeight = (int)(Math.Round((float)c_ScreenResolutionWidthPixels * ar));
                }
                else
                {
                    GraphicsDeviceManager.PreferredBackBufferWidth = c_ScreenResolutionWidthPixels;
                    GraphicsDeviceManager.PreferredBackBufferHeight = c_ScreenResolutionHeightPixels;
                }
            }
            if (orientation != null)
            {
                GraphicsDeviceManager.SupportedOrientations = orientation.Value; ;

            }
            GraphicsDeviceManager.ApplyChanges();
        }

        public void Init(AdMan adMan, bool bFullscreen, GameSystem gs)
        {
            GameSystem = gs;

            AdMan = adMan;

            GameData = new GameData(this);
            GameData.Load();

            GraphicsDeviceManager.IsFullScreen = bFullscreen;

            SetGraphicsOptions(false, DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight);

            Window.Title = "Rocket Jump";
        }
        bool DeviceResetting = false;

        public MainGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.DeviceResetting += (a, x) =>
            {
                DeviceResetting = true;
            };
            GraphicsDeviceManager.DeviceReset += (a, x) =>
            {
                DeviceResetting = false;
            };

            Content.RootDirectory = "Content";

            //**on Android, fullscreen just hides the menu bar.
            //On Desktop - it's, well fullscreen..

            this.IsMouseVisible = false;//Dnr
                                        //We can't have constructors because of XAML
                                        //Fuxking XAML

            ///Variable time setp.
            /////So with fixed stepping, XNA will call Upate() multiple times to keep up.
            //Setting this to false, makes it variable, and XNA executes Update/Draw in succession
            this.IsFixedTimeStep = true;

        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Res.Load(Content, this.GraphicsDevice);

            //Do not do any usage of GameSystem ehre

            ShowScreen = ShowScreen.Game;
        }
        protected override void UnloadContent()
        {
        }
        //float waitmax = 2.0f;
        //float wait = 0.0f;
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(dt);

            if (ShowScreen == ShowScreen.Game)
            {

                GameScreen = new GameScreen();
                GameScreen.Init(this);
                _objCurScreen = GameScreen;
            }

            ShowScreen = ShowScreen.None;

            if (_objCurScreen != null)
            {
                _objCurScreen.Update(dt);
            }
            //If we touch screen, then hide the nav if it isn't hidden.
            if (Input.Global.TouchState == TouchState.Press || Input.Global.TouchState == TouchState.Release)
            {
                GameSystem.HideNav();

            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (DeviceResetting)
            {
            }
            else
            {
                if (_objCurScreen != null)
                {
                    _objCurScreen.Draw();
                }
            }

            base.Draw(gameTime);
        }
    }
}
