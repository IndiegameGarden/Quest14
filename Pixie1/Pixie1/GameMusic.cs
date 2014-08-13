using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework.Media;
using TTMusicEngine;
using TTMusicEngine.Soundevents;

namespace Pixie1
{
    public class GameMusic: Gamelet
    {
        float vol = 1.5f;
        SoundEvent soundScript;
        RenderParams rp = new RenderParams();

        public GameMusic()
        {
            soundScript = new SoundEvent("GameMusic");
            SampleSoundEvent ev1 = new SampleSoundEvent("quest14_medley.ogg"); 
            soundScript.AddEvent(0.4, ev1);
            soundScript.AddEvent(1 * 630, ev1);
            soundScript.AddEvent(2 * 630, ev1);
            soundScript.AddEvent(3 * 630, ev1);
            soundScript.AddEvent(4 * 630, ev1);
        }

        public float Volume
        {
            get
            {
                return vol;
            }
            set
            {
                vol = value;
                if (vol < 0f) vol = 0f;
                if (vol > 1f) vol = 1f;

                rp.Ampl = vol;
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            rp.Time = SimTime; // gameTime.ElapsedGameTime.TotalSeconds;
            MusicEngine.GetInstance().Render(soundScript, rp);

        }

    }
}
