using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pixie1;
using Pixie1.Actors;
using Pixie1.Toys;

namespace Pixie1.Levels
{
    /// <summary>
    /// a level featuring a castle
    /// </summary>
    public class QuestLevel : Level
    {
        Vector2 PINKARTHUR_POSITION = new Vector2(557f, 281f);
        Vector2 TRACKING_HELPER_POINT = new Vector2(28f, 54f);
        Color LEVEL_FOREGROUND_COLOR = new Color(231, 231, 248);
        Color ITEM_BLOCK_COLOR = new Color(179, 102, 27); // 179,102,27 brown: block

        int[] KnightsStartingPositions = new int[]{22,58, 28,58, 33,58, 36,61 , 36,63, 34,72, 31,72, 28,72, 25,72, 22,72, 20,70, 20,67, 20,65, 20,60};

        int numberOfZoomOuts = 0;        

        public QuestLevel()
            : base()
        {           
            // Level settings
            SCREEN_MOTION_SPEED = 8.0f;
            DEFAULT_SCALE = 15f;// 15f; // 15f;
            HERO_STARTING_POS = new Vector2(36f, 65f); // in pixels        
            //PIXIE_STARTING_POS = new Vector2(242f, 155f); // debug
            //PIXIE_STARTING_POS = PRINCESS_POSITION + new Vector2(-10f,-10f); // debug
            //PIXIE_STARTING_POS += new Vector2(200f, 4f); // debug
            //PIXIE_STARTING_POS = new Vector2(73f, 10f); // debug
            //BG_STARTING_POS = new Vector2(30f, 155f); // in pixels; bg=background            
            BG_STARTING_POS = HERO_STARTING_POS; // +new Vector2(-12f, 0f); 
            //BG_STARTING_POS += new Vector2(200f, 4f); // debug
            //PIXIE_STARTING_POS = new Vector2(188f, 0f); // debug, close to win pos
            //BG_STARTING_POS = new Vector2(188f, 0f); // debug
        }

        protected override void InitPixie()
        {            
            base.InitPixie();
            Hero.PositionLog[0] = TRACKING_HELPER_POINT;
        }

        protected override void InitLevel()
        {
            base.InitLevel();

            // select bitmap bg
            Background = new LevelBackground("Level1.png");
            Background.ForegroundColor = LEVEL_FOREGROUND_COLOR;
            Background.TargetSpeed = SCREEN_MOTION_SPEED;
            Add(Background);
            Background.Target = HERO_STARTING_POS;
            Background.Position = BG_STARTING_POS;

            // bitmap for things/items to load
            ItemsMap = new LevelItemLoader("Level1Items.png");            
            ItemsMap.AddItems(this, ITEM_BLOCK_COLOR, typeof(Block));
        }

        protected override void InitBadPixels()
        {
            base.InitBadPixels();
            
            // boss
            boss = new Boss();
            Add(boss);

            for (int i = 0; i < 203; i++)
            {
                RedGuard bp = RedGuard.Create(); // Cloaky();
                bp.PositionAndTarget = new Vector2(RandomMath.RandomBetween(123f,720f), RandomMath.RandomBetween(9f,290f) );
                //bp.TargetSpeed = 18.0f; // TODO
                Add(bp);
                FindWalkableGround(bp);
            }

            for (int i = 0; i < 18; i++)
            {
                Servant s = Servant.Create();
                s.PositionAndTarget = new Vector2(RandomMath.RandomBetween(140f, 720f), RandomMath.RandomBetween(9f, 290f));
                Add(s);
                FindWalkableGround(s);
            }

            // servants at local hero's castle
            for (int i = 0; i < 3; i++)
            {
                Servant s = Servant.Create();
                s.AvoidingKnights.ChaseRange = 4f;
                s.AvoidingHero.ChaseRange = 4f;
                s.PositionAndTarget = new Vector2(RandomMath.RandomBetween(0f, 20f), RandomMath.RandomBetween(32f, 90f));
                Add(s);
                FindWalkableGround(s);
            }

            for (int i = 0; i < 14; i++) // XIV companions!
            {
                Knight cp = Knight.Create(); 
                cp.PositionAndTarget = new Vector2(KnightsStartingPositions[2*i],KnightsStartingPositions[2*i+1]);
                //bp.TargetSpeed = 18.0f; // TODO
                Add(cp);
                hero.Knights.Add(cp);
                FindWalkableGround(cp);
            }

        }

        protected void FindWalkableGround(Thing t)
        {
            // move down until on walkable area
            while(t.CollidesWithSomething(Vector2.Zero)) {
                t.PositionY += 1;
                // when unit goes off-screen, delete.
                if (t.PositionY > this.Background.Texture.Height)
                {
                    t.Delete = true;
                    break;
                }
            }
        }

        protected override void InitToys()
        {
            base.InitToys();

            InitToy(new MerlinTestToy(), 51, 82);
            InitToy(new ZoomOutToy(), 80, 144);
            InitToy(new ZoomOutToy(), 211,140);
            InitToy(new ZoomOutToy(), 413,173);
            InitToy(new PaintToy(), 309,143);
            InitToy(new PaintToy(), 307, 274);
            //InitToy(new PaintToy(), 33,154); // debug
            //InitToy(new InvisibilityToy(), 37, 154); // debug
            //InitToy(new SpeedModifyToy(2f), 41, 154); // debug
            InitToy(new InvisibilityToy(), 463,296);
            InitToy(new InvisibilityToy(), 173, 65);
            InitToy(new SpeedModifyToy(2f), 172,230);
            InitToy(new SpeedModifyToy(2f), 450, 291);
            InitToy(new ZoomOutToy(), 560,57);
        }

        protected void InitToy(Toy t, int x, int y)
        {
            t.PositionAndTarget = new Vector2(x,y);
            Add(t);
        }

        protected override void InitLevelSpecific()
        {
            Music = new GameMusic();
            Sound = new GameSound();
            Add(Music);
            Add(Sound);

            // pink arthur
            king = new PinkArthur();
            king.PositionAndTarget = PINKARTHUR_POSITION;
            //king.PositionAndTarget = new Vector2(90f,158f); // debug
            Add(king);

            // graphic title/logo
            Spritelet logo = new PixieLogo("title-logo.png");
            logo.Duration = 8f;
            logo.StartTime = 13f;                        
            Parent.AddNextUpdate(logo);

            SubtitleText t = new SubtitleText();
            t.AddText("Knights of honor!", 4f);
            t.AddText("Follow me! Together we will\nrescue Arthur, our King.", 4f);            
            t.AddText("(Knights:)\nFOR JUSTICE! FOR HONOR! FOR THE KING!", 4f).DrawInfo.DrawColor = Color.RoyalBlue;
            Subtitles.Show(0, t);
            t.StartTime = 1f;

            t = new SubtitleText();
            t.AddText("Galad the Golden, you must rescue Pink Arthur...", 5f);
            t.AddText("...from the clutches of the evil Red Guard.", 5f);
            t.AddText("The Fourteen Kobalt Knights will aid you!", 5f);
            t.AddText("Arthur was taken deep into Red's Keep...", 5f);
            t.AddText("...where rumors say the Giant Red Sorceress lives.", 5f);
            t.AddText("Get him out and take him back, valiant knight!", 5f);
            t.AddText("", 1f);
            t.AddText("Arrows, WASD = move   //   SPACE = Knights's Attack   //   X = Use Spell   //   Esc = Exit", 7f);
            t.AddText("Gamepad = move   //   Button-A = Knights's Attack   //   Button-B = Use Spell", 7f);
            t.AddText("", 1f);
            t.AddText("Coding & art by Indiegame Garden", 4f);
            t.AddText("Sounds by Jute and artisticdude (opengameart.org)", 3f);
            t.AddText("Music by John the Luteist", 3f);
            t.AddText("Sprite by Charles Gabriel (opengameart.org)", 3f);
            
            Parent.Add(t);
            t.ScaleVector = new Vector2(1f, 1f);
            t.Motion.Scale = 0.5f ;
            t.Motion.Position = new Vector2(Screen.Center.X,0.08f);
            //t.DrawInfo.Center = Vector2.Zero;
            t.StartTime = logo.StartTime + logo.Duration + 0.6f;
        }

        protected override bool ScreenBorderHit()
        {
            if (numberOfZoomOuts < 0)
            {
                numberOfZoomOuts++;
                Motion.Scale /= 2.0f;
                //Motion.ScaleTarget /= 2.0f;
                //Motion.ScaleSpeed = 0.2f;
                return false;
            }
            return true;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            // adapt scroll speed to how fast pixie goes
            Background.TargetSpeed = SCREEN_MOTION_SPEED * hero.Velocity;
        }
    }
}
