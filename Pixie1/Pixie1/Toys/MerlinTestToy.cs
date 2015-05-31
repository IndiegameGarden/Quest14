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
    public class MerlinTestToy: InvisibilityToy
    {
        public MerlinTestToy()
            : base()
        {
            UseTimeMax = 15f;
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

    }
}
