using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TTengine.Core;
using TTengine.Util;
using TTengine.Modifiers;
using TTMusicEngine;
using TTMusicEngine.Soundevents;
using Pixie1.Levels;
using Pixie1.Actors;

namespace Pixie1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PixieGame : Game
    {
        public Gamelet TreeRoot;
        private static PixieGame instance = null;
        GraphicsDeviceManager graphics;
        int myWindowWidth;
        int myWindowHeight; 
        PixieScreenlet mainScreenlet;
        MusicEngine musicEngine;
        Level level;
        float maxDtUpdate = 0f; // time keeping
        DebugMessage debugMsg;

        public PixieGame()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            myWindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            myWindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Window.IsBorderless = true;
            graphics.PreferredBackBufferHeight = myWindowHeight;
            graphics.PreferredBackBufferWidth = myWindowWidth;
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
        }

        public static PixieGame Instance
        {
            get
            {
                return instance;
            }
        }

        protected override void Initialize()
        {
            TTengineMaster.Create(this);

            // open the TTMusicEngine
            musicEngine = MusicEngine.GetInstance();
            musicEngine.AudioPath = "Content";
            if (!musicEngine.Initialize())
                throw new Exception(musicEngine.StatusMsg);

            // from here on, main screen
            mainScreenlet = new PixieScreenlet(myWindowWidth, myWindowHeight);
            TTengineMaster.ActiveScreen = mainScreenlet;
            TreeRoot = new FixedTimestepPhysics();
            TreeRoot.Add(mainScreenlet);

            // debug only
            debugMsg = new DebugMessage("");
            //mainScreenlet.Add(debugMsg);

            // finally call base to enumnerate all (gfx) Game components to init
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //mainScreenlet.Add(new FrameRateCounter(1.0f, 0f)); // TODO
            level = new QuestLevel();

            mainScreenlet.Add(level);

            base.LoadContent();
        }

        public void StartPlay()
        {
        }

        public void StopPlay()
        {
            Exit();
        }

        public void WinGame()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            long t0 = System.DateTime.Now.Ticks;
            TTengineMaster.Update(gameTime, TreeRoot);
            long t1 = System.DateTime.Now.Ticks;
            float dt = ((float)(t1 - t0)) * 0.001f; //convert to ms
            //if (dt > maxDtUpdate)
            maxDtUpdate = maxDtUpdate * 0.98f + 0.02f * dt;

            // update any other XNA components
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            TTengineMaster.Draw(gameTime, TreeRoot);
            //debugMsg.Text = Math.Round(maxDtUpdate) + " ms      ";

            // then draw other (if any) XNA game components on the screen
            base.Draw(gameTime);
        }

    }
}
