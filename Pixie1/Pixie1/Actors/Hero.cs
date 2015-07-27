using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class Hero: Thing
    {
        public PixieKeyControl KeyControl;

        public List<Knight> Knights = new List<Knight>();

        public Hero()
            : base("pixie")
        {            
            IsCollisionFree = false;
            SetColors(4f, Color.DarkGoldenrod, new Color(230, 210, 10));
            Velocity = 1.5f;
            Health = 15f;
            Pushing.Force = 10f; // force higher than companions.
            DrawInfo.LayerDepth = 0.4f;

        }

        protected override void OnInit()
        {
            base.OnInit();
            KeyControl = new PixieKeyControl();
            this.Add(KeyControl);
        }

        public void LeadAttack()
        {
            if (SimTime < 280 && SimTime > 21.6)
                ShowAttackDirectionString();
            if (SimTime > 45 && RandomMath.RandomUnit() < 0.4)
                Level.Current.Sound.PlayAttackSignal();
            foreach (Knight c in Knights)
                c.Attacking.TriggerAttack();
        }

        protected void ShowAttackDirectionString()
        {
            String t = "Knights attack\n";
            if (FacingDirection.X < 0 && FacingDirection.Y == 0)
                t += "west";
            if (FacingDirection.X > 0 && FacingDirection.Y == 0)
                t += "east";
            if (FacingDirection.X == 0 && FacingDirection.Y < 0)
                t += "north";
            if (FacingDirection.X == 0 && FacingDirection.Y > 0)
                t += "south";
            t += "ward.";
            Level.Current.StatusText.ClearText();
            Level.Current.StatusText.AddText(t, 3.3f);
            Level.Current.StatusText.Duration = -1; // FIXME terrible hack to avoid auto-deletion
        }

        public void ShowInventory()
        {
            String t = "You now hold\n";
            if (this.ToyActive==null)
                t = "You hold\nnothing.";
            else {
                t+= "\"" + this.ToyActive.ToyName() + "\"";
            }
            Level.Current.StatusText.ClearText();
            Level.Current.StatusText.AddText(t, 3.3f);
            Level.Current.StatusText.Duration = -1; // FIXME terrible hack to avoid auto-deletion
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }

        protected override void OnDies()
        {
            base.OnDies();
            ColorFx.Active = false;
            KeyControl.Active = false;
            Level.Current.Sound.PlayDiedSound(1.0f);
            
            Level.Current.LoseLevel();
        }

    }
}
