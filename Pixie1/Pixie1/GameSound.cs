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
    /**
     * plays game sounds
     */
    public class GameSound: Gamelet
    {
        public const float HEARING_RANGE = 60f;
        public enum SoundEffects { COMBAT1 = 0, COMBAT2, COMBAT3, COMBAT4, COMBAT5, COMBAT6, COMBAT7, DIE, PICKUP, USE_TOY, TRUMPET, NULL };

        SoundEvent soundScript = new SoundEvent();
        RenderParams rp = new RenderParams();
        SoundEvent[] soundsBank = new SoundEvent[(int)SoundEffects.NULL + 1];
        double lastPlayAttackSignaltime = -100;

        public GameSound()
        {
            MusicEngine.GetInstance();

            soundsBank[(int)SoundEffects.COMBAT1] = new SampleSoundEvent("sword-unsheathe.wav");
            soundsBank[(int)SoundEffects.COMBAT2] = new SampleSoundEvent("swing.wav");
            soundsBank[(int)SoundEffects.COMBAT3] = new SampleSoundEvent("swing2.wav");
            soundsBank[(int)SoundEffects.COMBAT4] = new SampleSoundEvent("swing3.wav");
            soundsBank[(int)SoundEffects.COMBAT5] = new SampleSoundEvent("hit_1.wav");
            soundsBank[(int)SoundEffects.COMBAT6] = new SampleSoundEvent("hit_2.wav");
            soundsBank[(int)SoundEffects.COMBAT7] = new SampleSoundEvent("hit_3.wav");
            soundsBank[(int)SoundEffects.DIE]     = new SampleSoundEvent("die2.wav");
            soundsBank[(int)SoundEffects.PICKUP] = new SampleSoundEvent("pickup.wav");
            soundsBank[(int)SoundEffects.USE_TOY] = new SampleSoundEvent("use-toy.wav");
            soundsBank[(int)SoundEffects.TRUMPET] = new SampleSoundEvent("trumpet.wav");
        }

        /// <summary>
        /// Adjust overal volume of game sounds 0.0-1.0
        /// </summary>
        public double Volume
        {
            get
            {
                return rp.Ampl;
            }
            set
            {
                double vol = value;
                if (vol < 0f) vol = 0f;
                if (vol > 1f) vol = 1f;

                rp.Ampl = vol;
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            rp.Time = SimTime;
            MusicEngine.GetInstance().Render(soundScript, rp);

        }

        /// <summary>
        /// Play method to play an effect from SoundEffects enum at specified volume
        /// </summary>
        /// <param name="effect">index into effect sounds bank (from SoundEffects enum)</param>
        /// <param name="volume">volume, typically 0-1 but can be higher to boost weak sounds</param>
        public void PlaySound(int effect, double volume)
        {
            SoundEvent sev = new SoundEvent();
            sev.AddEvent(0f,soundsBank[effect]);
            sev.Amplitude = volume;
            soundScript.AddEvent(SimTime + 0.010f, sev);
        }

        /// <summary>
        /// Play methodd to play an effect at a random volume between limits
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="volumeMin">min volume</param>
        /// <param name="volumeMax">max volume</param>
        public void PlaySoundRandVol(int effect, double volumeMin, double volumeMax)
        {
            PlaySound(effect, RandomMath.RandomBetween(volumeMin, volumeMax));
        }

        /// <summary>
        /// play a sound, adapted by a distance to player
        /// </summary>
        /// <param name="volumeMinFar">min volume (when very far from player, almost outside HEARING_RANGE)</param>
        /// <param name="volumeMaxClose">max volume (when very close to player, dist=0)</param>
        /// <param name="distToPlayer"></param>
        public void PlaySoundDist(int effect, double volume, double volumeMinFar, double volumeMaxClose, float distToPlayer)
        {
            float a = 1f;
            if (distToPlayer > 0f)
            {
                if (distToPlayer > HEARING_RANGE)
                    return;
                a = 1f - (distToPlayer / HEARING_RANGE);
            }
            PlaySound(effect, volume * (volumeMinFar + (volumeMaxClose - volumeMinFar) * a) );
        }

        /// <summary>
        /// play a random combat sound, optionally adapted by a distance to player
        /// </summary>
        /// <param name="volumeMin"></param>
        /// <param name="volumeMax"></param>
        /// <param name="distToPlayer"></param>
        public void PlayRandomCombatSound(float volumeMin, float volumeMax, float distToPlayer = 0f)
        {
            float a = 1f;
            if (distToPlayer >= 0f)
            {
                if (distToPlayer > HEARING_RANGE)
                    return;
                a = 1f - (distToPlayer / HEARING_RANGE);
            }
            else
            {
                return;
            }
            int n = RandomMath.RandomIntBetween((int) SoundEffects.COMBAT1, (int) SoundEffects.COMBAT7);
            PlaySoundDist(n, RandomMath.RandomBetween(volumeMin, volumeMax), 0.0, 1.0, distToPlayer);
        }

        public void PlayDiedSound(float volMultiplier = 1.0f, float distToPlayer = 0f)
        {
            PlaySoundDist( (int)SoundEffects.DIE, 0.85f * volMultiplier, 0.0, 1.0, distToPlayer);
        }

        public void PlayPickupSound()
        {
            PlaySound((int)SoundEffects.PICKUP, 1.0f);
        }
        
        public void PlayUseToySound()
        {
            PlaySound((int)SoundEffects.USE_TOY, 1.0f);
        }

        public void PlayAttackSignal()
        {
            if (SimTime < lastPlayAttackSignaltime + 8)
                return;
            PlaySound((int)SoundEffects.TRUMPET, 1.0f);
            lastPlayAttackSignaltime = SimTime;
        }

    }
}
