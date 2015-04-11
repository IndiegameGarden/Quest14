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
        float vol = 0.5f;
        SoundEvent soundScript = new SoundEvent();
        RenderParams rp = new RenderParams();
        SoundEvent[] soundsBank = new SoundEvent[20];
        double lastPlayAttackSignaltime = -100;

        public GameSound()
        {
            MusicEngine.GetInstance();

            soundsBank[0] = new SampleSoundEvent("sword-unsheathe.wav");
            soundsBank[1] = new SampleSoundEvent("swing.wav");
            soundsBank[2] = new SampleSoundEvent("swing2.wav");
            soundsBank[3] = new SampleSoundEvent("swing3.wav");
            soundsBank[4] = new SampleSoundEvent("hit_1.wav");
            soundsBank[5] = new SampleSoundEvent("hit_2.wav");
            soundsBank[6] = new SampleSoundEvent("hit_3.wav");
            soundsBank[7] = new SampleSoundEvent("die1.wav");
            soundsBank[8] = new SampleSoundEvent("pickup.wav");
            soundsBank[9] = new SampleSoundEvent("use-toy.wav");
            soundsBank[10] = new SampleSoundEvent("trumpet.wav");
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

        protected void Play(int effect, float volume)
        {
            SoundEvent sev = new SoundEvent();
            sev.AddEvent(0f,soundsBank[effect]);
            sev.Amplitude = volume;
            soundScript.AddEvent(SimTime + 0.020f, sev);
        }

        public void PlaySound(int effect, float volumeMin, float volumeMax)
        {
            Play(effect, RandomMath.RandomBetween(volumeMin, volumeMax));
        }

        public void PlayRandomCombatSound(float volumeMin, float volumeMax)
        {
            int n = RandomMath.RandomIntBetween(1,6);
            Play(n, RandomMath.RandomBetween(volumeMin, volumeMax));
        }

        public void PlayDiedSound(float vol)
        {
            Play(7, vol);
        }

        public void PlayPickupSound()
        {
            Play(8, 1.0f);
        }
        
        public void PlayUseToySound()
        {
            Play(9, 1.0f);
        }

        public void PlayAttackSignal()
        {
            if (SimTime < lastPlayAttackSignaltime + 8)
                return;
            Play(10, 1.0f);
            lastPlayAttackSignaltime = SimTime;
        }

    }
}
