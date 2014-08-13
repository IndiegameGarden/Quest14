using System;
using TTengine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pixie1
{
    public class PixieLogo: Spritelet
    {
        public PixieLogo(string logoGraphics)
            : base(logoGraphics)
        {
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
            DrawInfo.LayerDepth = 0.05f;
            Motion.Position = new Vector2(Screen.Center.X, Screen.Center.Y - 0.3f);
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }
    }
}
