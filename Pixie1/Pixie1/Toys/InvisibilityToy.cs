using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pixie1.Behaviors;

namespace Pixie1.Toys
{
    /// <summary>
    /// makes you invisible (this can trigger a chasing Thing to lose its trail)
    /// </summary>
    public class InvisibilityToy: Toy
    {
        protected BlinkBehavior blinking;

        public InvisibilityToy(): base()
        {
            UsedUponPickup = false;
            UseTimeMax = 16f;
            SetColors(1.9f, Color.LightSkyBlue, new Color(1f,1f,1f,0f));            
        }

        public override string ToyName()
        {
            return "Cloak of Transpaerance";
        }

        public override void StartUsing()
        {
            base.StartUsing();
            ParentThing.IsStealthy = true;
            blinking = new BlinkBehavior(1.4582f, 0.05f); // add a blinking which signals stealth
            ParentThing.AddNextUpdate(blinking);
            Level.Current.Subtitles.Show(2, "Wonders! My body faded away...", 3f);
        }

        public override void StopUsing()
        {
            base.StopUsing();
            ParentThing.IsStealthy = false;
            blinking.Delete = true; // get rid of the blinking-behavior
            Level.Current.Subtitles.Show(0, "My body is back again!", 3f);
        }
    }
}
