﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using TTengine.Comps;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Game1.Core;
using Game1.Actors;
using Game1.Comps;
using Artemis;

namespace Game1
{
    /// <summary>
    /// base class for all levels (common functions)
    /// </summary>
    public abstract class Level
    {
        /// <summary>the current Level singleton</summary>
        public static Level Current { get; private set; }

        public ScreenComp Screen;

        /// <summary>Entity that holds the level-related components, that are updated by Systems.</summary>
        public Entity LevelEntity = null;

        // some default colors and settings that may be changed by Level subclasses
        public static Color HERO_COLOR = new Color(251, 101, 159); // pink
        public float DEFAULT_SCALE = 20.0f;
        public float SCREEN_MOTION_SPEED = 15.0f;
        public float HERO_TARGETSPEED = 5.0f;
        public int DefaultPassableIntensityThreshold = 280;
        public Vector2 HERO_STARTING_POS = Vector2.Zero; // in pixels        
        public Vector2 BG_STARTING_POS = Vector2.Zero;    // in pixels; bg=background

        // specific crap TODO?
        public bool hasFoundPrincess = false;
        public bool hasWon = false;

        /// <summary>TODO scrolling screen trigger boundaries (in TTengine coordinates)</summary>
        public bool isBackgroundScrollingOn = true;
        public float BOUND_X = 0.3f;
        public float BOUND_Y = 0.3f;

        /// <summary>default color of the background (e.g. for areas not covered by the bg bitmap)</summary>
        public Color BackgroundColor = Color.Black;

        /// <summary>level music object</summary> 
        public GameMusic Music;

        /// <summary>level sounds object</summary>
        public GameSound Sound;

        /// <summary>background bitmap</summary>
        public LevelBackgroundComp Background;

        /// <summary>load items/toys/things to a level using a bitmap</summary>
        // FIXME public LevelItemLoader ItemsMap;

        public Entity Hero;

        public Entity Boss;

        public SubtitleManager Subtitles;

        public Level()
        {
            Subtitles = new SubtitleManager();
            Screen = TTGame.Instance.BuildScreen.GetComponent<ScreenComp>();
        }

        public static void SetCurrentLevel(Level l)
        {
            Current = l;
        }

        public void Init()
        {
            InitLevel();
            LevelEntity = GameFactory.CreateLevelet(this);
            InitHero();
            InitBadPixels();
            InitToys();
            InitLevelSpecific();
            
            //FIXME needed in screen.Zoom field
            //LevelEntity.GetComponent<ScaleComp>().Scale = DEFAULT_SCALE;
            //LevelEntity.GetComponent<ScaleComp>().ScaleTarget = DEFAULT_SCALE;

            LevelEntity.GetComponent<ThingComp>().Target = HERO_STARTING_POS;
            LevelEntity.GetComponent<ThingComp>().Position = BG_STARTING_POS;
            var sc = LevelEntity.GetComponent<ScriptComp>();
            sc.Scripts.Add(Music);
            sc.Scripts.Add(Sound);
            LevelEntity.Refresh();

            //FIXME to screen MySpriteBatch = new TTSpriteBatch(Screen.graphicsDevice,SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }

        /// <summary>
        /// Init scrolling level background itself plus some basic things. First Init method that is called
        /// </summary>
        protected abstract void InitLevel();

        /// <summary>
        /// Init: hero (a default implementation is in Level)
        /// </summary>
        protected virtual void InitHero()
        {
            this.Hero = Actors.Hero.Create();
            var tc = Hero.GetComponent<ThingComp>();
            tc.PositionAndTarget = HERO_STARTING_POS;
            tc.TargetSpeed = HERO_TARGETSPEED;            

            // TODO controls
            //keyControl = new PixieKeyControl();
            //pixie.Add(keyControl);
        }

        /// <summary>
        /// Init: bad pixels (enemies)
        /// </summary>
        protected abstract void InitBadPixels();

        /// <summary>
        /// Init: toys (=weapons)
        /// </summary>
        protected abstract void InitToys();

        public virtual void LoseLevel()
        {
            /*
            SubtitleText t = new SubtitleText();
            t.AddText("BADLY WOUNDED, YOU DIE.", 7f);
            t.AddText("Galad the Golden is no more.", 7f);
            t.AddText("The princess remains captive\nfor all her life.", 7f);
            Subtitles.Show(9,  t);
            pixie.PositionAndTarget = new Vector2(-200f,240f);
            isBackgroundScrollingOn = false;
             */
        }

        public void WinLevel()
        {
            if (!hasWon)
            {
                /*
                SubtitleText t = new SubtitleText();
                t.AddText("YOU WIN!", 5f);
                t.AddText("The princess\nis rescued.", 4f);
                t.AddText("", 2f);
                t.AddText("** THE END **", 13f);
                float playTime = (float)Math.Round(SimTime);
                t.AddText("(Rescue time: " + playTime + " heartbeats.)", 15f);
                Subtitles.Show(6, t);
                hasWon = true;
                 */
            }
        }

        public void FoundPrincess()
        {
            if (!hasFoundPrincess)
            {
                hasFoundPrincess = true;
                /*
                SubtitleText t = new SubtitleText();
                t.AddText("Princess! Here you are.", 4f);
                t.AddText("You look more fair than in any tale.", 4f);
                t.AddText("Follow me, out of this\ncursed place.", 7f);
                Subtitles.Show(8, t);
                 */
            }
        }

        /// <summary>
        /// Init: level-specific items (not fitting in the existing init categories) to be initialized by subclasses
        /// </summary>
        protected abstract void InitLevelSpecific();

        /// check keys specific for level
        /*
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

        }
        */

        // scroll the level background to match pixie
        /*
        protected virtual void ScrollBackground(ref UpdateParams p)
        {
            // scrolling background at borders
            Vector2 pixiePos = pixie.Motion.PositionAbs;

            if (pixiePos.X < BOUND_X || pixiePos.X > (Screen.Width - BOUND_X) ||
                pixiePos.Y < BOUND_Y || pixiePos.Y > (Screen.Height - BOUND_Y))
            {
                if (ScreenBorderHit())
                    Background.Target = pixie.Position;
            }
        }
         */

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
        /// <returns>true if passable for any ThingComp entity</returns>
        /*
        public bool CanPass(Vector2 pos)
        {
            return Background.IsWalkable(pos);
        }
        */

        /*
        protected override void OnUpdate(ref UpdateParams p)
        {
            // important: reflect the global viewpos (for sprites to use)
            ThingComp.ViewPos = Background.Position;

            // do some level tasks
            LevelKeyControl(ref p);
            if (isBackgroundScrollingOn)
                ScrollBackground(ref p);

            //debugMsg.Text = "Pixie: trg=" + pixie.Target +", pos=" + pixie.Position;
            // DEBUG sample pixel
            //Color c= Background.SamplePixel(pixie.Target);
            //debugMsg.Text += "Color: " + c.R + "," + c.G + "," + c.B + "," + c.A;

        }*/
    }
}
