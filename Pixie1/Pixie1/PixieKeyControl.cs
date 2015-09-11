// (c) 2010-2014 IndiegameGarden.com. Distributed under the FreeBSD license in LICENSE.txt
﻿using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TTengine.Core;
using Pixie1.Actors;

namespace Pixie1
{
    /**
     * handles all keyboard and gamepad input the user gives to control Hero
     */
    public class PixieKeyControl: ThingControl
    {
        float pressTime = 0f;
        Vector2 ctrlCur, ctrlPrev, ctrlPrevDifferent;
        bool isUseToyPressed = false;
        bool isAttackPressed = false; // attack key
        bool wasAttackPressed = false; // prev state of isAttackPressed
        Hero pixie = null;

        public PixieKeyControl()
            : base()
        {
            MoveSpeed = 1.5f;
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
            pixie = ParentThing as Hero; // TODO hardwired
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // keep previous loop's control input
            Vector2 ctrl = Vector2.Zero;
            ctrlPrev = ctrlCur;
            KeyboardState kb = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            Vector2 sticks = Vector2.Zero;
            const float AST = 0.1f; // AST = Analog Stick Threshold
            if (gp.IsConnected)
                sticks = gp.ThumbSticks.Left + gp.ThumbSticks.Right;            

            if (kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.W) || gp.IsButtonDown(Buttons.DPadUp)
                || (sticks.Y > AST && sticks.Y >= sticks.X) )
                    ctrl.Y += -1.0f;                
            if (kb.IsKeyDown(Keys.Down) || kb.IsKeyDown(Keys.S) || gp.IsButtonDown(Buttons.DPadDown)
                || (sticks.Y < -AST && sticks.Y <= sticks.X) )
                    ctrl.Y += 1.0f;
            if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A) || gp.IsButtonDown(Buttons.DPadLeft)
                || (sticks.X < -AST && sticks.X <= sticks.Y) )
                    ctrl.X += -1.0f;
            if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D) || gp.IsButtonDown(Buttons.DPadRight)
                || (sticks.X > AST && sticks.X >= sticks.Y) )
                    ctrl.X += 1.0f;

            // handle case where 2nd key pressed Left/Right or Up/Down
            if (IsDiag(ctrl) && ctrl.Y == ctrlPrevDifferent.Y )
                ctrl.Y = 0f;
            if (IsDiag(ctrl) && ctrl.X == ctrlPrevDifferent.X)
                ctrl.X = 0f;

            // test for diagonal input!
            if (IsDiag(ctrl))
            {
                ctrl -= ctrlPrevDifferent; // try to compensate - only new key mvt
                if (IsDiag(ctrl))
                {
                    // if still diagonal, use X dir only.
                    ctrl.Y = 0f;
                }
            }

            // transfer found control input of user input persistent var
            ctrlCur = ctrl;

            if (ctrlCur != ctrlPrev)
            {
                ctrlPrevDifferent = ctrlPrev; // keep track of 'previous control that was different'
            }

            if (ctrl.X != 0f || ctrl.Y != 0f)
            {
                if (pressTime == 0f)
                    TargetMove = ctrlCur;
                pressTime += p.Dt;  // keep amount of time a direction input has been given
            }

            // reset key repeat counter if nothing given as input
            if (ctrl.LengthSquared() == 0f)
            {
                pressTime = 0f;
            }

            // trigger attack
            wasAttackPressed = isAttackPressed; // keep old but pres state
            isAttackPressed = kb.IsKeyDown(Keys.Space) || gp.IsButtonDown(Buttons.A) || kb.IsKeyDown(Keys.RightControl)
                || gp.IsButtonDown(Buttons.RightTrigger);
            if (isAttackPressed && !wasAttackPressed)
            {
                pixie.LeadAttack();
            }

            // help msg
            if (kb.IsKeyDown(Keys.Q) || kb.IsKeyDown(Keys.F1)
                || kb.IsKeyDown(Keys.P) || kb.IsKeyDown(Keys.Enter) || gp.IsButtonDown(Buttons.Start) )
            {
                ShowKeysHelp();
            }

            // inventory msg
            if (kb.IsKeyDown(Keys.I) || gp.IsButtonDown(Buttons.B))
            {
                Level.Current.hero.ShowInventory();
            }

            // trigger Toy / magic item
            bool isTriggerKeyPressed = kb.IsKeyDown(Keys.X) || gp.IsButtonDown(Buttons.Y);
            Toy t = ParentThing.ToyActive; 
            if (!isUseToyPressed && isTriggerKeyPressed)
            {
                isUseToyPressed = true;

                // use toy                
                if (t != null && (!t.IsUsed && t.UsesLeft > 0))
                {
                    t.StartUsing();
                    if (t.IsUsed && t.UsesLeft <= 0)
                        ParentThing.ToyActive = null; // remove toy from inventory once used up.
                }
                else
                {
                    ShowNoToyString();
                }
            }
            else if (!isTriggerKeyPressed)
            {
                isUseToyPressed = false;
            }

            // send trigger state to Toy
            if (t != null)
            {
                t.IsTriggered = isTriggerKeyPressed;
            }

            // key repetition
            if (pressTime > 0.2f / ParentThing.Velocity ) 
                pressTime = 0f;

            // make user's requested motion vector            
            if (TargetMove.LengthSquared() > 0f)
                IsTargetMoveDefined = true;

        }

        protected void ShowNoToyString()
        {
            String t = "No magic\nitem to use!";
            Level.Current.StatusText.ClearText();
            Level.Current.StatusText.AddText(t, 3.3f);
            Level.Current.StatusText.Duration = -1; // FIXME terrible hack to avoid auto-deletion
        }

        protected void ShowKeysHelp()
        {
            Level.Current.HelpScroll.Show();
        }

        protected bool IsDiag(Vector2 v)
        {
            return (v.X != 0f && v.Y != 0f);
        }
    }

}
