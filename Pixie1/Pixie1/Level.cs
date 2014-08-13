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
        public static Color PIXIE_COLOR = new Color(251, 101, 159); // pink
        public float DEFAULT_SCALE = 20.0f;
        public float SCREEN_MOTION_SPEED = 15.0f;
        public float PIXIE_TARGETSPEED = 5.0f;
        public int DefaultPassableIntensityThreshold = 280;
        public Vector2 HERO_STARTING_POS = Vector2.Zero; // in pixels        
        public Vector2 BG_STARTING_POS = Vector2.Zero;    // in pixels; bg=background

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
        /// default color of the background (e.g. for areas not covered by the bg bitmap)
        /// </summary>
        public Color BackgroundColor = Color.Black;

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

        // class internal
        protected ThingControl keyControl; // for pixie
        //protected DebugMessage debugMsg;
        protected SubtitleText subTitles;
        float timeEscDown = 0f;        

        public Level(): base()
        {
            Current = this; // pointer to level instance singleton

            // create level's objects. These will be added as a child later.
            MotionB = new MotionBehavior();
            Subtitles = new SubtitleManager();
            //debugMsg = new DebugMessage();
        }

        /// <summary>
        /// Init: the scrolling level itself. First Init method that is called
        /// </summary>
        protected virtual void InitLevel()
        {
            Motion.Scale = DEFAULT_SCALE;
            Motion.ScaleTarget = DEFAULT_SCALE;
            // create own custom SpriteBatch for blocky graphics (PointClamp)
            MySpriteBatch = new TTSpriteBatch(Screen.graphicsDevice,SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }

        /// <summary>
        /// Init: pixie herself (a default implementation is in Level)
        /// </summary>
        protected virtual void InitPixie()
        {
            hero = new Hero();      
            hero.SetStartingPos(HERO_STARTING_POS);
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
            t.AddText("--- THE END ---", 7f);
            Subtitles.Show(9,  t);
            isBackgroundScrollingOn = false;

            t = new SubtitleText();
            t.AddText("Hold Esc to Exit", 7f);
            t.AddText("", 1f);
            t.AddText("Coding & art by Indiegame Garden 2012-2014", 4f);
            t.AddText("Sounds by Jute and artisticdude (opengameart.org)", 3f);
            t.AddText("Music by John the Luteist", 3f);
            t.AddText("Sprite by Charles Gabriel (opengameart.org)", 3f);

            t.ScaleVector = new Vector2(1f, 1f);
            t.Motion.Scale = 0.5f;
            t.Motion.Position = new Vector2(Screen.Center.X, 0.08f);
            t.StartTime = this.SimTime + 28.5f;
            Parent.AddNextUpdate(t);

        }

        public void WinLevel()
        {
            if (!hasWon)
            {
                SubtitleText t = new SubtitleText();
                t.AddText("YOU WIN!", 5f);
                t.AddText("Pink Arthur is rescued.", 4f);
                t.AddText("He rules for\nmany prosperous years.", 4f);
                t.AddText("", 2f);
                t.AddText("** THE END **", 13f);
                float playTime = (float)Math.Round(SimTime);
                t.AddText("(Rescue time: " + playTime + " heartbeats.)", 15f);
                Subtitles.Show(6, t);
                hasWon = true;
            }
        }

        public void FoundPinkArthur()
        {
            if (!hasFoundPinkArthur)
            {
                hasFoundPinkArthur = true;
                SubtitleText t = new SubtitleText();
                t.AddText("My King! Here you are.", 4f);
                t.AddText("We have come to rescue you.", 4f);
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
                MotionB.ScaleSpeed = 0.005f;
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
            if (st.IsKeyDown(Keys.D1))
            {
                Level.Current.hero.PositionAndTarget = new Vector2(567f, 262f);
                Level.Current.SCREEN_MOTION_SPEED = 150f;
            }
            else if (st.IsKeyDown(Keys.D2))
            {
                Level.Current.hero.PositionAndTarget = new Vector2(70f, 64f);
                Level.Current.king.PositionAndTarget = new Vector2(72f, 64f);
                Level.Current.SCREEN_MOTION_SPEED = 150f;
            }
            else
                Level.Current.SCREEN_MOTION_SPEED = 8f;
           if (st.IsKeyDown(Keys.F1))
               Level.Current.hero.IsCollisionFree = true;
           else if (st.IsKeyDown(Keys.F2))
               Level.Current.hero.IsCollisionFree = false;
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

        /// <summary>
        /// check whether the given pixel position in this level is currently passable
        /// </summary>
        /// <param name="pos">pixel position to check</param>
        /// <returns>true if passable for any Thing entity</returns>
        /*
        public bool CanPass(Vector2 pos)
        {
            return Background.IsWalkable(pos);
        }
        */

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // important: reflect the global viewpos (for sprites to use)
            Thing.ViewPos = Background.Position;

            // do some level tasks
            LevelKeyControl(ref p);
            if (isBackgroundScrollingOn)
                ScrollBackground(ref p);

            //debugMsg.Text = "Pixie: trg=" + pixie.Target +", pos=" + pixie.Position;
            // DEBUG sample pixel
            //Color c= Background.SamplePixel(pixie.Target);
            //debugMsg.Text += "Color: " + c.R + "," + c.G + "," + c.B + "," + c.A;

        }
    }
}
