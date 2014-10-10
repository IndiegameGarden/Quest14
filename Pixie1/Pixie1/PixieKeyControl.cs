// (c) 2010-2014 IndiegameGarden.com. Distributed under the FreeBSD license in LICENSE.txt
﻿using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TTengine.Core;
using Pixie1.Actors;

namespace Pixie1
{
    public class PixieKeyControl: ThingControl
    {
        float pressTime = 0f;
        Vector2 userInputDirection, userInputDirectionPrevious;
        bool isTriggerPressed = false;
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

            float dx = 0f, dy = 0f;

            KeyboardState kb = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            Vector2 sticks = Vector2.Zero;
            if (gp.IsConnected)
            {
                sticks = gp.ThumbSticks.Left + gp.ThumbSticks.Right;
            }

            if (kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.W) || gp.IsButtonDown(Buttons.DPadUp)
                || sticks.Y > 0f )
                    dy += -1.0f;                
            if (kb.IsKeyDown(Keys.Down) || kb.IsKeyDown(Keys.S) || gp.IsButtonDown(Buttons.DPadDown)
                || sticks.Y < 0f )
                    dy += 1.0f;
            if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A) || gp.IsButtonDown(Buttons.DPadLeft)
                || sticks.X < 0f)
                    dx += -1.0f;
            if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D) || gp.IsButtonDown(Buttons.DPadRight) ||
                sticks.X > 0f)
                    dx += 1.0f;

            userInputDirectionPrevious = userInputDirection;
            userInputDirection = new Vector2(dx, dy);

            // handle case where 2nd key pressed Left/Right or Up/Down
            if (userInputDirectionPrevious.Y != 0 && dy == userInputDirectionPrevious.Y && dx != 0)
                dy = 0f;
            if (userInputDirectionPrevious.X != 0 && dx == userInputDirectionPrevious.X && dy != 0)
            {
                dx = 0f;
                pressTime = 0f;
            }

            if (dx != 0f || dy != 0f)
            {
                if (pressTime == 0f)
                    TargetMove = new Vector2(dx, dy);
                pressTime += p.Dt;  // keep amount of time a direction input has been given
            }

            if (userInputDirection.LengthSquared() == 0f)
            {
                pressTime = 0f;
            }

            // trigger attack
            if (kb.IsKeyDown(Keys.Space) || gp.IsButtonDown(Buttons.A) || kb.IsKeyDown(Keys.RightControl))
            {
                pixie.LeadAttack();
            }

            // trigger Toy
            bool isTriggerKeyPressed = kb.IsKeyDown(Keys.X) || kb.IsKeyDown(Keys.LeftControl) || 
                                    gp.IsButtonDown(Buttons.B);
            Toy t = ParentThing.ToyActive; 
            if (!isTriggerPressed && isTriggerKeyPressed)
            {
                isTriggerPressed = true;

                // use toy                
                if (t != null)
                {
                    if (!t.IsUsed && t.UsesLeft > 0)
                        t.StartUsing();
                }
            }
            else if (!isTriggerKeyPressed)
            {
                isTriggerPressed = false;
            }

            // send trigger state to Toy
            if (t != null)
            {
                t.IsTriggered = isTriggerKeyPressed;
            }

            // key rep
            if (pressTime > 0.2f / ParentThing.Velocity ) 
                pressTime = 0f;

            // make user's requested motion vector
            
            if (TargetMove.LengthSquared() > 0f)
                IsTargetMoveDefined = true;

        }

    }
}
