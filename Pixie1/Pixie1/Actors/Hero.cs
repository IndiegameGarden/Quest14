﻿using System;
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

        public List<Knight> Knights = new List<Knight>();

        public const int LOG_LENGTH = 1024;
        public static Vector2[] PositionLog = new Vector2[LOG_LENGTH];
        public static uint PositionLogIndex = 0;

        protected float health = 12f;

        public Hero()
            : base("pixie")
        {            
            IsCollisionFree = false;
            SetColors(4f, Color.DarkGoldenrod, new Color(230, 210, 10));
            Velocity = 1.5f;

            Pushing.Force = 10f; // force higher than companions.

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

        public float Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
                if (health <= 0f)
                    Level.Current.LoseLevel();
            }
        }

        public void LeadAttack()
        {
            foreach (Knight c in Knights)
                c.Attacking.TriggerAttack();
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // keep pos log
            if (this.Target != PositionLog[CalcPositionLogIndex(0)])
            {
                // check if new pos is a straight-line extension of previous move...
                Vector2 vLog = PositionLog[CalcPositionLogIndex(0)];
                Vector2 vMove = this.Target - vLog;
                Vector2 vMovePrev = vLog - PositionLog[CalcPositionLogIndex(-1)];
                vMove.Normalize();
                if(vMovePrev.Length()>0f)
                    vMovePrev.Normalize();
                if (vMove.Equals(vMovePrev))
                {
                    //... it is, so just adjust current entry. Kind of compression of data = leaving out inbetween entries.
                    PositionLog[CalcPositionLogIndex(0)] = this.Target;
                }
                else
                {   // otherwise, update pos log normally
                    PositionLogIndex = CalcPositionLogIndex(+1);
                    PositionLog[CalcPositionLogIndex(0)] = this.Target;
                }
            }
        }

        public static uint CalcPositionLogIndex(int addValue)
        {
            if (-addValue > PositionLogIndex)
                return 0;
            return ((uint)(PositionLogIndex + addValue)) % LOG_LENGTH;
        }
    }
}
