using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class ButtonBase
    {
        public bool PressOrDown() { return TouchState == TouchState.Press || TouchState == TouchState.Down; }
        public bool Press() { return TouchState == TouchState.Press; }
        public bool Up() { return TouchState == TouchState.Up; }
        public bool Down() { return TouchState == TouchState.Down; }
        public bool Release() { return TouchState == TouchState.Release; }
        public bool ReleaseOrUp() { return TouchState == TouchState.Release || TouchState==TouchState.Up; }
        public TouchState TouchState { get; private set; } = TouchState.Up;
        public void Update(bool touched)
        {
            if (touched)
            {
                if (TouchState == TouchState.Up)
                {
                    TouchState = TouchState.Press;
                }
                else if (TouchState == TouchState.Press)
                {
                    TouchState = TouchState.Down;
                }
                else if (TouchState == TouchState.Down)
                {
                    TouchState = TouchState.Down;
                }
                else if (TouchState == TouchState.Release)
                {
                    TouchState = TouchState.Press;
                }
            }
            else
            {
                if (TouchState == TouchState.Up)
                {
                    TouchState = TouchState.Up;
                }
                else if (TouchState == TouchState.Press)
                {
                    TouchState = TouchState.Release;
                }
                else if (TouchState == TouchState.Down)
                {
                    TouchState = TouchState.Release;
                }
                else if (TouchState == TouchState.Release)
                {
                    TouchState = TouchState.Up;
                }
            }
        }
    }
    

    public class Joystick
    {
        public ButtonBase Up { get; private set; } = new ButtonBase();
        public ButtonBase Down { get; private set; } = new ButtonBase();
        public ButtonBase Left { get; private set; } = new ButtonBase();
        public ButtonBase Right { get; private set; } = new ButtonBase();
        //2 Auxillary buttons, Jump / Ok
        public ButtonBase Jump { get; private set; } = new ButtonBase();
        public ButtonBase Ok { get; private set; } = new ButtonBase();
        public ButtonBase Menu { get; private set; } = new ButtonBase();
        public ButtonBase Action { get; private set; } = new ButtonBase();

        public bool AIUp = false;
        public bool AIDown = false;
        public bool AILeft = false;
        public bool AIRight = false;
        public bool AIJump = false;
        public bool AIOk = false;
        public bool AIAction = false;

        bool AButtonPressOrDown = false;
        Screen Screen;

        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;

        public MenuButton Joypad;
        public MenuButton Stick;
        public MenuButton AButton;

        //Action AIAction = null;//If false this is a "virtual" joystick used to control the AI
        bool IsUser = false;

        vec2 startTouch;
        vec2 joystickBasePos;

        bool StickHeld = false;
        public bool ShowControls()
        {
            return Screen.Game.GameSystem.GetPlatform() == Platform.Android || 
                Screen.Game.GameSystem.GetPlatform() == Platform.iOS;
        }
        public Joystick(Screen screen, bool isUser)
        {
            Screen = screen;

            Visible = IsUser = isUser;
            
            if (IsUser && ShowControls())
            {
                Screen.MenuButtons.Add(Joypad = new MenuButton(Screen,
                    new vec2(6, 2), new vec2(2, 2),
                    new vec2(6, 2), new vec2(2, 2),
                    new vec2(
                        0, Screen.Viewport.HeightPixels - Screen.Game.Res.Tiles.TileWidthPixels * 2.0f),
                    true, Screen.Game.Res.Tiles.TileWidthPixels * 2.0f, Screen.Game.Res.Tiles.TileWidthPixels * 2.0f, Color.White * 0.5f));

                joystickBasePos = new vec2(Screen.Game.Res.Tiles.TileWidthPixels * 0.5f, Screen.Viewport.HeightPixels - Screen.Game.Res.Tiles.TileWidthPixels * 1.5f);

                Screen.MenuButtons.Add(Stick = new MenuButton(Screen,
                    new vec2(8, 2), new vec2(1, 1),
                    new vec2(8, 2), new vec2(1, 1),
                    new vec2(
                        joystickBasePos.x, joystickBasePos.y),
                    true, Screen.Game.Res.Tiles.TileWidthPixels * 1.0f, Screen.Game.Res.Tiles.TileWidthPixels * 1.0f, Color.White * 0.5f));
                Stick.Press = (x) =>
                {
                    //Don't put any extra logic in here. put in stargame
                    //  World.Res.Audio.PlaySound(World.Res.SfxBlip);
                    // World.StartGame();
                    StickHeld = true;
                    startTouch = Screen.Game.Input.LastTouch;
                };
                Stick.Release = (x) =>
                {
                    StickHeld = false;
                };

                Screen.MenuButtons.Add(AButton = new MenuButton(Screen,
                new vec2(0, 10), new vec2(2, 2),
                new vec2(0, 12), new vec2(2, 2),
                new vec2(
                    Screen.Viewport.WidthPixels - Screen.Game.Res.Tiles.TileWidthPixels*2, 
                    Screen.Viewport.HeightPixels - Screen.Game.Res.Tiles.TileWidthPixels * 2.0f),
                true, Screen.Game.Res.Tiles.TileWidthPixels * 2.0f, Screen.Game.Res.Tiles.TileWidthPixels * 2.0f, Color.White * 0.5f));
                AButton.Press = (x) => { AButtonPressOrDown = true; };
                AButton.Down = (x) => { AButtonPressOrDown = true; };
                AButton.Release = (x) => { AButtonPressOrDown = false; };
            }

        }

        public void Update(float dt)
        {
            if (Enabled == false)
            {
                return;
            }
            
            bool bUp = false;
            bool bDown = false;
            bool bLeft = false;
            bool bRight = false;
            bool bJump = false;
            bool bOk = false;
            bool bMenu = false;
            bool bAction = false; //E

            if (IsUser)
            {
                if (ShowControls())
                {
                    //Reset joystick in case we didnt' move it.

                    Stick.Pos = joystickBasePos;
                    //Move the Joypad
                    if (Screen.Game.Input.Global.TouchState == TouchState.Release || Screen.Game.Input.Global.TouchState == TouchState.Up)
                    {
                        StickHeld = false;
                    }
                    else
                    {
                        if (StickHeld)
                        {
                            vec2 vchange = (Screen.Game.Input.LastTouch - startTouch);

                            //Move at least 1/2 tile
                            if (vchange.Len2() >= Math.Pow(Screen.Game.Res.Tiles.TileHeightPixels * 0.5f, 2))
                            {
                                vchange.Normalize();

                                float angle = 0.2f;

                                vec2 up = new vec2(0, -1);
                                vec2 right = new vec2(1, 0);
                                float ud = up.Dot(vchange);
                                float rd = right.Dot(vchange);
                                vec2 vStickAngle = new vec2(0, 0);

                                if (ud > angle)
                                {
                                    bUp = true;
                                    vStickAngle += new vec2(0, Screen.Game.Res.Tiles.TileWidthPixels * -0.5f);
                                }
                                else if (ud < -angle)
                                {
                                    bDown = true;
                                    vStickAngle += new vec2(0, Screen.Game.Res.Tiles.TileWidthPixels * 0.5f);
                                }
                                if (rd > angle)
                                {
                                    bRight = true;
                                    vStickAngle += new vec2(Screen.Game.Res.Tiles.TileWidthPixels * 0.5f, 0);
                                }
                                else if (rd < -angle)
                                {
                                    bLeft = true;
                                    vStickAngle += new vec2(Screen.Game.Res.Tiles.TileWidthPixels * -0.5f, 0);
                                }

                                Stick.Pos = joystickBasePos + vStickAngle;
                            }
                        }
                    }

                    //Jump is up
                    bJump = AButtonPressOrDown;
                    bOk = AButtonPressOrDown;
                    bAction = AButtonPressOrDown;
                }
                else
                {
                    bUp = Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W);
                    bDown = Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S);
                    bLeft = Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A);
                    bRight = Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D);
                    bJump = Keyboard.GetState().IsKeyDown(Keys.Space);
                    bOk = Keyboard.GetState().IsKeyDown(Keys.Enter) || bUp || Screen.Game.Input.Global.PressOrDown();
                    bMenu = Keyboard.GetState().IsKeyDown(Keys.Q);
                    bAction = Keyboard.GetState().IsKeyDown(Keys.LeftShift);

                }
                
            }
            else
            {
                bUp = AIUp;
                bDown = AIDown;
                bLeft = AILeft;
                bRight = AIRight;
                bJump = AIJump;
                bOk = AIOk;
                bAction = AIAction;
            }
            Up.Update(bUp);
            Down.Update(bDown);
            Left.Update(bLeft);
            Right.Update(bRight);
            Jump.Update(bJump);
            Ok.Update(bOk);
            Menu.Update(bMenu);
            Action.Update(bAction);

        }
    }

    public class Input
    {
        public vec2 LastTouch { get; private set; } = new vec2(0, 0);
        public bool UIClicked { get; set; } = false;
        public ButtonBase Global { get; private set; } = new ButtonBase();

        public Input()
        {
        }
        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit


            //vec2 touchPos = new vec2(0, 0);
            bool touched = false;
            MouseState ms = Mouse.GetState();
            TouchCollection tc = TouchPanel.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                LastTouch = new vec2(ms.Position.X, ms.Position.Y);
                touched = true;
            }
            else if (tc.Count > 0)
            {
                LastTouch = new vec2(tc[0].Position);
                touched = true;
            }

            //Your basic button state
            Global.Update(touched);


        }
    }
}
