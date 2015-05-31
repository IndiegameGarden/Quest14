using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Pixie1.Actors;
using Pixie1.AStar;

namespace Pixie1
{
    /// <summary>
    /// base class for all levels (common functions)
    /// </summary>
    public abstract class Level: Drawlet
    {
        /// <summary>
        /// the current Level singleton
        /// </summary>
        public static Level Current = null;

        // some default colors and settings that may be changed by Level subclasses
        public Color PINK_COLOR = new Color(251, 101, 159); // pink
        public float DEFAULT_SCALE = 20.0f;
        public float SCREEN_MOTION_SPEED = 15.0f;
        public float PIXIE_TARGETSPEED = 5.0f;
        public int DefaultPassableIntensityThreshold = 280;
        public Vector2 HERO_STARTING_POS = Vector2.Zero; // in pixels        
        public Vector2 BG_STARTING_POS = Vector2.Zero;    // in pixels; bg=background
        public Vector2 HELPSCROLL_SHOW_POS;
        public Vector2 HELPSCROLL_HIDE_POS;

        // specific crap FIXME
        public bool hasFoundPinkArthur = false;
        public bool hasWon = false;

        /// <summary>
        /// scrolling screen trigger boundaries (in TTengine coordinates)
        /// </summary>
        public bool isBackgroundScrollingOn = true;
        public float BOUND_X = 0.3f;
        public float BOUND_Y = 0.3f;

        /// <summary>
        /// level moves/scrolls behavior for TTengine
        /// </summary>
        public MotionBehavior MotionB;

        /// <summary>
        /// level music object
        /// </summary> 
        public GameMusic Music;

        /// <summary>
        /// level sounds object
        /// </summary>
        public GameSound Sound;

        /// <summary>
        /// background bitmap
        /// </summary>
        public LevelBackground Background;

        /// <summary>
        /// path finding system for level
        /// </summary>
        public PathFindingSystem PathFinder;

        /// <summary>
        /// load items/toys/things to a level using a bitmap
        /// </summary>
        public LevelItemLoader ItemsMap;

        /// <summary>
        /// our hero
        /// </summary>
        public Hero hero;

        /// <summary>
        /// boss monster
        /// </summary>
        public Boss boss;

        /// <summary>
        /// king to be rescued
        /// </summary>
        public PinkArthur king;

        /// <summary>
        /// takes care of displaying subtitles in priority order
        /// </summary>
        public SubtitleManager Subtitles;

        public SubtitleText StatusText;

        public TimedSpritelet HelpScroll;

        public List<Thing> AllEnemies = new List<Thing>();

        // class internal
        protected ThingControl keyControl; // for pixie
        protected DebugMessage debugMsg = new DebugMessage();
        protected SubtitleText subTitles;
        float timeEscDown = 0f;        

        public Level(): base()
        {
            Current = this; // pointer to level instance singleton

            // create level's objects. These will be added as a child later.
            MotionB = new MotionBehavior();
            Subtitles = new SubtitleManager();
            StatusText = new SubtitleText();            
            HelpScroll = new TimedSpritelet("help-scroll.png");
            //debugMsg = new DebugMessage();
        }

        /// <summary>
        /// Init: the scrolling level itself. First Init method that is called
        /// </summary>
        protected virtual void InitLevel()
        {
            var t = StatusText;
            t.IsAutoPosition = false;
            t.ScaleVector = new Vector2(1f, 1f);
            t.Motion.Scale = 0.4f;
            t.Motion.Position = new Vector2(Screen.Center.X * 1.5f, 0.01f);
            t.ShadowVector = new Vector2(1f, 1f); // adapt shadow for small text
            t.DrawInfo.DrawColor = Color.Coral; // Color.LightSalmon;
            Parent.Add(t);

            HELPSCROLL_SHOW_POS = new Vector2(Screen.Width - 0.09f, 0.01f);
            HELPSCROLL_HIDE_POS = new Vector2(HELPSCROLL_SHOW_POS.X, -1f);
            HelpScroll.Motion.Position = HELPSCROLL_HIDE_POS;
            HelpScroll.Motion.TargetPos = HELPSCROLL_HIDE_POS;
            HelpScroll.Motion.TargetPosSpeed = 3f;
            HelpScroll.DrawInfo.Center = new Vector2(1f, 0f);
            HelpScroll.DrawInfo.LayerDepth = 0.01f;
            Parent.Add(HelpScroll);

            Motion.Scale = DEFAULT_SCALE;
            Motion.ScaleTarget = DEFAULT_SCALE;
            // create own custom SpriteBatch for blocky graphics (PointClamp)
            //MySpriteBatch = new TTSpriteBatch(Screen.graphicsDevice,SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }

        /// <summary>
        /// Init: pixie herself (a default implementation is in Level)
        /// </summary>
        protected virtual void InitPixie()
        {
            hero = new Hero();      
            hero.PositionAndTarget = HERO_STARTING_POS;
            hero.TargetSpeed = PIXIE_TARGETSPEED;
            Add(hero);
        }

        /// <summary>
        /// Init: bad pixels (enemies)
        /// </summary>
        protected virtual void InitBadPixels()
        {
        }

        /// <summary>
        /// Init: toys (=weapons)
        /// </summary>
        protected virtual void InitToys()
        {
        }

        public virtual void LoseLevel()
        {
            this.Subtitles.Children.Clear(); // clean out any remaining subtitles being shown.
            SubtitleText t = new SubtitleText();
            t.AddText("BADLY WOUNDED, YOU DIE.", 7f);
            t.AddText("Galad the Golden is no more.", 7f);
            t.AddText("Pink Arthur remains captive\nfor all his life.", 7f);
            t.AddText("** THE END **", 7f);
            Subtitles.Show(9,  t);
            isBackgroundScrollingOn = false; // stop the scrolling of the level around hero

            t = new SubtitleText();
            t.ScaleVector = new Vector2(1f, 1f);
            t.ShadowVector = new Vector2(1f, 1f);
            t.Motion.Scale = 0.5f;
            t.Motion.Position = new Vector2(Screen.Center.X, 0.08f);
            t.StartTime = this.SimTime + 28.5f;
            t.AddText("Coding & art by Indiegame Garden 2012-2015", 7f);
            Parent.AddNextUpdate(t);

        }

        public void WinLevel()
        {
            if (!hasWon)
            {
                float playTime = (float)Math.Round(SimTime);
                SubtitleText t = new SubtitleText();
                t.AddText("My King! WE WON!!", 5f).DrawInfo.DrawColor = Color.DarkGoldenrod;
                t.AddText("(Arthur:) I am home, at last.", 4f).DrawInfo.DrawColor = PINK_COLOR;
                t.AddText("A reward, Galad,\nawaits thee...", 5f).DrawInfo.DrawColor = PINK_COLOR;
                t.AddText("...in the Garden of Golden Chain!", 6f).DrawInfo.DrawColor = PINK_COLOR;
                t.AddText("", 1f);
                t.AddText("(Bard:) And so Arthur ruled for\nmany more prosperous years.", 4f);
                t.AddText("His Knights of the Square Table didth\ncommit a many legendary deeds.", 6f);
                t.AddText("", 2f);
                t.AddText("** THE END **", 4f);                
                t.AddText("(Rescue time: " + playTime + " heart-beats.)", 15f);
                t.AddText("** THE END **", 4f);
                Subtitles.Show(6, t);
                hasWon = true;

                // level changes o so slightly
                Background.OverlayTexture("Garden1.png");
            }
        }

        public void FoundPinkArthur()
        {
            if (!hasFoundPinkArthur)
            {
                hasFoundPinkArthur = true;
                SubtitleText t = new SubtitleText();
                t.AddText("My King! Here you are.", 4f);
                t.AddText("We have come, to rescue thee.", 4f);
                t.AddText("Follow me, out of this\ncursed place.", 7f);
                Subtitles.Show(8, t);
            }
        }

        /// <summary>
        /// Init: level-specific items (not fitting in the existing init categories) to be initialized by subclasses
        /// </summary>
        protected abstract void InitLevelSpecific();

        protected override void OnNewParent()
        {
            base.OnNewParent();

            //Parent.Add(debugMsg);
            Add(MotionB);

            InitLevel();
            PathFinder = new PathFindingSystem(Background, DefaultPassableIntensityThreshold);
            InitPixie();
            InitBadPixels();
            InitToys();
            InitLevelSpecific();

            Add(Subtitles);
        }

        /// check keys specific for level
        protected virtual void LevelKeyControl(ref UpdateParams p)
        {
            KeyboardState st = Keyboard.GetState();
            if (st.IsKeyDown(Keys.Escape))
            {
                timeEscDown += p.Dt;
                MotionB.ScaleTarget = 1.5f*DEFAULT_SCALE;
                MotionB.ScaleSpeed = 0.009f;
                //Motion.RotateModifier = timeEscDown * 0.05f;
                //PixieGame.Instance.Exit();
            }
            else
            {
                timeEscDown = 0f;
                MotionB.ScaleTarget = DEFAULT_SCALE; // TODO
            }
            if (timeEscDown > 0.45f)
            {
                PixieGame.Instance.StopPlay();
            }

#if DEBUG
            // debug keys
            if (st.IsKeyDown(Keys.D4))
            {
                hero.PositionAndTarget = new Vector2(567f, 262f);
                SCREEN_MOTION_SPEED = 250f;
            }
            else if (st.IsKeyDown(Keys.D1))
            {
                hero.PositionAndTarget = new Vector2(40f, 60f);
                SCREEN_MOTION_SPEED = 250f;
            }
            else if (st.IsKeyDown(Keys.D2))
            {
                hero.PositionAndTarget = new Vector2(20f, 180f);
                SCREEN_MOTION_SPEED = 250f;
            }
            else if (st.IsKeyDown(Keys.D3))
            {
                hero.PositionAndTarget = new Vector2(380f, 267f);
                SCREEN_MOTION_SPEED = 250f;
            }
            else
                SCREEN_MOTION_SPEED = 8f;

            if (st.IsKeyDown(Keys.F2))         // god mode
                hero.IsCollisionFree = true;
            else if (st.IsKeyDown(Keys.F1))     // normal mode
                hero.IsCollisionFree = false;
            else if (st.IsKeyDown(Keys.F3))
                king.PositionAndTarget = hero.Target + new Vector2(2f, 0f); // fetch king rightaway
            else if (st.IsKeyDown(Keys.OemMinus))
            {
                var m = Motion;
                m.ZoomTarget = 0.5f;
                m.ZoomSpeed = 0.005f;
                m.ZoomCenterTarget = hero.Motion;
            }
            else if (st.IsKeyDown(Keys.OemPlus))
            {
                var m = Motion;
                m.ZoomTarget = 1f;
                m.ZoomSpeed = 0.005f;
                m.ZoomCenterTarget = hero.Motion;
            }
#endif
        }

        // scroll the level background to match pixie
        protected virtual void ScrollBackground(ref UpdateParams p)
        {
            // scrolling background at borders
            Vector2 pixiePos = hero.Motion.PositionAbs;

            if (pixiePos.X < BOUND_X || pixiePos.X > (Screen.Width - BOUND_X) ||
                pixiePos.Y < BOUND_Y || pixiePos.Y > (Screen.Height - BOUND_Y))
            {
                if (ScreenBorderHit())
                    Background.Target = hero.Position;
            }
        }

        /// <summary>
        /// can be overridden with custom functions if screen border is hit by pixie
        /// </summary>
        protected virtual bool ScreenBorderHit()
        {
            return true;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            this.StatusText.Duration = -1; // FIXME terrible hack to avoid auto-deletion
            base.OnUpdate(ref p);

            // important: reflect the global viewpos (for sprites to use)
            Thing.ViewPos = Background.Position;

            // do some level tasks
            LevelKeyControl(ref p);
            if (isBackgroundScrollingOn)
                ScrollBackground(ref p);

            /*
            debugMsg.Text = "Pixie: trg=" + hero.Target + ", pos=" + hero.Position;
            // DEBUG sample pixel
            Color c = Background.SamplePixel(hero.Target);
            debugMsg.Text += "Color: " + c.R + "," + c.G + "," + c.B + "," + c.A;
            */
        }
    }
}
