// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TTengine.Core;

namespace Pixie1
{
    /**
     * Displays a subtitle on screen for a specified time (no rotation or scale at this moment)
     * It auto-positions.
     */
    public class SubtitleText : Drawlet
    {
        /// <summary>
        /// scaling vector for subtitles text (horizontal scale, vertical scale)
        /// </summary>
        public Vector2 ScaleVector = new Vector2(2f, 1.5f);

        /// <summary>
        /// displacement (in pixels) of the shadow below subtitles
        /// </summary>
        public Vector2 ShadowVector = new Vector2(2f, 2f);

        protected string[] text;
        protected float[] timings;
        protected SpriteFont spriteFont;
        bool doReplace;

        public SubtitleText( string initialText)
            : base()
        {
            text = new string[] { initialText };
            timings = new float[] { 0f };
            doReplace = false;
            DrawInfo.DrawColor = Color.White;
            spriteFont = TTengineMaster.ActiveGame.Content.Load<SpriteFont>("Subtitles1");
            AutoPosition();
        }

        public SubtitleText(string[] multiText, float[] timings, bool doReplace)
        {
            this.text = multiText;
            this.timings = timings;
            this.doReplace = doReplace;
            DrawInfo.DrawColor = Color.White;
            spriteFont = TTengineMaster.ActiveGame.Content.Load<SpriteFont>("Subtitles1");
            AutoPosition();
        }

        public string Text        
        {
            get
            {
                return text[0];
            }
            set
            {
                text[0] = value;
                timings = new float[] { 0f };
                AutoPosition();
            }
        }

        protected void AutoPosition()
        {
            Motion.Position = new Vector2(0.1f, 0.8f);
        }

        protected override void OnDraw(ref DrawParams p)
        {
            Vector2 pos = DrawInfo.DrawPosition;
            String curText = "";
            if (text.Length > 0)
            {
                for (int i = 0; i < text.Length; i++ )
                {
                    if (timings[i] <= SimTime)
                    {
                        if (doReplace)
                            curText = text[i];
                        else
                            curText += "\n" + text[i];
                    }
                }
            }
            float sc = Motion.ScaleAbs;
            MySpriteBatch.DrawString(spriteFont, curText, pos + ShadowVector, Color.Black, 0f, Vector2.Zero, ScaleVector * sc, SpriteEffects.None, DrawInfo.LayerDepth + 0.001f);
            MySpriteBatch.DrawString(spriteFont, curText, pos - ShadowVector, Color.DarkGray, 0f, Vector2.Zero, ScaleVector * sc, SpriteEffects.None, DrawInfo.LayerDepth + 0.001f);
            MySpriteBatch.DrawString(spriteFont, curText, pos, DrawInfo.DrawColor, 0f, Vector2.Zero, ScaleVector * sc, SpriteEffects.None, DrawInfo.LayerDepth);
        }
    }
}