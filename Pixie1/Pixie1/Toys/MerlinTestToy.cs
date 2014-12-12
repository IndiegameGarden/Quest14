using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pixie1.Toys
{
    /// <summary>
    /// to test spell usage with key in castle
    /// </summary>
    public class MerlinTestToy: Toy
    {
        public MerlinTestToy()
            : base()
        {
            UseTimeMax = 10f;
            UsedUponPickup = false;
            SetColors(1.9f, Color.LightSkyBlue, new Color(0.9f,0.4f,1f,0f));
        }

        public override string ToyName()
        {
            return "Merlin's Seeds of Stealth";
        }

        public override string ToyHint()
        {
            return "press X to swallow!";
        }

        public override void StartUsing()
        {
            base.StartUsing();
            ParentThing.Visible = false;
            Level.Current.Subtitles.Show(2, "Wonders! My body faded away...", 3f);
        }

        public override void StopUsing()
        {
            base.StopUsing();
            ParentThing.Visible = true;
            Level.Current.Subtitles.Show(0, "My body is back again!", 3f);
        }
    }
}
