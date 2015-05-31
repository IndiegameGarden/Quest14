using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pixie1.Actors;
using Pixie1.Behaviors;

namespace Pixie1.Toys
{
    /// <summary>
    /// to test spell usage with key in castle
    /// </summary>
    public class RedsConfusionToy: Toy
    {
        public RedsConfusionToy()
            : base()
        {
            UsedUponPickup = false;
            UseTimeMax = 35f;
            SetColors(1.9f, Color.RoyalBlue, Color.Red);
        }

        public override string ToyName()
        {
            return "Twisted Wand";
        }

        public override string ToyHint()
        {
            return "What does this do??";
        }

        public override void StartUsing()
        {
            base.StartUsing();
            foreach (Thing t in Level.Current.AllEnemies)
            {
                if (t is RedGuard)
                {
                    var rg = t as RedGuard;
                    rg.ComplexBehavior.ReverseBehavior();
                }
            }
            Level.Current.Subtitles.Show(2, "You wave the wand...", 3f);
        }

        public override void StopUsing()
        {
            base.StopUsing();
            foreach (Thing t in Level.Current.AllEnemies)
            {
                if (t is RedGuard)
                {
                    var rg = t as RedGuard;
                    rg.ComplexBehavior.ReverseBehavior();   // conjure it back again!
                }
            }
        }


    }
}
