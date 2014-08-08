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

        public List<Knight> Knights = new List<Knight>();

        public const int LOG_LENGTH = 128;
        public static Vector2[] PositionLog = new Vector2[LOG_LENGTH];
        public static int PositionLogIndex = 0, PrevPositionLogIndex = LOG_LENGTH - 1;

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
            if (this.Target != PositionLog[PositionLogIndex])
            {
                // check if new pos is a straight-line extension of previous move...
                Vector2 vMove = this.Target - PositionLog[PositionLogIndex];
                Vector2 vMovePrev = PositionLog[PositionLogIndex] - PositionLog[PrevPositionLogIndex];
                vMove.Normalize();
                vMovePrev.Normalize();
                if (vMove.Equals(vMovePrev))
                {
                    //... it is, so just adjust current entry. Kind of compression of data = leaving out inbetween entries.
                    PositionLog[PositionLogIndex] = this.Target;
                }
                else
                {   // otherwise, update pos log normally
                    PrevPositionLogIndex = PositionLogIndex;
                    PositionLogIndex = (PositionLogIndex + 1) % LOG_LENGTH;
                    PositionLog[PositionLogIndex] = this.Target;
                }
            }
        }

    }
}
