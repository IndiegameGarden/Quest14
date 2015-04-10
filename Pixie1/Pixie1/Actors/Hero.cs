using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class Hero: Thing
    {
        public PixieKeyControl KeyControl;

        public List<Knight> Knights = new List<Knight>();

        public const int LOG_LENGTH = 4096;
        public static Vector2[] PositionLog = new Vector2[LOG_LENGTH];
        public static int PositionLogIndex = 1;

        public Hero()
            : base("pixie")
        {            
            IsCollisionFree = false;
            SetColors(4f, Color.DarkGoldenrod, new Color(230, 210, 10));
            Velocity = 1.5f;
            Health = 12f;
            Pushing.Force = 10f; // force higher than companions.
            DrawInfo.LayerDepth = 0.4f;

        }

        protected override void OnInit()
        {
            base.OnInit();
            KeyControl = new PixieKeyControl();
            this.Add(KeyControl);
        }

        /// <summary>
        /// Sets Hero's starting Target/Position and pre-inits the PositionLog with that value.
        /// </summary>
        /// <param name="pos"></param>
        public void SetStartingPos(Vector2 pos)
        {
            PositionAndTarget = pos;
            for (int i = 0; i < LOG_LENGTH; i++)
                PositionLog[i] = pos;
        }

        public void LeadAttack()
        {
            if (SimTime < 200 && SimTime > 21.6)
                ShowAttackDirectionString();
            foreach (Knight c in Knights)
                c.Attacking.TriggerAttack();
        }

        protected void ShowAttackDirectionString()
        {
            String t = "Knights attack\n";
            if (FacingDirection.X < 0 && FacingDirection.Y == 0)
                t += "west";
            if (FacingDirection.X > 0 && FacingDirection.Y == 0)
                t += "east";
            if (FacingDirection.X == 0 && FacingDirection.Y < 0)
                t += "north";
            if (FacingDirection.X == 0 && FacingDirection.Y > 0)
                t += "south";
            t += "ward.";
            Level.Current.ControlsHelpText.ClearText();
            Level.Current.ControlsHelpText.AddText(t, 3.3f);
            Level.Current.ControlsHelpText.Duration = -1; // FIXME terrible hack to avoid auto-deletion
        }

        public void ShowInventory()
        {
            String t = "You now hold\n";
            if (this.ToyActive==null)
                t = "You hold\nnothing.";
            else {
                t+= "\"" + this.ToyActive.ToyName() + "\"";
            }
            Level.Current.ControlsHelpText.ClearText();
            Level.Current.ControlsHelpText.AddText(t, 3.3f);
            Level.Current.ControlsHelpText.Duration = -1; // FIXME terrible hack to avoid auto-deletion
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // keep pos log
            if (this.Target != GetPositionLog(PositionLogIndex))
            {
                // check if new pos is a straight-line extension of previous move...
                Vector2 vLog = GetPositionLog(PositionLogIndex);
                Vector2 vMove = this.Target - vLog;
                Vector2 vMovePrev = vLog - GetPositionLog(PositionLogIndex-1);
                vMove.Normalize();
                if(vMovePrev.Length()>0f)
                    vMovePrev.Normalize();
                if (vMove.Equals(vMovePrev))
                {
                    //... it is, so just adjust current entry. Kind of compression of data = leaving out inbetween entries.
                    SetPositionLog(PositionLogIndex, Target);
                }
                else
                {   // otherwise, update pos log normally
                    PositionLogIndex++;
                    SetPositionLog(PositionLogIndex, Target);
                }
            }

        }

        protected override void OnDies()
        {
            base.OnDies();
            ColorFx.Active = false;
            KeyControl.Active = false;
            
            Level.Current.LoseLevel();
        }

        public static Vector2 GetPositionLog(int idx)
        {
            if (idx < 0) idx = 0;
            return PositionLog[idx % LOG_LENGTH];
        }

        public static void SetPositionLog(int idx, Vector2 v)
        {
            if (idx < 0) idx = 0;
            PositionLog[idx % LOG_LENGTH] = v;
        }
    }
}
